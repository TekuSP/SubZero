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
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class YesNoCancelDialog : UserControl
    {
        /// <summary>
        /// If yes is pressed, true, if no is pressed, false, if cancel is pressed null is returned
        /// </summary>
        public bool? DialogResult { get; private set; }
        Action savedCallback;
        public YesNoCancelDialog(string title, PackIconKind icon, string message, Brush yesColor, Brush noColor, Brush titleColor, Action callback)
        {
            InitializeComponent();
            savedCallback = callback;
            this.title.Text = title;
            this.message.Text = message;
            this.icon.Kind = icon;
            this.yes.Background = yesColor;
            this.yes.BorderBrush = yesColor;
            this.no.BorderBrush = noColor;
            this.no.Background = noColor;
            this.title.Foreground = titleColor;
            this.icon.Foreground = titleColor;
            this.cancel.BorderBrush = Brushes.DarkRed;
            this.cancel.Background = Brushes.DarkRed;
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            savedCallback?.Invoke();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            savedCallback?.Invoke();
        }
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            savedCallback?.Invoke();
        }
        public static YesNoCancelDialog ShowWarningDialog(string message, Action callback) => new YesNoCancelDialog("Warning", PackIconKind.Warning, message, Brushes.Green, Brushes.Red, Brushes.Orange, callback);
        public static YesNoCancelDialog ShowInformationDialog(string message, Action callback) => new YesNoCancelDialog("Information", PackIconKind.Information, message, Brushes.Green, Brushes.Red, Brushes.LightBlue, callback);
        public static YesNoCancelDialog ShowErrorDialog(string message, Action callback) => new YesNoCancelDialog("Error", PackIconKind.Error, message, Brushes.Green, Brushes.Red, Brushes.Red, callback);

    }
}
