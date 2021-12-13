using System;
using System.Collections.Generic;
using System.Text;

namespace NPOTests
{
    class AssemblyApp
    {
        static void Main(string[] args)
        {
            //var dllPath = "C:\\Downloads\\LAb4SPP\\ClassLibraryTest\\ClassLibraryTest\\bin\\Debug\\ClassLibraryTest.dll";
            var dllPath = args[0];
            var assemblyBrowser = new AssemblyBrowser(dllPath);
            var map = assemblyBrowser.GetPublicTypes();

            foreach (var (namespaceName, publicTypes) in map)
            {
                if (publicTypes.Count == 0)
                    continue;
                Console.WriteLine("NameSpace: " + namespaceName);
                publicTypes.Sort();
                foreach (var publicType in publicTypes)
                {
                    Console.WriteLine("   " + publicType);
                }
            }
        }
    }
}
