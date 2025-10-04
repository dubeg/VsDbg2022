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
/// Toggle C# Interactive window.
/// </summary>
[Command(PackageIds.ToggleCSharpInteractive)]
internal sealed class ToggleCSharpInteractive : BaseCommand<ToggleCSharpInteractive> {

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        try {
            var windowGuid = new Guid(WindowGuidsEx.CSharpInteractive);
            var window = await VS.Windows.FindWindowAsync(windowGuid);
            var opened = window is not null ? await window.IsOnScreenAsync() : false;
            if (opened) {
                new PanelSwitcher(Package, Dock.Bottom).Switch();
            }
            else {
                var windowFrame = await VS.Windows.ShowToolWindowAsync(windowGuid);
                var vsWindowFrame = windowFrame as IVsWindowFrame;
                vsWindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);
            }
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle C# Interactive: {ex.Message}");
        }
    }
}




