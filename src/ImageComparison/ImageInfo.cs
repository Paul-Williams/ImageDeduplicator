//using PW.Diagnostics;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Threading.Tasks;

//namespace XnaFan.ImageComparison
//{

//  public class ImageInfo
//  {


//    public ImageInfo(string filePath) : this (new System.IO.FileInfo(filePath))
//    {
//    }

//    public ImageInfo(FileInfo fileInfo)
//    {
//      try
//      {
//      using (var image = Image.FromFile(fileInfo.FullName))
//      {
//        Bytes = Thumbnail.GetBytes(image);
//        FileInfo = new ImageFileInfo(fileInfo, image);
//      }
//      }
//      catch 
//      {

//        throw;
//      }


//    }



//    // HACK
//    public ImageInfo(ImageFileInfo fileInfo, byte[] bytes)
//    {
//      FileInfo = fileInfo;
//      Bytes = bytes;
//    }

//    public ImageFileInfo FileInfo { get; }

    
    

//      public byte[] Bytes { get; }

//    public static List<ImageInfo> UnsortedList(List<FileInfo> imageFiles)
//    {
//      var result = new List<ImageInfo>(imageFiles.Count);

//      // Without limiting the to 'ProcessorCount' was getting 'out of memory' on some images
//      // The exception was thrown by g.DrawImage() in ImageExtensions.Resize()
//      // Don't currently know why. Always appeared to be thrown on same images. 
//      // Limited the number of concurrent operations may just be a lucky 'fix'.
//      // Commenting out the g.[IMAGEQUALITY] lines had no effect.
//      var ops = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount*2 }; //DEBUG TODO Remove: *2

//      ////var createListCodeTimer = new CodeTimer("Non-parallel create ImageInfo list");
//      ////foreach (var imagePath in imagePaths) result.Add(new ImageInfo(imagePath));
//      ////createListCodeTimer.Stop();


//      var createListCodeTimer = new CodeTimer("Parallel create ImageInfo list");

//      Parallel.ForEach(imageFiles, ops,
//        () => new List<ImageInfo>(),  //local state initializer
//        (imagePath, pls, local) =>    //loop body
//        {
//          try { local.Add(new ImageInfo(imagePath)); }
//          catch (Exception ex) { Debug.WriteLine($"{imagePath}\n{ex.Message}"); }
//          return local;
//        },
//        local =>                      //local to global state combiner
//        {
//          lock (result)
//          {
//            result.AddRange(local);
//          }
//        }
//      );

//      createListCodeTimer.Stop();


//      Trace.WriteLine("SortedList::imagePaths.Count:= " + imageFiles.Count);
//      //Trace.WriteLine("SortedList::result.Count:= " + result.Count);

//      return result;

//    }

//    //public static List<ImageInfo> SortedList(List<FileInfo> imageFiles)
//    //{
//    //  var result = UnsortedList(imageFiles);
//    //  result.Sort(ImageInfoComparer.Instance);
//    //  return result;
//    //}



//  }
//}
