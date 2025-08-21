using EnvDTE;
using Microsoft.VisualStudio.Settings;
using VsDbg.Commands;

namespace VsDbg; 

[Command(PackageIds.ToggleJSDebugging)]
internal sealed class ToggleJSDebugging : BaseCommand<ToggleJSDebugging> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await SettingsUtils.ToggleWithStatusAsync(
            Package,
            SettingsScope.UserSettings,
            "Debugger",
            "EnableAspNetJavaScriptDebuggingOnLaunch",
            "JavaScript Debugging"
        );
    }

    //protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
    //    try {
    //        await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
    //        var debugger7 = await VS.GetServiceAsync<SVsShellDebugger, IVsDebugger7>();
    //        var debugger9 = await VS.GetServiceAsync<SVsShellDebugger, IVsDebugger9>();
    //        debugger7.IsJavaScriptDebuggingOnLaunchEnabled(out var enabled);
    //        enabled = !enabled;
    //        debugger9.SetEnableJavaScriptDebuggerOnBrowserLaunch(enabled ? 1 : 0);
    //        await VS.StatusBar.ShowMessageAsync($"JavaScript debugging: {(enabled ? "enabled" : "disabled")}");
    //        await Task.Delay(1000);
    //        await VS.StatusBar.ShowMessageAsync(null);
    //    }
    //    catch (Exception ex) {
    //        await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
    //    }
    //}



    ///// <summary>
    ///// Unused
    ///// </summary>
    //protected async Task ListePropertiesAsync(OleMenuCmdEventArgs e) {
    //    try {
    //        await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
    //        var dte = await VS.GetServiceAsync<DTE, DTE2>();
    //        var output = await VS.Windows.CreateOutputWindowPaneAsync("All Options Categories");
    //        await output.ClearAsync();
    //        await output.WriteLineAsync("=== All Available Options Categories ===");
    //        await output.WriteLineAsync();
    //        // Common debugging-related categories to check
    //            string[] categoriesToCheck = {
    //            "Debugger",
    //            "Debugging",
    //            "Environment",
    //            "TextEditor"
    //        };
    //        foreach (string category in categoriesToCheck) {
    //            try {
    //                // Try different page combinations
    //                string[] pagesToCheck = { "General", "Edit and Continue", "Just-In-Time", "Native", "Symbols" };
    //                foreach (string page in pagesToCheck) {
    //                    try {
    //                        var properties = dte.Properties[category, page];
    //                        if (properties.Count > 0) {
    //                            await output.WriteLineAsync($"✓ Found: {category} > {page} ({properties.Count} properties)");
    //                            // List first few properties as preview
    //                            int count = 0;
    //                            foreach (EnvDTE.Property prop in properties) {
    //                                if (count++ >= 3) break;
    //                                await output.WriteLineAsync($"    - {prop.Name}");
    //                            }
    //                            if (properties.Count > 3)
    //                                await output.WriteLineAsync($"    ... and {properties.Count - 3} more");
    //                            await output.WriteLineAsync();
    //                        }
    //                    }
    //                    catch {
    //                        // Page doesn't exist, continue
    //                    }
    //                }
    //            }
    //            catch {
    //                // Category doesn't exist, continue  
    //            }
    //        }
    //        await output.ActivateAsync();
    //        await VS.MessageBox.ShowWarningAsync("VsToggleJsDebugging", "Button clicked");
    //    }
    //    catch (Exception ex) {
    //        await VS.MessageBox.ShowErrorAsync("Error", $"Failed to enumerate options: {ex.Message}");
    //    }
    //}
}
