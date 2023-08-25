using CLSPhase2.Api.Handlers.AuthHandlers;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using CLSPhase2.Dal.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Data;
using ServiceStack;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SQLite;

namespace CLSPhase2.UnitTests.Factory
{
    public class ApiApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _environment;
        public ApiApplicationFactory(string environment = "Development") => _environment = environment;

        //public OrmLiteConnectionFactory dbFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);

        //public IDbConnection OpenConnection() => this.dbFactory.OpenDbConnection();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IDbConnectionFactory>(container =>
                {
                    var connection = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);
                    connection.Open();

                    return connection;
                });
            });


            // Works for: Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") in Program.cs
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _environment);

            // OR

            // Works for: builder.Environment.EnvironmentName in Program.cs
            //builder.UseEnvironment("Development");

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
