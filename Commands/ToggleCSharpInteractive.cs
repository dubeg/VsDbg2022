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
[Command(PackageIds.ToggleCSharpInteractive)]
internal sealed class ToggleCSharpInteractive : BaseCommand<ToggleCSharpInteractive> {

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        try {
            var terminalGuid = new Guid(WindowGuidsEx.CSharpInteractive);
            var terminalWindow = await VS.Windows.FindWindowAsync(terminalGuid);
            var opened = terminalWindow is not null ? await terminalWindow.IsOnScreenAsync() : false;
            if (opened) {
                new PanelSwitcher(Package, Dock.Bottom).Switch();
            }
            else {
                var windowFrame = await VS.Windows.ShowToolWindowAsync(terminalGuid);
                var vsWindowFrame = windowFrame as IVsWindowFrame;
                vsWindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);
            }
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle C# Interactive: {ex.Message}");
        }
    }
}




