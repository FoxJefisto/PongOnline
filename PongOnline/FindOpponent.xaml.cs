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
using System.Windows.Threading;

namespace PongOnline
{
    /// <summary>
    /// Логика взаимодействия для FindOpponent.xaml
    /// </summary>
    public partial class FindOpponent : Window
    {
        Task taskFind { get; set; }
        DispatcherTimer gameTimer = new DispatcherTimer();
        public FindOpponent()
        {
            InitializeComponent();
        }

        public FindOpponent(Task task)
        {
            InitializeComponent();
            taskFind = task;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(1000);
            gameTimer.Tick += FindLoop;
            gameTimer.Start();
        }

        private void FindLoop(object? sender, EventArgs e)
        {
            if (taskFind.IsCompleted)
            {
                gameTimer.Stop();
                DialogResult = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            DialogResult = false;
        }


    }
}
