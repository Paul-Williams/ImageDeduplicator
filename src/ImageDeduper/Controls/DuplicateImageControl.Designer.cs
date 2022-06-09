namespace ImageDeduper.Controls
{
  partial class DuplicateImageControl
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
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.FindNewButton = new System.Windows.Forms.Button();
      this.DimensionsLabel = new System.Windows.Forms.Label();
      this.PictureBox = new System.Windows.Forms.PictureBox();
      this.DeleteNewFileButton = new System.Windows.Forms.Button();
      this.CaptionLabel = new System.Windows.Forms.Label();
      this.FilePathLinkLabel = new System.Windows.Forms.LinkLabel();
      this.FileSizeLabel = new System.Windows.Forms.Label();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.button1 = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // FindNewButton
      // 
      this.FindNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.FindNewButton.Location = new System.Drawing.Point(519, 57);
      this.FindNewButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.FindNewButton.Name = "FindNewButton";
      this.FindNewButton.Size = new System.Drawing.Size(141, 48);
      this.FindNewButton.TabIndex = 17;
      this.FindNewButton.Text = "Show in Explorer";
      this.FindNewButton.UseVisualStyleBackColor = true;
      this.FindNewButton.Click += new System.EventHandler(this.FindNewButton_Click);
      // 
      // DimensionsLabel
      // 
      this.DimensionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.DimensionsLabel.AutoSize = true;
      this.DimensionsLabel.Location = new System.Drawing.Point(396, 57);
      this.DimensionsLabel.Name = "DimensionsLabel";
      this.DimensionsLabel.Size = new System.Drawing.Size(106, 17);
      this.DimensionsLabel.TabIndex = 16;
      this.DimensionsLabel.Text = "DimensionsLabel";
      // 
      // PictureBox
      // 
      this.PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PictureBox.Location = new System.Drawing.Point(15, 57);
      this.PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.PictureBox.Name = "PictureBox";
      this.PictureBox.Size = new System.Drawing.Size(360, 219);
      this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.PictureBox.TabIndex = 15;
      this.PictureBox.TabStop = false;
      // 
      // DeleteNewFileButton
      // 
      this.DeleteNewFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.DeleteNewFileButton.Location = new System.Drawing.Point(519, 185);
      this.DeleteNewFileButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.DeleteNewFileButton.Name = "DeleteNewFileButton";
      this.DeleteNewFileButton.Size = new System.Drawing.Size(141, 48);
      this.DeleteNewFileButton.TabIndex = 14;
      this.DeleteNewFileButton.Text = "Delete";
      this.DeleteNewFileButton.UseVisualStyleBackColor = true;
      this.DeleteNewFileButton.Click += new System.EventHandler(this.InternalDeleteButton_Click);
      // 
      // CaptionLabel
      // 
      this.CaptionLabel.AutoSize = true;
      this.CaptionLabel.Location = new System.Drawing.Point(12, 8);
      this.CaptionLabel.Name = "CaptionLabel";
      this.CaptionLabel.Size = new System.Drawing.Size(84, 17);
      this.CaptionLabel.TabIndex = 13;
      this.CaptionLabel.Text = "CaptionLabel";
      // 
      // FilePathLinkLabel
      // 
      this.FilePathLinkLabel.AutoSize = true;
      this.FilePathLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
      this.FilePathLinkLabel.Location = new System.Drawing.Point(12, 35);
      this.FilePathLinkLabel.Name = "FilePathLinkLabel";
      this.FilePathLinkLabel.Size = new System.Drawing.Size(105, 17);
      this.FilePathLinkLabel.TabIndex = 12;
      this.FilePathLinkLabel.TabStop = true;
      this.FilePathLinkLabel.Text = "FilePathLinkLabel";
      this.FilePathLinkLabel.UseMnemonic = false;
      this.FilePathLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FilePathLinkLabel_LinkClicked);
      // 
      // FileSizeLabel
      // 
      this.FileSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.FileSizeLabel.AutoSize = true;
      this.FileSizeLabel.Location = new System.Drawing.Point(396, 88);
      this.FileSizeLabel.Name = "FileSizeLabel";
      this.FileSizeLabel.Size = new System.Drawing.Size(81, 17);
      this.FileSizeLabel.TabIndex = 18;
      this.FileSizeLabel.Text = "FileSizeLabel";
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(519, 130);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(141, 48);
      this.button1.TabIndex = 19;
      this.button1.Text = "Move Replace Other";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.ReplaceOtherButton_Click);
      // 
      // DuplicateImageControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.button1);
      this.Controls.Add(this.FileSizeLabel);
      this.Controls.Add(this.FindNewButton);
      this.Controls.Add(this.DimensionsLabel);
      this.Controls.Add(this.PictureBox);
      this.Controls.Add(this.DeleteNewFileButton);
      this.Controls.Add(this.CaptionLabel);
      this.Controls.Add(this.FilePathLinkLabel);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "DuplicateImageControl";
      this.Size = new System.Drawing.Size(674, 280);
      this.Load += new System.EventHandler(this.DuplicateImageControl_Load);
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button FindNewButton;
    private System.Windows.Forms.Label DimensionsLabel;
    private System.Windows.Forms.PictureBox PictureBox;
    private System.Windows.Forms.Button DeleteNewFileButton;
    private System.Windows.Forms.Label CaptionLabel;
    private System.Windows.Forms.LinkLabel FilePathLinkLabel;
    private System.Windows.Forms.Label FileSizeLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
    }
}
