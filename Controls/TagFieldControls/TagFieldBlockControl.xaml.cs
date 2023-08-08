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
    public partial class TagFieldBlockControl : UserControl, ITagFieldControlBase
    {
        public TagFieldBlockControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
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
                MenuItem fieldPath = new MenuItem { Header = value.FieldPath };
                MenuItem blockLimit = new MenuItem { Header = "Maximum Element Count: " + value.MaximumElementCount };
                BlockContextMenu.Items.Add(fieldPath);
                BlockContextMenu.Items.Add(blockLimit);
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

        // I actually have to add text blocks for the non index items here. Why? I have absolutely 0 clue. It is so weird.
        private void AddComboBoxElement(int i)
        {
            if (!_TagField.Elements[i].ElementHeaderText.Contains(i + ".") && Properties.Settings.Default.ExtraIndices)
            {
                BlockListComboBox.Items.Add(i + ". " + _TagField.Elements[i].ElementHeaderText);
            }
            else
            {
                BlockListComboBox.Items.Add(new TextBlock { Text = _TagField.Elements[i].ElementHeaderText });
            }
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
            if (BlockListComboBox.SelectedIndex < 0) return;
            if (BlockListComboBox.Items.Count == 0) return;
            if (_TagField.Elements.Count == 0) return;
            if (BlockListComboBox.Items[0].ToString() == "NONE") return;
            if (BlockListComboBox.SelectedIndex > (_TagField.Elements.Count - 1)) return;
            // Stupid hacks
            ClearElementGrid();
            foreach (Bungie.Tags.TagField field in _TagField.Elements[BlockListComboBox.SelectedIndex].Fields)
            {
                ManagedBlam.Tags.AddFieldValues(ElementGrid, field);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Get rid of NONE first because it messes with the SelectedIndex
            if (_TagField.Elements.Count == 0 && BlockListComboBox.Items.Count == 1)
            {
                BlockListComboBox.Items.Clear();
            }
            if (_TagField.Elements.Count == _TagField.MaximumElementCount) return;
            _TagField.AddElement();
            RefreshBlock();
            BlockListComboBox.SelectedIndex = BlockListComboBox.Items.Count - 1;
            // Expand the tag block if it was collapsed
            if (_TagField.Elements.Count == 1)
            {
                ElementGrid.Visibility = Visibility.Visible;
                ExpandButton.Content = "-";
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.Elements.Count == _TagField.MaximumElementCount) return;
            //if (_TagField.Elements.Count <= 0) return;
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
            if (BlockListComboBox.Items[0].ToString() == "NONE" && _TagField.Elements.Count == 0) return;
            int CurrentIndex = BlockListComboBox.SelectedIndex;
            if ((_TagField.Elements.Count - 1) < CurrentIndex) return;
            if (CurrentIndex < 0)
            {
                BlockListComboBox.SelectedIndex = 0;
                return;
            }
            _TagField.RemoveElement(CurrentIndex);
            RefreshBlock();
            if (_TagField.Elements.Count <= 0) return; // Yes we do need this twice this is not an erroneous duplicate
            if (_TagField.Elements.Count > 1)
            {
                BlockListComboBox.SelectedIndex = CurrentIndex - 1;
            }  
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlockListComboBox.Items[0].ToString() == "NONE" && _TagField.Elements.Count == 0) return;
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

        private static string SourceFieldPath;

        public bool PasteEntireBlockEnabled
        {
            get
            {
                return _TagField.ClipboardContainsEntireBlock();
            }
        }

        public bool PasteBlockElementEnabled
        {
            get
            {
                return _TagField.ClipboardContainsBlockElement();
            }
        }

        private void CopyElement_Click(object sender, RoutedEventArgs e)
        {
            _TagField.CopyElement(BlockListComboBox.SelectedIndex);
            SourceFieldPath = _TagField.FieldPath;
        }

        private void ReplaceElement_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.Elements.Count <= 0) return;
            if (_TagField.ClipboardContainsBlockElement() && SourceFieldPath == _TagField.FieldPath)
            {
                _TagField.PasteReplaceElement(BlockListComboBox.SelectedIndex);
                RefreshBlock();
            }
        }
        private void InsertElement_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.ClipboardContainsBlockElement() && SourceFieldPath == _TagField.FieldPath)
            {
                _TagField.PasteInsertElement(BlockListComboBox.SelectedIndex);
                RefreshBlock();
            }
        }

        private void AppendElement_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.ClipboardContainsBlockElement() && SourceFieldPath == _TagField.FieldPath)
            {
                _TagField.PasteAppendElement();
                RefreshBlock();
            }
        }

        private void CopyBlock_Click(object sender, RoutedEventArgs e)
        {
            _TagField.CopyEntireTagBlock();
            SourceFieldPath = _TagField.FieldPath;
        }

        private void ReplaceBlock_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.ClipboardContainsEntireBlock() && SourceFieldPath == _TagField.FieldPath)
            {
                _TagField.PasteReplaceEntireBlock();
                RefreshBlock();
            }
        }

        private void AppendBlock_Click(object sender, RoutedEventArgs e)
        {
            if (_TagField.ClipboardContainsEntireBlock() && SourceFieldPath == _TagField.FieldPath)
            {
                _TagField.PasteAppendEntireBlock();
                RefreshBlock();
            }
        }

        private void BlockOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).ContextMenu == null) return;
            bool CopyElementEnabled = _TagField.ClipboardContainsBlockElement();
            bool CopyBlockEnabled = _TagField.ClipboardContainsEntireBlock();
            InsertElement.IsEnabled = CopyElementEnabled;
            ReplaceElement.IsEnabled = CopyElementEnabled;
            AppendElement.IsEnabled = CopyElementEnabled;
            ReplaceBlock.IsEnabled = CopyBlockEnabled;
            AppendBlock.IsEnabled = CopyBlockEnabled;
            ((FrameworkElement)sender).ContextMenu.IsOpen = true;
        }
    }
}
