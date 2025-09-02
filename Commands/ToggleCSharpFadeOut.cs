using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleCSharpFadeOut)]
internal class ToggleCSharpFadeOut : BaseCommand<ToggleCSharpFadeOut> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        var enabled = 
        await Utils.ToggleRoamingAsync(Package,"TextEditor.CSharp.Specific","FadeOutUnusedImports");
        await Utils.SetRoamingAsync(Package, "TextEditor.CSharp.Specific", "FadeOutUnusedMembers", enabled);
        await Utils.SetRoamingAsync(Package, "TextEditor.CSharp.Specific", "FadeOutUnreachableCode", enabled);
        Utils.ShowSettingStatusThenClear("C#: Fade Out", enabled);
    }
}
