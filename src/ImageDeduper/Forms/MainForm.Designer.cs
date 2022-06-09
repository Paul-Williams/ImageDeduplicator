using ImageDeduper.Controls;

namespace ImageDeduper
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      //_watcher?.Dispose();
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.ImagesFolderTextBox = new System.Windows.Forms.TextBox();
      this.ImagesFolderBrowseButton = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.ShowDatabaseDuplicatesInViewerButton = new System.Windows.Forms.Button();
      this.UpdateDatabaseButton = new System.Windows.Forms.Button();
      this.LogTextBox = new System.Windows.Forms.TextBox();
      this.LogTextBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.LogContextMenu_Clear = new System.Windows.Forms.ToolStripMenuItem();
      this.UseStoreProcCheckBox = new System.Windows.Forms.CheckBox();
      this.ClearDupeDetectorQueueButton = new System.Windows.Forms.Button();
      this.LogTextBoxContextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // ImagesFolderTextBox
      // 
      this.ImagesFolderTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.ImagesFolderTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
      this.ImagesFolderTextBox.Location = new System.Drawing.Point(109, 6);
      this.ImagesFolderTextBox.Name = "ImagesFolderTextBox";
      this.ImagesFolderTextBox.Size = new System.Drawing.Size(1123, 25);
      this.ImagesFolderTextBox.TabIndex = 0;
      // 
      // ImagesFolderBrowseButton
      // 
      this.ImagesFolderBrowseButton.Location = new System.Drawing.Point(1249, 6);
      this.ImagesFolderBrowseButton.Name = "ImagesFolderBrowseButton";
      this.ImagesFolderBrowseButton.Size = new System.Drawing.Size(36, 25);
      this.ImagesFolderBrowseButton.TabIndex = 1;
      this.ImagesFolderBrowseButton.Text = "...";
      this.ImagesFolderBrowseButton.UseVisualStyleBackColor = true;
      this.ImagesFolderBrowseButton.Click += new System.EventHandler(this.ImageFolderBrowseButton_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(91, 17);
      this.label1.TabIndex = 2;
      this.label1.Text = "Images Folder";
      // 
      // ShowDatabaseDuplicatesInViewerButton
      // 
      this.ShowDatabaseDuplicatesInViewerButton.Location = new System.Drawing.Point(15, 94);
      this.ShowDatabaseDuplicatesInViewerButton.Name = "ShowDatabaseDuplicatesInViewerButton";
      this.ShowDatabaseDuplicatesInViewerButton.Size = new System.Drawing.Size(170, 54);
      this.ShowDatabaseDuplicatesInViewerButton.TabIndex = 13;
      this.ShowDatabaseDuplicatesInViewerButton.Text = "Duplicates Viewer";
      this.ShowDatabaseDuplicatesInViewerButton.UseVisualStyleBackColor = true;
      this.ShowDatabaseDuplicatesInViewerButton.Click += new System.EventHandler(this.ShowDatabaseDuplicatesInViewerButton_Click);
      // 
      // UpdateDatabaseButton
      // 
      this.UpdateDatabaseButton.Location = new System.Drawing.Point(15, 164);
      this.UpdateDatabaseButton.Name = "UpdateDatabaseButton";
      this.UpdateDatabaseButton.Size = new System.Drawing.Size(170, 54);
      this.UpdateDatabaseButton.TabIndex = 17;
      this.UpdateDatabaseButton.Text = "Update Database";
      this.UpdateDatabaseButton.UseVisualStyleBackColor = true;
      this.UpdateDatabaseButton.Click += new System.EventHandler(this.UpdateDatabaseButton_ClickAsync);
      // 
      // LogTextBox
      // 
      this.LogTextBox.AcceptsTab = true;
      this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.LogTextBox.BackColor = System.Drawing.Color.Black;
      this.LogTextBox.ContextMenuStrip = this.LogTextBoxContextMenu;
      this.LogTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.LogTextBox.ForeColor = System.Drawing.Color.White;
      this.LogTextBox.Location = new System.Drawing.Point(191, 37);
      this.LogTextBox.Multiline = true;
      this.LogTextBox.Name = "LogTextBox";
      this.LogTextBox.ReadOnly = true;
      this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.LogTextBox.Size = new System.Drawing.Size(1094, 440);
      this.LogTextBox.TabIndex = 19;
      // 
      // LogTextBoxContextMenu
      // 
      this.LogTextBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LogContextMenu_Clear});
      this.LogTextBoxContextMenu.Name = "LogMenuStrip";
      this.LogTextBoxContextMenu.Size = new System.Drawing.Size(102, 26);
      // 
      // LogContextMenu_Clear
      // 
      this.LogContextMenu_Clear.Name = "LogContextMenu_Clear";
      this.LogContextMenu_Clear.Size = new System.Drawing.Size(101, 22);
      this.LogContextMenu_Clear.Text = "Clear";
      this.LogContextMenu_Clear.Click += new System.EventHandler(this.LogContextMenu_Clear_Click);
      // 
      // UseStoreProcCheckBox
      // 
      this.UseStoreProcCheckBox.AutoSize = true;
      this.UseStoreProcCheckBox.Location = new System.Drawing.Point(15, 67);
      this.UseStoreProcCheckBox.Name = "UseStoreProcCheckBox";
      this.UseStoreProcCheckBox.Size = new System.Drawing.Size(110, 21);
      this.UseStoreProcCheckBox.TabIndex = 22;
      this.UseStoreProcCheckBox.Text = "Use StoreProc";
      this.UseStoreProcCheckBox.UseVisualStyleBackColor = true;
      // 
      // ClearDupeDetectorQueueButton
      // 
      this.ClearDupeDetectorQueueButton.Location = new System.Drawing.Point(15, 234);
      this.ClearDupeDetectorQueueButton.Name = "ClearDupeDetectorQueueButton";
      this.ClearDupeDetectorQueueButton.Size = new System.Drawing.Size(170, 54);
      this.ClearDupeDetectorQueueButton.TabIndex = 23;
      this.ClearDupeDetectorQueueButton.Text = "Clear Detection Queue";
      this.ClearDupeDetectorQueueButton.UseVisualStyleBackColor = true;
      this.ClearDupeDetectorQueueButton.Click += new System.EventHandler(this.ClearDupeDetectorQueueButton_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1297, 489);
      this.Controls.Add(this.ClearDupeDetectorQueueButton);
      this.Controls.Add(this.UseStoreProcCheckBox);
      this.Controls.Add(this.LogTextBox);
      this.Controls.Add(this.UpdateDatabaseButton);
      this.Controls.Add(this.ShowDatabaseDuplicatesInViewerButton);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.ImagesFolderBrowseButton);
      this.Controls.Add(this.ImagesFolderTextBox);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "MainForm";
      this.Text = "Duplicate Image Finder";
      this.Load += new System.EventHandler(this.Form_Load);
      this.LogTextBoxContextMenu.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox ImagesFolderTextBox;
    private System.Windows.Forms.Button ImagesFolderBrowseButton;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button ShowDatabaseDuplicatesInViewerButton;
    private System.Windows.Forms.Button UpdateDatabaseButton;
    private System.Windows.Forms.TextBox LogTextBox;
    private System.Windows.Forms.CheckBox UseStoreProcCheckBox;
        private System.Windows.Forms.Button ClearDupeDetectorQueueButton;
    private System.Windows.Forms.ContextMenuStrip LogTextBoxContextMenu;
    private System.Windows.Forms.ToolStripMenuItem LogContextMenu_Clear;
  }
}

