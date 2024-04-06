using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;

namespace Robot.ImageProcessing
{
    public class Line
    {
        public Line(IntPoint p1, IntPoint p2)
        {
            Start = p1;
            End = p2;
        }

        public IntPoint Start;
        public IntPoint End;

        public bool  Vertical;
        public bool  Horizontal;
        public bool partial;
    }
}
