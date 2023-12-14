using Microsoft.AspNetCore.Cors.Infrastructure;
using MySql.Data.MySqlClient;
using System.Data;

namespace DB_Project.Entities
{
    public class DBManager
    {
        private static DBManager instance;
        private readonly MySqlConnection connection;

        private DBManager() 
        {
            string connectionString = "Server=localhost;UserID=root;port=3306;Password=_2273_Vfrc_;Database=restaurant";
            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = new()
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = "SET @\'CUR_ID\'=3;"
            };
            command.ExecuteNonQuery();
            //command.Parameters.Add("@id_dish", MySqlDbType.UInt32);

        }
        public static DBManager Instance
        {
            get
            {
                instance ??= new DBManager();
                return instance;
            }
        }
        public MySqlCommand GetCommand()
        {
            MySqlCommand command = new()
            {
                Connection = connection,
                CommandType = CommandType.Text
            };
            return command;
        }
        ~DBManager()
        {
            connection.Close();
        }
    }
}
