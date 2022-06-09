#nullable enable

using Data.Models;
using PW.FailFast;

namespace XnaFan.ImageComparison
{
  public class ExactMatchComparer : IImageMatchComparer
  {
    public bool Match(ImageEntity x, ImageEntity y)
    {
      Guard.NotNull(x, nameof(x));
      Guard.NotNull(y, nameof(y));
      return ImageInfoComparer.Instance.Compare(x, y) == 0;
    }
  }
}
