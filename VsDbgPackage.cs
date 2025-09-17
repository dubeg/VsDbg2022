global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;

namespace VsDbg;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.VsDbgString)]
public sealed class VsDbgPackage : ToolkitPackage {
    /// <summary>
    /// Major Visual Studio version number (eg. 16, 17, 18, ...)
    /// </summary>
    public int VsVersion { get; set; }

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
        await this.RegisterCommandsAsync();
        var version = await VS.Shell.GetVsVersionAsync();
        VsVersion = version.Major;
    }
}