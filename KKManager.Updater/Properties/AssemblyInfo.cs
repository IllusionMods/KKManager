using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;
using KKManager;

[assembly: AssemblyTitle("KKManager.Updater")]
[assembly: AssemblyDescription("Manager for Koikatu cards, scenes and mods")]
[assembly: AssemblyProduct("KKManager.Updater")]
[assembly: AssemblyCopyright("Copyright ©  2019")]

[assembly: NeutralResourcesLanguage("en")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

[assembly: Guid("b64c974d-9687-4e69-aeea-3f04f769e556")]

[assembly: AssemblyCompany(Constants.RepoLink)]
[assembly: AssemblyVersion(Constants.Version)]
