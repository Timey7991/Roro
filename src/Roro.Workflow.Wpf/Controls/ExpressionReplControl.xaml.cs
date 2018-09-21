using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Roro.Workflow.Wpf
{
    public partial class ExpressionReplControl : UserControl
    {
        private IEditablePage _page => this.DataContext as IEditablePage;

        public ExpressionReplControl()
        {
            InitializeComponent();
            this._inputTextBox.Focus();
        }

        private void _inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this._inputTextBox.Text.Length > 0)
            {
                var code = this._inputTextBox.Text;
                object result;
                try
                {
                    result = Expression.Evaluate(code, this._page);
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                }
                this._inputTextBox.Clear();
                if (this._outputTextBox.Text.Length > 0)
                {
                    this._outputTextBox.AppendText(Environment.NewLine);
                    this._outputTextBox.AppendText(Environment.NewLine);
                }
                this._outputTextBox.AppendText("> " + code);
                this._outputTextBox.AppendText(Environment.NewLine);
                this._outputTextBox.AppendText(result?.ToString());
                this._outputTextBox.ScrollToEnd();
            }
        }
    }
}
