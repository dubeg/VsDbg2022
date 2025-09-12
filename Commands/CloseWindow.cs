using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace VsDbg.Commands;

/// <summary>
/// Closes either a tool or a document window.
/// </summary>
[Command(PackageIds.CloseWindow)]
internal sealed class CloseWindow : BaseCommand<CloseWindow> {

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) { 
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        try {
            var windowFrame = await VS.Windows.GetCurrentWindowAsync();
            if (windowFrame is IVsWindowFrame vsWindowFrame) {
                vsWindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_Type, out var frameType);
                if ((int) frameType == (int)__WindowFrameTypeFlags.WINDOWFRAMETYPE_Tool) {
                    await VS.Commands.ExecuteAsync("Window.CloseToolWindow");
                }
                else if ((int)frameType == (int)__WindowFrameTypeFlags.WINDOWFRAMETYPE_Document) {
                    await VS.Commands.ExecuteAsync("Window.CloseDocumentWindow");
                }
            }
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Close Window", $"Failed to close window: {ex.Message}");
        }
    }
}