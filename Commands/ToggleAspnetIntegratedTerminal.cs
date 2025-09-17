using System.Threading;
using EnvDTE;
using Microsoft.Internal.VisualStudio.Shell.Interop;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleAspnetIntegratedTerminal)]
internal sealed class ToggleAspnetIntegratedTerminal : BaseCommand<ToggleAspnetIntegratedTerminal> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        var displayName = "AspNet: Use Integrated Terminal";
        var package = Package as VsDbgPackage;
        if (package.VsVersion < 18) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Toggling {displayName} isn't implemented (yet) in VS {package.VsVersion}");
            return;
        }
        await SettingsUtils.ToggleUnifiedSettingWithStatusAsync(
            Package,
            "projectsAndSolutions.aspNetCore.outputOptions.useIntegratedTerminal",
            displayName
        );
    }
}