# nullable enable

using System.Drawing;

// Change for 2D -> 1D arrays

namespace XnaFan.ImageComparison
{
  internal static class Thumbnail
  {
    private const int ResizedPicHeight = 16;
    private const int ResizedPicWidth = ResizedPicHeight;
    // This value MUST have an absolute square-root. 
    // This is so that, for example, a 16 by 16 image thumbnail can be flattened into a 256 pixel array
    public static int Pixels => ResizedPicWidth * ResizedPicHeight; // (was 16x16 2D matrix)

    //public byte[] Bytes { get; } = new byte[Pixels];

    //private byte this[int i]
    //{
    //  get { return Bytes[i]; }
    //  set { Bytes[i] = value; }
    //}

    private static byte[] Differences(byte[] tn1, byte[] tn2)
    {
      var r = new byte[Pixels];
      for (int i = 0; i < Pixels; i++) r[i] = (byte)System.Math.Abs(tn1[i] - tn2[i]);
      return r;
    }

    public static float PercentDifference(byte[] array1, byte[] array2, byte threshold = 3)
    {
      var dif = Differences(array1, array2);
      int diffPixels = 0;
      for (int i = 0; i < Pixels; i++) if (dif[i] > threshold) { diffPixels++; }
      return (diffPixels / (float)Pixels) * 100;
    }


    private static Bitmap CreateResizedGreyscale(Image image)
    {
      using var resized = image.Resize(ResizedPicWidth, ResizedPicHeight);
      return (Bitmap)resized.ToGreyScale();
    }

    public static byte[] GetBytes(Image image)
    {
      using var grey = CreateResizedGreyscale(image);
      var bytes = new byte[Pixels];
      var pixel = 0;

      for (int y = 0; y < ResizedPicHeight; y++)
        for (int x = 0; x < ResizedPicWidth; x++)
          bytes[pixel++] = grey.GetPixel(x, y).R;

      return bytes;
    }
  }

}
