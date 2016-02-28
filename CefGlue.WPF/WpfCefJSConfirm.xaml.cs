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
    /// Interaction logic for WpfCefJSConfirm.xaml
    /// </summary>
    public partial class WpfCefJSConfirm : Window
    {
        public WpfCefJSConfirm(string message)
        {
            InitializeComponent();
            this.messageTextBlock.Text = message;

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
