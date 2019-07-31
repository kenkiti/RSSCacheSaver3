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

    public class TickEventArgs : EventArgs
    {
        public string Topic;
        public string Time;
        public string Price;
        public string Volume;
        public string Kind;
        public string Vwap;
        public string Bid;
        public string Ask;
    }

    public class RSS
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


        public void CaluculateTick(Bottle args)
        {
            //string raw = Encoding.Default.GetString(args.Data).Trim('\0', ' ').ToString();

        }
    }
}
