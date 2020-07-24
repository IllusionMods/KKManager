using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;
using KKManager;

[assembly: AssemblyTitle("KKManager.SB3UGS")]
[assembly: AssemblyDescription("Manager for Koikatu cards, scenes and mods")]
[assembly: AssemblyProduct("KKManager.SB3UGS")]
[assembly: AssemblyCopyright("Copyright ©  2019")]

[assembly: NeutralResourcesLanguage("en")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

[assembly: Guid("b8b55e55-d95b-4e6a-9f33-c3737ad4487f")]

[assembly: AssemblyCompany(Constants.RepoLink)]
[assembly: AssemblyVersion(Constants.Version)]
