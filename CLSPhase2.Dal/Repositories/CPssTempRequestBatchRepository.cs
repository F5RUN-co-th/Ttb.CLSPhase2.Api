using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Infrastructure;
using CLSPhase2.Dal.Interfaces;
using CLSPhase2.Dal.UnitOfWork;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections;
using System.Linq;

namespace CLSPhase2.Dal.Repositories
{
    public class CPssTempRequestBatchRepository : GenericRepository<TempRequestBatch>, ICPssTempRequestBatchRepository
    {
        public CPssTempRequestBatchRepository(IConnectionFactory connectionFactory, IBaseSystem baseSystem)
            : base(connectionFactory, baseSystem)
        {

        }

        public async Task<bool> Delete(object[] idList)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                return await connection.ExecuteAsync($"DELETE FROM {SqlMapperExtensions.TableNameMapper(typeof(TempRequestBatch))} WHERE Id = @idList;", idList) > 0;
            }
        }

        public async Task<int> Create(Dictionary<string, object> dataStore)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                var sbValue = new System.Text.StringBuilder();
                var sbComlumn = new System.Text.StringBuilder();
                foreach (var item in dataStore)
                {
                    sbComlumn.Append($"{item.Key},");
                    sbValue.Append($"@{item.Key},");
                }
                var comlumn = sbComlumn.ToString().TrimEnd(',');
                var value = sbValue.ToString().TrimEnd(',').Replace("\"", "");
                var sql = $"INSERT INTO {SqlMapperExtensions.TableNameMapper(typeof(TempRequestBatch))} ({comlumn}) VALUES ({value}) RETURNING Id;";
                var mapParameter = new
                {
                    ReferenceCode = dataStore[$"\"{nameof(TempRequestBatch.ReferenceCode)}\""],
                    JsonDocOutboundRequest = new JsonBParameter(dataStore[$"\"{nameof(TempRequestBatch.JsonDocOutboundRequest)}\""]),
                    JsonDocOutboundResponse = new JsonBParameter(dataStore[$"\"{nameof(TempRequestBatch.JsonDocOutboundResponse)}\""]),
                    CreatedAt = dataStore[$"\"{nameof(TempRequestBatch.CreatedAt)}\""]
                };
                return await connection.ExecuteScalarAsync<int>(sql, mapParameter);
            }
        }

        public async Task<int> Create(Dictionary<string[], IEnumerable<Dictionary<string, object>>> dataStore)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                var sbValue = new System.Text.StringBuilder();
                var sbComlumn = new System.Text.StringBuilder();
                var keys = dataStore.SelectMany(s => s.Key);
                var datas = dataStore.SelectMany(s => s.Value);
                foreach (var Key in keys)
                {
                    sbComlumn.Append($"{Key},");
                    sbValue.Append($"@{Key},");
                }
                var comlumn = sbComlumn.ToString().TrimEnd(',');
                var value = sbValue.ToString().TrimEnd(',').Replace("\"", "");
                var sql = $"INSERT INTO {SqlMapperExtensions.TableNameMapper(typeof(TempRequestBatch))} ({comlumn}) VALUES ({value});";
                var mapParameter = datas.Select(s => new
                {
                    ReferenceCode = s[$"\"{nameof(TempRequestBatch.ReferenceCode)}\""],
                    JsonDocOutboundRequest = new JsonBParameter(s[$"\"{nameof(TempRequestBatch.JsonDocOutboundRequest)}\""]),
                    JsonDocOutboundResponse = new JsonBParameter(s[$"\"{nameof(TempRequestBatch.JsonDocOutboundResponse)}\""]),
                    CreatedAt = s[$"\"{nameof(TempRequestBatch.CreatedAt)}\""]
                });
                return await connection.ExecuteAsync(sql, mapParameter);
            }
        }

        public async Task<IEnumerable<TempRequestBatch>> GetTempRequestBatchByItemList(Dictionary<string[], IEnumerable<Dictionary<string, object>>> dataStore)
        {
            using (var connection = _connectionFactory.GetConnection)
            {
                var whereField = nameof(TempRequestBatch.ReferenceCode);
                var p = new ArrayList();
                var mapParameter = new DynamicParameters();
                var sbValue = new System.Text.StringBuilder();
                var sbComlumn = new System.Text.StringBuilder();

                var keys = dataStore.SelectMany(s => s.Key);
                var datas = dataStore.SelectMany(s => s.Value).Select((v, i) => (v, i));

                foreach (var Key in keys)
                    sbComlumn.Append($"{Key},");

                foreach (var (item, index) in datas)
                {
                    string paraIndex = $"@{whereField}" + index;
                    mapParameter.Add(paraIndex, item[$"\"{whereField}\""]);
                    p.Add(paraIndex);
                }
                var whereClause = string.Format("{0} in ({1})", $"\"{whereField}\"", String.Join(",", p.ToArray()));

                var comlumn = sbComlumn.ToString().TrimEnd(',');
                var sql = $"SELECT id, {comlumn} FROM {SqlMapperExtensions.TableNameMapper(typeof(TempRequestBatch))} WHERE {whereClause} ;";
                return await connection.QueryAsync<TempRequestBatch>(sql, mapParameter);
            }
        }
    }
}
