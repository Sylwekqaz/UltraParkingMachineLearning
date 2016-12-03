using System.IO;
using OpenCvSharp;

namespace Inż.utils
{
    public interface IIageSrc
    {
        Mat GetFrame();
    }

    class CameraSource : IIageSrc
    {
        private readonly FrameSource _cameraSource;

        public CameraSource(int deviceId)
        {
            _cameraSource = FrameSource.CreateCameraSource(deviceId);
        }

        public Mat GetFrame()
        {
            var mat = new Mat();
            _cameraSource.NextFrame(mat);
            return mat;
        }
    }

    class ImageSrc : IIageSrc
    {
        private readonly string src;

        public ImageSrc(string src)
        {
            if (!File.Exists(src))
            {
                throw new FileNotFoundException(src);
            }
            this.src = src;
        }

        public Mat GetFrame()
        {
            return new Mat(src);
        }
    }
}