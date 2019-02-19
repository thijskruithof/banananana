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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Banananana
{
    /// <summary>
    /// Interaction logic for EditNotesControl.xaml
    /// </summary>
    public partial class EditNotesControl : UserControl
    {
        public EditNotesControl()
        {
            InitializeComponent();
        }

        private void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            closeButton.Opacity = 1.0;
        }

        private void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Opacity = 0.25;
        }

        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            (Parent as Grid).ColumnDefinitions[2].Width = new GridLength(0);
            (Parent as Grid).ColumnDefinitions[1].Width = new GridLength(0);
        }
    }
}
