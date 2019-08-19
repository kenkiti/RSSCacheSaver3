using System;
using System.Collections.Generic;
using NDde.Client;

using Model;

namespace Queue
{
    public class Producer
    {
        //
        // BUFFER TO CONSUME FROM
        private Buffer _buffer;
        private string[] _codes;
        //
        // BOOLEAN TO MAKE THREAD KEEP ACTIVE
        private bool _running;

        private DdeClient[] _client;
 
        //
        // CONSTRUCTOR
        public Producer(Buffer buffer, string[] codes)
        {
            _buffer = buffer;
            _codes = codes;
        }

        public void Produce()
        {
            Console.WriteLine("producer runnning: ");
            _running = true;

            _client = new DdeClient[_codes.Length];
            var item = new RSSMapper().DictionaryItem;

            Console.WriteLine($"Start Advise：{_codes.Length}");

            try
            {
                for (int i = 0; i < _codes.Length; i++)
                {
                Console.WriteLine($"Start Advise：{_codes[i]}");

                // 監視開始
                _client[i] = new DdeClient("RSS", $"{_codes[i]}.T");
                    //_client[i] = new DdeClient("RSS", $"{_codes[i]}.T");
                    _client[i].Disconnected += new EventHandler<DdeDisconnectedEventArgs>(OnDisconnected);
                    _client[i].Connect();
                    foreach (string key in item.Keys)
                    {
                        _client[i].StartAdvise(key, 1, true, 60000);
                    }
                    _client[i].Advise += new EventHandler<DdeAdviseEventArgs>(OnAdvise);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"RSS起動してないのでは：{e}");
                throw;
            }
        }

        private void OnAdvise(object sender, DdeAdviseEventArgs args)
        {
            // PRODUCE NEW BOTTLE
            Bottle bottle = new Bottle();
            bottle.Topic = (sender as DdeClient).Topic;
            bottle.Time = CurrentTimeGetter.Now.ToString();
            //bottle.Time = (Int64)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            bottle.Item = args.Item;
            bottle.RawData = args.Data;
            //
            // ADD BOTTLE TO THE BUFFER
            _buffer.AddToBuffer(bottle);
        }

        public void OnDisconnected(object sender, DdeDisconnectedEventArgs args)
        {
            //
            // SETS THE BOOLEAN FOR WHILE LOOP
            _running = false;
            Console.WriteLine(
             "OnDisconnected: " +
             "IsServerInitiated=" + args.IsServerInitiated.ToString() + " " +
             "IsDisposed=" + args.IsDisposed.ToString());

           // var item = new RSSMapper().DictionaryItem;

           // 再接続を試みる
           //DdeClient c = sender as DdeClient;
           // var topic = c.Topic;
           // try
           // {
           //     監視開始
           //    c = new DdeClient("RSS", $"{topic}");
           //     c.Disconnected += new EventHandler<DdeDisconnectedEventArgs>(OnDisconnected);
           //     c.Connect();
           //     foreach (string key in item.Keys)
           //     {
           //         c.StartAdvise(key, 1, true, 60000);
           //     }
           //     c.Advise += new EventHandler<DdeAdviseEventArgs>(OnAdvise);
           // }
           // catch (Exception e)
           // {
           //     Console.WriteLine($"再接続できません：{e}");
           //     throw;
           // }


        }

    }

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

}
