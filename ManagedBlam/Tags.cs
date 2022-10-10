using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PresentationODST.Controls;
using PresentationODST.Utilities;

namespace PresentationODST.ManagedBlam
{
    class Tags
    {
        public static readonly Bungie.Tags.TagGroupType[] TagGroups = Bungie.Tags.TagGroupType.GetTagGroups();

        public static void OpenTag(string filename)
        {
            string[] OpenPath = Path.GetTagsRelativePath(filename).Split('.');
            Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile(Bungie.Tags.TagPath.FromPathAndExtension(OpenPath[0], OpenPath[1]));
            Xceed.Wpf.AvalonDock.Layout.LayoutDocument TagTab = new Xceed.Wpf.AvalonDock.Layout.LayoutDocument
            {
                Title = String.Join(".", OpenPath),
                Content = new TagView()
            };
            TagView NewTagView = (TagView)TagTab.Content;
            NewTagView.TagFile = NewTag;
            foreach (Bungie.Tags.TagField field in NewTag.Fields)
            {
                AddFieldValues(NewTagView.TagGrid, field);
            }
            MainWindow.TagTabs.Children.Add(TagTab);
            MainWindow.TagTabs.SelectedContentIndex = MainWindow.TagTabs.Children.IndexOf(TagTab);
        }

        // Migrate each case's operations to new methods
        public static void AddFieldValues(Grid grid, Bungie.Tags.TagField field)
        {
            int RowIndex;
            switch (field.FieldType)
            {
                case Bungie.Tags.TagFieldType.CharInteger:
                case Bungie.Tags.TagFieldType.ShortInteger:
                case Bungie.Tags.TagFieldType.LongInteger:
                case Bungie.Tags.TagFieldType.Int64Integer:
                case Bungie.Tags.TagFieldType.Real:
                    Bungie.Tags.TagFieldElement Element = (Bungie.Tags.TagFieldElement)field;
                    RowIndex = grid.Children.Add(new TagFieldElementControl { TagField = Element });
                    WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.CharEnum:
                case Bungie.Tags.TagFieldType.ShortEnum:
                case Bungie.Tags.TagFieldType.LongEnum:
                    RowIndex = grid.Children.Add(new TagFieldEnumControl { TagField = (Bungie.Tags.TagFieldEnum)field });
                    WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.Reference:
                    RowIndex = grid.Children.Add(new TagFieldReferenceControl { TagField = (Bungie.Tags.TagFieldReference)field });
                    WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.Block:
                    Bungie.Tags.TagFieldBlock TagBlock = (Bungie.Tags.TagFieldBlock)field;
                    RowIndex = grid.Children.Add(new TagFieldBlockControl { TagField = TagBlock });
                    WPF.AddNewRow(grid, RowIndex);
                    break;
                case Bungie.Tags.TagFieldType.Struct:
                    // Structs are stupid and I hate them 
                    Bungie.Tags.TagFieldStruct TagStruct = (Bungie.Tags.TagFieldStruct)field;
                    foreach (Bungie.Tags.TagElement StructElement in TagStruct.Elements)
                    {
                        foreach (Bungie.Tags.TagField subfield in StructElement.Fields)
                        {
                            AddFieldValues(grid, subfield); // Probably don't need a new control for struct fields?
                        }
                    }
                    break;
                case Bungie.Tags.TagFieldType.StringId:
                    // This *needs* to have its own control or it won't save, just duplicate the fieldelement and tack on some bolts for the string selector
                    RowIndex = grid.Children.Add(new TagFieldElementControl { TagField = (Bungie.Tags.TagFieldElementStringID)field });
                    WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                default:
                    //System.Diagnostics.Debug.WriteLine(field.FieldType + " could not be added to " + field.GetParentElement().ElementHeaderText + " as no case for the type exists");
                    break;
            }
        }
    }
}
