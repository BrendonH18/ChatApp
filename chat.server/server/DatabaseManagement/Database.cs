using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;
using Npgsql;

//namespace server.DatabaseManagement
namespace server
{
    public class Database : IDisposable
    {
        private readonly string connString = "Server=127.0.0.1;Port=5432;Username=postgres;Password=password;Database=chat_app";

        //public string SQLBuilder(string string table, )

        public void InsertNewMessage(UserConnection userConnection, string message)
        {
            using (var con = new NpgsqlConnection(connString))
            {
                using (var cmd = new NpgsqlCommand() )
                {
                    cmd.CommandText = $"INSERT INTO messsages_all (room, user, message) VALUES ('{userConnection.Room}', '{userConnection.User}' , '{message}')";

                    cmd.ExecuteNonQuery();

                    Console.WriteLine($"PostgreSQL Command: {cmd.CommandText}");
                }
            }
            
            
        }

        public void InsertNewUser(string connectionId, UserConnection userConnection)
        {
            using var con = new NpgsqlConnection(connString);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"INSERT INTO user_connections (connection_id, connection_room, connection_user) VALUES ('{connectionId}', '{userConnection.Room}' , '{userConnection.User}')";
            cmd.ExecuteNonQuery();

            con.Close();
            Console.WriteLine($"PostgreSQL Command: {cmd.CommandText}");
        }


        void IDisposable.Dispose() { }
        public void Dispose() { }
    }
}
