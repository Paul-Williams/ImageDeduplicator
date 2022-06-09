//using System;
//using System.Drawing;
//using System.IO;

//namespace XnaFan.ImageComparison
//{
//  public class ImageFileInfo
//  {
//    public string Path { get; }
//    public long FileSize { get; }
//    public Size Dimensions { get; }
//    public DateTime CreationTime { get; }
//    public DateTime LastWriteTime { get; }
//    public float HorizontalResolution { get; }
//    public float VerticalResolution { get; }

//    public ImageFileInfo(FileInfo file, Image image)
//    {
//      Path = file.FullName;
//      FileSize = file.Length/1024;
//      Dimensions = image.Size;
//      CreationTime = file.CreationTime;
//      LastWriteTime = file.LastWriteTime;
//      HorizontalResolution = image.HorizontalResolution;
//      VerticalResolution = image.VerticalResolution;
//    }

//    public ImageFileInfo(string path, long fileSize, Size dimensions, DateTime creationTime, DateTime lastWriteTime, float horizontalResolution, float verticalResolution)
//    {
//      Path = path;
//      FileSize = fileSize;
//      Dimensions = dimensions;
//      CreationTime = creationTime;
//      LastWriteTime = lastWriteTime;
//      HorizontalResolution = horizontalResolution;
//      VerticalResolution = verticalResolution;
//    }

//  }
//}
