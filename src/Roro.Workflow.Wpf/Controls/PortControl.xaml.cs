using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Roro.Workflow.Wpf
{
    public partial class PortControl : UserControl
    {
        private PageControl _pageControl => VisualTreeHelperEx.GetAncestor<PageControl>(this);

        private Canvas _pageCanvas => VisualTreeHelperEx.GetAncestor<Canvas>(this);

        //private Page _page => this._node.ParentPage;

        //private Node _node => this.DataContext as Node;

        private Port _port => this.DataContext as Port;

        public PortControl()
        {
            InitializeComponent();
        }

        #region MOUSE

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this._pageCanvas.Focus();

            e.Handled = true;
            this._mouseDownPoint = this.TranslatePoint(new Point(this.Width / 2, this.Height / 2), this._pageCanvas);

            // LINK
            this.MouseMove += PortControl_MouseMove_LinkStart;
            this.MouseUp += PortControl_MouseUp_LinkCancel;
        }

        private void PortControl_MouseUp_LinkCancel(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= PortControl_MouseMove_LinkStart;
            this.MouseUp -= PortControl_MouseUp_LinkCancel;
        }

        private void PortControl_MouseMove_LinkStart(object sender, MouseEventArgs e)
        {
            this.MouseMove -= PortControl_MouseMove_LinkStart;
            this.MouseUp -= PortControl_MouseUp_LinkCancel;
            //
            this.CaptureMouse();
            this.MouseMove += PortControl_MouseMove_Linking;
            this.MouseUp += PortControl_MouseUp_LinkEnd;
            this._pageControl.LinkStartingPoint = this._mouseDownPoint;
            this._pageControl.LinkEndingPoint = this._mouseDownPoint;
        }

        private void PortControl_MouseUp_LinkEnd(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= PortControl_MouseMove_Linking;
            this.MouseUp -= PortControl_MouseUp_LinkEnd;
            //
            this.ReleaseMouseCapture();

            var mouseUpPoint = e.GetPosition(this._pageCanvas);
            if (this._pageCanvas.InputHitTest(mouseUpPoint) is DependencyObject depObj &&
                VisualTreeHelperEx.GetAncestor<NodeControl>(depObj) is NodeControl nodeControl)
            {
                this._port.To = (nodeControl.DataContext as Node).Id;
            }
            else
            {
                this._port.To = Guid.Empty;
            }

            this._pageControl.LinkStartingPoint = new Point();
            this._pageControl.LinkEndingPoint = new Point();

            this._pageControl.UpdateLinks();
        }

        private void PortControl_MouseMove_Linking(object sender, MouseEventArgs e)
        {
            var mouseMovePoint = e.GetPosition(this._pageCanvas);
            this._pageControl.LinkEndingPoint = mouseMovePoint;
        }

        private Point _mouseDownPoint;

        #endregion
    }
}
