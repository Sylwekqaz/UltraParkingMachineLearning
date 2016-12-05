namespace Inż.Model
{
    public class ImageFeatures
    {
        public int SaturatedPixels { get; set; }
        public int EdgePixels { get; set; }
        public int MaskPixels { get; set; }
        public bool? IsOccupied { get; set; }
        public float EdgePixelsRatio => (float) EdgePixels/MaskPixels;
        public float SaturatedPixelsRatio => (float) SaturatedPixels/MaskPixels;
    }
}