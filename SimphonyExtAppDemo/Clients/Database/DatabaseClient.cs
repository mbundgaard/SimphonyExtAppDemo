using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SimphonyExtAppDemo.Factories.DbConnectionFactories;
using SimphonyExtAppDemo.Helpers;
using SimphonyExtAppDemo.Model;

namespace SimphonyExtAppDemo.Clients.Database
{
    public class DatabaseClient
    {
        private readonly SimphonyDbConnectionFactory _databaseConnectionFactory;

        public DatabaseClient()
        {
            _databaseConnectionFactory = new SimphonyDbConnectionFactory();
        }

        public IEnumerable<ClosedCheck> GetClosedChecksByRvc(int rvcNumber)
        {
            const string sql =
                "SELECT c.CheckNumber, c.Guid " +
                "FROM[CheckPostingDB].[dbo].[CHECKS] c " +
                "LEFT JOIN[CheckPostingDB].[dbo].[REVENUE_CENTER] rvc on c.RevCtrID = rvc.RevCtrID " +
                "WHERE rvc.ObjectNumber = @rvcNumber";

            var parameters = new[]
            {
                new KeyValuePair<string, object>("@rvcNumber", rvcNumber),
            };

            return  Read<ClosedCheck>(sql, parameters).ToList();
        }

        private IEnumerable<T> Read<T>(string sql, params KeyValuePair<string, object>[] parameters)
        {
            var dataTable = ExecuteQuery(sql, parameters);

            return (from DataRow row in dataTable.Rows select Entity.CreateInstance<T>(row)).ToList();
        }

        private DataTable ExecuteQuery(string sql, params KeyValuePair<string, object>[] parameters)
        {
            using (var conn = CreateConnection())
            {
                var cmd = CreateCommand(conn, sql, parameters);

                var output = new DataTable("result");

                output.Load(cmd.ExecuteReader());

                return output;
            }
        }

        private IDbConnection CreateConnection()
        {
            var conn = _databaseConnectionFactory.CreateConnection();

            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        private IDbCommand CreateCommand(IDbConnection conn, string sql, params KeyValuePair<string, object>[] parameters)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandTimeout = 180;
            cmd.CommandText = sql;

            for (var i = 0; i < parameters.Length; i++)
            {
                var dbParameter = cmd.CreateParameter();
                dbParameter.ParameterName = parameters[i].Key;
                dbParameter.Value = parameters[i].Value ?? DBNull.Value;
                cmd.Parameters.Add(dbParameter);
            }

            return cmd;
        }
    }
}