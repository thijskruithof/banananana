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
    /// Interaction logic for TaskPile.xaml
    /// </summary>
    public partial class TaskPile : UserControl
    {
        public TaskPile()
        {
            InitializeComponent();
        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskControl tc = new TaskControl();
            stackPanel.Children.Insert(1, tc);

            //Keyboard.Focus(tc.richTextBox);
            tc.richTextBox.Focus();
            //Keyboard.Focus(tc.richTextBox);
        }
    }
}
