using System;
using System.Collections.Generic;
using System.Text;

using Model;

namespace Queue
{
    class Bridge
    {
        //
        // BUFFER TO CONSUME FROM
        private Buffer _buffer;
        private BufferTick _bufferTicks;
        private Dictionary<string, string> _item = new RSSMapper().DictionaryItem;

        private ulong _preVolume = 0;
        private long _tick = 0;

        //
        // BOOLEAN TO MAKE THREAD KEEP ACTIVE
        private bool _running;


        //
        // CONSTRUCTOR
        public Bridge(Buffer buffer, BufferTick ticks)
        {
            _buffer = buffer;
            _bufferTicks = ticks;
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

                switch (k)
                {
                    case "出来高":
                        ulong volume = ulong.Parse(v);
                        _tick = (long)(volume - _preVolume);
                        _preVolume = volume;
                        break;

                    case "出来高加重平均":
                        if (_tick > 0)
                        {
                            // PRODUCE NEW TICK
                            TickBottle tb = new TickBottle();

                            string value = "";
                            if (_item.TryGetValue("現在値", out value))
                            {
                                tb.Price = long.Parse(value);
                            }
                            if (_item.TryGetValue("最良売気配値１", out value))
                            {
                                tb.Bid = double.Parse(value);
                            }
                            if (_item.TryGetValue("最良買気配値１", out value))
                            {
                                tb.Ask = double.Parse(value);
                            }

                            //
                            // ADD TICK TO THE BUFFER
                            _bufferTicks.AddToBuffer(tb);
                        }
                        break;
                }
            }
        }

    }
}
