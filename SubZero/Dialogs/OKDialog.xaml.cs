using MaterialDesignThemes.Wpf;

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

namespace SubZero.Dialogs
{
    /// <summary>
    /// Interaction logic for OKDialog.xaml
    /// </summary>
    public partial class OKDialog : UserControl
    {
        Action savedCallback;
        public OKDialog(string title, PackIconKind icon, string message, Brush okColor, Brush titleColor, Action callback)
        {
            InitializeComponent();
            savedCallback = callback;
            this.title.Text = title;
            this.message.Text = message;
            this.icon.Kind = icon;
            this.ok.BorderBrush = okColor;
            this.ok.Background = okColor;
            this.title.Foreground = titleColor;
            this.icon.Foreground = titleColor;
        }
        [Obsolete]
        public OKDialog(string title, PackIconKind icon, string message, Brush okColor, Brush titleColor, Action callback, bool special)
        {
            InitializeComponent();
            savedCallback = callback;
            this.title.Text = title;
            this.message.Text = message;
            this.icon.Kind = icon;
            this.ok.BorderBrush = okColor;
            this.ok.Background = okColor;
            this.title.Foreground = titleColor;
            this.icon.Foreground = titleColor;
            Width = 500;
            this.message.TextAlignment = TextAlignment.Center;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            savedCallback();
        }
        public static OKDialog ShowWarningDialog(string message, Action callback) => new OKDialog("Warning", PackIconKind.Warning, message, Brushes.Orange, Brushes.Orange, callback);
        public static OKDialog ShowInformationDialog(string message, Action callback) => new OKDialog("Information", PackIconKind.Information, message, Brushes.LightBlue, Brushes.LightBlue, callback);
        public static OKDialog ShowErrorDialog(string message, Action callback) => new OKDialog("Error", PackIconKind.Error, message, Brushes.Red, Brushes.Red, callback);
        [Obsolete]
        public static OKDialog ShowSettingsTemporaryDialog(string message, Action callback) => new OKDialog("Information", PackIconKind.Information, message, Brushes.LightBlue, Brushes.LightBlue, callback, true);

    }
}
