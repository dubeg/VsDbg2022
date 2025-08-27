using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleXamlHotReload)]
internal sealed class ToggleXamlHotReload : BaseCommand<ToggleXamlHotReload> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        try {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            var debugger = await Package.GetServiceAsync<SVsShellDebugger, IDebuggerInternal>();
            debugger.GetDebuggerOption(DEBUGGER_OPTIONS.Option_EnableXamlVisualDiagnostics, out var iEnabled);
            iEnabled = (uint)(iEnabled == 1 ? 0 : 1);
            debugger.SetDebuggerOption(DEBUGGER_OPTIONS.Option_EnableXamlVisualDiagnostics, iEnabled);
            await Package.SetStatusThenClearAsync($"XAML Hot Reload: {(iEnabled == 1 ? "enabled" : "disabled")}");
            // -------------------------------------------------------------------
            // Using the code below doesn't take effect until Visual Studio restarts,
            // so I went ahead and use the code above.
            // Strangely enough, the code above works OK, but the Tools -> Options doesn't reflect the change 
            // until the second showing.
            //var enabled = Convert.ToBoolean(iEnabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "Debugger", "EnableXamlVisualDiagnostics", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "Debugger", "XamlVisualDiagnosticsIsUwpEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "Debugger", "XamlVisualDiagnosticsIsWinUIEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "Debugger", "XamlVisualDiagnosticsIsWpfEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "XamarinHotReloadVSIX", "FormsHotReloadEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "XamarinHotReloadVSIX", "MAUIHotReloadEnabled", enabled);
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
        }
    }
}