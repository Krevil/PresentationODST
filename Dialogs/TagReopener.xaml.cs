using Bungie.Tags;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationODST.Dialogs
{
    public partial class TagReopener : Window
    {
        public List<string> TagsToOpen = new List<string>();

        public TagReopener()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TagsToOpen = new List<string>();
            foreach (CheckBox box in TagListBox.Items)
            {
                if (box.IsChecked != true) continue;
                TagsToOpen.Add(((TextBlock)box.Content).Text);
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TagListBox_Initialized(object sender, EventArgs e)
        {
            foreach (string path in Properties.Settings.Default.OpenTags)
            {
                TagsToOpen.Add(path);
                CheckBox box = new CheckBox
                {
                    Content = new TextBlock { Text = path }, // text blocks are used because of some weird access key stuff with underscores when using checkboxes
                    IsChecked = true
                };
                TagListBox.Items.Add(box);
            }
        }
    }
        
    
}
