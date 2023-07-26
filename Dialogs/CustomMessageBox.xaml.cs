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
        /// <summary>
        /// OK: Single middle button that returns true when clicked
        /// OKCancel: Left and right buttons, Cancel returns null
        /// YesNo: Left and right buttons, Yes returns true, No returns false
        /// YesNoCancel: Left, middle and right buttons Yes, No and Cancel in that order
        /// </summary>
        public enum ButtonType
        {
            OK,
            OKCancel,
            YesNo,
            YesNoCancel,
        }

        public static bool? Show(string bodyText, string windowTitle, ButtonType msgBoxButtons)
        {
            CustomMessageBox msgBox = new CustomMessageBox(bodyText, windowTitle, msgBoxButtons);
            msgBox.Owner = MainWindow.Main_Window;
            return msgBox.ShowDialog();
        }

        public static bool? Show(string bodyText = "", string windowTitle = "", ButtonType msgBoxButtons = ButtonType.OK, string MiddleButtonText = "OK", string LeftButtonText = "Yes", string RightButtonText = "No")
        {
            CustomMessageBox msgBox = new CustomMessageBox(bodyText, windowTitle, msgBoxButtons, MiddleButtonText, LeftButtonText, RightButtonText);
            msgBox.Owner = MainWindow.Main_Window;
            return msgBox.ShowDialog();
        }

        public CustomMessageBox(string bodyText, string windowTitle, ButtonType msgBoxButtons)
        {
            InitializeComponent();
            BodyText.Text = bodyText;
            Title = windowTitle;
            switch (msgBoxButtons)
            {
                case ButtonType.YesNoCancel:
                    LeftButton.Visibility = Visibility.Visible;
                    RightButton.Visibility = Visibility.Visible;
                    LeftButton.Click += YesOk_Click;
                    MiddleButton.Click += No_Click;
                    RightButton.Click += Cancel_Click;
                    LeftButton.Content = new TextBlock { Text = "Yes" };
                    MiddleButton.Content = new TextBlock { Text = "No" };
                    RightButton.Content = new TextBlock { Text = "Cancel" };
                    break;
                case ButtonType.YesNo:
                    MessageBoxGrid.ColumnDefinitions[1].Width = new GridLength(150);
                    MessageBoxGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    MessageBoxGrid.ColumnDefinitions[3].Width = new GridLength(150);
                    LeftButton.Visibility = Visibility.Visible;
                    RightButton.Visibility = Visibility.Visible;
                    MiddleButton.Visibility = Visibility.Collapsed;
                    LeftButton.Click += YesOk_Click;
                    RightButton.Click += No_Click;
                    break;
                case ButtonType.OKCancel:
                    MessageBoxGrid.ColumnDefinitions[1].Width = new GridLength(150);
                    MessageBoxGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    MessageBoxGrid.ColumnDefinitions[3].Width = new GridLength(150);
                    LeftButton.Visibility = Visibility.Visible;
                    RightButton.Visibility = Visibility.Visible;
                    MiddleButton.Visibility = Visibility.Collapsed;
                    LeftButton.Click += YesOk_Click;
                    RightButton.Click += Cancel_Click;
                    LeftButton.Content = new TextBlock { Text = "OK" };
                    RightButton.Content = new TextBlock { Text = "Cancel" };
                    break;
                case ButtonType.OK:
                    MiddleButton.Click += YesOk_Click;
                    break;
            }
        }

        public CustomMessageBox(string bodyText = "", string windowTitle = "", ButtonType msgBoxButtons = ButtonType.OK, string MiddleButtonText = "OK", string LeftButtonText = "Yes", string RightButtonText = "No")
        {
            InitializeComponent();
            BodyText.Text = bodyText;
            Title = windowTitle;
            LeftButton.Content = new TextBlock { Text = LeftButtonText };
            MiddleButton.Content = new TextBlock { Text = MiddleButtonText };
            RightButton.Content = new TextBlock { Text = RightButtonText };
            switch (msgBoxButtons)
            {
                case ButtonType.YesNoCancel:
                    LeftButton.Visibility = Visibility.Visible;
                    RightButton.Visibility = Visibility.Visible;
                    LeftButton.Click += YesOk_Click;
                    MiddleButton.Click += No_Click;
                    RightButton.Click += Cancel_Click;
                    break;
                case ButtonType.YesNo:
                    MessageBoxGrid.ColumnDefinitions[1].Width = new GridLength(150);
                    MessageBoxGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    MessageBoxGrid.ColumnDefinitions[3].Width = new GridLength(150);
                    LeftButton.Visibility = Visibility.Visible;
                    RightButton.Visibility = Visibility.Visible;
                    MiddleButton.Visibility = Visibility.Collapsed;
                    LeftButton.Click += YesOk_Click;
                    RightButton.Click += No_Click;
                    break;
                case ButtonType.OKCancel:
                    MessageBoxGrid.ColumnDefinitions[1].Width = new GridLength(150);
                    MessageBoxGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    MessageBoxGrid.ColumnDefinitions[3].Width = new GridLength(150);
                    LeftButton.Visibility = Visibility.Visible;
                    RightButton.Visibility = Visibility.Visible;
                    MiddleButton.Visibility = Visibility.Collapsed;
                    LeftButton.Click += YesOk_Click;
                    RightButton.Click += Cancel_Click;
                    break;
                case ButtonType.OK:
                    MiddleButton.Click += YesOk_Click;
                    break;
            }
        }

        private void YesOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        
        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }
    }
}
