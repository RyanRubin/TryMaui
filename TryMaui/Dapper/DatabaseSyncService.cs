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
    public class DatabaseSyncService : IDatabaseSyncService
    {
        // TODO Move these to a config file when MAUI has official config file support
        public const string LocalDatabaseConnectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=PeopleData";
        public const string ServerDatabaseConnectionString = "Server=localhost,1433;User=sa;Password=Password123!;Database=PeopleData";

        private readonly IConfiguration configuration;

        public DatabaseSyncService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<int> ExecuteSync(string srcConnectionStringName, string destConnectionStringName)
        {
            IEnumerable<PersonEntity> people;
            using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString(srcConnectionStringName)))
            {
                people = await connection.QueryAsync<PersonEntity>("SELECT * FROM dbo.People");
            }

            string peopleJson = JsonSerializer.Serialize(people);

            using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString(destConnectionStringName)))
            {
                int rowsAffected = await connection.ExecuteAsync(@"
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
                    WHEN NOT MATCHED BY SOURCE THEN
	                    DELETE;
                ", new { PeopleJson = peopleJson });

                return rowsAffected;
            }
        }
    }
}
