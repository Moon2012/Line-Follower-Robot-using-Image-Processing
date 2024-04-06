using AForge;
using AForge.Imaging;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Robot.ImageProcessing
{
    public unsafe static class BitmapExtensions
    {
       
        public static byte[,] ToPixelArray(this Bitmap subject)
        {
            int width = subject.Width;
            int height = subject.Height;

           
            byte[,]  pixels = new byte[width, height];
            byte[,]  original_pixels = new byte[width, height];
            


            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = subject.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int* colorData = (int*)bitmapData.Scan0.ToPointer();

            int x, y = 0;
            for (int i = 0; i < height * width; i++)
            {
                y = Math.DivRem(i, width, out x);
                Color c = Color.FromArgb(colorData[i]);
                byte luma = (byte)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                if (luma > 0) luma = 255;
                pixels[x, y] = luma;

            }


            subject.UnlockBits(bitmapData);
            return pixels;           
        }

        /// <summary>
        /// This method converts byte[,] bitmap data to bitmap and returns Bitmap
        /// </summary>
        /// <param name="subject"></param> This parameter is for image size Original Bitmap.
        /// <param name="pixels"></param>  This parameter is for pixel data.
        /// <returns></returns>

        public static Bitmap ToBitmap(this Bitmap subject, byte[,] pixels)
        {
            int width = subject.Width;
            int height = subject.Height;


            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int* colorData = (int*)bitmapData.Scan0.ToPointer();
            int x, y = 0;
            for (int i = 0; i < width * height; i++)
            {
                y = Math.DivRem(i, width, out x);
                int c = (int)pixels[x, y];
                colorData[i] = Color.FromArgb(c, c, c).ToArgb();
            }
            bitmap.UnlockBits(bitmapData);
            return bitmap;
            
        }

        private static Bitmap CreateNonIndexedImage(System.Drawing.Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public static double GetTurnAngle(this Bitmap bmp)
        {
            Bitmap b = bmp.ToGrayScale();
            b.ConvertToBinary();
            b.CannyEdge();
            LineCurve lc = new LineCurve(b);
            lc.TraceLines();
            b = CreateNonIndexedImage(b);
            double turnAngle = 0;



            if (lc.LineCurves.Count > 0)
            {
                Curve c = lc.LineCurves[0];
                if (c.LCurve.Count > 0)
                {

                    // Line 1

                    int X1 = c.LCurve[0].X;
                    int Y1 = c.LCurve[0].Y;

                    int X2 = c.LCurve[c.LCurve.Count - 1].X;
                    int Y2 = c.LCurve[c.LCurve.Count - 1].Y;

                    double theta1 = Math.Atan2(Y1 - Y2, X1 - X2);

                    // Line 2

                    int X3 = c.LCurve[0].X;
                    int Y3 = c.LCurve[0].Y;

                    int X4 = X3;
                    int Y4 = c.LCurve[c.LCurve.Count - 1].Y;

                    // Graphics.FromImage(b).DrawLine(new Pen(Color.Green, 1), X3, Y3, X4, Y4);

                    double theta2 = Math.Atan2(Y3 - Y4, X3 - X4);

                    //double diff = Math.Abs(theta1 - theta2);

                    double diff = Math.Abs(theta1 - theta2);

                    double diffdegree = diff * 180 / 3.14;
                    turnAngle = Math.Round(diffdegree, 2);


                }
            }

            return turnAngle;
        }

        public static int GetShift(this Bitmap bmp)
        {
            int shift = 0;

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
            
                     

            AForge.IntPoint p1 = new AForge.IntPoint((int)blobs[0].CenterOfGravity.X, 0);
            AForge.IntPoint p2 = new AForge.IntPoint((int)blobs[0].CenterOfGravity.X, 239);

            
            AForge.IntPoint p3 = new IntPoint(160, 0);
            AForge.IntPoint p4 = new IntPoint(160, 239);

            shift = (int)blobs[0].CenterOfGravity.X - 160;
            return shift;

        }

        public static System.Drawing.Point Intersect(System.Drawing.Point line1V1, System.Drawing.Point line1V2, System.Drawing.Point line2V1, System.Drawing.Point line2V2)
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
                return new System.Drawing.Point(0, 0);// null;//parallel lines
            }
            else
            {
                float x = (B2 * C1 - B1 * C2) / det;
                float y = (A1 * C2 - A2 * C1) / det;
                return new System.Drawing.Point((int)x,(int)y);
            }
        }

        public static void ProcessLines(Bitmap bmp)
        {
            List<Line> Lines = new List<Line>();

            Bitmap b = bmp.ToGrayScale();
            b.ConvertToBinary();

            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinWidth = 60;
            blobCounter.MinHeight = 60;
            // set ordering options
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            blobCounter.ProcessImage(bmp);
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



                Lines.Add(new Line(p1, p2));
                Lines.Add(new Line(p3, p4));

            }





        }

        
        public static List<Line> GetLines(this Bitmap bmp)
        {
            List<Line> Lines = new List<Line>();

            Bitmap b = bmp.ToGrayScale();
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




                if (p1.X > 0 && p2.X > 0  && p1.X < 317 && p2.X < 317)

                    Lines.Add(new Line(p1, p2));

                if (p3.X > 0 && p4.X > 0 && p3.X < 317 && p4.X < 317)
                    Lines.Add(new Line(p3, p4));

            }

            foreach(Line l in Lines)
            {
               
                if ((l.Start.Y < 3) && (l.End.Y > 237))
                { 
                    l.Vertical = true;
                }
                    

                if ((l.Start.X < 3) && (l.End.X > 317))
                {
                    l.Horizontal = true;
                }

                if(!(l.Horizontal || l.Vertical))
                {
                    l.partial = true;
                }

            }

            return Lines;

        }



    }
}
