using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.TaskStatusCenter;
using SettingsStoreExplorer;
using MessageBox = Community.VisualStudio.Toolkit.MessageBox;

namespace VsDbg.Commands;

public static class Utils {

    public static async Task SetBoolAsync(AsyncPackage package, SettingsScope scope, string collectionPath, string propertyName, bool enabled) {
        await package.JoinableTaskFactory.SwitchToMainThreadAsync();
        var dte = await VS.GetServiceAsync<DTE, DTE2>();
        var settingsManager = ((IVsSettingsManager)await VS.Services.GetSettingsManagerAsync());
        var shellSettingsManager = new ShellSettingsManager(settingsManager);
        var store = shellSettingsManager.GetWritableSettingsStore(scope);
        var iEnabled = Convert.ToUInt32(enabled);
        store.SetUInt32(collectionPath, propertyName, iEnabled);
    }

    public static async Task<bool> ToggleAsync(AsyncPackage package, SettingsScope scope, string collectionPath, string propertyName) {
        await package.JoinableTaskFactory.SwitchToMainThreadAsync();
        var dte = await VS.GetServiceAsync<DTE, DTE2>();
        var settingsManager = ((IVsSettingsManager)await VS.Services.GetSettingsManagerAsync());
        var shellSettingsManager = new ShellSettingsManager(settingsManager);
        var store = shellSettingsManager.GetWritableSettingsStore(scope);
        var enabled = false;
        if (store.CollectionExists(collectionPath)) {
            if (!store.PropertyExists(collectionPath, propertyName)) {
                store.SetUInt32(collectionPath, propertyName, 0);
            }
            else {
                var iEnabled = store.GetUInt32(collectionPath, propertyName);
                enabled = Convert.ToBoolean(iEnabled);
                enabled = !enabled;
                iEnabled = Convert.ToUInt32(enabled);
                store.SetUInt32(collectionPath, propertyName, iEnabled);
            }
        }
        return enabled;
    }

    public static async Task ToggleWithStatusAsync(AsyncPackage package, SettingsScope scope, string collectionPath, string propertyName, string statusDisplayName = "") {
        statusDisplayName ??= propertyName;
        try {
            await package.JoinableTaskFactory.SwitchToMainThreadAsync();
            var enabled = await ToggleAsync(package, scope, collectionPath, propertyName);
            // --
            ShowSettingStatusThenClear(statusDisplayName, enabled);
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
        }
    }

    /// <summary>
    /// The SVsSettingsPersistenceManager class
    /// </summary>
    [Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")]
    private class SVsSettingsPersistenceManager { }

    public static async Task ToggleRoamingWithStatusAsync(AsyncPackage package, string collectionPath, string propertyName, string statusDisplayName = "") {
        statusDisplayName ??= propertyName;
        try {
            var enabled = await ToggleRoamingAsync(package, collectionPath, propertyName);
            ShowSettingStatusThenClear(statusDisplayName, enabled);
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
        }
    }

    public static async Task<bool> ToggleRoamingAsync(AsyncPackage package, string collectionPath, string propertyName) {
        await package.JoinableTaskFactory.SwitchToMainThreadAsync();
        var dte = await VS.GetServiceAsync<DTE, DTE2>();
        var settingsManager = await VS.GetServiceAsync<SVsSettingsPersistenceManager, ISettingsManager>();
        var roamingSettings = new RoamingSettingsStore(settingsManager);
        var enabled = false;
        var props = roamingSettings.GetPropertyNames(collectionPath).ToList();
        var propExists = props.Any(x => x == propertyName);
        if (!propExists) {
            await roamingSettings.SetUInt32Async(collectionPath, propertyName, 0);
        }
        else {
            var iEnabled = roamingSettings.GetUint32(collectionPath, propertyName);
            enabled = Convert.ToBoolean(iEnabled);
            enabled = !enabled;
            iEnabled = Convert.ToUInt32(enabled);
            await roamingSettings.SetUInt32Async(collectionPath, propertyName, iEnabled);
        }
        return enabled;
    }

    public static async Task SetRoamingAsync(AsyncPackage package, string collectionPath, string propertyName, bool enabled) {
        await package.JoinableTaskFactory.SwitchToMainThreadAsync();
        var dte = await VS.GetServiceAsync<DTE, DTE2>();
        var settingsManager = await VS.GetServiceAsync<SVsSettingsPersistenceManager, ISettingsManager>();
        var roamingSettings = new RoamingSettingsStore(settingsManager);
        var iEnabled = Convert.ToUInt32(enabled);
        await roamingSettings.SetUInt32Async(collectionPath, propertyName, iEnabled);
    }

    public static void ShowSettingStatusThenClear(string settingName, bool enabled)
        => ShowMessageThenClear(
            $"{settingName}: {(enabled ? "enabled" : "disabled")}", 
            icon: enabled ? KnownMonikers.StatusOK : KnownMonikers.StatusNo, 
            duration: TimeSpan.FromSeconds(0.5)
        );

    public static void ShowMessageThenClear(string message, ImageMoniker icon, TimeSpan duration) 
        => DbgMessageBox.Show(message, icon, duration);
    
    
}
