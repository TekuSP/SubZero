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
    public partial class YesNoDialog : UserControl
    {
        /// <summary>
        /// If yes is pressed, true, if no is pressed, false
        /// </summary>
        public bool DialogResult { get; private set; }
        Action savedCallback;
        public YesNoDialog(string title, PackIconKind icon, string message, Brush yesColor, Brush noColor, Brush titleColor, Action callback)
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
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            savedCallback();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            savedCallback();
        }
        public static YesNoDialog ShowWarningDialog(string message, Action callback) => new YesNoDialog("Warning", PackIconKind.Warning, message, Brushes.Green, Brushes.Red, Brushes.Orange, callback);
        public static YesNoDialog ShowInformationDialog(string message, Action callback) => new YesNoDialog("Information", PackIconKind.Information, message, Brushes.Green, Brushes.Red, Brushes.LightBlue, callback);
        public static YesNoDialog ShowErrorDialog(string message, Action callback) => new YesNoDialog("Error", PackIconKind.Error, message, Brushes.Green, Brushes.Red,Brushes.Red, callback);

    }
}
