using System;
using System.Windows.Media;

namespace In¿.Model
{
    public class ImageSet
    {
        public ImageSubset Free { get; set; }
        public ImageSubset Taken { get; set; }

        public ImageSubset this[int value]
        {
            get
            {
                switch (value)
                {
                    case 0:
                        return Free;
                    case 1:
                        return Taken;
                    default:
                        throw new IndexOutOfRangeException(nameof(value));
                }
            }
        }
    }
}