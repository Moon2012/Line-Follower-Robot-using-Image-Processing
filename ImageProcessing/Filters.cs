using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Robot.ImageProcessing
{
    public static unsafe class Filters
    {
        public static Bitmap ToGrayScale(this Bitmap source)
        {
            Grayscale gFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            // apply the filter
            return gFilter.Apply(source);
        }

        public static void ConvertToBinary(this Bitmap source)
        {
            Threshold filter = new Threshold(40);
            // apply the filter
            filter.ApplyInPlace(source);

        }

        public static void CannyEdge(this Bitmap source)
        {
            AForge.Imaging.Filters.CannyEdgeDetector cfilter = new AForge.Imaging.Filters.CannyEdgeDetector();

            cfilter.ApplyInPlace(source);

        }

        public static void GaussianBlur(this Bitmap image)
        {
            GaussianBlur filter = new GaussianBlur(4, 11);
            // apply the filter
            filter.ApplyInPlace(image);
        }

        public static void BiggestBlob(this Bitmap image)
        {
            // create filter
            ExtractBiggestBlob filter = new ExtractBiggestBlob();
            // apply the filter
            Bitmap biggestBlobsImage = filter.Apply(image);
        }

        public static void FillHoles(this Bitmap image)
        {
            FillHoles filter = new FillHoles();
            filter.MaxHoleHeight = 20;
            filter.MaxHoleWidth = 20;
            filter.CoupledSizeFiltering = false;
            // apply the filter
            Bitmap result = filter.Apply(image);
        }

        public static void Invert(this Bitmap image)
        {
            // create filter
            Invert filter = new Invert();
            // apply the filter
            filter.ApplyInPlace(image);
        }

      
    }
}
