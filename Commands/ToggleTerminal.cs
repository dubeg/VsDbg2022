using Microsoft.VisualStudio.Shell.Interop;

namespace VsDbg.Commands;

/// <summary>
/// Toggle terminal window.
/// </summary>
[Command(PackageIds.ToggleTerminal)]
internal sealed class ToggleTerminal: BaseCommand<ToggleTerminal> {

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) { 
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        try {
            var uiShell = await VS.GetServiceAsync<SVsUIShell, IVsUIShell>();
            var terminalGuid = new Guid(WindowGuids.DeveloperPowerShell);
            var terminalWindow = await VS.Windows.FindWindowAsync(terminalGuid);
            var opened = terminalWindow is not null ? await terminalWindow.IsOnScreenAsync() : false;
            if (opened) await terminalWindow.HideAsync();
            else await VS.Windows.ShowToolWindowAsync(terminalGuid); // Or execute "View.Terminal"
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle Terminal: {ex.Message}");
        }
    }
}