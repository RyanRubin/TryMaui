using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TryMaui.Dapper
{
    public class SqlDataSyncService : IDatabaseSyncService
    {
        private readonly IConfiguration configuration;

        public SqlDataSyncService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<int> ExecuteSync(string srcConnectionStringName, string destConnectionStringName, bool isDeleteWhenNotMatchedBySource = true)
        {
            int rowsAffected = 0;

            rowsAffected += await SyncPeople(srcConnectionStringName, destConnectionStringName, isDeleteWhenNotMatchedBySource);

            return rowsAffected;
        }

        private async Task<int> SyncPeople(string srcConnectionStringName, string destConnectionStringName, bool isDeleteWhenNotMatchedBySource = true)
        {
            string peopleJson;

            using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString(srcConnectionStringName)))
            {
                var people = await connection.QueryAsync("SELECT * FROM dbo.People");
                peopleJson = JsonSerializer.Serialize(people);
            }

            using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString(destConnectionStringName)))
            {
                string whenNotMatchedBySourceClause = @"
                    WHEN NOT MATCHED BY SOURCE THEN
	                    DELETE
                ";

                string query = @$"
                    MERGE dbo.People AS dest
                    USING (
	                    SELECT * FROM OPENJSON(@PeopleJson) WITH (
		                    Id UNIQUEIDENTIFIER '$.Id',
		                    FirstName NVARCHAR(50) '$.FirstName',
		                    LastName NVARCHAR(50) '$.LastName',
		                    StateId INT '$.StateId'
	                    )
                    ) AS src (
	                    Id,
	                    FirstName,
	                    LastName,
	                    StateId
                    ) ON src.Id = dest.Id
                    WHEN NOT MATCHED BY TARGET THEN
	                    INSERT (
		                    Id,
		                    FirstName,
		                    LastName,
		                    StateId
	                    ) VALUES (
		                    src.Id,
		                    src.FirstName,
		                    src.LastName,
		                    src.StateId
	                    )
                    WHEN MATCHED THEN
	                    UPDATE SET
		                    FirstName = src.FirstName,
		                    LastName = src.LastName,
		                    StateId = src.StateId
                    { (isDeleteWhenNotMatchedBySource ? whenNotMatchedBySourceClause : "") }
                    ;
                ";

                int rowsAffected = await connection.ExecuteAsync(query, new { PeopleJson = peopleJson });

                return rowsAffected;
            }
        }
    }
}
