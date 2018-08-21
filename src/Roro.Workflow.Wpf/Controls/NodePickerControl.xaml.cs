using System;
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
        public ObservableCollection<NodePickerItem> TreeViewSource { get; } = new ObservableCollection<NodePickerItem>();

        public NodePickerControl()
        {
            InitializeComponent();
            InitializeItemSources();
        }

        private void InitializeItemSources()
        {
            this.DataContext = this;
            var types = Node.GetActivityTypes();

            var nodes = new NodePickerItem("General", null);
            nodes.Items.Add(new NodePickerItem("Action", typeof(ActionNode)));
            nodes.Items.Add(new NodePickerItem("Decision", typeof(DecisionNode)));
            nodes.Items.Add(new NodePickerItem("Assign", typeof(PreparationNode)));
            nodes.Items.Add(new NodePickerItem("Variable", typeof(VariableNode)));
            nodes.Items.Add(new NodePickerItem("Loop", typeof(LoopStartNode)));
            nodes.Items.Add(new NodePickerItem("Page", typeof(PageNode)));
            nodes.Items.Add(new NodePickerItem("End", typeof(EndNode)));
            this.TreeViewSource.Add(nodes);

            // TreeViewSource
            foreach (var type in types)
            {
                var typeName = type.Name;
                var typeNamespace = type.Namespace;
                if (this.TreeViewSource.FirstOrDefault(x => x.Text == typeNamespace) is null)
                {
                    this.TreeViewSource.Add(new NodePickerItem(typeNamespace, null));
                }
                this.TreeViewSource.First(x => x.Text == typeNamespace).Items.Add(new NodePickerItem(typeName, type));
            }
        }

        private void ViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is NodePickerItem nodePickerItem)
            {
                if (nodePickerItem.Value is Type type)
                {
                    DragDrop.DoDragDrop(this, type, DragDropEffects.Copy);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }

    public class NodePickerItem
    {
        public string Text { get; }

        public object Value { get; }

        public List<NodePickerItem> Items { get; }

        public NodePickerItem(string text, object value)
        {
            this.Text = text;
            this.Value = value;
            this.Items = new List<NodePickerItem>();
        }
    }
}
