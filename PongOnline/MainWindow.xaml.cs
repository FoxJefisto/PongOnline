using PongOnline.Connect;
using PongOnline.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace PongOnline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        public Player player = new Player();
        public TCPClient tcp = new TCPClient("127.0.0.1", 8888);
        int racketSpeed = 15;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if(player.MoveUp && Canvas.GetTop(player.thisRacket) > 0)
            {
                Canvas.SetTop(player.thisRacket, Canvas.GetTop(player.thisRacket) - racketSpeed);
                var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
                var coords = string.Join(' ', response);
                Canvas.SetTop(player.opponentRacket, Convert.ToDouble(coords[0]));
                Canvas.SetLeft(player.opponentRacket, Convert.ToDouble(coords[1]));
            }
            if (player.MoveDown && Canvas.GetTop(player.thisRacket) + player.thisRacket.Height < MyCanvas.ActualHeight)
            {
                Canvas.SetTop(player.thisRacket, Canvas.GetTop(player.thisRacket) + racketSpeed);
                var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
                var coords = string.Join(' ', response);
                Canvas.SetTop(player.opponentRacket, Convert.ToDouble(coords[0]));
                Canvas.SetLeft(player.opponentRacket, Convert.ToDouble(coords[1]));
            }
            if (player.MoveLeft && Canvas.GetLeft(player.thisRacket) - player.thisRacket.Width > 0)
            {
                Canvas.SetLeft(player.thisRacket, Canvas.GetLeft(player.thisRacket) - racketSpeed);
                var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
                var coords = string.Join(' ', response);
                Canvas.SetTop(player.opponentRacket, Convert.ToDouble(coords[0]));
                Canvas.SetLeft(player.opponentRacket, Convert.ToDouble(coords[1]));
            }
            if (player.MoveRight && Canvas.GetLeft(player.thisRacket) + player.thisRacket.Width < MyCanvas.ActualWidth / 2)
            {
                Canvas.SetLeft(player.thisRacket, Canvas.GetLeft(player.thisRacket) + racketSpeed);
                var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
                var coords = string.Join(' ', response);
                Canvas.SetTop(player.opponentRacket, Convert.ToDouble(coords[0]));
                Canvas.SetLeft(player.opponentRacket, Convert.ToDouble(coords[1]));
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            registerWindow.Owner = this;
            var answerDialog = registerWindow.ShowDialog();
            if (answerDialog.Value)
            {
                tcp.Connect();
                var playerId = tcp.Start();
                Debug.WriteLine(playerId);
                string status;
                do
                {
                    Thread.Sleep(1000);
                    status = tcp.Status();
                    Debug.WriteLine(status);
                } while (status != "True");
                MyCanvas.Focus();
                if (playerId == "1")
                {
                    Canvas.SetTop(leftRacket, MyCanvas.ActualHeight / 2);
                    Canvas.SetLeft(leftRacket, 20);
                    
                    player.thisRacket = leftRacket;
                    player.opponentRacket = rightRacket;
                }
                else
                {
                    Canvas.SetLeft(rightRacket, MyCanvas.ActualWidth - 20);
                    Canvas.SetTop(rightRacket, MyCanvas.ActualHeight / 2);
                    player.thisRacket = rightRacket;
                    player.opponentRacket = leftRacket;

                }
                player.thisRacket.Visibility = Visibility.Visible;
                player.opponentRacket.Visibility = Visibility.Visible;
                var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
                Thread.Sleep(5000);
                gameTimer.Interval = TimeSpan.FromMilliseconds(100);
                gameTimer.Tick += GameLoop;
                gameTimer.Start();
            }
        }

        private void MyCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                player.MoveUp = true;
            }
            if (e.Key == Key.S)
            {
                player.MoveDown = true;
            }
            if (e.Key == Key.A)
            {
                player.MoveLeft = true;
            }
            if (e.Key == Key.D)
            {
                player.MoveRight = true;
            }
            if (e.Key == Key.Escape)
            {
                leftRacket.Visibility = Visibility.Collapsed;
                rightRacket.Visibility = Visibility.Collapsed;
                gameTimer.Stop();
            }
        }

        private void MyCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.W)
            {
                player.MoveUp = false;
            }
            if (e.Key == Key.S)
            {
                player.MoveDown = false;
            }
            if (e.Key == Key.A)
            {
                player.MoveLeft = false;
            }
            if (e.Key == Key.D)
            {
                player.MoveRight = false;
            }
        }
    }
}
