namespace VsDbg.Commands;

/// <summary>
/// When firing this command, it will invoke the built-in "Refactor.Rename" without arguments,
/// ie. without the annoying rename popup.
/// </summary>
[Command(PackageIds.RefactorRename)]
internal sealed class RefactorRename : BaseCommand<RefactorRename> {

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        await VS.Commands.ExecuteAsync("Refactor.Rename");
        var doc = await VS.Documents.GetActiveDocumentViewAsync();
        // InlineRenameAdornmentProvider.AdornmentLayerName = "RoslynRenameDashboard"
        // Those classes are defined in Microsoft.CodeAnalysis.EditorFeatures.dll
        var layer = doc.TextView.GetAdornmentLayer("RoslynRenameDashboard");
        if (layer is not null) {
            layer.RemoveAllAdornments();
        }
    }
}