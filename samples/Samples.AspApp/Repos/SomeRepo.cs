using Dapper;
using Microsoft.Data.SqlClient;

namespace Samples.AspApp.Repos
{
    public class SomeRepo(IConfiguration configuration) : ISomeRepo
    {
        public async Task<(int IntValue, string StringValue)> GetDataAsync(CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("WEB");
            using var connection = new SqlConnection(connectionString);

            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<(int IntValue, string StringValue)>(GetData.ToString());
        }

        public async Task<int> UpdateDataAsync(CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("WEB");
            using var connection = new SqlConnection(connectionString);

            connection.Open();

            return await connection.ExecuteAsync(UpdateData.ToString());
        }

        private static FormattableString GetData => $@"
			SELECT TOP (1)  
				SomeInt AS IntValue,
				SomeString AS StringValue
			from dbo.SomeTable";

        private static FormattableString UpdateData => $@"
			UPDATE dbo.SomeTable
                SET SomeString = 'kokos'
            WHERE SomeInt = 0";
    }
}

