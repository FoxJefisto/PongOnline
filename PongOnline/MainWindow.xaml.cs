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
        public Player player;
        public Ball ball;
        public TCPClient tcp = new TCPClient("127.0.0.1", 8888);
        int racketSpeed = 15;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (player.MoveUp && Canvas.GetTop(player.thisRacket.element) > 0)
            {
                Canvas.SetTop(player.thisRacket.element, Canvas.GetTop(player.thisRacket.element) - racketSpeed);
            }
            if (player.MoveDown && Canvas.GetTop(player.thisRacket.element) + player.thisRacket.element.Height < MyCanvas.ActualHeight)
            {
                Canvas.SetTop(player.thisRacket.element, Canvas.GetTop(player.thisRacket.element) + racketSpeed);
            }
            if (player.MoveLeft && Canvas.GetLeft(player.thisRacket.element) - player.thisRacket.element.Width > 0)
            {
                Canvas.SetLeft(player.thisRacket.element, Canvas.GetLeft(player.thisRacket.element) - racketSpeed);
            }
            if (player.MoveRight && Canvas.GetLeft(player.thisRacket.element) + player.thisRacket.element.Width < MyCanvas.ActualWidth)
            {
                Canvas.SetLeft(player.thisRacket.element, Canvas.GetLeft(player.thisRacket.element) + racketSpeed);
            }
            var response = tcp.Update(Canvas.GetTop(player.thisRacket.element).ToString(), Canvas.GetLeft(player.thisRacket.element).ToString(),
    Canvas.GetTop(ball.element).ToString(), Canvas.GetLeft(ball.element).ToString(), ball.dy.ToString(), ball.dx.ToString());
            var coords = response.Split(' ');
            Canvas.SetTop(player.opponentRacket.element, double.Parse(coords[0]));
            Canvas.SetLeft(player.opponentRacket.element, double.Parse(coords[1]));
            Canvas.SetTop(ball.element, double.Parse(coords[2]));
            Canvas.SetLeft(ball.element, double.Parse(coords[3]));
            ball.dy = int.Parse(coords[4]);
            ball.dx = int.Parse(coords[5]);
            ball.Move();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            registerWindow.Owner = this;
            var answerDialog = registerWindow.ShowDialog();
            if (answerDialog.Value)
            {
                try
                {
                    tcp.Connect();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
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
                player = new Player();
                var leftRacket = new Racket(elemLeftRacket);
                var rightRacket = new Racket(elemRightRacket);
                if (playerId == "1")
                {
                    Canvas.SetTop(elemLeftRacket, (int)(MyCanvas.ActualHeight / 2));
                    Canvas.SetLeft(elemLeftRacket, 20);

                    player.thisRacket = leftRacket;
                    player.opponentRacket = rightRacket;
                }
                else
                {
                    Canvas.SetLeft(elemRightRacket, (int)(MyCanvas.ActualWidth - 20));
                    Canvas.SetTop(elemRightRacket, (int)(MyCanvas.ActualHeight / 2));
                    player.thisRacket = rightRacket;
                    player.opponentRacket = leftRacket;
                }
                player.thisRacket.element.Visibility = Visibility.Visible;
                player.opponentRacket.element.Visibility = Visibility.Visible;
                elBall.Visibility = Visibility.Visible;
                Canvas.SetLeft(elBall, (int)(MyCanvas.ActualWidth / 2));
                Canvas.SetTop(elBall, (int)(MyCanvas.ActualHeight / 2));
                ball = new Ball(MyCanvas, player, elBall, 10, 10);
                var response = tcp.Update(Canvas.GetTop(player.thisRacket.element).ToString(), Canvas.GetLeft(player.thisRacket.element).ToString(),
                    Canvas.GetTop(ball.element).ToString(), Canvas.GetLeft(ball.element).ToString(), ball.dy.ToString(), ball.dx.ToString());
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
                elemLeftRacket.Visibility = Visibility.Collapsed;
                elemRightRacket.Visibility = Visibility.Collapsed;
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
