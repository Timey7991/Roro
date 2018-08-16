using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Roro.Workflow.Wpf
{
    public partial class NodePickerControl : UserControl
    {
        public ObservableCollection<NodePickerTreeViewItem> TreeViewSource { get; } = new ObservableCollection<NodePickerTreeViewItem>();

        public NodePickerControl()
        {
            InitializeComponent();
            this.DataContext = this;
            var types = Node.GetActivityTypes();
            foreach (var type in types)
            {
                var typeName = type.Name;
                var typeNamespace = type.Namespace;
                if (this.TreeViewSource.FirstOrDefault(x => x.Text == typeNamespace) is null)
                {
                    this.TreeViewSource.Add(new NodePickerTreeViewItem(typeNamespace, null));
                }
                this.TreeViewSource.First(x => x.Text == typeNamespace).Items.Add(new NodePickerTreeViewItem(typeName, type));
            }
        }

        private void TreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is NodePickerTreeViewItem nodePickerTreeViewItem)
            {
                if (nodePickerTreeViewItem.Value != null)
                {
                    DragDrop.DoDragDrop(this, nodePickerTreeViewItem, DragDropEffects.Copy);
                }
            }
        }
    }

    public class NodePickerTreeViewItem
    {
        public string Text { get; }

        public object Value { get; }

        public List<NodePickerTreeViewItem> Items { get; }

        public NodePickerTreeViewItem(string text, object value)
        {
            this.Text = text;
            this.Value = value;
            this.Items = new List<NodePickerTreeViewItem>();
        }
    }
}
