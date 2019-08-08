using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public struct Bottle
    {
        public string Topic { get; set; }
        public Int64 Time { get; set; }
        public string Item { get; set; }
        public Byte[] RawData { get; set; }
        public string Data { get; set; }
    }

    public struct TickBottle
    {
        public string Topic;
        public Int64 Time;
        public double Price;
        public long Volume;
        public string Kind;
        public double Vwap;
        public double Bid;
        public double Ask;
    }

    public class TickEventArgs : EventArgs
    {
        public string Topic;
        public Int64 Time;
        public double Price;
        public long Volume;
        public string Kind;
        public double Vwap;
        public double Bid;
        public double Ask;
    }

    public class RSSMapper
    {
        public Dictionary<string, string> DictionaryItem = new Dictionary<string, string>()
            {
                { "最良売気配値１", ""},
                { "最良買気配値１", ""},
                { "最良買気配数量１", ""},
                { "最良売気配数量１", ""},
                { "現在値", ""},
                { "出来高", ""},
                { "出来高加重平均", ""},
            };

        public TickEventArgs InitializeTick(TickEventArgs e)
        {
            e.Topic = "";
            e.Time = 0;
            e.Price = 0;
            e.Volume = 0;
            e.Kind = "";
            e.Vwap = 0;
            e.Bid = 0;
            e.Ask = 0;
            return e;
        }

    }
}
