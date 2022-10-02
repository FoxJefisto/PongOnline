using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PongOnline.Model
{
    public class Player
    {
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public Rectangle thisRacket { get; set; }
        public Rectangle opponentRacket { get; set; }


    }
}
