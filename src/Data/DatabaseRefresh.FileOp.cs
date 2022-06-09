# nullable enable

using PW.IO.FileSystemObjects;

namespace Data
{
  public partial class DatabaseRefresh
  {
    private struct FileOp
    {
      public FilePath File;
      public DbOperation Operation;
      public FileOp(FilePath file, DbOperation operation) { File = file; Operation = operation; }

    }

  }

}
