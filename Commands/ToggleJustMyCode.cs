using EnvDTE;
using Microsoft.VisualStudio.Settings;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleJustMyCode)]
internal sealed class ToggleJustMyCode : BaseCommand<ToggleJustMyCode> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await Utils.ToggleWithStatusAsync(
            Package,
            SettingsScope.UserSettings,
            "Debugger",
            "JustMyCode",
            "Just My Code"
        );
    }
}