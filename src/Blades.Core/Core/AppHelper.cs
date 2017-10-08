using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;

namespace Blades.Core
{
    public static class AppHelper
    {
        public static IEnumerable<Type> GetAllTypes()
        {
            return GetAssemblies().SelectMany(a => a.GetTypes());
        }

        public static List<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateCompilationLibrary(library))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
        {
            return compilationLibrary.Name == ("Specify")
                || compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("Specify"));
        }
    }
}
