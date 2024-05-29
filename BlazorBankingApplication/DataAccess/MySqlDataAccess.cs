using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace BlazorBankingApplication.DataAccess
{
    public class MySqlDataAccess : IMySqlDataAccess
    {
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);
                return rows.ToList();
            }
        }

        //Task SaveData saves the data in the database
        public async Task SaveData<T>(string sql, T parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                //return connection.ExecuteAsync(sql, parameters);
                await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
