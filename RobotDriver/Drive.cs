using Robot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Robot.Driver
{
    public enum CarState
    {
        OnRoad,
        OutOfFocus,
        NeedAdjustment,
        None
    }

    public delegate void QDataAdded(string TurnSide, double TurnAngle, double Shift, int milliSeconds);

    public class Drive
    {
        DriveInstructionsQueue CommandQ = new DriveInstructionsQueue(25);

        BlobAnalysis LastBlobAnalysis;

        private string fspip = "192.168.1.6";
        string baseUrl = "http://192.168.1.6/rotate_wheel?wheel_dir=";
        public string FSPIP
        {
            get
            { 
                return fspip;
            }
            set
            {
                fspip = value;
                baseUrl = "http://" + fspip + "/rotate_wheel?wheel_dir=";
            }
        } 

        HttpClient client = new HttpClient();
        

        bool block = false;



        CarState state = CarState.None;
        Range AngleRange = new Range();

        public Drive()
        {

        }

        public event QDataAdded DataHandler;

       
        public void RaiseEventevent (string TurnSide, double TurnAngle, double Shift, int milliSeconds)
        {
            if (DataHandler != null) DataHandler(TurnSide, TurnAngle, Shift, milliSeconds);
        }

        public void DriveCar(BlobAnalysis bla)
        {

            if (bla.ShiftfromCentre < 100)
            {
                LastBlobAnalysis = bla;
               
            }
            else
            {
                LastBlobAnalysis = CommandQ.GetPreviousRecord();
            }
            CommandQ.Add(bla);

            Direction d = bla.TurnDirection;
                double shift = Math.Abs(bla.ShiftfromCentre);

            RaiseEventevent(bla.TurnDirection.ToString(), bla.TurnAngle, shift, 100);

            if (bla.ShiftfromCentre == 100 )
            {
                if (LastBlobAnalysis != null)
                {
                    if(LastBlobAnalysis.ShiftfromCentre < 100)

                     ReverseAdjustShift();
                }
                else
                {
                    ReverseAdjustShift();
                }

            }

            else if (shift > 40)
            {

                AdjustShift(bla.ShiftfromCentre);
            }

            else
            {
       
                StartMoving(bla);
            }

        }


        public void AdjustShift(double shift)
        {
            if(LastBlobAnalysis == null)
            {
                MoveStraight();
            }


            if (LastBlobAnalysis.TurnDirection == Direction.Right)
            {
                TurnRight();
            }


            if (LastBlobAnalysis.TurnDirection == Direction.Left)
            {
                TurnLeft();
            }

        }

        public void ReverseAdjustShift()
        {
            if (LastBlobAnalysis != null)
            {
                if (LastBlobAnalysis.TurnDirection == Direction.Right)
                {
                    TakeLeftReverse();
                   // MoveStraight(50);
                    TurnRight(150);
                }
                if (LastBlobAnalysis.TurnDirection == Direction.Left)
                {
                    //TurnLeft();
                    TakeRightReverse();
                    //MoveStraight(50);
                    TurnLeft(150);
                }
            }
            else
            {
                TakeReverse();
            }

        }

        public void AdjustShiftCentre(double shift)
        {
            BlobAnalysis privious_bla = CommandQ.GetPreviousRecord();
            if (privious_bla != null && privious_bla.TurnDirection == Direction.Right)
            {
                
                TurnRight(50);
                MoveStraight(50);
                TurnLeft(50);
            }
            if (privious_bla != null && privious_bla.TurnDirection == Direction.Left)
            {
               
                TurnLeft(50);
                MoveStraight(50);
                TurnRight(50);
            }

        }

        public void StartMoving(BlobAnalysis bla)
        {
           

            int mSec = AngleRange.GetValue(bla.TurnAngle);
            Direction td;

           // if (mSec == 0)
            if(bla.TurnAngle <= 20)
            {
                td = Direction.Straight;
                //mSec = 100;
            }
            else
                td = bla.TurnDirection;

            Move(bla.TurnAngle, td, mSec );

        }

        public void Move( double angle, Direction d, int millSeconds = 100)
        {

            string url = "";
            string sidetoRotate = "";

            switch (d)
            {
                case Direction.Right: sidetoRotate = "Left"; break;
                case Direction.Left: sidetoRotate = "Right"; break;
                case Direction.Straight: sidetoRotate = "Both"; break;
            }

            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "FORWARD&rotation_period=" + millSeconds + rotateString;


            SendCommand(url);

        }

        public void TakeReverse(int millSeconds = 100)
        {
            string url = "";
            string sidetoRotate = "Both";
            

            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "BACKWARD&rotation_period=" + millSeconds + rotateString;

            SendCommand(url);
        }

        public void TakeLeftReverse(int millSeconds = 100)
        {
            string url = "";
            string sidetoRotate = "Left";


            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "BACKWARD&rotation_period=" + millSeconds + rotateString;

            SendCommand(url);
        }

        public void TakeRightReverse(int millSeconds = 100)
        {
            string url = "";
            string sidetoRotate = "Right";


            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "BACKWARD&rotation_period=" + millSeconds + rotateString;

            SendCommand(url);
        }

        public void MoveStraight(int milliSeconds = 100)
        {
            string url = "";
            string sidetoRotate = "Both";
            

            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "FORWARD&rotation_period=" + milliSeconds + rotateString;

            SendCommand(url);
        }

        public void TurnRight(int milliSeconds = 100)
        {
            string url = "";
            string sidetoRotate = "Left";
            
            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "FORWARD&rotation_period=" + milliSeconds + rotateString;

            SendCommand(url);
        }

        public void TurnLeft(int milliSeconds = 100)
        {
            string url = "";
            string sidetoRotate = "Right";
            

            string rotateString = "&button_" + sidetoRotate.ToLower() + "=Rotate+" + sidetoRotate + "+wheel";
            url = baseUrl + "FORWARD&rotation_period=" + milliSeconds + rotateString;

            SendCommand(url);
        }

            

        public bool SendCommand(string url)
        {
            try
            {
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
                var task = Task.Run(() => client.SendAsync(msg));
                task.Wait();
                var response = task.Result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
