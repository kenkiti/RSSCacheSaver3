using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Model;

namespace Database
{

    public class Database
    {

        public void CreateDatabase(string code)
        {
            // TODO
        }

        public void Insert()
        {
            // TODO

        }


    }

    public class SQLite
    {
        private SQLiteConnection _connection;
        //private long _prevVolume = 0;
        //private long _tick = 0;

        //private Dictionary<string, string> _item = new RSS().DictionaryItem;

        /// <summary>
        /// 日毎のデータベースを作成する
        /// </summary>
        /// <param name="code"></param>
        private void OpenCreateDatabase(string code)
        {
            string path = $"{DateTime.Now.ToString("yyyyMMdd")}_RSS3_{code}.db"; ;
            System.Diagnostics.Debug.WriteLine($"{path}");

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={path}"))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS RSS(" +
                        "No INTEGER NOT NULL PRIMARY KEY," +
                        "Time TEXT NOT NULL," +
                        "Item TEXT NOT NULL," +
                        "Value REAL NOT NULL);";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Tick(" +
                        "No INTEGER NOT NULL PRIMARY KEY," +
                        "Time TEXT NOT NULL," +
                        "Tick INTEGER NOT NULL," +
                        "Kind TEXT NOT NULL," + // Yellow, Red, Green
                        "Vwap REAL NOT NULL," +
                        "Bid REAL NOT NULL," +
                        "Ask REAL NOT NULL," +
                        "Price REAL NOT NULL);";
                    cmd.ExecuteNonQuery();
                }
            }
            _connection = new SQLiteConnection($"Data Source={path}");
        }

        public void InsertValue(TickEventArgs e)
        {
            using (SQLiteCommand cmd = _connection.CreateCommand())
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                cmd.CommandText =
                                $"insert into rss(Time,Item,Value) values ('{now}','{e.Price}','{e.Volume}')";
                cmd.ExecuteNonQuery();

            }
        }

    }
}
