using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AssemblyLoader
{
    private readonly Dictionary<string, List<TypeDefinition>> _loadedTypesByAssemblyName = new Dictionary<string, List<TypeDefinition>>();

    public void LoadAssemblies(string[] assemblyPaths)
    {
        foreach (string assemblyPath in assemblyPaths)
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

            if (!_loadedTypesByAssemblyName.ContainsKey(assembly.Name.Name))
            {
                _loadedTypesByAssemblyName.Add(assembly.Name.Name, new List<TypeDefinition>());
            }

            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                _loadedTypesByAssemblyName[assembly.Name.Name].Add(type);
            }
        }
    }

    public TypeDefinition GetTypeDefinition(string assemblyName, string typeName)
    {
        if (!_loadedTypesByAssemblyName.ContainsKey(assemblyName))
        {
            return null;
        }

        foreach (TypeDefinition type in _loadedTypesByAssemblyName[assemblyName])
        {
            if (type.Name == typeName)
            {
                return type;
            }
        }

        return null;
    }

    public IEnumerable<TypeDefinition> GetTypesByAssemblyName(string assemblyName)
    {
        if (!_loadedTypesByAssemblyName.ContainsKey(assemblyName))
        {
            return Enumerable.Empty<TypeDefinition>();
        }

        return _loadedTypesByAssemblyName[assemblyName];
    }
}
