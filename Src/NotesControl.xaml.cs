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
    /// Interaction logic for NotesControl.xaml
    /// </summary>
    public partial class NotesControl : UserControl
    {
        private Workspace.Task mTask;
        private TaskControl mParentTaskControl;

        public NotesControl(TaskControl inParentTaskControl, Workspace.Task inTask)
        {
            mParentTaskControl = inParentTaskControl;
            mTask = inTask;

            InitializeComponent();
        }

        private void NotesImage_MouseEnter(object sender, MouseEventArgs e)
        {
            notesImage.Opacity = 1.0;
        }

        private void NotesImage_MouseLeave(object sender, MouseEventArgs e)
        {
            notesImage.Opacity = 0.25;
        }

        private void NotesImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenEditNotesControl();
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mParentTaskControl.DeleteNotesAndControl(this);
        }

        private void editMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenEditNotesControl();
        }

        private void OpenEditNotesControl()
        {
            mParentTaskControl.ParentPileControl.ParentWindow.OpenEditNotesControl(mTask);
        }
    }
}
