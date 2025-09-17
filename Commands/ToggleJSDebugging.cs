using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using VsDbg.Commands;

namespace VsDbg; 

[Command(PackageIds.ToggleJSDebugging)]
internal sealed class ToggleJSDebugging : BaseCommand<ToggleJSDebugging> {
    //protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
    //    await Utils.ToggleWithStatusAsync(
    //        Package,
    //        SettingsScope.UserSettings,
    //        "Debugger",
    //        "EnableAspNetJavaScriptDebuggingOnLaunch",
    //        "JavaScript Debugging"
    //    );
    //}

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        try {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            var debugger7 = await VS.GetServiceAsync<SVsShellDebugger, IVsDebugger7>();
            var debugger9 = await VS.GetServiceAsync<SVsShellDebugger, IVsDebugger9>();
            debugger7.IsJavaScriptDebuggingOnLaunchEnabled(out var enabled);
            enabled = !enabled;
            debugger9.SetEnableJavaScriptDebuggerOnBrowserLaunch(enabled ? 1 : 0);
            SettingsUtils.ShowSettingStatusThenClear("JavaScript debugging", enabled);
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
        }
    }
}
