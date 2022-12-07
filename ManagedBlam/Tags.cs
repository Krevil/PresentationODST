using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PresentationODST.Controls;
using PresentationODST.Controls.TagFieldControls;
using PresentationODST.Controls.LayoutDocuments;
using PresentationODST.Utilities;
using Xceed.Wpf.AvalonDock.Layout;

namespace PresentationODST.ManagedBlam
{
    class Tags
    {
        public static readonly Bungie.Tags.TagGroupType[] TagGroups = Bungie.Tags.TagGroupType.GetTagGroups();
        public static List<Bungie.Tags.TagFile> OpenTags = new List<Bungie.Tags.TagFile>();

        public static void OpenTag(string filename)
        {
            string[] OpenPath = Path.GetTagsRelativePath(filename).Split('.');
            if (Bungie.Tags.TagGroupType.GetGroupTypeFromExtension(OpenPath[1]) == null)
            {
                MessageBox.Show("Unsupported file type", "Tag Load Error");
                return;
            }
            Bungie.Tags.TagPath OpenTagPath = Bungie.Tags.TagPath.FromPathAndExtension(OpenPath[0], OpenPath[1]);
            if (!System.IO.File.Exists(Path.ODSTEKTagsPath + OpenTagPath.RelativePathWithExtension))
            {
                MessageBox.Show("Couldn't find tag file", "Tag Load Error");
                return;
            }
            // Maybe stop users from opening the same tag more than once? Won't break anything by doing so, other than the user's fragile sanity
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
            ldp.SelectedContentIndex = ldp.Children.IndexOf(TagTab);
            OpenTags.Add(NewTag);
        }

        public static void NewTag()
        {
            MainWindow.GroupSelector = new Dialogs.TagGroupSelector();
            if (MainWindow.GroupSelector.ShowDialog() == true)
            {
                LayoutDocumentPane ldp = MainWindow.Main_Window.TagDock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile();
                Bungie.Tags.TagGroupType SelectedItem = (Bungie.Tags.TagGroupType)MainWindow.GroupSelector.TagListBox.SelectedItem;
                Bungie.Tags.TagPath NewPath = Bungie.Tags.TagPath.FromPathAndExtension("tag" + MainWindow.NewTagCount, SelectedItem.Extension);
                NewTag.New(NewPath);
                LayoutDocument TagTab = new LayoutDocument
                {
                    Title = "tag" + MainWindow.NewTagCount + "." + SelectedItem.Extension,
                    Content = new TagView()
                };
                TagView NewTagView = (TagView)TagTab.Content;
                NewTagView.TagFile = NewTag;
                foreach (Bungie.Tags.TagField field in NewTag.Fields)
                {
                    AddFieldValues(NewTagView.TagGrid, field);
                }
                ldp.Children.Add(TagTab);
                ldp.SelectedContentIndex = ldp.IndexOfChild(TagTab);
                MainWindow.NewTagCount++;
            }
        }

