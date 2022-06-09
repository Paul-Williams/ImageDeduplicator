using Data;
using Data.Models;
using ImageDeduper;
using PW;
using PW.Collections;
using PW.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using XnaFan.ImageComparison;

// See: https://blog.stephencleary.com/2012/11/async-producerconsumer-queue-using.html
// See: https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/walkthrough-creating-a-dataflow-pipeline

namespace TestBed
{
  internal static class Program
  {




    public static void Main()
    {
      try
      {

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);

      }
      Console.WriteLine("Finished");
      Console.ReadLine();

    }

    //private static void FindDupesById()
    //{
    //  using var context = new DataContext();

    //  var img = context.Images.Find(294648);//.First();

    //  var param = new SqlParameter("@bytes", img.Bytes);
    //  var paras = new SqlParameter[] { param };

    //  var s1 = DateTime.Now.Ticks;

    //  var result = context.Database.SqlQuery<ImageEntity>("FuzzyMatchBytes @bytes", paras).ToArray();

    //  var s2 = DateTime.Now.Ticks;

    //  Console.WriteLine(TimeSpan.FromTicks(s2 - s1).TotalMilliseconds.ToString() + "ms");

    //  Console.WriteLine(result.Length.ToString());
    //}


    //private static void FindDupesInSingleDirectory(string directory)

    //{

    //  var groups = new List<ImageGroup>(1000);

    //  var dupeIds = new List<int>(1000);

    //  using var context = new DataContext();

    //  var imgs = context.Images.Where(x => x.Path.StartsWith(directory)).ToArray();


    //  var s1 = DateTime.Now.Ticks;
    //  //var groupIndex = 1;



    //  foreach (var img in imgs)
    //  {
    //    if (dupeIds.Contains(img.Id)) continue;
    //    var param1 = new SqlParameter("@bytes", img.Bytes)
    //    {
    //      SqlDbType = System.Data.SqlDbType.Binary,
    //      Size = 256
    //    };
    //    var param2 = new SqlParameter("@directory", directory)
    //    {
    //      SqlDbType = System.Data.SqlDbType.NVarChar,
    //      Size = 2000
    //    };
    //    var result = context.Database.SqlQuery<ImageEntity>("FuzzyMatchBytesWithinDirectory @bytes,@directory", param1, param2).ToList();
    //    if (result.Count > 1)
    //    {
    //      groups.Add(new ImageGroup(result));
    //      dupeIds.AddRange(result.Select(x => x.Id));
    //    }
    //  }

    //  var s2 = DateTime.Now.Ticks;


    //  Console.WriteLine(TimeSpan.FromTicks(s2 - s1).TotalMilliseconds.ToString() + "ms");


    //  var counter = new Accumulator(1);

    //  foreach (var group in groups)
    //  {
    //    Console.WriteLine($"Group: {counter.Next.ToString()}");
    //    group.ForEach(x => Console.WriteLine(x.Path));
    //    Console.WriteLine();
    //  }

    //}
    //private static async Task BasicLoop()
    //{
    //  var output = new List<ImageInfoDTO>();
    //  var files = GetImageEnumerator();

    //  await Task.Run(() =>
    // {
    //   foreach (var file in files)
    //   {
    //     using (var image = Image.FromFile(file.FullName))
    //     {
    //       var thumbnail = Thumbnail.FromImage(image);
    //       output.Add(new ImageInfoDTO(Guid.NewGuid(), file.FullName, thumbnail.Matrix.ToVector(), DateTime.Now.Ticks, DateTime.Now, DateTime.Now));
    //     }
    //   }
    // });
    //}

    //private static async Task ProcessUsingParrallelAsync()
    //{

    //  var output = new ConcurrentBag<ImageInfoDTO>();

    //  await Task.Run(() =>
    //  {
    //    var files = GetImageEnumerator();//.Where(file => file.EndsWith(@"\69.jpg")).ToArray();
    //    var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

    //    Parallel.ForEach(files, parallelOptions,
    //      file =>
    //      {
    //        using (var image = Image.FromFile(file.FullName))
    //        {
    //          var hash = Thumbnail.FromImage(image).Matrix.ToVector();
    //          // TODO: Id (GUID) is not being used -- SQL Server is currently generating it's own Id (Integer)
    //          output.Add(new ImageInfoDTO(Guid.NewGuid(), file.FullName, hash, file.Length, file.CreationTime, file.LastWriteTime));
    //        }
    //      });

    //    AddToDB(output);


    //  });

    //}


    //private static IEnumerable<FileInfo> GetImageEnumerator() =>
    //  new DirectoryInfo(@"C:\Windows\Web\Screen\").EnumerateGdiSupportedImages(SearchOption.AllDirectories);

