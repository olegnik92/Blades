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
            return loadedAssemblies.Value;
        }

        private static Lazy<List<Assembly>> loadedAssemblies = new Lazy<List<Assembly>>(() =>
        {
            //В .net framework DependencyContext.Default возвращает null
#if NET461
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
#endif

            return GetAssembliesFromDependencyContext();
        });

        private static List<Assembly> GetAssembliesFromDependencyContext()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
                catch
                {
                    //Есди не смогли загрузить сборку, то просто пропускаем эту ошибку
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
