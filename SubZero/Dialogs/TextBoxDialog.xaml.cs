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
    public partial class TextBoxDialog : UserControl
    {
        /// <summary>
        /// If yes is pressed, true, if no is pressed, false
        /// </summary>
        public string DialogResult { get; private set; }
        Action savedCallback;
        public TextBoxDialog(string title, PackIconKind icon, Brush titleColor, Action callback)
        {
            InitializeComponent();
            savedCallback = callback;
            this.title.Text = title;
            this.icon.Kind = icon;
            this.accept.Background = Brushes.Green;
            this.accept.BorderBrush = Brushes.Green;
            this.cancel.BorderBrush = Brushes.Red;
            this.cancel.Background = Brushes.Red;
            this.title.Foreground = titleColor;
            this.icon.Foreground = titleColor;
        }

        private void accept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
                DialogResult = null;
            else
                DialogResult = message.Text;
            savedCallback?.Invoke();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            savedCallback?.Invoke();
        }
        public static TextBoxDialog ShowWarningDialog(string title, Action callback) => new TextBoxDialog($"Warning - {title}", PackIconKind.Warning, Brushes.Orange, callback);
        public static TextBoxDialog ShowInformationDialog(string title, Action callback) => new TextBoxDialog($"Information - {title}", PackIconKind.Information, Brushes.LightBlue, callback);
        public static TextBoxDialog ShowErrorDialog(string title, Action callback) => new TextBoxDialog($"Error - {title}", PackIconKind.Error, Brushes.Red, callback);
    }
}
