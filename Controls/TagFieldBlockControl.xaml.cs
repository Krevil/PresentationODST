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
namespace PresentationODST.Controls
{
    /// <summary>
    /// Interaction logic for TagFieldBlockControl.xaml
    /// </summary>
    public partial class TagFieldBlockControl : UserControl
    {
        public TagFieldBlockControl()
        {
            InitializeComponent();
        }
        
        private Bungie.Tags.TagFieldBlock _TagField;
        public Bungie.Tags.TagFieldBlock TagField
        {
            get
            {
                return _TagField;
                
            }
            set
            {
                _TagField = value;
                NameTextBlock.Text = value.FieldName;

                for (int i = 0; i < value.Elements.Count; i++)
                {
                    if (!value.Elements[i].ElementHeaderText.Contains(i + ".") && Properties.Settings.Default.ExtraIndices)
                        BlockListComboBox.Items.Add(i + ". " + value.Elements[i].ElementHeaderText);
                    else
                        BlockListComboBox.Items.Add(value.Elements[i].ElementHeaderText);
                }
                // set this behind a preference setting
                if (value.Elements.Count > 0)
                {
                    BlockListComboBox.SelectedIndex = 0;
                    foreach (Bungie.Tags.TagField field in _TagField.Elements[BlockListComboBox.SelectedIndex].Fields)
                    {
                        ManagedBlam.Tags.AddFieldValues(ElementGrid, field);
                    }
                }
                else
                {
                    BlockListComboBox.Items.Add("NONE"); // Delete this when adding anything
                    BlockListComboBox.SelectedIndex = 0;
                    return;
                }

            }
        }
       

        private void BlockListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (BlockListComboBox.Items[0].ToString() == "NONE") return;
            // Stupid hack
            ElementGrid.Children.Clear();
            ElementGrid.RowDefinitions.Clear();
            foreach (Bungie.Tags.TagField field in _TagField.Elements[BlockListComboBox.SelectedIndex].Fields)
            {
                ManagedBlam.Tags.AddFieldValues(ElementGrid, field);
            }
        }


    }
}
