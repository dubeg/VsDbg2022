using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace SettingsStoreExplorer;

public enum Scope {
    Config = __VsEnclosingScopes.EnclosingScopes_Configuration,
    User = __VsEnclosingScopes.EnclosingScopes_UserSettings,
    Remote = __VsEnclosingScopes2.EnclosingScopes_Remote,
    Roaming
}