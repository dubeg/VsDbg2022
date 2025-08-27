using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VsDbg.Commands;

public partial class DbgMessageBox : DialogWindow {
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.5);
    public ImageMoniker IconMoniker { get; set; } = KnownMonikers.StatusOK;
    public string Message { get; set; }

    public static void Show(string message, ImageMoniker icon, TimeSpan duration) {
        var dlg = new DbgMessageBox() {
            Message = message,
            IconMoniker = icon,
            Duration = duration,
        };
        dlg.InitializeComponent();
        dlg.ShowModal();
    }

    protected override void OnContentRendered(EventArgs e) {
        base.OnContentRendered(e);
        Task.Delay(Duration).ContinueWith(_ => {
            Dispatcher.Invoke(() => this.Close());
        });
    }
}
