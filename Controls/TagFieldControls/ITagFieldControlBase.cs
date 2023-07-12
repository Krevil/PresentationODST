using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresentationODST.Controls.TagFieldControls
{
    public interface ITagFieldControlBase
    {
        Bungie.Tags.TagField GetTagField();
    }
}