    //private static async Task ProcessUsingDataFlowBlocksAsync()
    //{
    //  var output = new ConcurrentBag<ImageInfoDTO>();//new List<ImageInfoDTO>();

    //  var openImageBlock = NewOpenImageBlock;
    //  var createInfoBlock = NewCreateInfoBlock;
    //  var outputBlock = new ActionBlock<ImageInfoDTO>((dto) => output.Add(dto));

    //  var linkOptions = new DataflowLinkOptions() { PropagateCompletion = true };
    //  openImageBlock.LinkTo(createInfoBlock, linkOptions);
    //  createInfoBlock.LinkTo(outputBlock, linkOptions);

    //  var files = GetImageEnumerator();


    //  // TODO: Would this run faster using Parallel.ForEach() ?
    //  //       Is currently only using 35-50% CPU
    //  // On reflection I don't think it would make much difference. 
    //  // Surely this will already be waiting on the opening and processing of the image
    //  // as the bounded capacity is only 16. - Might be worth a try though... 
    //  var ProduceAsync = Task.Run(async () =>
    //  {
    //    foreach (var file in files) await openImageBlock.SendAsync(file.FullName);
    //    openImageBlock.Complete();
    //  });


    //  await Task.WhenAll(ProduceAsync, outputBlock.Completion);

    //  foreach (var dto in output) Console.WriteLine(dto.Path);
    //  Console.WriteLine(output.Count);
    //  Console.ReadLine();
    //}


    ///// <summary>
    ///// Takes an image path and returns the path and opened image
    ///// </summary>
    //static TransformBlock<string, (string, Image)> NewOpenImageBlock
    //    => new TransformBlock<string, (string, Image)>(path => (path, Image.FromFile(path)), new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 8, BoundedCapacity = 16 });

    ///// <summary>
    ///// Takes an image path and corresponding image and returns an ImageInfoDTO object
    ///// </summary>
    //static TransformBlock<(string, Image), ImageInfoDTO> NewCreateInfoBlock =>
    //  new TransformBlock<(string, Image), ImageInfoDTO>((input) =>
    //  {
    //    var (path, image) = input;
    //    var thumbnail = Thumbnail.FromImage(image);
    //    image.Dispose();
    //    //Console.WriteLine("Disposed: " + path);
    //    var dto = new ImageInfoDTO(Guid.NewGuid(), path, thumbnail.Matrix.ToVector(), DateTime.Now.Ticks, DateTime.Now, DateTime.Now);
    //    return dto;
    //  }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 8, BoundedCapacity = 16 });


    //private static byte[] Get73()
    //{
    //  using (var con = new SqlConnection("Server=Paul1;Database=Images;Trusted_Connection=yes;"))
    //  {
    //    con.Open();
    //    using (var cmd = con.CreateCommand())
    //    {
    //      cmd.CommandText = "SELECT Hash FROM [FileHashes] WHERE Id=73";
    //      cmd.CommandType = System.Data.CommandType.Text;

    //      return (byte[])cmd.ExecuteScalar();
    //    }


    //  }
    //}


    //private static void AddToDB(IEnumerable<ImageInfoDTO> dtos)
    //{
    //  Guard.NotNull(dtos, nameof(dtos));

    //  using (var con = new SqlConnection("Server=Paul1;Database=Images;Trusted_Connection=yes;"))
    //  {
    //    con.Open();
    //    using (var cmd = con.CreateCommand())
    //    {
    //      cmd.CommandText = "INSERT INTO [FileHashes] (Path,SizeBytes,Created,LastModified,Hash) VALUES (@path,@size,@created,@lastmodified,@hash)";
    //      cmd.CommandType = System.Data.CommandType.Text;

    //      foreach (var dto in dtos)
    //      {
    //        cmd.Parameters.Clear();
    //        cmd.Parameters.AddRange(dto.GetParams());
    //        cmd.ExecuteNonQuery();
    //      }
    //    }
    //  }
    //}

    //private static SqlParameter[] GetParams(this ImageInfoDTO dto)
    //  => new[] {
    //    new SqlParameter("@path", System.Data.SqlDbType.NVarChar,508 ){ Value=dto.Path },
    //    new SqlParameter("@size", System.Data.SqlDbType.BigInt  ){ Value=dto.Size},
    //    new SqlParameter("@created", System.Data.SqlDbType.DateTime ){ Value=dto.CreateDate},
    //    new SqlParameter("@lastmodified",System.Data.SqlDbType.DateTime){ Value=dto.EditDate},
    //    new SqlParameter("@hash", System.Data.SqlDbType.Binary,256 ){ Value=dto.Hash}
    //    };

  }

}
