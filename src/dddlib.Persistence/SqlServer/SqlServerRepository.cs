﻿// <copyright file="SqlServerRepository.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Persistence.SqlServer
{
    using dddlib.Persistence.Sdk;

    /// <summary>
    /// Represents a SQL Server repository.
    /// </summary>
    /// <typeparam name="T">The type of aggregate root.</typeparam>
    public abstract class SqlServerRepository<T> : Repository<T> where T : AggregateRoot
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerRepository{T}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlServerRepository(string connectionString)
            : base(new SqlServerIdentityMap(connectionString))
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerRepository{T}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="schema">The schema.</param>
        public SqlServerRepository(string connectionString, string schema)
            : base(new SqlServerIdentityMap(connectionString, schema))
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        protected string ConnectionString
        {
            get { return this.connectionString; }
        }
    }
}
