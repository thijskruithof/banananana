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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            TaskPile pile = new TaskPile();
            pile.VerticalAlignment = VerticalAlignment.Top;
            this.stackPanel.Children.Insert(0, pile);
        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskPile pile = new TaskPile();
            pile.VerticalAlignment = VerticalAlignment.Top;
            this.stackPanel.Children.Insert(this.stackPanel.Children.Count-1, pile);
        }
    }
}
