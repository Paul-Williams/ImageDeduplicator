#nullable enable 

using Data.Models;
using PW.Extensions;
using static PW.Functional.FuncExtensions;
using PW.WinForms;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImageDeduper
{


  /// <summary>
  /// Used by the duplicate image view form. Represents a set of duplicate images.
  /// </summary>
  public sealed class ImageGroupCollection : INotifyPropertyChanged, IPreviewImage, IEnumerable<ImageEntity>
  {
    /// <summary>
    /// 
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 
    /// </summary>
    public BindingList<ImageEntity> Images { get; } = new BindingList<ImageEntity>();

#pragma warning disable CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
    Image? IPreviewImage.Image => FirstAvailableImage();  //File.Exists(Images[0].Path) ? Image.FromFile(Images[0].Path) : null;
#pragma warning restore CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).

    private Image? FirstAvailableImage() =>
      Images.Select(x => x.Path).FirstOrDefault(File.Exists).IfNotNull(TryLoadImage);

    // This should use the method, in PW.Common, that does not lock the file.
    private Image? TryLoadImage(string path) => ValueOrDefault(path, x=>Image.FromFile(path));


    /// <summary>
    /// Creates a new, empty, instance.
    /// </summary>
    public ImageGroupCollection()
    {
      Images.ListChanged += Images_ListChanged;
    }

    /// <summary>
    /// Creates a new instance containing a set of images.
    /// </summary>
    public ImageGroupCollection(IEnumerable<ImageEntity> images)
    {
      images.ForEach(Images.Add);
      Images.ListChanged += Images_ListChanged;
    }

    /// <summary>
    /// Handles the binding list changed event and informs listeners via <see cref="PropertyChanged"/> event.
    /// </summary>
    private void Images_ListChanged(object? sender, ListChangedEventArgs e) =>
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Images)));

    /// <summary>
    /// Adds images to the group
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<ImageEntity> items) => items.ForEach(Images.Add);

    /// <summary>
    /// Returns an enumeration of images in the group.
    /// </summary>
    public IEnumerator<ImageEntity> GetEnumerator() => Images.GetEnumerator();


    /// <summary>
    /// Returns an enumeration of images in the group.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => Images.GetEnumerator();
  }


}
