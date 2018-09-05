using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Roro.Workflow.Wpf
{
    public partial class NodePropertyEditor : Window
    {
        private IEditableNode _node => this.DataContext as IEditableNode;

        public NodePropertyEditor()
        {
            InitializeComponent();
        }

        public NodePropertyEditor(IEditableNode node) : this()
        {
            this.DataContext = node;
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._node.SyncArguments();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._node is StartNode)
            {
                this._node.Arguments.Add(new InArgument()
                {
                    Name = "Input " + (this._node.Arguments.Count + 1),
                    ArgumentType = Argument.Types.First()
                });
            }
            if (this._node is EndNode)
            {
                this._node.Arguments.Add(new OutArgument()
                {
                    Name = "Output " + (this._node.Arguments.Count + 1),
                    ArgumentType = Argument.Types.First()
                });
            }
            if (this._node is PreparationNode)
            {
                this._node.Arguments.Add(new InOutArgument()
                {
                    Name = "Variable " + (this._node.Arguments.Count + 1),
                    ArgumentType = Argument.Types.First()
                });
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            ArgumentsDataGrid.SelectedItems.Cast<Argument>().ToList()
                .ForEach(x => this._node.Arguments.Remove(x));
            if (ArgumentsDataGrid.SelectedItems.Count == 0)
            {
                ArgumentsDataGrid.SelectedItem = this._node.Arguments.LastOrDefault();
            }
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            this._node.Arguments.ToList()
                .Where(x => ArgumentsDataGrid.SelectedItems.Contains(x)).ToList()
                .ForEach(x => {
                    var index = this._node.Arguments.IndexOf(x);
                    this._node.Arguments.Move(index, Math.Max(index - 1, 0));
                });
            if (ArgumentsDataGrid.SelectedItems.Count == 0)
            {
                ArgumentsDataGrid.SelectedItem = this._node.Arguments.FirstOrDefault();
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            this._node.Arguments.ToList()
                .Where(x => ArgumentsDataGrid.SelectedItems.Contains(x))
                .Reverse().ToList()
                .ForEach(x => {
                    var index = this._node.Arguments.IndexOf(x);
                    this._node.Arguments.Move(index, Math.Min(index + 1, this._node.Arguments.Count - 1));
                });
            if (ArgumentsDataGrid.SelectedItems.Count == 0)
            {
                ArgumentsDataGrid.SelectedItem = this._node.Arguments.LastOrDefault();
            }
        }
    }
}
