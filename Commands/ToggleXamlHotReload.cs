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
            //var enabled = await SettingsUtils.ToggleAsync(Package, SettingsScope.UserSettings, "Debugger", "EnableXamlVisualDiagnostics");
            //await SettingsUtils.ToggleAsync(Package, SettingsScope.UserSettings, "Debugger", "XamlVisualDiagnosticsIsUwpEnabled");
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "Debugger", "XamlVisualDiagnosticsIsWinUIEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "Debugger", "XamlVisualDiagnosticsIsWpfEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "XamarinHotReloadVSIX", "FormsHotReloadEnabled", enabled);
            //await SettingsUtils.SetBoolAsync(Package, SettingsScope.UserSettings, "XamarinHotReloadVSIX", "MAUIHotReloadEnabled", enabled);
            
            var debugger = await Package.GetServiceAsync<SVsShellDebugger, IDebuggerInternal>();
            debugger.GetDebuggerOption(DEBUGGER_OPTIONS.Option_EnableXamlVisualDiagnostics, out var iEnabled);
            iEnabled = (uint)(iEnabled == 1 ? 0 : 1);
            debugger.SetDebuggerOption(DEBUGGER_OPTIONS.Option_EnableXamlVisualDiagnostics, iEnabled);

            // This won't take be shown in the options page until the second showing.
            // --
            await VS.StatusBar.ShowMessageAsync($"XAML Hot Reload: {iEnabled}");
            await Task.Delay(1000);
            await VS.StatusBar.ShowMessageAsync(null);
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
        }
    }
}