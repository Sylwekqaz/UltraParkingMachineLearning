using System.IO;
using OpenCvSharp;

namespace Inż.utils
{
    public interface IIageSrc
    {
        Mat GetFrame();
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