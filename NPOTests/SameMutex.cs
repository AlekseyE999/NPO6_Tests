using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NPOTests
{
    class Mutex
    {
        private int mutexVariable;
        private int free = 0;
        private int BusyByCurrentTread
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }
        public Mutex()
        {

        }
        public void Lock()
        {
            while (Interlocked.CompareExchange(ref mutexVariable, BusyByCurrentTread, free) != free)
            {
                Thread.Sleep(100);
            }
        }
        public void Unlock()
        {
            if (Interlocked.CompareExchange(ref mutexVariable, free, BusyByCurrentTread) == BusyByCurrentTread)
            {
                Console.WriteLine("Thread " + BusyByCurrentTread + " complete");
            }
        }
    }
}
