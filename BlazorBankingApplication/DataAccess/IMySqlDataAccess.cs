﻿namespace BlazorBankingApplication.DataAccess
{
    public interface IMySqlDataAccess
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionString);
        Task SaveData<T>(string sql, T parameters, string connectionString);
    }
}