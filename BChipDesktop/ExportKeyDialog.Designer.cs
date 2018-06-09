namespace BChipDesktop
{
    partial class ExportKeyDialog
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
            this.Close = new System.Windows.Forms.Button();
            this.qrCodeImage = new System.Windows.Forms.PictureBox();
            this.copyKeyIcon = new System.Windows.Forms.PictureBox();
            this.keyAddressLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.qrCodeImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.copyKeyIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // Close
            // 
            this.Close.Location = new System.Drawing.Point(195, 596);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(119, 51);
            this.Close.TabIndex = 0;
            this.Close.Text = "Close";
            this.Close.UseVisualStyleBackColor = true;
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // qrCodeImage
            // 
            this.qrCodeImage.Location = new System.Drawing.Point(135, 86);
            this.qrCodeImage.Name = "qrCodeImage";
            this.qrCodeImage.Size = new System.Drawing.Size(252, 237);
            this.qrCodeImage.TabIndex = 1;
            this.qrCodeImage.TabStop = false;
            // 
            // copyKeyIcon
            // 
            this.copyKeyIcon.Image = global::BChipDesktop.Properties.Resources.Editing_Copy_icon;
            this.copyKeyIcon.Location = new System.Drawing.Point(70, 351);
            this.copyKeyIcon.Name = "copyKeyIcon";
            this.copyKeyIcon.Size = new System.Drawing.Size(45, 45);
            this.copyKeyIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.copyKeyIcon.TabIndex = 2;
            this.copyKeyIcon.TabStop = false;
            this.copyKeyIcon.Click += new System.EventHandler(this.copyKey_Click);
            // 
            // keyAddressLabel
            // 
            this.keyAddressLabel.AutoSize = true;
            this.keyAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyAddressLabel.Location = new System.Drawing.Point(126, 358);
            this.keyAddressLabel.Name = "keyAddressLabel";
            this.keyAddressLabel.Size = new System.Drawing.Size(173, 32);
            this.keyAddressLabel.TabIndex = 3;
            this.keyAddressLabel.TabStop = true;
            this.keyAddressLabel.Text = "(remove me)";
            // 
            // ExportKeyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.keyAddressLabel);
            this.Controls.Add(this.copyKeyIcon);
            this.Controls.Add(this.qrCodeImage);
            this.Controls.Add(this.Close);
            this.Name = "ExportKeyDialog";
            this.Size = new System.Drawing.Size(508, 835);
            this.Load += new System.EventHandler(this.ExportKeyDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.qrCodeImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.copyKeyIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Close;
        private System.Windows.Forms.PictureBox qrCodeImage;
        private System.Windows.Forms.PictureBox copyKeyIcon;
        private System.Windows.Forms.LinkLabel keyAddressLabel;
    }
}
