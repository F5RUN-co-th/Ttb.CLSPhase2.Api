using CLSPhase2.Dal.Infrastructure;
using CLSPhase2.Dal.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace CLSPhase2.Dal.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly IConnectionFactory _connectionFactory;
        private string _userId;
        private const string CreatedAt = "CreatedAt";
        private const string UpdatedAt = "UpdatedAt";
        private const string DeletedAt = "DeletedAt";

        private const string CreatedBy = "CreatedBy";
        private const string UpdatedBy = "UpdatedBy";
        private const string DeletedBy = "DeletedBy";

        public GenericRepository(
            IConnectionFactory connectionFactory,
            IBaseSystem baseSys
            )
        {
            _connectionFactory = connectionFactory;
            _connectionFactory.DbUserName = baseSys.cpssModel.DbUserName;
            _connectionFactory.DbPassword = baseSys.cpssModel.DbPassword;
            _userId = baseSys.UserId;
        }

        public async Task<TEntity> Get(int Id)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                return await connection.GetAsync<TEntity>(Id);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                return await connection.GetAllAsync<TEntity>();
            }
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            long objId = 0;
            using (var connection = _connectionFactory.GetConnection)
            {
                objId = await connection.InsertAsync(entity);
                return await connection.GetAsync<TEntity>(objId);
            }
        }

        public async Task<bool> Update(TEntity entity)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                return await connection.UpdateAsync(entity);
            }
        }

        public async Task<bool> Delete(TEntity entity)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                return await connection.DeleteAsync(entity);
            }
        }

        public class JsonBParameter : SqlMapper.ICustomQueryParameter
        {
            private readonly object json;

            public JsonBParameter(object json)
            {
                this.json = json;
            }

            public void AddParameter(IDbCommand command, string name)
            {
                var parameter = (NpgsqlParameter)command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = json;
                parameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
                command.Parameters.Add(parameter);
            }
        }
    }
}
