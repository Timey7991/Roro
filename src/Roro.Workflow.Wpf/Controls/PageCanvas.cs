using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Roro.Workflow.Wpf
{
    public class PageCanvas : Canvas
    {
        private PageControl _pageControl => VisualTreeHelperEx.GetAncestor<PageControl>(this);

        private Page _page => this.DataContext as Page;

        #region MOUSE

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Focus();

            e.Handled = true;
            this._mouseDownPoint = e.GetPosition(this);

            // SELECT
            this.MouseMove += PageCanvas_MouseMove_SelectManyStart;
            this.MouseLeftButtonUp += PageCanvas_MouseLeftButtonUp_SelectEnd;
        }

        private Point _mouseDownPoint;

        #region MOUSE: SELECT

        private void PageCanvas_MouseLeftButtonUp_SelectEnd(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= PageCanvas_MouseMove_SelectManyStart;
            this.MouseLeftButtonUp -= PageCanvas_MouseLeftButtonUp_SelectEnd;
            //
            this._page.SelectedNodes.ToList().ForEach(x => x.Selected = false);
        }

        private void PageCanvas_MouseMove_SelectManyStart(object sender, MouseEventArgs e)
        {
            this.MouseMove -= PageCanvas_MouseMove_SelectManyStart;
            this.MouseLeftButtonUp -= PageCanvas_MouseLeftButtonUp_SelectEnd;
            //
            this.MouseMove += PageCanvas_MouseMove_SelectingMany;
            this.MouseLeftButtonUp += PageCanvas_MouseLeftButtonUp_SelectManyEnd;
            this.CaptureMouse();
        }

        private void PageCanvas_MouseMove_SelectingMany(object sender, MouseEventArgs e)
        {
            var mouseMovePoint = e.GetPosition(this);
            var rect = new Rect();
            rect.X = Math.Min(this._mouseDownPoint.X, mouseMovePoint.X);
            rect.Y = Math.Min(this._mouseDownPoint.Y, mouseMovePoint.Y);
            rect.Width = Math.Max(this._mouseDownPoint.X, mouseMovePoint.X) - rect.X;
            rect.Height = Math.Max(this._mouseDownPoint.Y, mouseMovePoint.Y) - rect.Y;
            this._pageControl.SelectingRect = rect;
        }

        private void PageCanvas_MouseLeftButtonUp_SelectManyEnd(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= PageCanvas_MouseMove_SelectingMany;
            this.MouseLeftButtonUp -= PageCanvas_MouseLeftButtonUp_SelectManyEnd;
            //
            var selectingRect = this._pageControl.SelectingRect;

            var ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            if (ctrl == false)
            {
                this._page.SelectedNodes.ToList()
                    .ForEach(x => x.Selected = false);
            }

            this._page.Nodes.Where(x => selectingRect.Contains(new Point(x.Bounds.X, x.Bounds.Y))).ToList()
                .ForEach(x => x.Selected = true);

            this._pageControl.SelectingRect = new Rect();
            this.ReleaseMouseCapture();
        }

        #endregion

        #endregion

        #region KEYBOARD

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this._page.Delete();
            }
            else if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var clipText = this._page.Cut();
                if (!string.IsNullOrWhiteSpace(clipText))
                {
                    Clipboard.SetText(clipText);
                }
            }
            else if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var clipText = this._page.Copy();
                if (!string.IsNullOrWhiteSpace(clipText))
                {
                    Clipboard.SetText(clipText);
                }
            }
            else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var clipText = Clipboard.GetText();
                if (!string.IsNullOrWhiteSpace(clipText))
                {
                    this._page.Paste(clipText);
                }
            }
            else if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this._page.Undo();
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this._page.Redo();
            }
            else if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this._page.Nodes.ForEach(x => x.Selected = true);
            }
            else
            {
                return;
            }

            this._pageControl.UpdateLinks();
        }

        #endregion

        #region DRAGDROP

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(NodePickerTreeViewItem)))
            {
                var item = e.Data.GetData(typeof(NodePickerTreeViewItem)) as NodePickerTreeViewItem;
                if (item.Value is Type type)
                {
                    if (Node.CreateNodeFromActivity(type.FullName) is Node node)
                    {
                        this._page.CommitPendingChanges();
                        node.Bounds = new NodeRect()
                        {
                            X = (int)e.GetPosition(this).X - node.Bounds.Width / 2,
                            Y = (int)e.GetPosition(this).Y - node.Bounds.Height / 2,
                            Width = node.Bounds.Width,
                            Height = node.Bounds.Height
                        };
                        this._page.Nodes.Add(node);
                        this._page.CommitPendingChanges();
                    }
                }
            }
        }

        #endregion

    }
}
