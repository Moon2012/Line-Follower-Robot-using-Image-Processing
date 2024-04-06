using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge.Imaging;
using AForge;
using Point = AForge.Point;

namespace Robot.ImageProcessing
{
    public class BlobAnalysis
    {
        public BlobAnalysis(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            Analyse();
        }

        public double     ShiftfromCentre;
        public double     TurnAngle;
        public Direction  TurnDirection;
        public AForge.Point  CentreOfGravity;
        public double     CentreOfGravityAngle;

        List<Point> TurnPoints = new List<Point>();

        public Bitmap bitmap;
        List<Line> Lines;
        List<Line> VerticalLines;
        List<Line> IncompleteLines;
        public Bitmap LinesMap;

        public void Analyse()
        {
            GetCentreofGravity();
            CalculateAngleAndShift();
        }

        public List<Line> GetLines()
        {
            List<Line> Lines = new List<Line>();

            Bitmap b = bitmap.ToGrayScale();
            b.ConvertToBinary();

            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinWidth = 70;
            blobCounter.MinHeight = 70;
            // set ordering options
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            blobCounter.ProcessImage(b);
            Blob[] blobs = blobCounter.GetObjectsInformation();

                



            // process each blob
            foreach (Blob blob in blobs)
            {
                List<IntPoint> leftPoints, rightPoints, edgePoints = new List<IntPoint>();

                // get blob's edge points
                blobCounter.GetBlobsLeftAndRightEdges(blob, out leftPoints, out rightPoints);


                IntPoint p1 = rightPoints[0];
                IntPoint p2 = rightPoints[rightPoints.Count - 1];
                IntPoint p3 = leftPoints[0];
                IntPoint p4 = leftPoints[leftPoints.Count - 1];




                if (p1.X > 0 && p2.X > 0 && p1.X < 317 && p2.X < 317)

                    Lines.Add(new Line(p1, p2));

                if (p3.X > 0 && p4.X > 0 && p3.X < 317 && p4.X < 317)
                    Lines.Add(new Line(p3, p4));

            }

            foreach (Line l in Lines)
            {

                if ((l.Start.Y < 3) && (l.End.Y > 237))
                {
                    l.Vertical = true;
                }


                if ((l.Start.X < 3) && (l.End.X > 317))
                {
                    l.Horizontal = true;
                }

                if (!(l.Horizontal || l.Vertical))
                {
                    l.partial = true;
                }

            }

            return Lines;

        }
        public void CalculateAngleAndShift()
        {

            Lines = GetLines();

            ShiftfromCentre = Math.Round(((160 - CentreOfGravity.X)/160) * 100, 0) ;


            if (CentreOfGravity.X >= 160)
                TurnDirection = Direction.Right;
            else
                TurnDirection = Direction.Left;

            // Get Angle to vertical from Centre of Gravity

            int cX1 = (int)CentreOfGravity.X;
            int cY1 = (int) CentreOfGravity.Y;

            int cX2 = 160;
            int cY2 = 239;

            double cTheta = Math.Atan2(cY1 - cY2, cX1 - cX2);
                      
            Double ang = cTheta * 180 / 3.14;

            //Generally for right side angles it will be negative.
            // If angle is in negative means measurement is from opposite side.
            // It will be 180 - angle.
            // But we need for angle from vertical line.
            CentreOfGravityAngle = 90 - Math.Abs(ang);

            VerticalLines = new List<Line>();
            IncompleteLines = new List<Line>();

            foreach (Line l in Lines)
            {
                if(l.Vertical)
                {
                    VerticalLines.Add(l);
                }
                if(l.partial)
                {
                    IncompleteLines.Add(l);
                }
            }


            if (VerticalLines.Count > 0)
            {
                // Line 1

                int X1 = VerticalLines[0].Start.X;
                int Y1 = VerticalLines[0].Start.Y;

                int X2 = VerticalLines[0].End.X;
                int Y2 = VerticalLines[0].End.Y;

                double theta1 = Math.Atan2(Y1 - Y2, X1 - X2);
                double ang1 = theta1 * 180 / 3.14;
               
                //Generally for right side angles it will be negative.
                // If angle is in negative means measurement is from opposite side.
                // It will be 180 - angle.
                // But we need for angle from vertical line.
                TurnAngle = 90 - Math.Abs(ang1);
                TurnAngle = Math.Round(TurnAngle, 0);

                TurnAngle = Math.Abs(TurnAngle);






                // Line 2 - This will be always 90 degree.

                int X3 = VerticalLines[0].End.X;
                int Y3 = VerticalLines[0].End.Y;

                int X4 = X3;
                int Y4 = VerticalLines[0].Start.Y;

                double theta2 = Math.Atan2(Y3 - Y4, X3 - X4);

                double diff = Math.Abs(theta1 - theta2);

                double diffdegree = diff * 180 / 3.14;
                double ang2 = theta2 * 180 / 3.14;


               
                
            }

            if(IncompleteLines.Count > 1)
            {
                for (int i = 0; i < IncompleteLines.Count - 1; i++)
                {
                    Point p = GetIntersectionPoints(IncompleteLines[i], IncompleteLines[i+1]);
                    if (p.X > 0 && p.Y > 0)
                        TurnPoints.Add(p);
                }
            }
            CreateinesMap();
        }

