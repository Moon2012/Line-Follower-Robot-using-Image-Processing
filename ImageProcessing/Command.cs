using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.ImageProcessing
{
    public enum Direction
    {
        Right,
        Left,
        Straight
    }


    public class Command
    {
        public double    Angle;
        public double    Shift;
        public Direction MovementDirection;
    }
}
