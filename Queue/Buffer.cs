using System;
using System.Collections.Generic;
using System.Threading;
using static System.Console;

using Model;

namespace Queue
{
    public class Buffer
    {
        //
        // OBJECT TO LOCK RESOURCES
        private object _lock = new object();
        //
        // A QUEUE TO HOLD DATA
        private List<Bottle> _queue;
        //
        // THE SIZE OF THE BUFFER
        int _size;

        //
        // CONSTRUCTOR
        public Buffer(int size)
        {
            _queue = new List<Bottle>(size);
            _size = size;
        }

        public void AddToBuffer(Bottle bottle)
        {
            //
            // LOCK THE RESOURCES / ISOLATING THE THREADS
            lock (_lock)
            {
                while (_queue.Count >= _size)
                    Monitor.Wait(_lock);

                // ADD OBJECT TO BUFFER
                _queue.Add(bottle);
                Monitor.PulseAll(_lock);
            }
        }

        public Bottle TakeFromBuffer()
        {
            //
            // LOCK THE RESOURCES / ISOLATING THE THREADS
            lock (_lock)
            {
                Bottle bottle = new Bottle();

                while (_queue.Count <= 0)
                    Monitor.Wait(_lock);

                bottle = _queue[0];
                _queue.RemoveAt(0);

                //
                // WAKES ALL THREADS THERE ARE WAITING FOR THE LOCK
                Monitor.PulseAll(_lock);
                WriteLine($"{bottle.Time}: Bottle Taken: {bottle.Topic} {bottle.Item} /{_queue.Count}");
                return bottle;
            }
        }
    }
}
