using Robot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Driver
{
    public class DriveInstructionsQueue
    {
        int QueueSize;
        List<BlobAnalysis> blobanalysis;
        public DriveInstructionsQueue(int size)
        {
            QueueSize = size;
            blobanalysis = new List<BlobAnalysis>();
        }

        public void Add(BlobAnalysis bla)
        {
            RemoveOldRecord();
            blobanalysis.Add(bla);
        }

        public void RemoveOldRecord()
        {
            if (blobanalysis.Count >= QueueSize)
            {
                blobanalysis.RemoveAt(blobanalysis.Count - 1);
            }
        }
        public BlobAnalysis GetPreviousRecord()
        {
            if (blobanalysis.Count > 1)
            {
                return blobanalysis[blobanalysis.Count - 2];
            }
            else
                return null;

        }

        public BlobAnalysis[] GetList()
        {
            BlobAnalysis[] arr = new BlobAnalysis[QueueSize];
            blobanalysis.CopyTo(arr, 0);
            return arr;

        }
    }
}
