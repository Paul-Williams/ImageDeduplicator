# nullable enable

using PW.FailFast;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace XnaFan.ImageComparison
{
  internal  static class ImageExtensions
  {
    /// <summary>
    /// The color-matrix needed to greyscale an image
    /// Source: http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    /// </summary>
    static readonly ColorMatrix _colorMatrix = new(new float[][]
    {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
    });

    /// <summary>
    /// Gets the lightness of the image in 256 sections (16x16)
    /// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    /// </summary>
    /// <param name="image">The image to get the lightness for</param>
    /// <returns>A doublearray (16x16) containing the lightness of the 256 sections</returns>
    public static Image ToGreyScale(this Image original)
    {
      Guard.NotNull(original, nameof(original));
      
      Bitmap? newBitmap = null;
      try
      {
        newBitmap = new Bitmap(original.Width, original.Height);
        using (var g = Graphics.FromImage(newBitmap))
        using (var attributes = new ImageAttributes())
        {
          attributes.SetColorMatrix(_colorMatrix);
          g.DrawImage(
            original, 
            new Rectangle(0, 0, original.Width, original.Height), 
            0, 
            0, 
            original.Width, 
            original.Height, 
            GraphicsUnit.Pixel, 
            attributes
            );
        }
        return newBitmap;
      }
      catch (Exception)
      {
        newBitmap?.Dispose();
        throw;
      }
    }

    /// <summary>
    /// Resizes an image
    /// </summary>
    /// <param name="originalImage">The image to resize</param>
    /// <param name="newWidth">The new width in pixels</param>
    /// <param name="newHeight">The new height in pixels</param>
    /// <returns>A resized version of the original image</returns>
    public static Image Resize(this Image originalImage, int newWidth, int newHeight)
    {
      Image? resized = null;
      try
      {
        resized = new Bitmap(newWidth, newHeight);
        using (var g = Graphics.FromImage(resized))
        {
          g.SmoothingMode = SmoothingMode.HighQuality;
          g.InterpolationMode = InterpolationMode.HighQualityBicubic;
          g.PixelOffsetMode = PixelOffsetMode.HighQuality;
          g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
        }
        return resized;
      }

      catch (Exception)
      {
        resized?.Dispose();
        throw;
      }

    }

  }
}
