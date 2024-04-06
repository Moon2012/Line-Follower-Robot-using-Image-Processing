using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Driver
{
    public class AngleRotationTime
    {
        public AngleRotationTime(double angle, int mTime)
        {
            Angle = angle;
            MilliSeconds = mTime;
        }

        public double Angle;
        public int MilliSeconds;
    }

    /// <summary>The Range class.</summary>
    /// <typeparam name="T">Generic parameter.</typeparam>
    public class Range
    {
        List<AngleRotationTime> DataRange = new List<AngleRotationTime>();

        public Range()
        {
            Init();
        }

        private void Init()
        {

            AngleRotationTime a1 = new AngleRotationTime(10, 100);
            AngleRotationTime a2 = new AngleRotationTime(20, 100);
            AngleRotationTime a3 = new AngleRotationTime(30, 100);
            AngleRotationTime a4 = new AngleRotationTime(40, 100);
            AngleRotationTime a5 = new AngleRotationTime(50, 100);
            AngleRotationTime a6 = new AngleRotationTime(60, 100);
            AngleRotationTime a7 = new AngleRotationTime(70, 100);
            AngleRotationTime a8 = new AngleRotationTime(80, 100);
            AngleRotationTime a9 = new AngleRotationTime(90, 100);

            DataRange.Add(a1);
            DataRange.Add(a2);
            DataRange.Add(a3);
            DataRange.Add(a4);
            DataRange.Add(a5);
            DataRange.Add(a6);
            DataRange.Add(a7);
            DataRange.Add(a8);
            DataRange.Add(a9);


        }

        public int GetValue(double angle)
        {
            foreach(AngleRotationTime a in DataRange)
            {
                if (a.Angle >= angle)
                    return a.MilliSeconds;
            }

            return 0;
        }


    }
}
