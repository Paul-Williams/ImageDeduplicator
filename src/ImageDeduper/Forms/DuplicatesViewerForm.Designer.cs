using System;

namespace ImageDeduper
{
  partial class DuplicatesViewerForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;



    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.VerticalSplitContainer = new System.Windows.Forms.SplitContainer();
      this.GroupsListBox = new PW.WinForms.Controls.ImageListBox();
      this.GroupCountLabel = new System.Windows.Forms.Label();
      this.HorizontalSplitContainer = new System.Windows.Forms.SplitContainer();
      this.PictureBox = new System.Windows.Forms.PictureBox();
      this.DataGrid = new ImageDeduper.FileInfoGrid();
      ((System.ComponentModel.ISupportInitialize)(this.VerticalSplitContainer)).BeginInit();
      this.VerticalSplitContainer.Panel1.SuspendLayout();
      this.VerticalSplitContainer.Panel2.SuspendLayout();
      this.VerticalSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.HorizontalSplitContainer)).BeginInit();
      this.HorizontalSplitContainer.Panel1.SuspendLayout();
      this.HorizontalSplitContainer.Panel2.SuspendLayout();
      this.HorizontalSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.DataGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // VerticalSplitContainer
      // 
      this.VerticalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.VerticalSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.VerticalSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.VerticalSplitContainer.Name = "VerticalSplitContainer";
      // 
      // VerticalSplitContainer.Panel1
      // 
      this.VerticalSplitContainer.Panel1.Controls.Add(this.GroupsListBox);
      this.VerticalSplitContainer.Panel1.Controls.Add(this.GroupCountLabel);
      // 
      // VerticalSplitContainer.Panel2
      // 
      this.VerticalSplitContainer.Panel2.Controls.Add(this.HorizontalSplitContainer);
      this.VerticalSplitContainer.Size = new System.Drawing.Size(876, 551);
      this.VerticalSplitContainer.SplitterDistance = 192;
      this.VerticalSplitContainer.SplitterWidth = 10;
      this.VerticalSplitContainer.TabIndex = 2;
      // 
      // GroupsListBox
      // 
      this.GroupsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.GroupsListBox.BackColor = System.Drawing.Color.Black;
      this.GroupsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.GroupsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.GroupsListBox.FormattingEnabled = true;
      this.GroupsListBox.IntegralHeight = false;
      this.GroupsListBox.ItemHeight = 120;
      this.GroupsListBox.Location = new System.Drawing.Point(0, 35);
      this.GroupsListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.GroupsListBox.Name = "GroupsListBox";
      this.GroupsListBox.Size = new System.Drawing.Size(192, 516);
      this.GroupsListBox.TabIndex = 0;
      // 
      // GroupCountLabel
      // 
      this.GroupCountLabel.BackColor = System.Drawing.Color.Black;
      this.GroupCountLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.GroupCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.GroupCountLabel.Location = new System.Drawing.Point(0, 0);
      this.GroupCountLabel.Margin = new System.Windows.Forms.Padding(0);
      this.GroupCountLabel.Name = "GroupCountLabel";
      this.GroupCountLabel.Size = new System.Drawing.Size(192, 35);
      this.GroupCountLabel.TabIndex = 1;
      this.GroupCountLabel.Text = "label1";
      this.GroupCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // HorizontalSplitContainer
      // 
      this.HorizontalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.HorizontalSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.HorizontalSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.HorizontalSplitContainer.Name = "HorizontalSplitContainer";
      this.HorizontalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // HorizontalSplitContainer.Panel1
      // 
      this.HorizontalSplitContainer.Panel1.Controls.Add(this.PictureBox);
      // 
      // HorizontalSplitContainer.Panel2
      // 
      this.HorizontalSplitContainer.Panel2.Controls.Add(this.DataGrid);
      this.HorizontalSplitContainer.Size = new System.Drawing.Size(674, 551);
      this.HorizontalSplitContainer.SplitterDistance = 331;
      this.HorizontalSplitContainer.SplitterIncrement = 2;
      this.HorizontalSplitContainer.SplitterWidth = 10;
      this.HorizontalSplitContainer.TabIndex = 0;
      // 
      // PictureBox
      // 
      this.PictureBox.BackColor = System.Drawing.Color.Black;
      this.PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.PictureBox.Location = new System.Drawing.Point(0, 0);
      this.PictureBox.Name = "PictureBox";
      this.PictureBox.Size = new System.Drawing.Size(674, 331);
      this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.PictureBox.TabIndex = 0;
      this.PictureBox.TabStop = false;
      // 
      // DataGrid
      // 
      this.DataGrid.AllowUserToAddRows = false;
      this.DataGrid.AllowUserToDeleteRows = false;
      this.DataGrid.AllowUserToOrderColumns = true;
      this.DataGrid.AllowUserToResizeRows = false;
      this.DataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.DataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
      this.DataGrid.BackgroundColor = System.Drawing.SystemColors.Control;
      this.DataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.DataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.DataGrid.Location = new System.Drawing.Point(0, 0);
      this.DataGrid.Name = "DataGrid";
      this.DataGrid.RowHeadersVisible = false;
      this.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.DataGrid.ShowCellToolTips = false;
      this.DataGrid.Size = new System.Drawing.Size(674, 210);
      this.DataGrid.TabIndex = 0;
      this.DataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellDoubleClick);
      this.DataGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGrid_KeyDown);
      this.DataGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DataGrid_MouseClick);
      // 
      // DuplicatesViewerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(876, 551);
      this.Controls.Add(this.VerticalSplitContainer);
      this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "DuplicatesViewerForm";
      this.Text = "Duplicate Images (Grouped)";
      this.Load += new System.EventHandler(this.Form_Load);
      this.VerticalSplitContainer.Panel1.ResumeLayout(false);
      this.VerticalSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.VerticalSplitContainer)).EndInit();
      this.VerticalSplitContainer.ResumeLayout(false);
      this.HorizontalSplitContainer.Panel1.ResumeLayout(false);
      this.HorizontalSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.HorizontalSplitContainer)).EndInit();
      this.HorizontalSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.DataGrid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private PW.WinForms.Controls.ImageListBox GroupsListBox;
    private System.Windows.Forms.SplitContainer VerticalSplitContainer;
    private System.Windows.Forms.SplitContainer HorizontalSplitContainer;
    private System.Windows.Forms.PictureBox PictureBox;
    private FileInfoGrid DataGrid;
    private System.Windows.Forms.Label GroupCountLabel;
  }
}