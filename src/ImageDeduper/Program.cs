#nullable enable

using Data;
using Data.Models;
using Prism.Events;
using PW.IO.FileSystemObjects;
using PW.WinForms.Bootstrappers;
using System;
using System.Linq;
using System.Windows.Forms;
using Unity;

namespace ImageDeduper
{
  internal static class Program
  {
    private static PubSubEvents.DatabaseEvents.ImageDeletedEvent? ImageDeletedEvent { get; set; }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
#if !DEBUG
      try
      {
        // Register app for LaunchPad
        PW.AppRegistration.RegistrationManager.Register("Image Deduplicator", Application.ExecutablePath);
      }
      catch (Exception ex)
      {
        PW.WinForms.MsgBox.ShowError(ex, "Error creating app registration.");
      }
#endif



      var config = PrismBootstrapperConfiguration.Default();
      config.OnStartup = OnStartup;
      config.OnStartupComplete = OnStartupComplete;
      Application.Run(new PrismBootstrapper<MainForm>(config));
    }

    private static void OnStartup(IUnityContainer container)
    {
      container.RegisterType<MainForm>();
      container.RegisterType<DuplicateDetectedForm>();
      container.RegisterType<DuplicatesViewerForm>();

      //container.RegisterType<Func<DuplicateDetectedForm>>( 
      //  new InjectionFactory(c=> 
      //  new Func<DuplicateDetectedForm> (()=>c.Resolve<DuplicateDetectedForm>())));

      container.RegisterFactory<Func<DuplicateDetectedForm>>(
        c => new Func<DuplicateDetectedForm>(() => c.Resolve<DuplicateDetectedForm>()));

      container.RegisterFactory<Func<DuplicatesViewerForm>>(
        c => new Func<DuplicatesViewerForm>(() => c.Resolve<DuplicatesViewerForm>()));

      container.RegisterFactory<Func<DataContext>>(
        c => new Func<DataContext>(() => new DataContext()));

    }

    private static void OnStartupComplete(IEventAggregator eventAggregator)
    {
      ImageDeletedEvent = eventAggregator.GetEvent<PubSubEvents.DatabaseEvents.ImageDeletedEvent>();
      eventAggregator.GetEvent<PubSubEvents.FilesDeletedEvent>().Subscribe(OnFilesDeleted, ThreadOption.BackgroundThread);
    }


    /// <summary>
    /// Deletes files from database when they are deleted from the file system.
    /// </summary>    
    private static void OnFilesDeleted(FilePath[] files)
    {
      if (ImageDeletedEvent is null) throw new InvalidOperationException(nameof(ImageDeletedEvent) + " has not been instantiated.");

      using var context = new DataContext();
      foreach (var file in files)
      {
        // NB: This relies on the db string match being case insensitive.
        if (context.Images.FirstOrDefault(x => x.Path == file.Value) is ImageEntity entity)
        {
          context.Images.Remove(entity);
          ImageDeletedEvent.Publish((FilePath)entity.Path);
        }
      }
      context.SaveChanges();
    }


  }


}
