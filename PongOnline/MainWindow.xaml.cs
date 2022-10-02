using PongOnline.Connect;
using PongOnline.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
            if (player.MoveUp && Canvas.GetTop(player.thisRacket) > 0)
            {
                Canvas.SetTop(player.thisRacket, Canvas.GetTop(player.thisRacket) - racketSpeed);
            }
            if (player.MoveDown && Canvas.GetTop(player.thisRacket) + player.thisRacket.Height < MyCanvas.ActualHeight)
            {
                Canvas.SetTop(player.thisRacket, Canvas.GetTop(player.thisRacket) + racketSpeed);
            }
            if (player.MoveLeft && Canvas.GetLeft(player.thisRacket) - player.thisRacket.Width > 0)
            {
                Canvas.SetLeft(player.thisRacket, Canvas.GetLeft(player.thisRacket) - racketSpeed);
            }
            if (player.MoveRight && Canvas.GetLeft(player.thisRacket) + player.thisRacket.Width < MyCanvas.ActualWidth)
            {
                Canvas.SetLeft(player.thisRacket, Canvas.GetLeft(player.thisRacket) + racketSpeed);
            }
            var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
            var matchCoords = Regex.Match(response, @"([\d,.-]+) ([\d,.-]+)");
            var top = Convert.ToInt32(matchCoords.Groups[1].Value);
            var left = Convert.ToInt32(matchCoords.Groups[2].Value);
            Canvas.SetTop(player.opponentRacket, top);
            Canvas.SetLeft(player.opponentRacket, left);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            registerWindow.Owner = this;
            var answerDialog = registerWindow.ShowDialog();
            if (answerDialog.Value)
            {
                tcp.Connect();
                var playerId = tcp.Register();
                Debug.WriteLine(playerId);
                var cts = new CancellationTokenSource();
                var token = cts.Token;
                var taskFindOpponent = Task.Run(() => tcp.Wait(token));
                var findOpponentWindow = new FindOpponent(taskFindOpponent);
                findOpponentWindow.Owner = this;
                answerDialog = findOpponentWindow.ShowDialog();
                if (!answerDialog.Value)
                {
                    cts.Cancel();
                    tcp.Cancel();
                    return;
                }
                MyCanvas.Focus();
                if (playerId == "1")
                {
                    Canvas.SetTop(leftRacket, Math.Round(MyCanvas.ActualHeight / 2));
                    Canvas.SetLeft(leftRacket, 20);

                    player.thisRacket = leftRacket;
                    player.opponentRacket = rightRacket;
                }
                else
                {
                    Canvas.SetLeft(rightRacket, Math.Round(MyCanvas.ActualWidth - 20));
                    Canvas.SetTop(rightRacket, Math.Round(MyCanvas.ActualHeight / 2));
                    player.thisRacket = rightRacket;
                    player.opponentRacket = leftRacket;

                }
                player.thisRacket.Visibility = Visibility.Visible;
                player.opponentRacket.Visibility = Visibility.Visible;
                var response = tcp.Update(Canvas.GetTop(player.thisRacket).ToString(), Canvas.GetLeft(player.thisRacket).ToString());
                Thread.Sleep(2000);
                gameTimer.Interval = TimeSpan.FromMilliseconds(50);
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
                tcp.Cancel();
                gameTimer.Stop();
            }
        }

        private void MyCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
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
