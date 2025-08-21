using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleCodeLens)]
internal sealed class ToggleCodeLens : BaseCommand<ToggleCodeLens> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await SettingsUtils.ToggleRoamingWithStatusAsync(
            Package,
            "TextEditorGlobalOptions",
            "IsCodeLensEnabled",
            "Code Lens"
        );
    }
}