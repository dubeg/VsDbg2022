using System.Threading;
using EnvDTE;
using Microsoft.Internal.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Settings;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleJustMyCode)]
internal sealed class ToggleJustMyCode : BaseCommand<ToggleJustMyCode> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        var displayName = "Just My Code";
        var package = Package as VsDbgPackage;
        if (package.VsVersion >= 18) {
            await SettingsUtils.ToggleUnifiedSettingWithStatusAsync(
                Package,
                "debugging.general.justMyCode",
                displayName
            );
        }
        else if (package.VsVersion == 17) {
            await SettingsUtils.ToggleWithStatusAsync(
                Package,
                SettingsScope.UserSettings,
                "Debugger",
                "JustMyCode",
                displayName
            );
        }
    }
}