using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Controls;
using Microsoft.VisualStudio.Platform.WindowManagement;
using System.Linq;
using System.Diagnostics;

namespace VsDbg.Commands;

/// <summary>
/// Toggle terminal window.
/// </summary>
[Command(PackageIds.ToggleTerminal)]
internal sealed class ToggleTerminal: BaseCommand<ToggleTerminal> {

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) { 
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        try {
            var terminalGuid = new Guid(WindowGuids.DeveloperPowerShell);
            var terminalWindow = await VS.Windows.FindWindowAsync(terminalGuid);
            var opened = terminalWindow is not null ? await terminalWindow.IsOnScreenAsync() : false;
            if (opened) {
                // await terminalWindow.HideAsync();
                new PanelSwitcher(Package, Dock.Bottom).Switch();
            }
            else {
                var windowFrame = await VS.Windows.ShowToolWindowAsync(terminalGuid);
                var vsWindowFrame = windowFrame as IVsWindowFrame;
                vsWindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);
            }
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle Terminal: {ex.Message}");
        }
    }
}