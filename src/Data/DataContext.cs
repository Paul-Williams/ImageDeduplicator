using Data.Models;
using PW.FailFast;
using PW.IO.FileSystemObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.SqlClient;
using System.Linq;

namespace Data;

public class DataContext : DbContext
{
  const string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Data.DataContext;Integrated Security=True;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
  public DataContext() : base(ConnectionString)
  {
    //Database.SetInitializer(new DataContextInitializer());

#if DEBUG
    Database.Log = s => System.Diagnostics.Trace.WriteLine(s);
#endif
  }

  public DbSet<ImageEntity> Images { get; set; } = null!;

  public DbSet<Models.Library> Libraries { get; set; } = null!;

  public DbSet<Models.Directory> Directories { get; set; } = null!;

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
    Guard.NotNull(modelBuilder, nameof(modelBuilder));

    // Otherwise gets the stupid name of 'ImageInfoes'
    modelBuilder.Entity<ImageEntity>().ToTable(ImageEntity.TableName);

    // Define the primary key as non-clustered, as we are going to cluster on the 'Bytes' field.
    modelBuilder.Entity<ImageEntity>().HasKey(x => x.Id, config => config.IsClustered(false));

    // Define the 'Bytes' field as binary(256) and create a clustered index on it.
    modelBuilder.Entity<ImageEntity>().Property(x => x.Bytes)
      .HasColumnType("binary")
      .HasMaxLength(256)
      .IsFixedLength()
      .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Bytes") { IsClustered = true, IsUnique = false }));

    modelBuilder.Entity<ImageEntity>()
      .Property(x => x.Path).IsRequired().HasMaxLength(2000);

    modelBuilder.Entity<ImageEntity>().HasIndex(x => x.Path).IsUnique().HasName("IX_Path");
  }

  /// <summary>
  /// Determines if an image already exists with the specified file path.
  /// </summary>
  public bool ImageExists(FilePath file) =>
    Images.Select(x => x.Path).Where(path => path == file.Value).FirstOrDefault() != null;


  private static SqlParameter IdParamFactory(int id) =>
    new("@id", id) { SqlDbType = System.Data.SqlDbType.Int };

  public ImageEntity? FuzzyMatchBytesFirstOrDefault(ImageEntity newImage)
  {
    return newImage.Id switch
      {
        < 0 => throw new InvalidProgramException($"{nameof(ImageEntity)}'s id less than zero. Unable to fuzzy match on unsaved entities."),
        0 => throw new InvalidProgramException($"{nameof(ImageEntity)}'s id is zero. This is not supported."),
        _ => Database.SqlQuery<ImageEntity>("FuzzyMatchBytesFirst @id", IdParamFactory(newImage.Id)).FirstOrDefault()
      };
  }


  public ImageEntity? ImageByPath(string path) => Images.Where(x => x.Path == path).FirstOrDefault();

  public List<ImageEntity> ImagesWithinDirectory(string directoryPath) => Images.Where(x => x.Path.StartsWith(directoryPath)).ToList();



  private const string PathParamName = "@path";

  private static SqlParameter CreateImagePathParam(string value) => CreateImagePathParam(PathParamName, value);

  private static SqlParameter CreateImagePathParam(string name, string value) =>
    new(name, value)
    {
      SqlDbType = System.Data.SqlDbType.NVarChar,
      Size = 2000
    };

  /// <summary>
  /// Immediately deletes all images within the specified directory AND sub-directories. Call to <see cref="DbContext.SaveChanges"/> is not required.
  /// </summary>
  /// <param name="directory"></param>
  public int DeleteAllImages(DirectoryPath directoryPath)
  {
    var sql = $"DELETE FROM [{ImageEntity.TableName}] WHERE [{nameof(ImageEntity.Path)}] LIKE @LikeValue";

    // If the directory path does not end with a back-slash, then we may inadvertently delete images with a similar name.
    // E.g. "DELETE WHERE [PATH] LIKE 'C:\MyImages\Folder1'" 
    // Would delete all images from 'C:\MyImages\Folder1', but would also delete the image 'C:\MyImages\Folder1Thumbnail.jpg'
    var likeValue = directoryPath.Value[^1] == '\\' ? directoryPath + "%" : directoryPath + @"\%";

    return Database.ExecuteSqlCommand(sql, CreateImagePathParam("@LikeValue", likeValue));
  }
  /// <summary>
  /// Fires an SQL Command to immediately delete the specified image. Call to <see cref="DbContext.SaveChanges"/> is not required.
  /// </summary>
  public int DeleteImage(FilePath filePath) =>
    Database.ExecuteSqlCommand($"DELETE FROM [{ImageEntity.TableName}] WHERE [{nameof(ImageEntity.Path)}]={PathParamName}", CreateImagePathParam((string)filePath));

  public int ChangeImagePath(FilePath newPath, FilePath oldPath)
  {
    var sql = $"UPDATE [{ImageEntity.TableName}] SET [{nameof(ImageEntity.Path)}]=@newPath WHERE {nameof(ImageEntity.Path)}=@oldPath";

    return Database.ExecuteSqlCommand(sql, CreateImagePathParam("@newPath", (string)newPath), CreateImagePathParam("@oldPath", (string)oldPath));
  }
}


//public class DataContextInitializer : CreateDatabaseIfNotExists<DataContext>
//{
//  protected override void Seed(DataContext context)
//  {
//    base.Seed(context);
//  }
//}




