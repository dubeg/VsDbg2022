using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using SettingsStoreExplorer;

namespace VsDbg.Commands;

public static class SettingsUtils {

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
            await VS.StatusBar.ShowMessageAsync($"{statusDisplayName}: {(enabled ? "enabled" : "disabled")}");
            await Task.Delay(1000);
            await VS.StatusBar.ShowMessageAsync(null);
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
            await package.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = await VS.GetServiceAsync<DTE, DTE2>();
            var settingsManager = await VS.GetServiceAsync<SVsSettingsPersistenceManager, ISettingsManager>();
            var roamingSettings = new RoamingSettingsStore(settingsManager);
            var enabled = false;

            var collectionParts = collectionPath.Split(['.'], StringSplitOptions.RemoveEmptyEntries);
            if (collectionParts.Length > 1) {
                throw new System.ArgumentException(nameof(collectionPath), $"Collection sub paths are not implemented");
            }

            var cols = roamingSettings.GetSubCollectionNames("").ToList();
            var colExists = cols.Any(x => x == collectionPath);
            if (!colExists) {
                throw new ArgumentOutOfRangeException(nameof(collectionPath), $"Collection '{collectionPath}' doesn't exist.");
            }

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
            // --
            await VS.StatusBar.ShowMessageAsync($"{statusDisplayName}: {(enabled ? "enabled" : "disabled")}");
            await Task.Delay(1000);
            await VS.StatusBar.ShowMessageAsync(null);
        }
        catch (Exception ex) {
            await VS.MessageBox.ShowErrorAsync("Error", $"Failed to toggle option: {ex.Message}");
        }
    }
}
