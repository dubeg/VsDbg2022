using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleCSharpFadeOut)]
internal class ToggleCSharpFadeOut : BaseCommand<ToggleCSharpFadeOut> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        // 2025-09-10 v18.0.0: this setting hasn't been migrated to UnifiedSettings yet.
        var enabled = 
        await SettingsUtils.ToggleRoamingAsync(Package,"TextEditor.CSharp.Specific","FadeOutUnusedImports");
        await SettingsUtils.SetRoamingAsync(Package, "TextEditor.CSharp.Specific", "FadeOutUnusedMembers", enabled);
        await SettingsUtils.SetRoamingAsync(Package, "TextEditor.CSharp.Specific", "FadeOutUnreachableCode", enabled);
        SettingsUtils.ShowSettingStatusThenClear("C#: Fade Out", enabled);
    }
}
