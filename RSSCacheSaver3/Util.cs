using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using NDde.Client;

namespace RSSCacheSaver3
{
    internal class CurrentTimeGetter
    {
        private static int lastTicks = -1;
        private static DateTime lastDateTime = DateTime.MinValue;

        /// <summary>        
        /// Gets the current time in an optimized fashion.        
        /// </summary>        
        /// <value>Current time.</value>        

        public static DateTime Now
        {
            get
            {
                int tickCount = Environment.TickCount;
                if (tickCount == lastTicks)
                {
                    return lastDateTime;
                }
                DateTime dt = DateTime.Now;
                lastTicks = tickCount;
                lastDateTime = dt;
                return dt;
            }
        }
    }

    class Calculate
    {
//        string raw = Encoding.Default.GetString(args.Data).Trim('\0', ' ').ToString();
//            if (raw == "") return;

//            if (_item.TryGetValue(args.Item, out string value))
//            {
//                using (SQLiteCommand cmd = connection.CreateCommand())
//                {
//                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
//    cmd.CommandText =
//                        $"insert into rss(Time,Item,Value) values ('{now}','{args.Item}','{raw}')";
//                    cmd.ExecuteNonQuery();

//                    string k = $"{args.Item}";
//    string v = $"{raw}";
//    _item[k] = v;

//                    switch (k)
//                    {
//                        case "出来高":
//                            long volume = long.Parse(v);
//    tick = volume - prevVolume;
//                            prevVolume = volume;
//                            break;
//                        case "出来高加重平均":
//                            if (tick > 0)
//                            {
//                                double price = 0;
//                                if (_item["現在値"] != "")
//                                {
//                                    price = double.Parse(_item["現在値"]);
//}
//double bid = price;
//double ask = price;

//                                if (_item["最良売気配値１"] != "")
//                                {
//                                    bid = double.Parse(_item["最良売気配値１"]);
//                                }
//                                if (_item["最良買気配値１"] != "")
//                                {
//                                    ask = double.Parse(_item["最良買気配値１"]);
//                                }

//                                string kind = "";
//                                if (preBid - price<price - preAsk)
//                                {
//                                    kind = "R";
//                                }
//                                else
//                                {
//                                    kind = "G";
//                                }
//                                preBid = bid;
//                                preAsk = ask;

//                                var t = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
//cmd.CommandText = $"insert into tick (No, Time,Tick,Kind,Vwap,Bid,Ask,Price) values (" +
//                                    $"'{t}','{now}','{tick}','{kind}','{v}','{bid}','{ask}','{price}')";
//                                cmd.ExecuteNonQuery();
//                            }
//                            break;
//                    }


//                    _count += 1;
//                }
//            }

    }



    class Util
    {

        private long prevVolume = 0;
        private double preBid = 0;
        private double preAsk = 0;
        private long tick = 0;


        public Form1 f;
        private Dictionary<string, string> _item;
        private SQLiteConnection connection;

        /// <summary>
        /// 自動終了処理
        /// </summary>
        private void IsRestTime()
        {
            // 現在時を取得
            DateTime d = DateTime.Now;
            //時間を設定
            DateTime d_0900 = new DateTime(d.Year, d.Month, d.Day, 08, 50, 30); //年, 月, 日, 時間, 分, 秒
            DateTime d_1130 = new DateTime(d.Year, d.Month, d.Day, 11, 30, 30); //年, 月, 日, 時間, 分, 秒
            DateTime d_1230 = new DateTime(d.Year, d.Month, d.Day, 12, 29, 30); //年, 月, 日, 時間, 分, 秒
            DateTime d_1500 = new DateTime(d.Year, d.Month, d.Day, 15, 00, 30); //年, 月, 日, 時間, 分, 秒
            DateTime d_1505 = new DateTime(d.Year, d.Month, d.Day, 15, 05, 00); //年, 月, 日, 時間, 分, 秒

            var hoge = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


            //現在の時間が設定の時間になった時の処理
            if ((d_1230 >= d & d >= d_1130) | (d_0900 >= d | d >= d_1500))
            {
                //背景を黒くする
                f.BackColor = Color.Blue;
            }
            else
            {
                f.BackColor = SystemColors.Control;
            }
            if (d >= d_1505) { Application.Exit(); }
        }

        /// <summary>
        /// 日毎のデータベースを作成する
        /// </summary>
        /// <param name="code"></param>
        private void CreateDatabase(string code)
        {
            string path = $"{DateTime.Now.ToString("yyyyMMdd")}_RSS_{code}.db"; ;
            connection = new SQLiteConnection("Data Source=" + path);
            System.Diagnostics.Debug.WriteLine($"{path}");

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + path))
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
        }

        //    /// <summary>
        //    /// 監視開始関数
        //    /// </summary>
        //    private void Start()
        //    {
        //        _item = new Dictionary<string, string>()
        //        {
        //            { "最良売気配値１", ""},
        //            { "最良買気配値１", ""},
        //            { "最良買気配数量１", ""},
        //            { "最良売気配数量１", ""},
        //            { "現在値", ""},
        //            { "出来高", ""},
        //            { "出来高加重平均", ""},
        //        };

        //        // DB作成
        //        CreateDatabase(txtCode.Text);

        //        // 監視開始
        //        client = new DdeClient("RSS", $"{txtCode.Text}.T", this);
        //        client.Disconnected += new EventHandler<DdeDisconnectedEventArgs>(OnDisconnected);
        //        client.Connect();
        //        foreach (string key in _item.Keys)
        //        {
        //            client.StartAdvise(key, 1, true, 60000);
        //            _count += 1;
        //        }
        //        client.Advise += new EventHandler<DdeAdviseEventArgs>(OnAdvise);
        //        connection.Open();
        //    }

    }
}
