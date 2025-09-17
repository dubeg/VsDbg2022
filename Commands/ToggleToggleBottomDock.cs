using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Community.VisualStudio.Toolkit;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Controls;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.Shell;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleBottomDock)]
internal sealed class ToggleBottomDock : BaseCommand<ToggleBottomDock> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        try {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            new PanelSwitcher(Package, Dock.Bottom).Switch();
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle bottom dock: {ex.Message}");
        }
    }
}

// -----------------------------------------------
// https://github.com/qalisander/SidePanelSwitcher
// -----------------------------------------------
public class PanelSwitcher {
    private readonly Dock _dockDirection;
    private readonly IServiceProvider _package;

    public PanelSwitcher(IServiceProvider package, Dock dockDirection) {
        _package = package;
        _dockDirection = dockDirection;
    }

    public void Switch() {
        ThreadHelper.ThrowIfNotOnUIThread();

        try {
            var toolWindowFrames = _package
                .GetToolWindowFrames()
                .Select(fr => fr as Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame);

            ApplySwitch(
                toolWindowFrames.Where(IsDocked),
                toolWindowFrames.Where(IsBookmarked));
        }
        catch (Exception e) {
            VsShellUtilities.LogError(e.Source, e.ToString());
        }
    }

    private static void ApplySwitch(IEnumerable<Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame> docked, IEnumerable<Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame> bookmarked) {
        if (bookmarked.Any())
            bookmarked.DockBookmarks();
        else if (docked.Any())
            docked.AutoHideDockPanel();
    }

    private bool IsDocked(Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame frame) {
        if (!(frame.FrameView is ViewElement viewElement))
            return false;

        //verifying whether there is viewElement's ancestor type of DockRoot
        if (!DockOperations.CanAutoHide(viewElement))
            return false;

        if (OrientationOfDocked(viewElement) == _dockDirection)
            return true;

        return false;
    }

    private bool IsBookmarked(Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame frame) {
        if (!(frame.FrameView is ViewElement viewElement))
            return false;

        //verifying whether there is viewElement's ancestor type of AutoHideChannel
        if (!AutoHideChannel.IsAutoHidden(viewElement))
            return false;

        if (OrientationOfAutoHidden(viewElement) == _dockDirection)
            return true;

        return false;
    }

    private static Dock OrientationOfDocked(ViewElement viewElement) {
        var autoHideCenter = viewElement.GetAutoHideCenter();
        var commonAncestor = viewElement.FindCommonAncestor(autoHideCenter, el => el.Parent);

        if (commonAncestor is null)
            throw new ArgumentNullException(nameof(commonAncestor));

        if (!(commonAncestor is DockGroup dockGroup))
            throw new InvalidOperationException();

        var subtreeIndex1 = dockGroup.FindSubtreeIndex(autoHideCenter);
        var subtreeIndex2 = dockGroup.FindSubtreeIndex(viewElement);
        var dock = dockGroup.Orientation != Orientation.Horizontal
            ? subtreeIndex2 >= subtreeIndex1 ? Dock.Bottom : Dock.Top
            : subtreeIndex2 >= subtreeIndex1 ? Dock.Right : Dock.Left;

        return dock;
    }

    private static Dock OrientationOfAutoHidden(ViewElement viewElement) {
        return viewElement.FindAncestor<AutoHideChannel>().Dock;
    }
}

public static class Extension {
    //public static WindowFrame GetActiveWindowFrame(IEnumerable<IVsWindowFrame> frames, DTE2 dte)
    //{
    //    return (from vsWindowFrame in frames
    //            let window = GetWindow(vsWindowFrame)
    //            where window == dte.ActiveWindow
    //            select vsWindowFrame as WindowFrame)
    //        .FirstOrDefault();
    //}

    //public static Window GetWindow(IVsWindowFrame vsWindowFrame)
    //{
    //    object window;
    //    ErrorHandler.ThrowOnFailure(vsWindowFrame.GetProperty((int) __VSFPROPID.VSFPROPID_ExtWindowObject,
    //        out window));

    //    return window as Window;
    //}

    public static int FindSubtreeIndex(this ViewGroup rootElement, ViewElement subtreeElement) {
        while (subtreeElement.Parent != rootElement)
            subtreeElement = subtreeElement.Parent;

        return rootElement.Children.IndexOf(subtreeElement);
    }

    public static ViewElement GetAutoHideCenter(this ViewElement element) {
        return ((ViewSite)ViewElement.FindRootElement(element)).Find(AutoHideRoot.GetIsAutoHideCenter);
    }
}

public static class EnumerableExt {
    public static void DockBookmarks(this IEnumerable<Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame> bookmarkedFrames) {
        ThreadHelper.ThrowIfNotOnUIThread();

        foreach (var frame in bookmarkedFrames) {
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out var frameMode);

            // We should make sure that bookmarks were not docked before
            if ((VsFrameMode)frameMode != VsFrameMode.AutoHide)
                continue;

            frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VsFrameMode.Dock);
        }
    }

    public static void AutoHideDockPanel(this IEnumerable<Microsoft.VisualStudio.Platform.WindowManagement.WindowFrame> bookmarkedFrames) {
        ThreadHelper.ThrowIfNotOnUIThread();

        foreach (var frame in bookmarkedFrames) {
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out var frameMode);

            // We should make sure that bookmarks were not auto hidden before
            if ((VsFrameMode)frameMode != VsFrameMode.Dock)
                continue;

            frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VsFrameMode.AutoHide);
        }
    }
}

public static class ServerProviderExt {
    public static IEnumerable<IVsWindowFrame> GetVsWindowFrames(this IServiceProvider serviceProvider) {
        //NOTE: https://www.pmichaels.net/tag/throwifnotonuithread/
        ThreadHelper.ThrowIfNotOnUIThread();

        //NOTE: https://docs.microsoft.com/en-us/dotnet/api/microsoft.assumes?view=visualstudiosdk-2019
        var uiShell = serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
        Assumes.Present(uiShell);

        // ReSharper disable once PossibleNullReferenceException
        ErrorHandler.ThrowOnFailure(uiShell.GetDocumentWindowEnum(out var windowEnumerator));

        if (windowEnumerator.Reset() != VSConstants.S_OK)
            yield break;

        var frames = new IVsWindowFrame[1];
        bool hasMoreWindows;
        do {
            hasMoreWindows = windowEnumerator.Next(1, frames, out var fetched) == VSConstants.S_OK && fetched == 1;

            if (!hasMoreWindows || frames[0] == null)
                continue;

            yield return frames[0];
        }
        while (hasMoreWindows);
    }

    public static IEnumerable<IVsWindowFrame> GetToolWindowFrames(this IServiceProvider serviceProvider) {
        ThreadHelper.ThrowIfNotOnUIThread();

        var uiShell = serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
        Assumes.Present(uiShell);

        // ReSharper disable once PossibleNullReferenceException
        ErrorHandler.ThrowOnFailure(uiShell.GetToolWindowEnum(out var windowEnumerator));

        if (windowEnumerator.Reset() != VSConstants.S_OK)
            yield break;

        var frames = new IVsWindowFrame[1];
        bool hasMoreWindows;
        do {
            hasMoreWindows = windowEnumerator.Next(1, frames, out var fetched) == VSConstants.S_OK && fetched == 1;

            if (!hasMoreWindows || frames[0] == null)
                continue;

            yield return frames[0];
        }
        while (hasMoreWindows);
    }
}