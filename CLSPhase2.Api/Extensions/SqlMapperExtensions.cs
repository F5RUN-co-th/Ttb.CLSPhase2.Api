using CLSPhase2.Api.Handler.Sql;
using CLSPhase2.Dal.Entities;
using Dapper;
using Dapper.Contrib.Extensions;

namespace CLSPhase2.Api.Extensions
{
    public static class SqlMapperServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlMapperExtensions(this IServiceCollection services, IConfiguration config)
        {
            var schema = config["Database:SchemaCpss"] ?? "";

            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
            SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
            SqlMapperExtensions.TableNameMapper = entityType =>
            {
                if (entityType == typeof(TempRequestBatch))
                {
                    return $"{schema}\"{entityType.Name}\"";
                }
                throw new NotImplementedException();
            };

            return services;
        }
    }
}