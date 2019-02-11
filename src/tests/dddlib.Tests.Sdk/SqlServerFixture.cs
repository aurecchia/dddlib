// <copyright file="SqlServerFixture.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Tests.Sdk
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;
    using Microsoft.Extensions.Configuration;

    public class SqlServerFixture : IDisposable
    {
        private readonly string databaseName = Guid.NewGuid().ToString("N");
        private readonly string connectionString;

        public SqlServerFixture()
        {
            // On Windows we can rely on the system authenticating us to the database, on other platforms we retrieve
            // the credentials from user secrets
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.connectionString = ConfigurationManager.ConnectionStrings["SqlDatabase"].ConnectionString;
            }
            else
            {
                var builder = new SqlConnectionStringBuilder(
                    ConfigurationManager.ConnectionStrings["SqlDatabaseLinux"].ConnectionString
                );

                var configuration = new ConfigurationBuilder().AddUserSecrets<SqlServerFixture>()
                                                              .Build();
                var password = configuration["DbPassword"];
                if (!string.IsNullOrWhiteSpace(password))
                {
                    builder.Password = password;
                }

                this.connectionString = builder.ConnectionString;
            }

            using (var connection = new SqlConnection(this.connectionString))
            {
                var serverConnection = new ServerConnection(connection);
                var server = new Server(serverConnection);
                var database = new Database(server, this.databaseName);

                database.Create();
            }
        }

        public string ConnectionString
        {
            get { return this.connectionString; }
        }

        public string DatabaseName
        {
            get { return this.databaseName; }
        }

        public void Dispose()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var serverConnection = new ServerConnection(connection);
                var server = new Server(serverConnection);

                server.KillDatabase(this.databaseName);
            }
        }
    }
}
