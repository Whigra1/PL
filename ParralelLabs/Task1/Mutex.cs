using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ParralelLabs.Task1
{
    public class Mutex
    {
        private Thread _currentThread = Thread.CurrentThread;
        private BlockingCollection<Thread> _waitingThreads;

        private void Acquire()  {
            
            if (Thread.CurrentThread == _currentThread)
                throw new Exception("You can't lock mutex 2 or more times");

            while (Interlocked.CompareExchange(ref _currentThread, Thread.CurrentThread,null) == null)
                Thread.Yield();
            
            Console.WriteLine("Mutex took by: " + Thread.CurrentThread.Name);
        }

        private void Release() {
            if (Thread.CurrentThread == _currentThread) 
                throw new Exception("You can't call unlock when you don't have lock");

            Console.WriteLine("Mutex unlocked by: " + Thread.CurrentThread.Name);
            _currentThread = null;
        }
        
        public void Wait() {
            Thread thread = Thread.CurrentThread;
            if (thread != _currentThread) {
                throw new Exception("You should lock mutex before use of wait method");
            }

            _waitingThreads.Add(thread);
            Console.WriteLine("Waiting: " + Thread.CurrentThread.Name);
            Release();

            while (_waitingThreads.Contains(thread)) Thread.Yield();

            Acquire();
            Console.WriteLine("No waiting any more: " + Thread.CurrentThread.Name);
        }
        
        public void Notify() {
            _waitingThreads.Take();
            Console.WriteLine("Notify: " + Thread.CurrentThread.Name);
        }

        public void NotifyAll() {
            _waitingThreads = new BlockingCollection<Thread>();
            Console.WriteLine("Notify all: " + Thread.CurrentThread.Name);
        }
    }
}