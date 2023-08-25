using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CLSPhase2.Dal.Infrastructure
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionStr;
        public ConnectionFactory(IConfiguration config)
        {
            _connectionStr = config["ApiSettings:Database:PostgresConnectionStr"] ?? "";
        }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public IDbConnection GetConnection
        {
            get
            {
                DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
                var factory = DbProviderFactories.GetFactory("Npgsql");

                var conn = factory.CreateConnection();
                if (conn is null) throw new Exception($"\"{conn}\" did not have a valid database provider registered");

                conn.ConnectionString = string.Format(_connectionStr,DbUserName,DbPassword);
                conn.Open();
                if (conn.State != ConnectionState.Open) throw new InvalidOperationException("connection should be open!");

                return conn;
            }
        }
    }
}
