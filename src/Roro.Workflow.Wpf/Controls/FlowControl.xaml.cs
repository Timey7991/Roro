using System.Windows;
using System.Windows.Controls;

namespace Roro.Workflow.Wpf
{
    public partial class FlowControl : UserControl
    {
        private IEditableFlow _flow => this.DataContext as IEditableFlow;

        public FlowControl()
        {
            InitializeComponent();
        }

        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBlock).DataContext is IEditablePage selectedPage)
            {
                this.myTabControl.SelectedItem = selectedPage;
                if (MessageBoxResult.Yes == MessageBox.Show(string.Format("Delete '{0}' page?", selectedPage.Name), string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    this._flow.RemovePage(selectedPage);
                }
            }
        }

        public void AddPageButton_Click(object sender, RoutedEventArgs e)
        {
            this._flow.AddPage(new Page(string.Empty));
        }
    }
}
