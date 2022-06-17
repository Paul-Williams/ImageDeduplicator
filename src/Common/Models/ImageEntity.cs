# nullable enable

using PW.Extensions;
using System;

namespace Data.Models
{
  [System.Diagnostics.DebuggerDisplay("{Path}")]
  public class ImageEntity
  {
    public const string TableName = "Images";

    public ImageEntity() { }

    public ImageEntity(int id) { Id = id; }

    public int Id { get; set; }


    public string Path { get; set; } = null!;

    public long FileSize { get; set; }

    public string FileSizeString  => FileSize.ToStringByteSize();


    public int Width { get; set; }
    public int Height { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime LastWriteTime { get; set; }
    public float HorizontalResolution { get; set; }
    public float VerticalResolution { get; set; }

    [System.ComponentModel.Browsable(false)]
    public byte[] Bytes { get; set; } = null!;


    /// <summary>
    /// Updates this <see cref="ImageEntity"/> with values from another instance. <see cref="ImageEntity.Id"/> is left unchanged.
    /// </summary>    
    public void MergeChangesFrom(ImageEntity other)
    {
      if (ReferenceEquals(this, other)) return;

      Bytes = other.Bytes;
      FileSize = other.FileSize;
      Height = other.Height;
      HorizontalResolution = other.HorizontalResolution;
      LastWriteTime = other.LastWriteTime;
      VerticalResolution = other.VerticalResolution;
      Width = other.Width;
      Path = other.Path;

    }

  }
}
