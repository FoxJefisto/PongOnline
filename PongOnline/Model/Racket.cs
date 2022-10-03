using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PongOnline.Model
{
    public class Racket
    {
        public Racket(Rectangle element)
        {
            this.element = element;
        }

        public Rectangle element { get; set; }

        public bool isIntersect(double x, double y)
        {
            double X = Canvas.GetLeft(element);
            double Y = Canvas.GetTop(element);
            return (x >= X && x <= X + element.Width && y >= Y &&
                y <= Y + element.Height);
        }
    }
}
