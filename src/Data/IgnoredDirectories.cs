using static ImageDeduper.Globals;
using PW.Extensions;
using System;
using System.IO;
using System.Linq;
using PW.IO.FileSystemObjects;

namespace Data
{
  /// <summary>
  /// Directories to be ignored.
  /// </summary>
  public static class IgnoredDirectories
  {

    // HARDCODE
    private const string Ignore =
      @"P:\Porn\Exclude\|P:\Porn\From Games\|P:\Porn\Named\~\|P:\Porn\Tools\|P:\Porn\Caps\";

    /// <summary>
    /// WIP: Set of directories to be ignored by import and watcher.
    /// NB: Compared directory MUST have trailing slash '\'
    /// </summary>
    private static readonly string[] _ignoreList = Ignore.Split('|');

    //public static bool IsIgnored(string path)
    //{
    //  if (path is null) throw new ArgumentNullException(nameof(path));
    //  return path.StartsWithAny(_ignoreList, FileSystemPathComparison);
    //}


    public static bool IsIgnored(DirectoryPath directory!!)
    {
      return directory.Value.StartsWithAny(_ignoreList, StringComparison.OrdinalIgnoreCase); //IsIgnored(directory.Value);
    }

    public static bool IsIgnored(FilePath file!!)
    {
      return IsIgnored(file.DirectoryPath);//IsIgnored(file.DirectoryName);
    }


  }
}
