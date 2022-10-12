﻿using System;
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
    /// Interaction logic for TagFieldEnumControl.xaml
    /// </summary>
    public partial class TagFieldEnumControl : UserControl
    {
        public TagFieldEnumControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private Bungie.Tags.TagFieldEnum _TagField;
        public Bungie.Tags.TagFieldEnum TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                foreach (Bungie.Tags.TagValueEnumItem EnumItem in value.Items)
                {
                    if (!Properties.Settings.Default.ExtraIndices) 
                        ValueComboBox.Items.Add(EnumItem.EnumName);
                    else
                        ValueComboBox.Items.Add(EnumItem.EnumIndex + ". " + EnumItem.EnumName);
                }
                ValueComboBox.SelectedIndex = value.Value;
                NameTextBlock.Text = value.FieldName;
                TypeTextBlock.Text = value.FieldType.ToString().ToLower();
                TypeTextBlock.Visibility = Properties.Settings.Default.FieldTypes ? Visibility.Visible : Visibility.Hidden;
                if (value.Description.Length > 0)
                {
                    HintTextBlock.Visibility = Visibility.Visible;
                    ((ToolTip)ToolTipService.GetToolTip(HintTextBlock)).Content = value.Description;
                }
            }
        }

        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _TagField.Value = ValueComboBox.SelectedIndex;
        }
    }
}
