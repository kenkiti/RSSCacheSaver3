using System;
using System.Collections.Generic;
using System.Text;

using Model;

namespace Queue
{
    public class Consumer
    {
        //
        // BUFFER TO CONSUME FROM
        private Buffer _buffer;
        private TickEventArgs[] _ticks = { new TickEventArgs(), new TickEventArgs()};
        private Dictionary<string, string> _item = new RSSMapper().DictionaryItem;


        //
        // BOOLEAN TO MAKE THREAD KEEP ACTIVE
        private bool _running;

        //デリゲートの宣言
        //TimeEventArgs型のオブジェクトを返すようにする
        public delegate void TickEventHandler(object sender, TickEventArgs e);
        public event TickEventHandler Tick;

        protected virtual void OnTick(TickEventArgs e)
        {
            if (Tick != null)
            {
                Tick(this, e);
            }
        }

        //
        // CONSTRUCTOR
        public Consumer(Buffer buffer, Database.Database db)
        {
            _buffer = buffer;
        }

        public void Consume()
        {
            Console.WriteLine("consumer runnning: ");
            string decode(byte[] d) => Encoding.Default.GetString(d).Trim('\0', ' ').ToString();
            

            //
            // ENABLES THE WHILE LOOP
            //_running = true;
            while (true)
            {
                //
                // CONSUME BOTTLE FROM BUFFER
                Bottle bottle = _buffer.TakeFromBuffer();
                bottle.Data = decode(bottle.RawData);

                string k = $"{bottle.Item}";
                string v = $"{bottle.Data}";

                _item[k] = v;

                switch(k)
                {
                    case "出来高":
                        long vol = long.Parse(v);
                        _ticks[0].Volume = vol - _ticks[1].Volume;
                        _ticks[1].Volume = vol;
                        break;

                    case "出来高加重平均":
                        if(_ticks[0].Volume > 0)
                        {
                            string value = "";
                            double price = 0;
                            if (_item.TryGetValue("現在値",out value))
                            {
                                _ticks[0].Price = long.Parse(value);
                            }
                            double bid = price;
                            double ask = price;

                            if (_item.TryGetValue("最良売気配値１", out value))
                            {
                                _ticks[0].Bid = double.Parse(value);
                            }
                            if (_item.TryGetValue("最良買気配値１", out value))
                            {
                                ask = double.Parse(value);
                            }

                        }
                        break;
                }
            }
        }

        public void CaluculateTick(Bottle bottle)
        {
            //string raw = Encoding.Default.GetString(args.Data).Trim('\0', ' ').ToString();
            TickEventArgs e = new TickEventArgs();
            e.Topic = $"取ってきたよ：{bottle.Item}=>{bottle.Data}";
            //イベントの発生
            OnTick(e);

        }

        public void StopConsumer()
        {
            //
            // SETS THE BOOLEAN FOR WHILE LOOP
            _running = false;
        }
    }



}
