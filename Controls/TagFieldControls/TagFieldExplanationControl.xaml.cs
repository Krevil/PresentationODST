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
    public partial class TagFieldExplanationControl : UserControl, ITagFieldControlBase
    {
        public TagFieldExplanationControl()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
        }

        private Bungie.Tags.TagFieldExplanation _TagField;
        public Bungie.Tags.TagFieldExplanation TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                TitleTextBlock.Text = value.FieldName;
                DescriptionTextBlock.Text = value.Explanation;
                if (value.Explanation.Length <= 0) 
                {
                    DescriptionTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
