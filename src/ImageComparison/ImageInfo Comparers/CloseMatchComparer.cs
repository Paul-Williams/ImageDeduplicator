#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using PW.FailFast;

namespace XnaFan.ImageComparison
{
  public class CloseMatchComparer : IImageMatchComparer
  {

    public byte Threshold { get; }
    public float MaxPercentMismatch { get; }

    public CloseMatchComparer()
    {
      Threshold = 3;
      MaxPercentMismatch = 10f;
    }

    public CloseMatchComparer(byte theshold, float maxPercentMismatch)
    {
      Threshold = theshold;
      MaxPercentMismatch = maxPercentMismatch;
    }

    public bool Match(ImageEntity x, ImageEntity y)
    {
      Guard.NotNull(x, nameof(x));
      Guard.NotNull(y, nameof(y));
      return (Thumbnail.PercentDifference(x.Bytes, y.Bytes, Threshold) < MaxPercentMismatch);
    }
  }
}
