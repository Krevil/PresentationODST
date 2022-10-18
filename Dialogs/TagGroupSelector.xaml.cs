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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationODST.Dialogs
{
    /// <summary>
    /// Interaction logic for TagGroupSelector.xaml
    /// </summary>
    public partial class TagGroupSelector : Window
    {
        public TagGroupSelector()
        {
            DataContext = this;
            InitializeComponent();
            TagListBox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Extension", System.ComponentModel.ListSortDirection.Ascending));
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

        public Bungie.Tags.TagGroupType[] TagGroups
        {
            get
            {
                return ManagedBlam.Tags.TagGroups;
            }
        }

        private void Command_Close_Binding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
        
    
}
