using MySql.Data.MySqlClient;
using MySql.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Elements
{
    public class DB
    {
        public const string pathsql = "host=127.0.0.1;port = 3306;user=root;password=1235;database=DataShoot";


        public void InsertBD(string Command) {

            MySqlConnection con = new MySqlConnection(pathsql);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandText = (Command);
            cmd.Connection = con;
            cmd.ExecuteNonQuery();

            con.Close();

        }

        public MySqlDataReader toBD(string Command)
        {

            MySqlConnection con = new MySqlConnection(pathsql);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandText = (Command);
            cmd.Connection = con;
            cmd.ExecuteNonQuery();

            con.Close();
            return cmd.ExecuteReader();
        }

    }
}
