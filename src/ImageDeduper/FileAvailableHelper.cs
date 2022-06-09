//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ImageDeduper
//{
//  static class FileAvailableHelper
//  {

//    private static bool IsFileReady(string filename)
//    {
//      // If the file can be opened for exclusive access it means that the file
//      // is no longer locked by another process.
//      try
//      {
//        if (!File.Exists(filename)) return false;

//        using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
//          return inputStream.Length > 0;
//      }
//      catch (Exception)
//      {
//        return false;
//      }
//    }

//    /// <summary>
//    /// WARNING: This is thread-blocking
//    /// </summary>
//    public static bool WaitForReady(this FileInfo file, TimeSpan timeout) => WaitForFileReady(file.FullName, timeout);

//    /// <summary>
//    /// WARNING: This is thread-blocking
//    /// </summary>
//    public static bool WaitForFileReady(string filename, TimeSpan timeout)
//    {
//      var start = DateTime.Now.Ticks;
//      while (!IsFileReady(filename))
//      {
//        System.Threading.Thread.Sleep(100);
//        if (TimeSpan.FromTicks(DateTime.Now.Ticks - start) >= timeout) return false;
//        //if (((DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond) >= millisecondsTimeout) return false;
        
//      }
//      return true;
//    }

//  }
//}
