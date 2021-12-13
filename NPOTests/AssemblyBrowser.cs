using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NPOTests
{
    class AssemblyBrowser
    {
        private readonly Assembly dll;

        public AssemblyBrowser(string dllPath)
        {
            try
            {
                this.dll = Assembly.LoadFile(dllPath);
            }
            catch
            {
                throw new ArgumentException("Unable to load dll: " + dllPath);
            }
        }
        private static void InitializeNameSpace(IDictionary<string, List<string>> dictionary, string namespaceName)
        {
            if (!dictionary.ContainsKey(namespaceName))
            {
                dictionary.Add(namespaceName, new List<string>());
            }
        }

        public IDictionary<string, List<string>> GetPublicTypes()
        {
            var typesMap = new SortedDictionary<string, List<string>>();
            foreach (var type in dll.GetExportedTypes())
            {
                InitializeNameSpace(typesMap, type.Namespace);
                var list = typesMap[type.Namespace];
                if (type.IsPublic)
                {
                    list.Add(type.Name);
                }
            }
            return typesMap;
        }
    }
}
}
