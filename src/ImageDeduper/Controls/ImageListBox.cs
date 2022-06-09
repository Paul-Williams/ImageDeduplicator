////Moved code to: PW.WinForms.Controls

//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Windows.Forms;
//using XnaFan.ImageComparison;

//// See: https://stackoverflow.com/questions/472897/c-sharp-can-i-display-images-in-a-list-box

//* TODO:
// * Remove items from dictionary when list content changes.
// * Use BackColor brush when filling item background rectangle.
// */

//namespace ImageDeduper
//{
//  internal class ImageListBox : ListBox
//  {
//    const int PicHeight = 120;
//    const int PicPadding = 4;
//    const int PicBottomMargin = 4;

//    public ImageListBox()
//    {
//      DrawMode = DrawMode.OwnerDrawFixed;
//      ItemHeight = PicHeight + (PicPadding * 2) + PicBottomMargin;
//      IntegralHeight = false;
//      BorderStyle = BorderStyle.None;
//    }

//    private readonly Dictionary<IPreviewImage, Image> Thumbs = new Dictionary<IPreviewImage, Image>();

//    private const int SelectionRectanglePenWidth = 4;
//    private readonly Pen SelectedItemBorderPen = new Pen(SystemColors.HotTrack, SelectionRectanglePenWidth);
//    private Pen NotSelectedItemBorderPen;
//    private Brush BackgroundBrush;

//    protected override void OnDrawItem(DrawItemEventArgs e)
//    {
//      if (e.Index < 0 || Items.Count == 0) return;
//      ItemHeight = PicHeight + (PicPadding * 2) + PicBottomMargin;

//      if (Items[e.Index] is IPreviewImage ip)
//      {
//        if (BackgroundBrush is null) BackgroundBrush = new SolidBrush(BackColor);
//        if (NotSelectedItemBorderPen is null) NotSelectedItemBorderPen = new Pen(Color.White, SelectionRectanglePenWidth);
//        e.Graphics.FillRectangle(BackgroundBrush, e.Bounds);

//        // Code to fill background
//        //e.Graphics.FillRectangle(IsSelected(e.State) ? SystemBrushes.HotTrack : Brushes.Black, e.Bounds);

//        Image preview = GetPreview(ip);
//        var imageTopLeft = GetImageDrawPoint(e.Bounds, preview.Width);
//        e.Graphics.DrawImage(preview, imageTopLeft);

//        var selectionRect = GetImageBoundsRectangle(imageTopLeft, preview.Size);

//        e.Graphics.DrawRectangle(IsSelected(e.State) ? SelectedItemBorderPen : NotSelectedItemBorderPen, selectionRect);

//        // DEBUG: Item size guide border
//        // e.Graphics.DrawRectangle(Pens.Black, e.Bounds);

//        //if (IsSelected(e.State)) e.Graphics.DrawRectangle(SelectionRectanglePen, selectionRect);

//      }
//    }

//    private static PointF GetImageDrawPoint(Rectangle listItemBounds, int imageWidth)
//    {
//      int x = (listItemBounds.Width - imageWidth) / 2;
//      int y = listItemBounds.Y + PicPadding;
//      return new PointF(x, y);
//    }

//    private static Rectangle GetImageBoundsRectangle(PointF topLeft, Size imageSize)
//    {
//      return new Rectangle(
//          (int)topLeft.X - SelectionRectanglePenWidth / 2,
//          (int)topLeft.Y - SelectionRectanglePenWidth / 2,
//          imageSize.Width + SelectionRectanglePenWidth,
//          imageSize.Height + SelectionRectanglePenWidth);
//    }


//    private static bool IsSelected(DrawItemState state) => (state & DrawItemState.Selected) == DrawItemState.Selected;

//    private Image GetPreview(IPreviewImage ip)
//    {
//      if (!Thumbs.TryGetValue(ip, out Image thumb))
//      {
//        using (var img = ip.Image)
//        {
//          var scale = (double)((PicHeight) - (PicPadding * 2)) / (img.Height - (PicPadding * 2));
//          thumb = img.Resize((int)(img.Width * scale), PicHeight - (PicPadding * 2));

//          //thumb = img.Resize((int)(img.Width * ((double)PicHeight - (Padding * 2) / img.Height)), PicHeight-(Padding*2));
//          Thumbs.Add(ip, thumb);
//        }
//      }

//      return thumb;
//    }

//    private void InitializeComponent()
//    {
//      this.SuspendLayout();
//      // 
//      // ImageListBox
//      // 
//      //this.Resize += new System.EventHandler(this.ImageListBox_Resize);
//      this.ResumeLayout(false);

//    }

//    protected override void OnResize(EventArgs e)
//    {
//      Invalidate();
//      base.OnResize(e);
//    }

//    //private void ImageListBox_Resize(object sender, System.EventArgs e)
//    //{
//    //  Invalidate();
//    //}
//  }
//}
