using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ImageDeduper
{
  public static class TraceHelper
  {
    //See: https://www.infoq.com/articles/csharp-nullable-reference-case-study/

    [Conditional("TRACE")]
    public static void WriteLineThreadId(string message = "", [CallerMemberName] string caller = "")
      => Trace.WriteLine($"TID: {System.Environment.CurrentManagedThreadId}: {caller} : {message}");

  }

  public static class DebugHelper
  {

    [Conditional("DEBUG")]
    public static void WriteLineThreadId(string message = "", [CallerMemberName] string caller = "")
      => Debug.WriteLine($"TID: {System.Environment.CurrentManagedThreadId}: {caller} : {message}");


  }


}
