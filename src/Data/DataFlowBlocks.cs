using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Data;

public static class DataFlowBlocks
{

  private class JustNewItemFilter
  {
    private readonly Dictionary<string, HashSet<string>> _directoryFiles;

    public JustNewItemFilter()
    {
      _directoryFiles = GetDictionary();
    }

    private static Dictionary<string, HashSet<string>> GetDictionary()
    {
      var r = new Dictionary<string, HashSet<string>>();
      using (var dc = new DataContext())
      {
        var grouped = dc.Images
          .Select(ii => ii.Path)
          .ToList()
          .Select(p => (Directory: Path.GetDirectoryName(p), FileName: Path.GetFileName(p)))
          .GroupBy(x => x.Directory);

        foreach (var group in grouped)
        {
          var directory = group.Key;
          var files = group.Select(g => g.FileName).ToList();
          r.Add(group.Key!, new HashSet<string>(files, StringComparer.OrdinalIgnoreCase));
        }

      }
      return r;
    }


    public bool Exists(FileInfo file)
    {
      return _directoryFiles.TryGetValue(file.DirectoryName!, out var files) && files.Contains(file.Name);
    }

  }


  public static BufferBlock<FileInfo> JustNewFilterBlock()
  {
    var block = new BufferBlock<FileInfo>();

    // Discard any items which alrady exist in the database.
    block.LinkTo(DataflowBlock.NullTarget<FileInfo>(), new JustNewItemFilter().Exists);

    return block;

  }


}