        public void CreateinesMap()
        {
            Bitmap bmap = new Bitmap(320, 240, bitmap.PixelFormat);
            using (Graphics g = Graphics.FromImage(bmap))
            {
                // Draw processed Lines.
                foreach (Line l in Lines)
                {
                    g.DrawLine(new Pen(Color.Black), l.Start.X, l.Start.Y, l.End.X, l.End.Y);
                }



                // Draw Centre Line
                g.DrawLine(new Pen(Color.Red), 160, 0, 160, 239);

                // Draw Centre of Gravity Line

                g.DrawLine(new Pen(Color.Blue), CentreOfGravity.X, 0, CentreOfGravity.X, 239);

                foreach (Line l in VerticalLines)
                {
                    g.DrawLine(new Pen(Color.Yellow), l.End.X, l.End.Y, l.End.X, l.Start.Y);
                }

                Font f = new Font("Calibri", 8, FontStyle.Regular);
                g.DrawString("Turn Angle: "+ TurnAngle , f, Brushes.Blue, new System.Drawing.Point(160, 20));
                g.DrawString("Shift: " + ShiftfromCentre +"%", f, Brushes.Green, new System.Drawing.Point(160, 40));
                g.DrawString("Direction: " + TurnDirection.ToString(), f, Brushes.Yellow, new System.Drawing.Point(160, 60));

                if(TurnPoints.Count > 0)
                {
                    foreach (Point p in TurnPoints)
                        FillCircle(g, Brushes.Red, p.X, p.Y, 3);
                }


            }

            LinesMap = bmap;
        }

        public static void DrawCircle(Graphics g, Pen pen,
                                 float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public Point GetIntersectionPoints(Line line1, Line line2)
        {
            float A1 = line1.End.Y - line1.Start.Y;
            float B1 = line1.Start.X - line1.End.X;
            float C1 = A1 * line1.Start.X + B1 * line1.Start.Y;

            //Line2
            float A2 = line2.End.Y - line2.Start.Y;
            float B2 = line2.Start.X - line2.End.X;
            float C2 = A2 * line2.Start.X + B2 * line2.Start.Y;

            float det = A1 * B2 - A2 * B1;

            if (det == 0)
            {
                return new Point(0, 0);     // null;//parallel lines
            }
            else
            {
                float x = (B2 * C1 - B1 * C2) / det;
                float y = (A1 * C2 - A2 * C1) / det;
                return new Point((int)x, (int)y);
            }
        }

        public System.Drawing.Point GetIntersectionPoints(System.Drawing.Point line1V1, System.Drawing.Point line1V2, System.Drawing.Point line2V1, System.Drawing.Point line2V2)
        {
            //Line1
            float A1 = line1V2.Y - line1V1.Y;
            float B1 = line1V1.X - line1V2.X;
            float C1 = A1 * line1V1.X + B1 * line1V1.Y;

            //Line2
            float A2 = line2V2.Y - line2V1.Y;
            float B2 = line2V1.X - line2V2.X;
            float C2 = A2 * line2V1.X + B2 * line2V1.Y;

            float det = A1 * B2 - A2 * B1;
            if (det == 0)
            {
                return new System.Drawing.Point(0, 0);     // null;//parallel lines
            }
            else
            {
                float x = (B2 * C1 - B1 * C2) / det;
                float y = (A1 * C2 - A2 * C1) / det;
                return new System.Drawing.Point((int)x, (int)y);
            }
        }

        public void GetCentreofGravity()
        {
            int shift = 0;
            Bitmap bmp = (Bitmap)bitmap.Clone();

            Bitmap b = bmp.ToGrayScale();
            b.ConvertToBinary();
            b.Invert();

            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinWidth = 70;
            blobCounter.MinHeight = 70;
            // set ordering options
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            blobCounter.ProcessImage(b);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            if (blobs.Length > 0)
            CentreOfGravity = blobs[0].CenterOfGravity;

        }
    }
}
