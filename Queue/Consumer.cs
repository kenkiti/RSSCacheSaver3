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
        private Dictionary<string, TickEventArgs> _topic = new Dictionary<string, TickEventArgs>();

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
        public Consumer(BufferTick buffer, Database.Database db)
        {
            _buffer = buffer;
        }

        public void Consume()
        {
            Console.WriteLine("consumer runnning: ");
            
            //
            // ENABLES THE WHILE LOOP
            _running = true;
            while (_running)
            {
                //
                // CONSUME BOTTLE FROM BUFFER
                Bottle bottle = _buffer.TakeFromBuffer();
                bottle.Data = decode(bottle.RawData);

                string k = $"{bottle.Item}";
                string v = $"{bottle.Data}";

                _item[k] = v;
                TickEventArgs result;
                if (!_topic.TryGetValue(t, out result))
                {
                    _topic.Add(t, new TickEventArgs());
                    _topic[t].Topic = t;
                }

                switch (k)
                {
                    case "出来高":
                        long vol = long.Parse(v);
                        _topic[t].Volume = vol - _topic[t].VolumeP;
                        _topic[t].VolumeP = vol;
                        break;

                    case "出来高加重平均":
                        if(_topic[t].Volume > 0)
                        {
                            _topic[t].Time = bottle.Time;
                            string value = "";
                            double price = 0;
                            if (_item.TryGetValue("現在値",out value))
                            {
                                _topic[t].Price = (value != "") ? double.Parse(value) : 0;
                            }

                            if (_item.TryGetValue("最良売気配値１", out value))
                            {
                                _topic[t].Bid = (value != "") ? double.Parse(value) : price;
                            }
                            if (_item.TryGetValue("最良買気配値１", out value))
                            {
                                _topic[t].Ask = (value != "") ? double.Parse(value) : price;
                            }

                            Console.WriteLine($"{_topic[t].Time} [{_topic[t].Topic}] => {_topic[t].Price} {_topic[t].Volume}");
                            OnTick(_topic[t]);
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
