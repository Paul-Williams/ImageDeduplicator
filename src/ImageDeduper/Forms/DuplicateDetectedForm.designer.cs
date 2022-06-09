using System.Windows.Forms;

namespace ImageDeduper
{
  partial class DuplicateDetectedForm
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
      this.DeleteBothButton = new System.Windows.Forms.Button();
      this.ExistingDuplicateImageControl = new ImageDeduper.Controls.DuplicateImageControl();
      this.NewDuplicateImageControl = new ImageDeduper.Controls.DuplicateImageControl();
      this.TablePanel = new System.Windows.Forms.TableLayoutPanel();
      this.TablePanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // DeleteBothButton
      // 
      this.DeleteBothButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.DeleteBothButton.Location = new System.Drawing.Point(465, 258);
      this.DeleteBothButton.Name = "DeleteBothButton";
      this.DeleteBothButton.Size = new System.Drawing.Size(121, 44);
      this.DeleteBothButton.TabIndex = 19;
      this.DeleteBothButton.Text = "Delete Both";
      this.DeleteBothButton.UseVisualStyleBackColor = true;
      this.DeleteBothButton.Click += new System.EventHandler(this.DeleteBothButton_Click);
      // 
      // ExistingDuplicateImageControl
      // 
      this.ExistingDuplicateImageControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.ExistingDuplicateImageControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ExistingDuplicateImageControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ExistingDuplicateImageControl.Location = new System.Drawing.Point(3, 284);
      this.ExistingDuplicateImageControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ExistingDuplicateImageControl.Name = "ExistingDuplicateImageControl";
      this.ExistingDuplicateImageControl.Size = new System.Drawing.Size(674, 272);
      this.ExistingDuplicateImageControl.TabIndex = 18;
      // 
      // NewDuplicateImageControl
      // 
      this.NewDuplicateImageControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.NewDuplicateImageControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.NewDuplicateImageControl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.NewDuplicateImageControl.Location = new System.Drawing.Point(3, 4);
      this.NewDuplicateImageControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.NewDuplicateImageControl.Name = "NewDuplicateImageControl";
      this.NewDuplicateImageControl.Size = new System.Drawing.Size(674, 272);
      this.NewDuplicateImageControl.TabIndex = 17;
      // 
      // TablePanel
      // 
      this.TablePanel.ColumnCount = 1;
      this.TablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.TablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.TablePanel.Controls.Add(this.NewDuplicateImageControl, 0, 0);
      this.TablePanel.Controls.Add(this.ExistingDuplicateImageControl, 0, 1);
      this.TablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.TablePanel.Location = new System.Drawing.Point(0, 0);
      this.TablePanel.Name = "TablePanel";
      this.TablePanel.RowCount = 2;
      this.TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TablePanel.Size = new System.Drawing.Size(674, 560);
      this.TablePanel.TabIndex = 20;
      // 
      // DuplicateDetectedForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(674, 560);
      this.Controls.Add(this.DeleteBothButton);
      this.Controls.Add(this.TablePanel);
      this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MinimumSize = new System.Drawing.Size(16, 500);
      this.Name = "DuplicateDetectedForm";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Duplicate Images Detected";
      this.Load += new System.EventHandler(this.DuplicateDetectedForm_Load);
      this.TablePanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private Controls.DuplicateImageControl NewDuplicateImageControl;
    private Controls.DuplicateImageControl ExistingDuplicateImageControl;
        private System.Windows.Forms.Button DeleteBothButton;
        private System.Windows.Forms.TableLayoutPanel TablePanel;
    }
}