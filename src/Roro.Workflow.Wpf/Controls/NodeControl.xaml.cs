using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Roro.Workflow.Wpf
{
    public partial class NodeControl : UserControl
    {
        private IEditableNode _node => this.DataContext as IEditableNode;

        private PageControl _pageControl => VisualTreeHelperEx.GetAncestor<PageControl>(this);

        private Canvas _pageCanvas => VisualTreeHelperEx.GetAncestor<Canvas>(this);

        public NodeControl()
        {
            InitializeComponent();
        }

        #region MOUSE

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this._pageCanvas.Focus();

            e.Handled = true;
            this._mouseDownPoint = e.GetPosition(this._pageCanvas);

            // SELECT
            this.MouseMove += NodeControl_MouseMove_SelectCancel;
            this.MouseLeftButtonUp += NodeControl_MouseLeftButtonUp_SelectEnd;

            // MOVE
            this.MouseMove += NodeControl_MouseMove_MoveStart;
            this.MouseLeftButtonUp += NodeControl_MouseLeftButtonUp_MoveCancel;
        }

        private Point _mouseDownPoint;

        #region MOUSE: SELECT

        private void NodeControl_MouseMove_SelectCancel(object sender, MouseEventArgs e)
        {
            this.MouseMove -= NodeControl_MouseMove_SelectCancel;
            this.MouseLeftButtonUp -= NodeControl_MouseLeftButtonUp_SelectEnd;
        }

        private void NodeControl_MouseLeftButtonUp_SelectEnd(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= NodeControl_MouseMove_SelectCancel;
            this.MouseLeftButtonUp -= NodeControl_MouseLeftButtonUp_SelectEnd;
            //
            var ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            if (ctrl)
            {
                this._node.Selected = !this._node.Selected;
            }
            else
            {
                this._node.ParentPage.Nodes.Where(x => x.Selected).ToList()
                    .ForEach(x => x.Selected = false);
                this._node.Selected = true;
            }
        }

        #endregion

        #region MOUSE: MOVE

        private void NodeControl_MouseLeftButtonUp_MoveCancel(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= NodeControl_MouseMove_MoveStart;
            this.MouseLeftButtonUp -= NodeControl_MouseLeftButtonUp_MoveCancel;
        }

        private Dictionary<IEditableNode, Point> _nodeLocations;

        private void NodeControl_MouseMove_MoveStart(object sender, MouseEventArgs e)
        {
            this.MouseMove -= NodeControl_MouseMove_MoveStart;
            this.MouseLeftButtonUp -= NodeControl_MouseLeftButtonUp_MoveCancel;
            //
            this.CaptureMouse();
            this.MouseMove += NodeControl_MouseMove_Moving;
            this.MouseLeftButtonUp += NodeControl_MouseLeftButtonUp_MoveEnd;

            this._nodeLocations = new Dictionary<IEditableNode, Point>();
            if (!this._node.Selected)
            {
                this._node.ParentPage.Nodes.Where(x => x.Selected).ToList()
                    .ForEach(x => x.Selected = false);
                this._node.Selected = true;
            }
            this._node.ParentPage.Nodes.Where(x => x.Selected).ToList()
                .ForEach(x => this._nodeLocations.Add(x, new Point(x.Rect.X, x.Rect.Y)));

            this._node.ParentPage.CommitPendingChanges();
        }

        private void NodeControl_MouseMove_Moving(object sender, MouseEventArgs e)
        {
            var mouseMovePoint = e.GetPosition(this._pageCanvas);
            var offsetX = mouseMovePoint.X - this._mouseDownPoint.X;
            var offsetY = mouseMovePoint.Y - this._mouseDownPoint.Y;
            this._nodeLocations.ToList()
                .ForEach(x =>
                {
                    x.Key.SetLocation((int)(x.Value.X + offsetX), (int)(x.Value.Y + offsetY));
                });

            this._pageControl.UpdateLinks();
        }

        private void NodeControl_MouseLeftButtonUp_MoveEnd(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= NodeControl_MouseMove_Moving;
            this.MouseLeftButtonUp -= NodeControl_MouseLeftButtonUp_MoveEnd;
            //
            this.ReleaseMouseCapture();

            this._node.ParentPage.CommitPendingChanges();
        }

        #endregion

        #region MOUSE: PROPERTIES

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (this._node is ActionNode actionNode)
            {
                actionNode.SyncArguments();
                new NodePropertyEditor(actionNode).ShowDialog();
            }
        }

        #endregion

        #endregion
    }
}
