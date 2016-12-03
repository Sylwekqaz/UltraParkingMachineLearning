using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Inż.utils
{
    partial class Gu
    {
        public static int[] GetSizes(this Mat mat)
        {
            return new[] { mat.Size(0), mat.Size(1) };
        }

        public static Mat BitwiseAnd(this Mat mat1,Mat mat2)
        {
            var ret = new Mat();
            Cv2.BitwiseAnd(mat1,mat2,ret);
            return ret;
        }
    }
}
