#nullable enable

using Prism.Events;
using PW.IO.FileSystemObjects;
using System;
namespace ImageDeduper
{

  [PropertyChanged.AddINotifyPropertyChangedInterface()]
  internal class MainFormController : PW.WinForms.DataBinding.BindingObjectBase
  {

    public MainFormController(IEventAggregator ea)
    {
      //EA = ea ?? throw new ArgumentNullException(nameof(ea));
      LibraryPath = (DirectoryPath)@"P:\Porn";
      LibraryWatcher = new ImageLibraryWatcher(LibraryPath, ea);

      LibraryWatcherEnabled = true;
    }


    public enum ImageCompareMethods
    {
      ExactMatch,
      CloseMatch
    };

    private ImageLibraryWatcher LibraryWatcher { get; }
    //private IEventAggregator EA { get; }



    public ImageCompareMethods ImageCompareMethod { get; set; } = ImageCompareMethods.CloseMatch;


    public DirectoryPath LibraryPath { get; set; }

    /// <summary>
    /// String version of <see cref="LibraryPath"/>, used for control binding.
    /// </summary>
    public string LibraryPathString
    {
      get => (string)LibraryPath;
      set => LibraryPath = (DirectoryPath)value;
    }

    public bool LibraryWatcherEnabled
    {
      get => LibraryWatcher.Enabled;
      set => LibraryWatcher.Enabled = value;
    }

    public bool LibraryPathIsValid => LibraryPath.Exists;

    public bool CanStart => LibraryPathIsValid;


  }
}
