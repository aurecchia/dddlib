// <copyright file="MemoryNotificationService.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Persistence.EventDispatcher.Memory
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using dddlib.Sdk;
    using dddlib.Persistence.EventDispatcher.Sdk;

    internal sealed class MemoryNotificationService : INotificationService, IDisposable
    {
        private static readonly IJsonSerializer Serializer = new JavaScriptSerializer();

        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);

        public MemoryNotificationService()
        {
            Task.Factory.StartNew(this.GetData);
        }

        public event EventHandler<BatchPreparedEventArgs> OnBatchPrepared;

        public event EventHandler<EventCommittedEventArgs> OnEventCommitted;

        public void Dispose()
        {
            this.resetEvent.Set();
            this.resetEvent.Dispose();
        }

        // LINK (Cameron): http://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
        private void GetData()
        {
            // NOTE (Alessio): This path needs to be the same that we use in `dddlib.Persistence.Memory.MemoryEventStore`
            var tempFileName = Path.Combine(Path.GetTempPath(), "MemoryEventStore2");
            const int bufferSize = 10 * 1024 * 1024;

            // TODO (Alessio): Not sure if we can create multiple memory mapped files from the same physical file on
            //                 disk. If an existing file cannot be used with the `MemoryMappedFile.CreateFromFile()`
            //                 method, then we need to implement IPC differently
            using (var fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize))
            using (var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset))
            using (var file = MemoryMappedFile.CreateFromFile(fileStream, null, bufferSize, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true))
            {
                var offset = 0L;

                while (WaitHandle.WaitAny(new WaitHandle[] { this.resetEvent, waitHandle }) == 1)
                {
                    var sequenceNumber = 0L;
                    var length = 0;
                    do
                    {
                        using (var accessor = file.CreateViewAccessor(offset, 2))
                        {
                            length = accessor.ReadUInt16(0);
                            if (length == 0)
                            {
                                // NOTE (Cameron): Reset event occurred but no changes, so exit.
                                break;
                            }
                        }

                        var buffer = new byte[length];
                        using (var accessor = file.CreateViewAccessor(offset + 2, length))
                        {
                            accessor.ReadArray(0, buffer, 0, length);
                        }

                        var serializedEvent = Encoding.UTF8.GetString(buffer);
                        var memoryMappedEvent = Serializer.Deserialize<MemoryMappedEvent>(serializedEvent);

                        sequenceNumber = memoryMappedEvent.SequenceNumber;
                        offset += 2 + buffer.Length;
                    }
                    while (length > 0);

                    if (this.OnEventCommitted != null)
                    {
                        this.OnEventCommitted.Invoke(this, new EventCommittedEventArgs(sequenceNumber));
                    }

                    // HACK (Cameron): Remove.
                    var batchId = 0;
                    if (this.OnBatchPrepared != null)
                    {
                        this.OnBatchPrepared.Invoke(this, new BatchPreparedEventArgs(batchId));
                    }
                }
            }

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
        }

        private class MemoryMappedEvent
        {
            public Guid StreamId { get; set; }

            public string Type { get; set; }

            public string Payload { get; set; }

            public long SequenceNumber { get; set; }

            public string State { get; set; }
        }
    }
}
