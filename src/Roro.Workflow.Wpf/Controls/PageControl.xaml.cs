using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Roro.Workflow.Wpf
{
    public partial class PageControl : UserControl, INotifyPropertyChanged
    {
        private IEditablePage _page => this.DataContext as IEditablePage;

        public Rect SelectingRect
        {
            get => this._selectingRect;
            set => this.OnPropertyChanged(ref this._selectingRect, value);
        }
        private Rect _selectingRect;

        public Point LinkStartingPoint
        {
            get => this._linkStartingPoint;
            set => this.OnPropertyChanged(ref this._linkStartingPoint, value);
        }
        private Point _linkStartingPoint;

        public Point LinkEndingPoint
        {
            get => this._linkEndingPoint;
            set => this.OnPropertyChanged(ref this._linkEndingPoint, value);
        }
        private Point _linkEndingPoint;

        public PageControl()
        {
            InitializeComponent();
        }

        private void myCanvasNodes_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateLinks();
        }

        private void myCanvasNodes_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is null))
            {
                this.UpdateLinks();
            }
        }
        
        public void UpdateLinks()
        {
            var pathFinder = new PathFinder(this._page.Nodes.Select(x => x.Rect));
            myCanvasLink.Children.Clear();
            this._page.Nodes.ToList().ForEach(sourceNode =>
                sourceNode.Ports.ToList().ForEach(sourcePort =>
                {
                    if (this._page.Nodes.FirstOrDefault(x => x.Id == sourcePort.To) is Node targetNode)
                    {
                        var anchorLinks = new List<PortAnchorLink>();
                        foreach (var sourcePortAnchor in sourcePort.Anchors)
                        {
                            foreach (var targetNodeAnchor in targetNode.Anchors)
                            {
                                var portAnchorLink = new PortAnchorLink(sourcePort, sourcePortAnchor, targetNode, targetNodeAnchor);
                                portAnchorLink.Path = pathFinder.GetPath(portAnchorLink.StartPoint, portAnchorLink.EndPoint);
                                anchorLinks.Add(portAnchorLink);
                            }
                        }
                        if (anchorLinks
                                .OrderBy(x => x.Path.Split('Q').Length)
                                .ThenBy(x => x.Path.Length).FirstOrDefault() is PortAnchorLink anchorLink)
                        {
                            sourcePort.CurrentAnchor = anchorLink.SourcePortAnchor;
                            myCanvasLink.Children.Add(new Path()
                            {
                                Data = Geometry.Parse(anchorLink.Path),
                                Stroke = Brushes.Gray
                            });
                        }
                        else
                        {
                            sourcePort.CurrentAnchor = sourcePort.DefaultAnchor;
                        }
                    }
                    else
                    {
                        sourcePort.CurrentAnchor = sourcePort.DefaultAnchor;
                    }
                }));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string name = default(string))
        {
            property = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
