using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Xilium.CefGlue.WPF
{
    /// <summary>
    /// Interaction logic for WpfCefJSPrompt.xaml
    /// </summary>
    public partial class WpfCefJSPrompt : Window
    {
        public string Input { get { return this.inputTextBox.Text; } }

        public WpfCefJSPrompt(string message, string defaultText)
        {
            InitializeComponent();
            this.messageTextBlock.Text = message;
            this.inputTextBox.Text = defaultText;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
