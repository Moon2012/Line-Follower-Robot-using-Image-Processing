using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Robot.ImageProcessing
{
    public class Camera
    {
        HttpClient client = new HttpClient();
        public string CameraIP { get; set; }

        public Camera()
        {

        }

        public Bitmap GetPicture()
        {
            try
            {
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, "http://"+ CameraIP + "/capture?");

                var task = Task.Run(() => client.SendAsync(msg));
                task.Wait();
                var res = task.Result.Content.ReadAsStreamAsync().Result;
                Bitmap bitmap = (Bitmap)Bitmap.FromStream(res);
                //bitmap.Save(@"D:\AditiProject\Arduino\Images\img1.bmp");
                return bitmap;

            }
            catch (Exception ex)
            {
                
                return null;
            }
        }
    }
}
