using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TryMaui.Dapper
{
    public class DatabaseSyncService
    {
        // TODO Move these to a config file when MAUI has official config file support
        public const string LocalDatabaseConnectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=PeopleData";
        public const string ServerDatabaseConnectionString = "Server=localhost,1433;User=sa;Password=Password123!;Database=PeopleData";

        private readonly string srcConnectionString;
        private readonly string destConnectionString;

        public DatabaseSyncService(string srcConnectionString, string destConnectionString)
        {
            this.srcConnectionString = srcConnectionString;
            this.destConnectionString = destConnectionString;
        }

        public async Task<int> ExecuteSync()
        {
            IEnumerable<Person> people;
            using (IDbConnection connection = new SqlConnection(srcConnectionString))
            {
                people = await connection.QueryAsync<Person>("SELECT * FROM dbo.People");
            }

            string json = JsonSerializer.Serialize(people);

            using (IDbConnection connection = new SqlConnection(destConnectionString))
            {
                int rowsAffected = await connection.ExecuteAsync(@"
                    MERGE dbo.People AS dest
                    USING (
	                    SELECT * FROM OPENJSON(@Json) WITH (
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
                ", new { Json = json });

                return rowsAffected;
            }
        }
    }
}
