using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Roro.Workflow.Wpf
{
    public partial class FlowControl : UserControl
    {
        private IEditableFlow _flow => this.DataContext as IEditableFlow;

        public FlowControl()
        {
            InitializeComponent();
        }

        public void AddPageButton_Click(object sender, RoutedEventArgs e)
        {
            this._flow.AddPage(new Page(string.Empty));
        }

        private void ClosePageButton_Click(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBlock).DataContext is IEditablePage pageToRemove)
            {
                this.myTabControl.SelectedItem = pageToRemove;
                if (MessageBoxResult.Yes == MessageBox.Show(string.Format("Delete '{0}' page?", pageToRemove.Name), string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    this._flow.RemovePage(pageToRemove);
                }
            }
        }
    }
}
