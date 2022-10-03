using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PongOnline.Model
{
    public class Ball
    {
        public Canvas canvas;
        public Player player;
        public Rectangle element;

        public int dv = 5;
        public int dx;
        public int dvx;
        public int dy;
        public int dvy;

        public Ball(Canvas canvas, Player player, Rectangle element, int dx, int dy)
        {
            this.canvas = canvas;
            this.player = player;
            this.element = element;
            this.dx = dx;
            this.dy = dy;
        }

        public void Move()
        {
            double X = Canvas.GetLeft(element);
            double Y = Canvas.GetTop(element);
            if (X + dx >= canvas.ActualWidth || X + dx < 0 ||
                player.thisRacket.isIntersect(X + dx, Y) ||
                player.opponentRacket.isIntersect(X + dx, Y))
            {
                dx *= -1;
            }
            if (Y + dy < 0 || Y + dy >= canvas.ActualHeight ||
                player.thisRacket.isIntersect(X, Y + dy) ||
                player.opponentRacket.isIntersect(X, Y + dy))
            {
                dy *= -1;
            }
            Canvas.SetLeft(element, X + dx);
            Canvas.SetTop(element, Y + dy);
        }
    }
}
