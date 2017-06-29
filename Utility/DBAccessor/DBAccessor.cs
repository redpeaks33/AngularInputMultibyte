using System;
using System.Data;
using System.Data.SqlClient;

namespace DataBaseAccessor
{
    public static class DBAccessor
    {
        #region Word

        //Release
        private static string CONNECTION_STR_WORD = "Server=tcp:redpeaks.database.windows.net,1433;Database=WordChunk;User ID=redpeak3@redpeaks;Password=redmine9!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        //Debug
        //private static string CONNECTION_STR_WORD = "Server=tcp:redpeaks.database.windows.net,1433;Database=WordChunkTest;User ID=redpeak3@redpeaks;Password=redmine9!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        //private static string CONNECTION_STR_WORD = "Server=tcp:wordchunk.database.windows.net,1433;Database=WordChunk;User ID=redpeak3@wordchunk;Password=redmine9!;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        #endregion Word

        #region ASIN

        private static string CONNECTION_STR_ASIN = "Server=tcp:redpeaks.database.windows.net,1433;Database=Asin;User ID=redpeak3@redpeaks;Password=redmine9!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        //private static string CONNECTION_STR_BOT = "Server=tcp:gomhyc7gpr.database.windows.net,1433;Database=imagebot;User ID=redpeaks@gomhyc7gpr;Password=redmine9!;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        #endregion ASIN

        public static string GetConnectionStr(DataBaseType dataBaseType)
        {
            var str = "";
            switch (dataBaseType)
            {
                case DataBaseType.WORD: str = CONNECTION_STR_WORD; break;
                case DataBaseType.ASIN: str = CONNECTION_STR_ASIN; break;
                default: break;
            }
            return str;
        }

        public static int ExecuteSQL(string sql, DataBaseType dataBaseType)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(GetConnectionStr(dataBaseType)))
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.CommandTimeout = 10800;
                result = command.ExecuteNonQuery();
            }

            return result;
        }

        public static int ExecuteSQLScalar(string sql, DataBaseType dataBaseType)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(GetConnectionStr(dataBaseType)))
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                result = Convert.ToInt32(command.ExecuteScalar());
            }

            return result;
        }

        public static DataSet ExecuteSQLToGetDataBase(string sql, DataBaseType dataBaseType)
        {
            var dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection(GetConnectionStr(dataBaseType)))
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                var adapter = new SqlDataAdapter(command);
                command.CommandTimeout = 3000;
                adapter.Fill(dataSet);
            }

            return dataSet;
        }

        public static int ExecuteSQL(SqlCommand command, DataBaseType dataBaseType)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(GetConnectionStr(dataBaseType)))
            {
                connection.Open();
                command.Connection = connection;
                result = (int)command.ExecuteNonQuery();
            }

            return result;
        }


        public static int ExecuteSQLScalar(SqlCommand command, DataBaseType dataBaseType)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(GetConnectionStr(dataBaseType)))
            {
                connection.Open();
                command.Connection = connection;
                result = (int)command.ExecuteScalar();
            }

            return result;
        }
    }
}