using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using KKManager;

[assembly: AssemblyTitle("KKManager")]
[assembly: AssemblyDescription("Manager for Koikatu cards, scenes and mods")]
[assembly: AssemblyProduct("KKManager")]
[assembly: AssemblyCopyright("Copyright © 2018")]

[assembly: NeutralResourcesLanguage("en")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

[assembly: Guid("04e36698-5b58-4335-b4d6-218a0fa8b57e")]

[assembly: AssemblyCompany(Constants.RepoLink)]
[assembly: AssemblyVersion(Constants.Version)]
