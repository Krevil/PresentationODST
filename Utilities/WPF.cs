using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace PresentationODST.Utilities
{
    class WPF
    {
        public static void AddNewRow(Grid grid, int rowindex, Int32 length)
        {
            RowDefinition NewRow = new RowDefinition { Height = new GridLength(length) };
            grid.RowDefinitions.Add(NewRow);
            Grid.SetRow(grid.Children[rowindex], grid.RowDefinitions.IndexOf(NewRow));
        }

        public static void AddNewRow(Grid grid, int rowindex)
        {
            RowDefinition NewRow = new RowDefinition();
            grid.RowDefinitions.Add(NewRow);
            Grid.SetRow(grid.Children[rowindex], grid.RowDefinitions.IndexOf(NewRow));
        }

        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static readonly SolidColorBrush BlueBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
        public static readonly SolidColorBrush BlackBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        public static readonly SolidColorBrush WhiteBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

    }
}
