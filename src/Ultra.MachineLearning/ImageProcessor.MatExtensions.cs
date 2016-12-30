using OpenCvSharp;

namespace Ultra.MachineLearning
{
    public partial class ImageProcessor
    {
        public static Mat BitwiseAnd(this Mat mat1,Mat mat2)
        {
            var ret = new Mat();
            Cv2.BitwiseAnd(mat1,mat2,ret);
            return ret;
        }
    }
}
