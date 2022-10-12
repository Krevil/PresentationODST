using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PresentationODST.Controls;
using PresentationODST.Controls.LayoutDocuments;
using PresentationODST.Utilities;
using Xceed.Wpf.AvalonDock.Layout;

namespace PresentationODST.ManagedBlam
{
    class Tags
    {
        public static readonly Bungie.Tags.TagGroupType[] TagGroups = Bungie.Tags.TagGroupType.GetTagGroups();

        public static void OpenTag(string filename)
        {
            string[] OpenPath = Path.GetTagsRelativePath(filename).Split('.');
            Bungie.Tags.TagPath OpenTagPath = Bungie.Tags.TagPath.FromPathAndExtension(OpenPath[0], OpenPath[1]);
            if (!System.IO.File.Exists(filename)) return;
            LayoutDocumentPane ldp = MainWindow.Main_Window.TagDock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile(OpenTagPath);
            LayoutDocument TagTab = new LayoutDocument
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
            ldp.Children.Add(TagTab);
            ldp.SelectedContentIndex = MainWindow.TagTabs.Children.IndexOf(TagTab);
        }

        // Migrate each case's operations to new methods
        public static void AddFieldValues(Grid grid, Bungie.Tags.TagField field)
        {
            int RowIndex;
            bool FieldVisible = WPF.ExpertModeVisibility(field);
            switch (field.FieldType)
            {
                case Bungie.Tags.TagFieldType.CharInteger:
                case Bungie.Tags.TagFieldType.ShortInteger:
                case Bungie.Tags.TagFieldType.LongInteger:
                case Bungie.Tags.TagFieldType.Int64Integer:
                case Bungie.Tags.TagFieldType.Real:
                    Bungie.Tags.TagFieldElement Element = (Bungie.Tags.TagFieldElement)field;
                    RowIndex = grid.Children.Add(new TagFieldElementControl { TagField = Element, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible) 
                        WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.CharEnum:
                case Bungie.Tags.TagFieldType.ShortEnum:
                case Bungie.Tags.TagFieldType.LongEnum:
                    RowIndex = grid.Children.Add(new TagFieldEnumControl { TagField = (Bungie.Tags.TagFieldEnum)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.Reference:
                    RowIndex = grid.Children.Add(new TagFieldReferenceControl { TagField = (Bungie.Tags.TagFieldReference)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.Block:
                    Bungie.Tags.TagFieldBlock TagBlock = (Bungie.Tags.TagFieldBlock)field;
                    RowIndex = grid.Children.Add(new TagFieldBlockControl { TagField = TagBlock, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
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
                    RowIndex = grid.Children.Add(new TagFieldStringIDControl { TagField = (Bungie.Tags.TagFieldElementStringID)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                case Bungie.Tags.TagFieldType.ByteFlags:
                case Bungie.Tags.TagFieldType.WordFlags:
                case Bungie.Tags.TagFieldType.Flags:
                    Bungie.Tags.TagFieldFlags Flags = (Bungie.Tags.TagFieldFlags)field;
                    RowIndex = grid.Children.Add(new TagFieldFlagsControl { TagField = Flags, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 23 + ((Flags.Items.Length - 1) * 19)); //flag fields size varies
                    break;
                case Bungie.Tags.TagFieldType.CharBlockIndex:
                case Bungie.Tags.TagFieldType.ShortBlockIndex:
                case Bungie.Tags.TagFieldType.LongBlockIndex:
                    RowIndex = grid.Children.Add(new TagFieldBlockIndexControl { TagField = (Bungie.Tags.TagFieldBlockIndex)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 25);
                    break;
                default:
                    //System.Diagnostics.Debug.WriteLineIf(field.FieldName == "type", field.FieldType + " " + field.FieldSubtype);
                    //System.Diagnostics.Debug.WriteLine(field.FieldType + " could not be added to " + field.GetParentElement().ElementHeaderText + " as no case for the type exists");
                    break;
            }
        }
    }
}
