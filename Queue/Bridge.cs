using System;
using System.Collections.Generic;
using System.Text;

using Model;

namespace Queue
{
    //public class Bridge
    //{
    //    //
    //    // BUFFER TO CONSUME FROM
    //    private Buffer _buffer;
    //    private BufferTick _bufferTicks;
    //    private Dictionary<string, string> _item = new RSSMapper().DictionaryItem;

    //    private ulong _preVolume = 0;
    //    private long _tick = 0;

    //    //
    //    // BOOLEAN TO MAKE THREAD KEEP ACTIVE
    //    private bool _running;

    //    //
    //    // CONSTRUCTOR
    //    public Bridge(Buffer buffer, BufferTick ticks)
    //    {
    //        _buffer = buffer;
    //        _bufferTicks = ticks;
    //    }

    //    public void Consume()
    //    {
    //        Console.WriteLine("consumer runnning: ");

    //        //
    //        // ENABLES THE WHILE LOOP
    //        _running = true;
    //        while (true)
    //        {
    //            //
    //            // CONSUME BOTTLE FROM BUFFER
    //            Bottle bottle = _buffer.TakeFromBuffer();
    //            bottle.Data = Encoding.Default.GetString(bottle.RawData).Trim('\0', ' ').ToString();

    //            string k = $"{bottle.Item}";
    //            string v = $"{bottle.Data}";

    //            _item[k] = v;

    //        }
    //    }
    //}
}
