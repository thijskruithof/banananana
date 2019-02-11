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
        public String Target { get; set; }
        public TaskControl ParentTask { get; set; }


        public ExternalLinkControl(TaskControl inParentTask)
        {
            ParentTask = inParentTask;

            InitializeComponent();
        }


        public WorkspaceData.ExternalLink GetWorkspaceLinkData()
        {
            WorkspaceData.ExternalLink data = new WorkspaceData.ExternalLink();
            data.Target = Target;

            return data;
        }

        public void SetWorkspaceLinkData(WorkspaceData.ExternalLink inData)
        {
            Target = inData.Target;
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
            ParentTask.DeleteExternalLink(this);
        }

        private void LinkImage_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            targetMenuItem.Header = Target;
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditExternalLinkWindow window = new EditExternalLinkWindow(Target);
            window.Owner = this.ParentTask.ParentPile.ParentWindow;

            if (window.ShowDialog() == true)
            {
                Target = window.Target;
            }
        }

        private void linkImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Target);
        }
    }
}
