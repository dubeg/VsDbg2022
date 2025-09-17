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
        var displayName = "Code Lens";
        var package = Package as VsDbgPackage;
        if (package.VsVersion >= 18) {
            // 2026
            await SettingsUtils.ToggleUnifiedSettingWithStatusAsync(
                Package,
                "textEditor.codeLens.enabled",
                displayName
            );
        }
        else if (package.VsVersion == 17) {
            // 2022
            await SettingsUtils.ToggleRoamingWithStatusAsync(
                Package,
                "TextEditorGlobalOptions",
                "IsCodeLensEnabled",
                displayName
            );
        }
    }
}