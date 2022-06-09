#nullable enable

using PW.IO.FileSystemObjects;
using System;

namespace ImageDeduper
{
  public class FileRenamePair : IEquatable<FileRenamePair>
  {
    public FilePath OldPath { get; }
    public FilePath NewPath { get; }

    public FileRenamePair(FilePath oldPath!!, FilePath newPath!!)
    {
      OldPath = oldPath;
      NewPath = newPath;
    }

    public override bool Equals(object? obj)
    {
      if (obj is null) return false;
      if (obj.GetType() != typeof(FileRenamePair)) return false;
      var other = (FileRenamePair)obj;
      return (NewPath == other.NewPath && OldPath == other.OldPath);
    }

    public override int GetHashCode() => PW.Helpers.Misc.GetCompositeHashCode(OldPath, NewPath);

    public static bool operator ==(FileRenamePair left, FileRenamePair right) => left is not null && left.Equals(right);

    public static bool operator !=(FileRenamePair left, FileRenamePair right) => !(left == right);

    public bool Equals(FileRenamePair? other) => Equals(other);

    public static FileRenamePair From(FilePath oldPath, FilePath newPath) => new(oldPath, newPath);

  }
}
