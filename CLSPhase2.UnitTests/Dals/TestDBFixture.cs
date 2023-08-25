using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.UnitTests.Factory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CLSPhase2.UnitTests.Dals
{
    public class TestDBFixture : IClassFixture<ApiApplicationFactory<Program>>
    {
        private readonly ApiApplicationFactory<Program> _apiApplicationFactory;

        public TestDBFixture(ApiApplicationFactory<Program> apiApplicationFactory)
        {
            _apiApplicationFactory = apiApplicationFactory;
        }

        //[Fact]
        //public async Task TestDatabase()
        //{
        //    using (var db = _apiApplicationFactory.OpenConnection())
        //    {
        //        Assert.True(db.State is ConnectionState.Open);
        //    }
        //    );
        //}
    }
}
