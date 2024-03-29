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

namespace PresentationODST.Controls.TagFieldControls
{
    public partial class TagFieldStringIDControl : UserControl, ITagFieldControlBase
    {
        public TagFieldStringIDControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
        }

        private Bungie.Tags.TagFieldElementStringIDWithMenu _SubTagField; // I do not remember what this is for but it probably has something to do with the neat menus Guerilla has. Someone will have to remind me to go back to this later.
        public bool HasSubType = false;

        private Bungie.Tags.TagFieldElementStringID _TagField;
        public Bungie.Tags.TagFieldElementStringID TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                if (value.FieldSubtype == "sted")
                {
                    _SubTagField = (Bungie.Tags.TagFieldElementStringIDWithMenu)value;
                    HasSubType = true;
                }
                ValueTextBox.Text = value.GetStringData();
                NameTextBlock.Text = value.FieldName;
                if (value.Units.Length > 0)
                {
                    TypeTextBlock.Visibility = Visibility.Visible;
                    if (Properties.Settings.Default.FieldTypes)
                    {
                        TypeTextBlock.Text = value.Units + " (" + value.FieldType.ToString().ToLower() + ")";
                    }
                    else
                    {
                        TypeTextBlock.Text = value.Units;
                    }
                }
                else
                {
                    TypeTextBlock.Text = "(" + value.FieldType.ToString().ToLower() + ")";
                    TypeTextBlock.Visibility = Properties.Settings.Default.FieldTypes ? Visibility.Visible : Visibility.Hidden;
                }
                if (value.Description.Length > 0)
                {
                    HintTextBlock.Visibility = Visibility.Visible;
                    ((ToolTip)ToolTipService.GetToolTip(HintTextBlock)).Content = value.Description;
                }
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (ValueTextBox.Text.Length > TagField.MaxLength)
            {
                ValueTextBox.Foreground = Utilities.WPF.RedBrush;
            }
            ValueTextBox.Foreground = Utilities.WPF.BlackBrush;
            _TagField.SetStringData(ValueTextBox.Text);
        }
    }
}
