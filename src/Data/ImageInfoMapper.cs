# nullable enable

//using Data.Models;
//using System.Drawing;
//using XnaFan.ImageComparison;

//namespace Data
//{
//  public static class ImageInfoMapper
//  {
//    public static ImageInfoEntity ToEntity(this ImageInfoEntity imageInfo)
//    {
//      return new ImageInfoEntity()
//      {
//        Bytes = imageInfo.Bytes,
//        CreationTime = imageInfo.FileInfo.CreationTime,
//        FileSize = imageInfo.FileInfo.FileSize,
//        Height = imageInfo.FileInfo.Dimensions.Height,
//        HorizontalResolution = imageInfo.FileInfo.HorizontalResolution,
//        LastWriteTime = imageInfo.FileInfo.LastWriteTime,
//        Path = imageInfo.FileInfo.Path,
//        VerticalResolution = imageInfo.FileInfo.VerticalResolution,
//        Width = imageInfo.FileInfo.Dimensions.Width
//      };
//    }

//    public static ImageInfoEntity ToEntity(this XnaFan.ImageComparison.ImageInfo i, int id)
//    {
//      var r = ToEntity(i);
//      r.Id = id;
//      return r;
//    }


//    public static ImageInfo ToImageInfo(this ImageInfoEntity entity)
//    {
//      var info = new ImageFileInfo(
//        entity.Path,
//        entity.FileSize,
//        new Size(entity.Width, entity.Height),
//        entity.CreationTime,
//        entity.LastWriteTime,
//        entity.HorizontalResolution,
//        entity.VerticalResolution
//        );

//      return new ImageInfo(info, entity.Bytes);
//    }

//    public static ImageInfoEntity UpdateFrom(this ImageInfoEntity entity, ImageInfo ii)
//    {
//      entity.Bytes = ii.Bytes;
//      entity.CreationTime = ii.FileInfo.CreationTime;
//      entity.FileSize = ii.FileInfo.FileSize;
//      entity.Height = ii.FileInfo.Dimensions.Height;
//      entity.HorizontalResolution = ii.FileInfo.HorizontalResolution;
//      entity.LastWriteTime = ii.FileInfo.LastWriteTime;
//      entity.Path = ii.FileInfo.Path; //This should not have changed, but updated for completeness
//      entity.VerticalResolution = ii.FileInfo.VerticalResolution;
//      entity.Width = ii.FileInfo.Dimensions.Width;

//      return entity;
//    }

//  }


//}
