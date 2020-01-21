using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using KKManager;

[assembly: AssemblyTitle("KKManager.Core")]
[assembly: AssemblyDescription("Manager for Koikatu cards, scenes and mods")]
[assembly: AssemblyProduct("KKManager.Core")]
[assembly: AssemblyCopyright("Copyright ©  2019")]

[assembly: NeutralResourcesLanguage("en")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

[assembly: Guid("09fc4ef5-9694-4961-a136-dcfd8b5f1368")]

[assembly: AssemblyCompany(Constants.RepoLink)]
[assembly: AssemblyVersion(Constants.Version)]
