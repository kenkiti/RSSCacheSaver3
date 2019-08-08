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
                //
                // CHECKS IF THE BUFFER IS FULL
                if (_queue.Count == _size)
                {
                    // RELEASE THE LOCK AND WAIT TO GET IT BACK 
                    Monitor.Wait(_lock);
                }
                //
                // ADD OBJECT TO BUFFER
                _queue.Add(bottle);
                WriteLine($"Bottle Added:{_queue.Count} {bottle.Topic} => {bottle.Item}");
            }
        }

        public Bottle TakeFromBuffer()
        {
            Bottle bottle = new Bottle();
            //
            // LOCK THE RESOURCES / ISOLATING THE THREADS
            lock (_lock)
            {
                // CHECKS IF THERE ARE ANYTHING IN THE BUFFER
                if (_queue.Count == 0)
                {
                    //
                    // WAKES ALL THREADS THERE ARE WAITING FOR THE LOCK
                    Monitor.PulseAll(_lock);
                }

                //
                // TAKES AN OBJECT FROM THE BUFFER
                if (_queue.Count > 0)
                {
                    bottle = _queue[0];
                    _queue.RemoveAt(0);
                    WriteLine($"Bottle Taken: {bottle.Topic}  / {_queue.Count}");
                }
            }
            return bottle;
        }

    }

    public class BufferTick
    {
        //
        // OBJECT TO LOCK RESOURCES
        private object _lock = new object();
        //
        // A QUEUE TO HOLD DATA
        private List<TickBottle> _queue;
        //
        // THE SIZE OF THE BUFFER
        int _size;

        //
        // CONSTRUCTOR
        public BufferTick(int size)
        {
            _queue = new List<TickBottle>(size);
            _size = size;
        }

        public void AddToBuffer(TickBottle tick)
        {
            //
            // LOCK THE RESOURCES / ISOLATING THE THREADS
            lock (_lock)
            {
                //
                // CHECKS IF THE BUFFER IS FULL
                if (_queue.Count == _size)
                {
                    // RELEASE THE LOCK AND WAIT TO GET IT BACK 
                    Monitor.Wait(_lock);
                }
                //
                // ADD OBJECT TO BUFFER
                _queue.Add(tick);
                WriteLine($"Bottle Added:{_queue.Count} {tick.Topic} => {tick.Price}");
            }
        }

        public TickBottle TakeFromBuffer()
        {
            TickBottle tick = new TickBottle();
            //
            // LOCK THE RESOURCES / ISOLATING THE THREADS
            lock (_lock)
            {
                // CHECKS IF THERE ARE ANYTHING IN THE BUFFER
                if (_queue.Count == 0)
                {
                    //
                    // WAKES ALL THREADS THERE ARE WAITING FOR THE LOCK
                    Monitor.PulseAll(_lock);
                }

                //
                // TAKES AN OBJECT FROM THE BUFFER
                if (_queue.Count > 0)
                {
                    tick = _queue[0];
                    _queue.RemoveAt(0);
                    WriteLine($"Bottle Taken: {tick.Topic}  / {_queue.Count}");
                }
            }
            return tick;
        }

    }


}
