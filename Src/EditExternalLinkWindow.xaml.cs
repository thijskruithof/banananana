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

namespace Banananana
{
    /// <summary>
    /// Interaction logic for EditExternalLinkWindow.xaml
    /// </summary>
    public partial class EditExternalLinkWindow : Window
    {
        public String Target
        {
            get { return targetTextBox.Text; }
        }


        public EditExternalLinkWindow(String inTarget)
        {
            InitializeComponent();

            if (inTarget == null)
            {
                Title = "Add external link";
                targetTextBox.Text = "http://www.google.com";
            }
            else
            {
                Title = "Edit external link";
                targetTextBox.Text = inTarget;
            }

            targetTextBox.Focus();
            targetTextBox.Select(targetTextBox.Text.Length, 0);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
