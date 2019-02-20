using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BillsManager.Views.Controls
{
    public class GridSplitterEx : GridSplitter
    {
        public static RoutedCommand ShowOnlyPrevious = new RoutedCommand();
        public static RoutedCommand ShowBothEqually = new RoutedCommand();
        public static RoutedCommand ShowOnlyNext = new RoutedCommand();

        protected int splitterIndex;
        protected int splitterSpan;
        protected int index1;
        protected int index2;

        static GridSplitterEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitterEx), new FrameworkPropertyMetadata(typeof(GridSplitterEx)));
        }

        public GridSplitterEx()
        {
            this.CommandBindings.Add(new CommandBinding(GridSplitterEx.ShowOnlyPrevious, this.ShowOnlyPreviousDefs));
            this.CommandBindings.Add(new CommandBinding(GridSplitterEx.ShowBothEqually, this.ShowBothDefs));
            this.CommandBindings.Add(new CommandBinding(GridSplitterEx.ShowOnlyNext, this.ShowOnlyNextDefs));
        }

        private GridResizeDirection GetEffectiveResizeDirection()
        {
            GridResizeDirection direction = ResizeDirection;

            if (direction == GridResizeDirection.Auto)
            {
                // When HorizontalAlignment is Left, Right or Center, resize Columns
                if (this.HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    direction = GridResizeDirection.Columns;
                }
                else if (this.VerticalAlignment != VerticalAlignment.Stretch)
                {
                    direction = GridResizeDirection.Rows;
                }
                else if (this.ActualWidth <= this.ActualHeight) // Fall back to Width vs Height
                {
                    direction = GridResizeDirection.Columns;
                }
                else
                {
                    direction = GridResizeDirection.Rows;
                }

            }
            return direction;
        }
        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            GridResizeBehavior resizeBehavior = ResizeBehavior;

            if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
            {
                if (direction == GridResizeDirection.Columns)
                {
                    switch (this.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case HorizontalAlignment.Right:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
                else
                {
                    switch (this.VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case VerticalAlignment.Bottom:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
            }
            return resizeBehavior;
        }

        protected void ResizePrevNext(GridLength previousLength, GridLength nextLength)
        {
            var grid = this.Parent as Grid;
            if (grid == null) return;

            var isHorizontal = this.GetEffectiveResizeDirection() == GridResizeDirection.Columns;

            this.splitterIndex = (int)this.GetValue(isHorizontal ? Grid.ColumnProperty : Grid.RowProperty);
            this.splitterSpan = (int)this.GetValue(isHorizontal ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);

            switch (this.GetEffectiveResizeBehavior(this.GetEffectiveResizeDirection()))
            {
                case GridResizeBehavior.PreviousAndNext:
                    this.index1 = this.splitterIndex - 1;
                    this.index2 = this.splitterIndex + this.splitterSpan;
                    break;
                case GridResizeBehavior.CurrentAndNext:
                    this.index1 = this.splitterIndex;
                    this.index2 = this.splitterIndex + this.splitterSpan;
                    break;
                default: // GridResizeBehavior.PreviousAndCurrent
                    this.index1 = this.splitterIndex - 1;
                    this.index2 = this.splitterIndex;
                    break;
            }

            if (isHorizontal)
            {
                grid.ColumnDefinitions[index1].Width = previousLength;
                grid.ColumnDefinitions[index2].Width = nextLength;
            }
            else
            {
                grid.RowDefinitions[index1].Height = previousLength;
                grid.RowDefinitions[index2].Height = nextLength;
            }
        }

        private void ShowBothDefs(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResizePrevNext(new GridLength(0.5, GridUnitType.Star), new GridLength(0.5, GridUnitType.Star));
        }

        private void ShowOnlyPreviousDefs(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResizePrevNext(new GridLength(1, GridUnitType.Star), new GridLength(0, GridUnitType.Star));
        }

        private void ShowOnlyNextDefs(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResizePrevNext(new GridLength(0, GridUnitType.Star), new GridLength(1, GridUnitType.Star));
        }
    }
}