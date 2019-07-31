using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Model;

namespace Queue
{
    public class Consumer
    {
        //
        // BUFFER TO CONSUME FROM
        private Buffer _buffer;
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

                //
                // MAKE THREAD SLEEP FOR 0.5 SECONDS
                //Thread.Sleep(500);
                if (bottle.Item != null)
                {
                    bottle.Data = decode(bottle.RawData);
                    TickEventArgs e = new TickEventArgs();
                    e.Topic = $"取ってきたよ：{bottle.Item}=>{bottle.Data}";
                    //イベントの発生
                    OnTick(e);
                }
            }
        }

        public void StopConsumer()
        {
            //
            // SETS THE BOOLEAN FOR WHILE LOOP
            _running = false;
        }
    }



}
