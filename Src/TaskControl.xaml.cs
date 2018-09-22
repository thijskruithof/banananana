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
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        public delegate void TaskControlHandler(TaskControl inTask);

        public event TaskControlHandler OnDelete;

        public TaskControl()
        {
            InitializeComponent();

            //richTextBox.Focus();
            //Keyboard.Focus(richTextBox);
            //richTextBox.Document.Blocks.Clear();
            //richTextBox.Document.Blocks.Add(new Paragraph(new Run("Text")));
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            OnDelete(this);
        }

        private void richTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //richTextBox.IsEnabled = true;
            //Keyboard.Focus(richTextBox);
            //richTextBox.SelectAll();
        }

        private void richTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //richTextBox.IsEnabled = false;
        }
    }
}
