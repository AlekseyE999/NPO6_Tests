using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NPOTests
{
    public class taskQueue:IDisposable
    {
        public delegate void TaskDelegate();
        private Queue<TaskDelegate> tasksQueue = new Queue<TaskDelegate>();
        private Thread[] threads;
        private bool working = true;

        public taskQueue(int threadsCount)
        {
            threads = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i] = new Thread(QueueConsumeTask) { Name = "Thread " + i };
                threads[i].Start();
                Console.WriteLine(threads[i].Name);
            }
        }
        public virtual void EnqueueTask(TaskDelegate taskDelegate)
        {
            lock (tasksQueue)
            {
                tasksQueue.Enqueue(taskDelegate);
                Monitor.Pulse(tasksQueue);
            }

        }
        public virtual void Dispose()
        {
            while (true)
            {
                Thread.Sleep(2000);
                lock (tasksQueue)
                {
                    if (tasksQueue.Count == 0) break;
                }
            }
            lock (tasksQueue)
            {
                working = false;
                Monitor.PulseAll(tasksQueue);
            }
        }
        private void QueueConsumeTask()
        {
            while (working || (tasksQueue.Count != 0))
            {
                TaskDelegate taskDelegate;
                bool isPulled;

                lock (tasksQueue)
                {
                    if (tasksQueue.Count == 0)
                    {
                        Monitor.Wait(tasksQueue);
                    }

                    isPulled = tasksQueue.TryDequeue(out taskDelegate);
                }

                if (isPulled)
                {
                    taskDelegate();
                }
            }
        }
    }
}
