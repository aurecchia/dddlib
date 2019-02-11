dddlib
======

[![Join the chat at https://gitter.im/dddlib/dddlib](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dddlib/dddlib?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

A domain driven design library for .NET


## Testing

Some of the tests require a database connection to run. The connection string
to the database can be set in the `app.config` file (for
`dddlib.Persistence.Tests` and `dddlib.Persistence.EventDispatcher.Tests`).

On Windows, the system authentication service is leveraged when connecting to
the database, whereas on othe platforms the database password is expected to be
stored in the user secrets as follows:

```
$ dotnet user-secrets set DbPassword 'YourStrongPassw0rd!'
```

On Linux, Microsoft SQL Server can be installed follwing this guide:
[https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup?view=sql-server-2017]