        // Migrate each case's operations to new methods
        public static void AddFieldValues(Grid grid, Bungie.Tags.TagField field)
        {
            int RowIndex;
            bool FieldVisible = WPF.ExpertModeVisibility(field);
            switch (field.FieldType)
            {
                case Bungie.Tags.TagFieldType.RealFraction:
                case Bungie.Tags.TagFieldType.Angle:
                case Bungie.Tags.TagFieldType.CharInteger:
                case Bungie.Tags.TagFieldType.ShortInteger:
                case Bungie.Tags.TagFieldType.LongInteger:
                case Bungie.Tags.TagFieldType.Int64Integer:
                case Bungie.Tags.TagFieldType.Real:
                    Bungie.Tags.TagFieldElement Element = (Bungie.Tags.TagFieldElement)field;
                    RowIndex = grid.Children.Add(new TagFieldElementControl { TagField = Element, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible) 
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.CharEnum:
                case Bungie.Tags.TagFieldType.ShortEnum:
                case Bungie.Tags.TagFieldType.LongEnum:
                    RowIndex = grid.Children.Add(new TagFieldEnumControl { TagField = (Bungie.Tags.TagFieldEnum)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.Reference:
                    RowIndex = grid.Children.Add(new TagFieldReferenceControl { TagField = (Bungie.Tags.TagFieldReference)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.Block:
                    Bungie.Tags.TagFieldBlock TagBlock = (Bungie.Tags.TagFieldBlock)field;
                    RowIndex = grid.Children.Add(new TagFieldBlockControl { TagField = TagBlock, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex);
                    break;
                case Bungie.Tags.TagFieldType.Struct:
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
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.OldStringId: // Why does this exist?
                    RowIndex = grid.Children.Add(new TagFieldOldStringIDControl { TagField = (Bungie.Tags.TagFieldElementOldStringID)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.LongString:
                case Bungie.Tags.TagFieldType.String:
                    RowIndex = grid.Children.Add(new TagFieldStringControl { TagField = (Bungie.Tags.TagFieldElementString)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.ByteFlags:
                case Bungie.Tags.TagFieldType.WordFlags:
                case Bungie.Tags.TagFieldType.Flags:
                    Bungie.Tags.TagFieldFlags Flags = (Bungie.Tags.TagFieldFlags)field;
                    RowIndex = grid.Children.Add(new TagFieldFlagsControl { TagField = Flags, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 29 + ((Flags.Items.Length - 1) * 19)); //flag fields size varies
                    break;
                case Bungie.Tags.TagFieldType.BlockFlags:
                case Bungie.Tags.TagFieldType.ByteBlockFlags:
                case Bungie.Tags.TagFieldType.WordBlockFlags:
                    Bungie.Tags.TagFieldBlockFlags BlockFlags = (Bungie.Tags.TagFieldBlockFlags)field;
                    RowIndex = grid.Children.Add(new TagFieldBlockFlagsControl { TagField = BlockFlags, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 29 + ((BlockFlags.Items.Length - 1) * 19)); //flag fields size varies
                    break;
                case Bungie.Tags.TagFieldType.ShortBlockIndexCustomSearch:
                case Bungie.Tags.TagFieldType.CharBlockIndexCustomSearch:
                case Bungie.Tags.TagFieldType.LongBlockIndexCustomSearch:
                case Bungie.Tags.TagFieldType.CharBlockIndex:
                case Bungie.Tags.TagFieldType.ShortBlockIndex:
                case Bungie.Tags.TagFieldType.LongBlockIndex:
                    RowIndex = grid.Children.Add(new TagFieldBlockIndexControl { TagField = (Bungie.Tags.TagFieldBlockIndex)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealQuaternion:
                    Bungie.Tags.TagFieldElementArray Element4d = (Bungie.Tags.TagFieldElementArray)field;
                    RowIndex = grid.Children.Add(new TagFieldElement4dControl { TagField = Element4d, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealVector3d:
                case Bungie.Tags.TagFieldType.RealEulerAngles3d:
                case Bungie.Tags.TagFieldType.RealPlane3d:
                case Bungie.Tags.TagFieldType.RealPoint3d:
                    Bungie.Tags.TagFieldElementArray Element3d = (Bungie.Tags.TagFieldElementArray)field;
                    RowIndex = grid.Children.Add(new TagFieldElement3dControl { TagField = Element3d, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealEulerAngles2d:
                case Bungie.Tags.TagFieldType.AngleBounds:
                case Bungie.Tags.TagFieldType.Point2d:
                case Bungie.Tags.TagFieldType.RealPlane2d:
                case Bungie.Tags.TagFieldType.RealPoint2d:
                case Bungie.Tags.TagFieldType.RealVector2d:
                case Bungie.Tags.TagFieldType.Rectangle2d:
                case Bungie.Tags.TagFieldType.RealFractionBounds:
                case Bungie.Tags.TagFieldType.RealBounds:
                    Bungie.Tags.TagFieldElementArray Element2d = (Bungie.Tags.TagFieldElementArray)field;
                    RowIndex = grid.Children.Add(new TagFieldElement2dControl { TagField = Element2d, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                default:
                    //System.Diagnostics.Debug.WriteLineIf(field.FieldName == "type", field.FieldType + " " + field.FieldSubtype);
                    System.Diagnostics.Debug.WriteLine(field.FieldType + " could not be added to " + field.GetParentElement().ElementHeaderText + " as no case for the type exists");
                    break;
            }
        }
    }
}
