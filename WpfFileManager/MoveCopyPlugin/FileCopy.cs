using System;
using System.IO;

namespace MoveCopyPlugin
{
    public class FileCopy
    {
        private readonly int mChunkSize;
        private double mPrevProgress;

        public FileCopy(int chunkSize)
        {
            mChunkSize = chunkSize;
        }

        public void Copy(string from, string to)
        {
            var fromStream = new FileStream(from, FileMode.Open, FileAccess.Read);
            var toStream = new FileStream(to, FileMode.CreateNew, FileAccess.ReadWrite);
            CopyStream(fromStream, toStream);
            fromStream.Dispose();
            toStream.Dispose();
        }

        public void CopyStream(Stream from, Stream to)
        {
            var length = from.Length;
            double read = 0;
            var buffer = new byte[mChunkSize];
            var bytesRead = from.Read(buffer, 0, mChunkSize);
            while (bytesRead > 0)
            {
                read += bytesRead;
                to.Write(buffer, 0, bytesRead);
                bytesRead = from.Read(buffer, 0, mChunkSize);
                ReportProgress(read, length);
            }
        }

        private void ReportProgress(double read, long length)
        {
            var progress = (int )((read/length)*100);
            if (Math.Abs(mPrevProgress - progress) >= 1)
            {
                OnProgress(progress);
                mPrevProgress = progress;
            }
        }

        public event EventHandler<double> Progress;

        protected virtual void OnProgress(double e)
        {
            var handler = Progress;
            if (handler != null)
                handler(this, e);
        }
    }
}

    