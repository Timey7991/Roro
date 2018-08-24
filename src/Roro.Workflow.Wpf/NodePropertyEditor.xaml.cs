using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Roro.Workflow.Wpf
{
    /// <summary>
    /// Interaction logic for NodePropertyEditor.xaml
    /// </summary>
    public partial class NodePropertyEditor : Window
    {
        public NodePropertyEditor()
        {
            InitializeComponent();
            actionTypeComboBox.ItemsSource = Node.GetActivityTypes().Where(x => typeof(IAction).IsAssignableFrom(x)).Select(x => (XmlTypeHelper)x);
        }

        public NodePropertyEditor(Node node) : this()
        {
            this.DataContext = node;
        }

        private void actionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (this.DataContext as ActionNode).SyncArguments();
        }
    }
}
