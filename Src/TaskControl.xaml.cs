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

        static int mCounter = 0;

        public enum EDragState
        {
            NoDraggingActive,
            IsBeingDragged,
            IsNotBeingDragged
        }

        private EDragState mDragState = EDragState.NoDraggingActive;

        private Brush mOriginalBackground;

        public EDragState DragState
        {
            get { return mDragState; }

            set
            {
                if (mDragState != value)
                {
                    switch (value)
                    {
                        case EDragState.NoDraggingActive:
                            RenderTransform = Transform.Identity;
                            border.BorderBrush = null;
                            border.Background = mOriginalBackground;
                            break;
                        case EDragState.IsBeingDragged:
                            RenderTransform = new TranslateTransform(12.0, 0.0);
                            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 220, 0));
                            border.Background = mOriginalBackground;

                            break;
                        case EDragState.IsNotBeingDragged:
                            RenderTransform = Transform.Identity;
                            border.BorderBrush = null;
                            border.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));

                            break;
                    }
                }

                mDragState = value;
            }
        }

        public TaskControl()
        {
            InitializeComponent();

            mOriginalBackground = border.Background;

            //richTextBox.Focus();
            //Keyboard.Focus(richTextBox);
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(String.Format("Task {0}", mCounter++))));
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
