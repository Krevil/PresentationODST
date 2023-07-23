using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PresentationODST.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public enum ButtonType
        {
            OK,
            OKCancel,
            YesNo,
        }

        public static bool? Show(string bodyText = "", string windowTitle = "", ButtonType msgBoxButtons = ButtonType.OK, string okButtonText = "OK", string yesButtonText = "Yes", string noButtonText = "No", string cancelButtonText = "Cancel")
        {
            CustomMessageBox msgBox = new CustomMessageBox(bodyText, windowTitle, msgBoxButtons, okButtonText, yesButtonText, noButtonText, cancelButtonText);
            msgBox.Owner = MainWindow.Main_Window;
            return msgBox.ShowDialog();
        }

        public CustomMessageBox(string bodyText = "", string windowTitle = "", ButtonType msgBoxButtons = ButtonType.OK, string okButtonText = "OK", string yesButtonText = "Yes", string noButtonText = "No", string cancelButtonText = "Cancel")
        {
            InitializeComponent();
            BodyText.Text = bodyText;
            Title = windowTitle;
            switch (msgBoxButtons)
            {
                case ButtonType.YesNo:
                    MessageBoxGrid.ColumnDefinitions[1].Width = new GridLength(150);
                    MessageBoxGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    MessageBoxGrid.ColumnDefinitions[3].Width = new GridLength(150);
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    OKButton.Visibility = Visibility.Collapsed;
                    YesButton.Content = yesButtonText;
                    NoButton.Content = noButtonText;
                    break;
                case ButtonType.OKCancel:
                    MessageBoxGrid.ColumnDefinitions[1].Width = new GridLength(150);
                    MessageBoxGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    MessageBoxGrid.ColumnDefinitions[3].Width = new GridLength(150);
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    OKButton.Visibility = Visibility.Collapsed;
                    YesButton.Content = okButtonText;
                    NoButton.Content = cancelButtonText;
                    break;
                case ButtonType.OK:
                    OKButton.Content = okButtonText;
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
