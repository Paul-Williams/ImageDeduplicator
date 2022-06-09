# nullable enable

using System;
using System.IO;

namespace Data
{
  public static partial class DataContextHelper
  {
    //public class ChangeInfo
    //{
    //  public int Id { get; set; }
    //  public DateTime LastWriteTime { get; set; }
    //  public string Path { get; set; }

    //  public bool RequiresUpdate => !IsMissingInternal() && File.GetLastWriteTime(Path).Difference(LastWriteTime) > TimeSpan.FromMilliseconds(30);//ThirtySeconds;
    //  public bool IsMissing => IsMissingInternal();

    //  private bool? _missing;
    //  private bool IsMissingInternal()
    //  {
    //    if (!_missing.HasValue) _missing = File.Exists(Path) == false;
    //    return _missing.Value;
    //  }


    //}

  }
}
