#nullable enable 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data.Models;
using System.IO;
using PW.IO;
using static PW.WinForms.MsgBox;
using PW.Extensions;

namespace ImageDeduper.Controls
{
  internal partial class DuplicateImageControl : UserControl
  {

    public event EventHandler? DeleteButton_Click;
    public event EventHandler? ReplaceOther_Click;

    public DuplicateImageControl()
    {
      InitializeComponent();      
    }

    public ImageEntity? ImageEntity { get; private set; }

    private void DuplicateImageControl_Load(object sender, EventArgs e)
    {

    }

    public void ProvideData(string caption, ImageEntity imageEntity)
    {
      CaptionLabel.Text = caption;
      ImageEntity = imageEntity;
      FilePathLinkLabel.Text = imageEntity.Path;
      PictureBox.ImageLocation = imageEntity.Path;
      DimensionsLabel.Text = imageEntity.Width.ToString() + "x" + imageEntity.Height.ToString();
      FileSizeLabel.Text = imageEntity.FileSizeString;
    }

    private void InternalDeleteButton_Click(object sender, EventArgs e)
    {
      DeleteButton_Click?.Invoke(this, e);
    }

    private void FilePathLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      if (ImageEntity is object) TryLaunch(ImageEntity.Path);
    }

    private static void TryLaunch(string path)
    {
      try { new FileInfo(path).Launch(); }
      catch (Exception ex) { ShowError(ex, "Error opening image"); }
    }

    private static void TrySelectInExplorer(ImageEntity? ie)
    {
      if (ie is null) return;
      try
      {
        var f = new FileInfo(ie.Path);
        if (f.Exists) f.SelectInExplorer();
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }
    }

    private void FindNewButton_Click(object sender, EventArgs e)
    {
      TrySelectInExplorer(ImageEntity);
    }

    private void ReplaceOtherButton_Click(object sender, EventArgs e)
    {
      ReplaceOther_Click?.Invoke(this, e);
    }
  }
}
