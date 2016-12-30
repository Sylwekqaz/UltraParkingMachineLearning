namespace Contract.Model
{
    public class ImageFeatures
    {
        public int SaturatedPixels { get; set; }
        public int EdgePixels { get; set; }
        public int MaskPixels { get; set; }
        public bool IsOccupied { get; set; }
        public float EdgePixelsRatio => (float) EdgePixels/MaskPixels;
        public float SaturatedPixelsRatio => (float) SaturatedPixels/MaskPixels;
        public float SaturationMean { get; set; }
        public float SaturationStddev { get; set; }
        public float ValueMean { get; set; }
        public float ValueStddev { get; set; }
    }
}