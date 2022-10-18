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
namespace PresentationODST.Controls.TagFieldControls
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
                    AddComboBoxElement(i);
                }
                if (_TagField.Elements.Count == 0)
                {
                    ExpandButton.Content = "+";
                    ElementGrid.Visibility = Visibility.Collapsed;
                }
                AddBlockItems();
                HandleVisibility();
            }
        }

        private void AddComboBoxElement(int i)
        {
            if (!_TagField.Elements[i].ElementHeaderText.Contains(i + ".") && Properties.Settings.Default.ExtraIndices)
                BlockListComboBox.Items.Add(i + ". " + _TagField.Elements[i].ElementHeaderText);
            else
                BlockListComboBox.Items.Add(_TagField.Elements[i].ElementHeaderText);
        }

        private void AddBlockItems()
        {
            if (_TagField.Elements.Count > 0)
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

        private void ClearElementGrid()
        {
            ElementGrid.Children.Clear();
            ElementGrid.RowDefinitions.Clear();
        }

        // Lazy method to repopulate the block. Should redo for big blocks with lots of elements
        private void RefreshBlock()
        {
            AllowSelectionChanged = false;
            ClearElementGrid();
            BlockListComboBox.Items.Clear();
            TagField = _TagField;
            AllowSelectionChanged = true;
            HandleVisibility();
        }

        private void HandleVisibility()
        {
            if (_TagField.Elements.Count == _TagField.MaximumElementCount)
            {
                AddButton.IsEnabled = false;
                InsertButton.IsEnabled = false;
                DuplicateButton.IsEnabled = false;
                return;
            }
            else
            {
                AddButton.IsEnabled = true;
            }
            if (_TagField.Elements.Count > 0)
            {
                InsertButton.IsEnabled = true;
                DuplicateButton.IsEnabled = true;
            }
        }

        private bool AllowSelectionChanged = true;
        private void BlockListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (!AllowSelectionChanged) return;
            if (BlockListComboBox.Items.Count == 0) return;
            if (BlockListComboBox.Items[0].ToString() == "NONE") return;
            // Stupid hacks
            ClearElementGrid();
            foreach (Bungie.Tags.TagField field in _TagField.Elements[BlockListComboBox.SelectedIndex].Fields)
            {
                ManagedBlam.Tags.AddFieldValues(ElementGrid, field);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.Elements.Count == _TagField.MaximumElementCount) return;
            _TagField.AddElement();
            RefreshBlock();
            BlockListComboBox.SelectedIndex = BlockListComboBox.Items.Count - 1;
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.Elements.Count == _TagField.MaximumElementCount) return;
            if (_TagField.Elements.Count <= 0) return;
            int Index = BlockListComboBox.SelectedIndex;
            _TagField.InsertElement(Index);
            RefreshBlock();
            BlockListComboBox.SelectedIndex = Index;
        }

        private void DuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.Elements.Count == _TagField.MaximumElementCount) return;
            if (_TagField.Elements.Count <= 0) return;
            int CurrentIndex = BlockListComboBox.SelectedIndex;
            _TagField.DuplicateElement(CurrentIndex);
            RefreshBlock();
            BlockListComboBox.SelectedIndex = CurrentIndex + 1;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.Elements.Count <= 0) return;
            if (BlockListComboBox.Items[0].ToString() == "NONE") return;
            int CurrentIndex = BlockListComboBox.SelectedIndex;
            _TagField.RemoveElement(CurrentIndex);
            RefreshBlock();
            if (_TagField.Elements.Count <= 0) return; // Yes we do need this twice this is not an erroneous duplicate
            BlockListComboBox.SelectedIndex = CurrentIndex - 1;
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlockListComboBox.Items[0].ToString() == "NONE") return;
            _TagField.RemoveAllElements();
            ClearElementGrid();
            BlockListComboBox.Items.Clear();
            BlockListComboBox.Items.Add("NONE"); // Delete this when adding anything
            BlockListComboBox.SelectedIndex = 0;
            HandleVisibility();
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (ElementGrid.Visibility == Visibility.Visible)
            {
                ElementGrid.Visibility = Visibility.Collapsed;
                ExpandButton.Content = "+";
            }
            else if (_TagField.Elements.Count > 0)
            {
                ElementGrid.Visibility = Visibility.Visible;
                ExpandButton.Content = "-";
            }
        }
    }
}
