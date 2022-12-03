using HttpServer.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class MyORM
    {
        public string TableName;
        readonly string connectionString;

        public MyORM(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            TableName = tableName;
        }

        public async Task<T[]> SelectAsync<T>()
        {
            string sqlExpression = $"SELECT * FROM [dbo].[{TableName}]";
            return await ExecuteQueryAsync<T>(sqlExpression);
        }

        public async Task<T[]> SelectWhereAsync<T>(Dictionary<string, object> conditions)
        {
            var stringConditions = conditions.Select(c => $"{c.Key}=N'{c.Value.ToString().Replace("'", "''")}'");
            string sqlExpression = $"SELECT * FROM [dbo].[{TableName}] WHERE {string.Join(" AND ", stringConditions)}";

            return await ExecuteQueryAsync<T>(sqlExpression);
        }

        public T? Select<T>(int id) => Select<T>("id", id.ToString());

        public T? Select<T>(string primaryKeyColumn, string primaryKeyValue) 
            => SelectWhereAsync<T>(new Dictionary<string, object>() { { primaryKeyColumn, primaryKeyValue } }).Result.FirstOrDefault();

        public async Task<int> InsertAsync<T>(T item, bool idExists = true)
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(FieldDB)) is not null)
                .ToDictionary(p => ((FieldDB)p.GetCustomAttribute(typeof(FieldDB))).ColumnName, p => $"{(((FieldDB)p.GetCustomAttribute(typeof(FieldDB))).IsCyrillic ? "N" : "")}'{p.GetValue(item).ToString().Replace("'", "''")}'");

            string sqlExpression = $"INSERT INTO [dbo].[{TableName}]({string.Join(',', properties.Keys)}) {(idExists ? "OUTPUT inserted.id AS 'id'" : "")} VALUES ({string.Join(',', properties.Values)})";

            int insertedId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                if (idExists)
                    return (int)(await command.ExecuteScalarAsync() ?? 0);
                else
                    await command.ExecuteNonQueryAsync();
            }

            return insertedId;
        }

        public async void UpdateAsync<T>(int id, T item)
        {
            var changes = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(FieldDB)) is not null)
                .Select(p =>
                    $"{((FieldDB)p.GetCustomAttribute(typeof(FieldDB))).ColumnName} = {(((FieldDB)p.GetCustomAttribute(typeof(FieldDB))).IsCyrillic ? "N" : "")}'{p.GetValue(item).ToString().Replace("'", "''")}'");

            string sqlExpression = $"UPDATE [dbo].[{TableName}] SET {string.Join(',', changes)} WHERE id={id}";

            await ExecuteNonQueryAsync(sqlExpression);
        }
        

        public async void DeleteWhereAsync(Dictionary<string, object> conditions)
        {
            var stringConditions = conditions.Select(c => $"{c.Key}=N'{c.Value.ToString().Replace("'", "''")}'");
            string sqlExpression = $"DELETE FROM [dbo].[{TableName}] WHERE {string.Join(" AND ", stringConditions)}";

            await ExecuteNonQueryAsync(sqlExpression);
        }

        public void Delete(int id) => Delete("id", id.ToString());

        public void Delete(string primaryKeyColumn, string primaryKeyValue) 
            => DeleteWhereAsync(new Dictionary<string, object>() { { primaryKeyColumn, primaryKeyValue } });

        private async Task ExecuteNonQueryAsync(string sqlExpression)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task<T[]> ExecuteQueryAsync<T>(string sqlExpression)
        {
            var result = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        var newItem = Activator.CreateInstance(typeof(T), GetValues(reader));
                        if (newItem is T item)
                            result.Add(item);
                    }
                }

                reader.Close();
            }
            return result.ToArray();
        }

        private object[] GetValues(SqlDataReader reader)
        {
            var values = new List<object>();

            for (int i = 0; i < reader.FieldCount; i++)
                values.Add(reader.GetValue(i));

            return values.ToArray();
        }
    }
}
