using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.ImageProcessing
{
    public  class LineCurve
    {
        public List<Curve> LineCurves = new List<Curve>();
        byte[,] Pixels;
        int width;
        int height;
        Bitmap Source;

        public LineCurve(Bitmap source)
        {
            Source = source;
            Pixels = source.ToPixelArray();
            width  = source.Width;
            height = source.Height;
        }

        public void TraceLines()
        {
            Curve c;
            int j = height - 2;

          
            for (int i = 0; i< width - 2; i++)
            {
              
                    if (Pixels[i, j] == 255)
                    {
                        c = new Curve();
                        c.Start = new Point(i,j);
                        Trace(i, j,  c);
                        LineCurves.Add(c);

                    }
                
            }

        }

        private void Trace(int i, int j, Curve c)
        {
            Point nextPixel = new Point(i, j);
            BottomUpTrace(nextPixel, c);
        }

        private void GetNeighbour(Point nextPixel, Curve c)
        {
            int i = nextPixel.X;
            int j = nextPixel.Y;
            
            if(j > 1 && j < height -2 && i > 1 && i< width - 2)
            {
               if( Pixels[i-1,j -1] == 255)
                {
                    c.LCurve.Add(nextPixel);
                    nextPixel = new Point();
                    nextPixel.X = i - 1;
                    nextPixel.Y = j - 1;
                }

                if (Pixels[i , j - 1] == 255)
                {
                    c.LCurve.Add(nextPixel);
                    nextPixel = new Point();
                    nextPixel.X = i;
                    nextPixel.Y = j - 1;
                }

                if (Pixels[i + 1, j - 1] == 255)
                {
                    c.LCurve.Add(nextPixel);
                    nextPixel = new Point();
                    nextPixel.X = i + 1;
                    nextPixel.Y = j - 1;
                }

                GetNeighbour(nextPixel, c);

            }

        }

        private void BottomUpTrace(Point nextPixel, Curve c)
        {
            int i = nextPixel.X;
            int j = nextPixel.Y;

            int max_i = 318;
            int max_j = 238;

            bool found = false;


            if (i > 1 && i < width - 1)
            {
                if (j > 1 && j < height - 1)
                {

                    // Top

                    if (!found)
                    {
                        if (Pixels[i, j - 1] == 255)
                        {
                            c.LCurve.Add(nextPixel);
                            nextPixel = new Point();
                            nextPixel.X = i;
                            nextPixel.Y = j - 1;
                            found = true;
                        }
                    }

                    // Top Right
                    if (!found)
                    {
                        if (Pixels[i + 1, j - 1] == 255)
                        {
                            c.LCurve.Add(nextPixel);
                            nextPixel = new Point();
                            nextPixel.X = i + 1;
                            nextPixel.Y = j - 1;
                            found = true;
                        }
                    }


                    //Top Left
                    if (!found)
                    {
                        if (Pixels[i - 1, j - 1] == 255)
                        {
                            c.LCurve.Add(nextPixel);
                            nextPixel = new Point();
                            nextPixel.X = i - 1;
                            nextPixel.Y = j - 1;
                            found = true;
                        }
                    }

                    
                    if(found)
                     BottomUpTrace(nextPixel, c);
                    else 
                    {  
                        //break the loop     
                    }

                }

                // Here we have end point for j
                c.End = new Point(nextPixel.X, nextPixel.Y);
            }

                // Here we have end point for i

        }


    }

    public class Curve
    {
        public Point Start;
        public Point End;
        public List<Point> LCurve = new List<Point>();
       
    }



}
