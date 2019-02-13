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
    /// Interaction logic for ExternalLinkControl.xaml
    /// </summary>
    public partial class ExternalLinkControl : UserControl
    {
        private Workspace.ExternalLink mExternalLink;
        private TaskControl mParentTaskControl;
        
        public Workspace.ExternalLink ExternalLink
        {
            get
            {
                return mExternalLink;
            }
        }

        public TaskControl ParentTaskControl
        {
            get
            {
                return mParentTaskControl;
            }
        }


        public ExternalLinkControl(TaskControl inParentTask, Workspace.ExternalLink inExternalLink)
        {
            mExternalLink = inExternalLink;
            mParentTaskControl = inParentTask;

            InitializeComponent();
        }

        private void LinkImage_MouseEnter(object sender, MouseEventArgs e)
        {
            linkImage.Opacity = 1.0;
        }

        private void LinkImage_MouseLeave(object sender, MouseEventArgs e)
        {
            linkImage.Opacity = 0.25;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mParentTaskControl.DeleteExternalLinkAndControl(this);
        }

        private void LinkImage_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            targetMenuItem.Header = mExternalLink.Target;
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditExternalLinkWindow window = new EditExternalLinkWindow(mExternalLink.Target);
            window.Owner = mParentTaskControl.ParentPileControl.ParentWindow;

            if (window.ShowDialog() == true)
                mExternalLink.Target = window.Target;
        }

        private void linkImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Open the target URL directly in our browser
            System.Diagnostics.Process.Start(mExternalLink.Target);
        }
    }
}
