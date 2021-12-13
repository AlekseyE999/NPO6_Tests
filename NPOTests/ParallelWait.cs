using System;
using System.Collections.Generic;
using System.Threading;

namespace NPOTests
{
    class ParallelWait
    {
        private int threadsCount = 3;
        public void WaitAll(taskQueue.TaskDelegate[] tasks)
        {
            var threadPool = new taskQueue(threadsCount);
            List<AutoResetEvent> waitHandles = new List<AutoResetEvent>();
            foreach (var task in tasks)
            {
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                threadPool.EnqueueTask(() =>
                {
                    task();
                    waitHandle.Set();
                });
                waitHandles.Add(waitHandle);
            }
            foreach (var waitHandle in waitHandles)
            {
                waitHandle.WaitOne();
            }
            threadPool.Dispose();
        }
        public ParallelWait()
        {

        }
    }
}
