using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace NPOTests
{
    class ParallelCopy
    {
        private int filesCount;
        private readonly taskQueue treadPool;

        public ParallelCopy(taskQueue TreadPool)
        {
            treadPool = TreadPool;
        }
        public void Copy(string pathFrom, string pathTo)
        {
            using (treadPool)
            {
                GetFilesInFolder(pathFrom).Where(File.Exists).ToList().ForEach(src =>
                {
                    var dest = src.Replace(pathFrom, pathTo);
                    CopyOneFile(src, dest);
                });
            }
            Console.WriteLine("Copied files: " + filesCount);
        }
        private void CreateDirectoryIfNotExist(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (IOException e)
                {
                    Console.Error.Write("Directory creation error: " + e);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.Error.Write("Can't get access to file creation " + path);
                }
            }
        }
        private IEnumerable<string> GetFilesInFolder(string folder)
        {
            var files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(folder))
                    files.AddRange(GetFilesInFolder(directory));
            }
            catch (UnauthorizedAccessException)
            {
                Console.Error.Write("Can't get access to folder " + folder);
            }
            return files;
        }
        private void CopyOneFile(string src, string dest)
        {
            CreateDirectoryIfNotExist(dest);
            treadPool.EnqueueTask(() =>
            {
                try
                {
                    File.Copy(src, dest, true);
                    Interlocked.Increment(ref filesCount);
                }
                catch (IOException e)
                {
                    Console.Error.Write("File copying error: " + e);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.Error.Write("Can't get access to " + dest);
                }
            });
        }
    }
}
