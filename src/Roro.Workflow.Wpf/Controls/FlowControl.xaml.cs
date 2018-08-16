using System.Windows;
using System.Windows.Controls;

namespace Roro.Workflow.Wpf
{
    public partial class FlowControl : UserControl
    {
        private Flow _flow => this.DataContext as Flow;

        public FlowControl()
        {
            InitializeComponent();
        }

        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBlock).DataContext is Page selectedPage)
            {
                this.myTabControl.SelectedItem = selectedPage;
                if (MessageBoxResult.Yes == MessageBox.Show(string.Format("Delete '{0}' page?", selectedPage.Name), string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    this._flow.Pages.Remove(selectedPage);
                }
            }
        }

        public void AddPageButton_Click(object sender, RoutedEventArgs e)
        {
            this._flow.Pages.Add(new Page(string.Empty));
        }
    }
}
