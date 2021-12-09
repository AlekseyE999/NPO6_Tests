using NUnit.Framework;
using Moq;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace NPOTests
{
    public class Tests
    {
        private string src = "C:\\Downloads\\images";
        private string dest = "C:\\Downloads\\imagesTest";
        private string dest2 = "C:\\Downloads\\imagesNew";
        private int testCount = 5;
        private int threadCount = 5;
        private DynamicArray<string> array;
        [SetUp]
        public void Setup()
        {
            array = new DynamicArray<string>();
        }

        [Test]
        public void TaskQueue_Exactly5Times()
        {

            var mock = new Mock<taskQueue.TaskDelegate>();
            var threadPool = new taskQueue(3);
            for (var i = 0; i < testCount; i++)
            {
                threadPool.EnqueueTask(mock.Object);
            }
            Thread.Sleep(1000);
            threadPool.Dispose();
            mock.Verify(f => f(), Times.Exactly(testCount));
        }
        [Test]
        public void TaskQueue_Never()
        {
            int count = 0;
            var threadPool = new taskQueue(count);
            threadPool.EnqueueTask(() => count++);
            Thread.Sleep(1000);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ParallelCopy_CopyexpectedCountOfFiles()
        {
            var directoryInfo = new DirectoryInfo(src);
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories).Length;
            var threadPool = new taskQueue(3);
            var parallelCopy = new ParallelCopy(threadPool);
            parallelCopy.Copy(src, dest);
            Assert.AreEqual(files, 6);
        }

        [Test]
        public void ParallelCopy_CopyFilesToNewDirectoty()
        {

            var directoryInfo = new DirectoryInfo(src);
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories).Length;
            var parallelCopy = new ParallelCopy(new taskQueue(threadCount));
            parallelCopy.Copy(src, dest2);
            var newDirectoryInfo = new DirectoryInfo(dest2);
            var newFiles = newDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(files, newFiles);
            Assert.AreEqual("imagesNew", newDirectoryInfo.Name);
        }

        [Test]
        public void Mutex_SameThreadInSameTime()
        {
            var mutex = new Mutex();
            var counter = 0;
            var arr = new bool[threadCount];
            var threads = new Thread[threadCount];
            var isInvalidValue = false;

            for (var i = 0; i < threadCount; i++)
            {
                var currIndex = i;
                threads[i] = new Thread(o =>
                {
                    mutex.Lock();
                    counter++;
                    Thread.Sleep(50);
                    if (!isInvalidValue && counter != 1)
                    {
                        isInvalidValue = true;
                    }
                    Thread.Sleep(50);
                    counter--;
                    arr[currIndex] = true;
                    mutex.Unlock();
                })
                { Name = "Thread " + i };
                threads[i].Start();
            }
            for (var i = 0; i < threadCount; i++)
                threads[i].Join();

            for (var i = 0; i < threadCount; i++)
            {
                Assert.That(arr[i], Is.EqualTo(true));
            }
            Assert.That(isInvalidValue, Is.EqualTo(false));
        }

        [Test]
        public void Mutex_TryToUnlockAlienMutex()
        {
            var mutex = new Mutex();
            var counter = 0;

            var thread1 = new Thread(o =>
            {
                mutex.Lock();
                counter++;
                Thread.Sleep(400);
                counter--;
                mutex.Unlock();
            })
            { Name = "Thread 1" };

            thread1.Start();

            Thread.Sleep(100);
            mutex.Unlock();
            mutex.Lock();
            Assert.That(counter, Is.EqualTo(0));
            mutex.Unlock();
        }

        [Test]
        public void DynamicArray_AddMethod()
        {
            array.Add("Aleksey");
            array.Add("Ivan");

            Assert.That(array.Count, Is.EqualTo(2));
            Assert.That(array.Items[0], Is.EqualTo("Aleksey"));
        }

        [Test]
        public void DynamicArray_RemoveAtMethod()
        {
            array.Add("Aleksey");
            array.Add("Ivan");
            array.Add("Anna");
            array.Add("Dasha");
            array.RemoveAt(2);

            Assert.That(array.Count, Is.EqualTo(3));
            Assert.That(array.Items, Is.EqualTo(new string[] { "Aleksey", "Ivan", "Dasha" }));
        }

        [Test]
        public void DynamicArray_RemoveMethod()
        {
            array.Add("Aleksey");
            array.Add("Ivan");
            array.Add("Anna");
            array.Add("Dasha");

            Assert.That(array.Remove("Ivan"), Is.True);

            Assert.That(array.Count, Is.EqualTo(3));
            Assert.That(array.Items, Is.EqualTo(new string[] { "Aleksey", "Anna", "Dasha" }));

            Assert.That(array.Remove("Ivan"), Is.False);
        }

        [Test]
        public void DynamicArray_ClearMethod()
        {
            array.Add("Aleksey");
            array.Add("Ivan");
            array.Add("Anna");
            array.Add("Dasha");
            array.Clear();

            Assert.That(array.Count, Is.EqualTo(0));
            Assert.That(array.Items, Is.EqualTo(new string[] { }));

            array.Add("Anna");

            Assert.That(array.Count, Is.EqualTo(1));
            Assert.That(array.Items[0], Is.EqualTo("Anna"));
        }
        [Test]
        public void Dynamic_ExtendingArray()
        {
            array = new DynamicArray<string>(8);
            for (int i = 0; i < 400; i++)
            {
                array.Add("Aleksey");
            }
            Assert.That(array.Count, Is.EqualTo(400));
            Assert.That(array.Items.Length, Is.EqualTo(400));
        }
    }
}