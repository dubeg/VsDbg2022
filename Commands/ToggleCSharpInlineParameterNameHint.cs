using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsDbg.Commands;

[Command(PackageIds.ToggleCSharpInlineParameterNameHint)]
internal class ToggleCSharpInlineParameterNameHint : BaseCommand<ToggleCSharpInlineParameterNameHint> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        var enabled = await Utils.ToggleRoamingAsync(Package,"TextEditor.CSharp.Specific", "InlineParameterNameHints");
        Utils.ShowSettingStatusThenClear("C#: Inline Parameter Name Hint", enabled);
    }
}
