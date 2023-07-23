using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PresentationODST.Dialogs
{
    /// <summary>
    /// Interaction logic for TagRenamer.xaml
    /// </summary>
    public partial class TagRenamer : Window
    {
        public TagRenamer()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NameTextBox.Text.Contains("."))
            {
                CustomMessageBox.Show("Tag name should contain an extension");
                return;
            }
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
