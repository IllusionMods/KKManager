using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;
using KKManager;

[assembly: AssemblyTitle("StandaloneUpdater")]
[assembly: AssemblyDescription("Automatic update downloader for scripting")]
[assembly: AssemblyProduct("StandaloneUpdater")]
[assembly: AssemblyCopyright("Copyright ©  2019")]

[assembly: NeutralResourcesLanguage("en")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]

[assembly: Guid("ae2af306-3a62-4efe-982b-2e448b4c113a")]

[assembly: AssemblyCompany(Constants.RepoLink)]
[assembly: AssemblyVersion(Constants.Version)]
