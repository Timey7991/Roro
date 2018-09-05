using Microsoft.Win32;
using System.Linq;
using System.Windows;

namespace Roro.Workflow.Wpf
{
    public partial class MainWindow : Window
    {
        private IEditableFlow _flow => this.DataContext as IEditableFlow;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new Flow("Test-Flow");
            this._flow.Dispatch = App.Current.Dispatcher.Invoke;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var xmlFlow = this._flow.ToString();
            var dialog = new SaveFileDialog();

            dialog.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._flow.AddPage(new Page("Page " + this._flow.Pages.Count()));
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            this._flow.Run();
        }
    }
}
