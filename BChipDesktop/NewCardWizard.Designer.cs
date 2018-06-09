namespace BChipDesktop
{
    partial class NewCardWizard
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
            this.createButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.currencyComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.wifInputBox = new System.Windows.Forms.TextBox();
            this.generatePKBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cardIdTextbox = new System.Windows.Forms.TextBox();
            this.cardTypeSelection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // createButton
            // 
            this.createButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createButton.Location = new System.Drawing.Point(70, 672);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(223, 78);
            this.createButton.TabIndex = 0;
            this.createButton.Text = "Create Key";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(319, 672);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(235, 78);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel / Delete";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // currencyComboBox
            // 
            this.currencyComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currencyComboBox.ItemHeight = 32;
            this.currencyComboBox.Items.AddRange(new object[] {
            "BTC",
            "ETH",
            "CUSTOM"});
            this.currencyComboBox.Location = new System.Drawing.Point(319, 433);
            this.currencyComboBox.Name = "currencyComboBox";
            this.currencyComboBox.Size = new System.Drawing.Size(235, 40);
            this.currencyComboBox.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(78, 436);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 32);
            this.label1.TabIndex = 13;
            this.label1.Text = "Private Key Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(78, 488);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 32);
            this.label2.TabIndex = 14;
            this.label2.Text = "WIF:";
            // 
            // wifInputBox
            // 
            this.wifInputBox.Location = new System.Drawing.Point(158, 488);
            this.wifInputBox.Multiline = true;
            this.wifInputBox.Name = "wifInputBox";
            this.wifInputBox.Size = new System.Drawing.Size(396, 144);
            this.wifInputBox.TabIndex = 15;
            // 
            // generatePKBtn
            // 
            this.generatePKBtn.Location = new System.Drawing.Point(70, 523);
            this.generatePKBtn.Name = "generatePKBtn";
            this.generatePKBtn.Size = new System.Drawing.Size(81, 109);
            this.generatePKBtn.TabIndex = 16;
            this.generatePKBtn.Text = "Create Keys Locally";
            this.generatePKBtn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(280, 326);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 32);
            this.label3.TabIndex = 17;
            this.label3.Text = "Card ID:";
            // 
            // cardIdTextbox
            // 
            this.cardIdTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardIdTextbox.Location = new System.Drawing.Point(286, 373);
            this.cardIdTextbox.Name = "cardIdTextbox";
            this.cardIdTextbox.ReadOnly = true;
            this.cardIdTextbox.Size = new System.Drawing.Size(268, 39);
            this.cardIdTextbox.TabIndex = 18;
            // 
            // cardTypeSelection
            // 
            this.cardTypeSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardTypeSelection.ItemHeight = 32;
            this.cardTypeSelection.Items.AddRange(new object[] {
            "BCHIP",
            "LOCAL"});
            this.cardTypeSelection.Location = new System.Drawing.Point(84, 373);
            this.cardTypeSelection.Name = "cardTypeSelection";
            this.cardTypeSelection.Size = new System.Drawing.Size(196, 40);
            this.cardTypeSelection.TabIndex = 19;
            this.cardTypeSelection.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(78, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 32);
            this.label4.TabIndex = 20;
            this.label4.Text = "Card Type:";
            // 
            // NewCardWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cardTypeSelection);
            this.Controls.Add(this.cardIdTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.generatePKBtn);
            this.Controls.Add(this.wifInputBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.currencyComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.createButton);
            this.Name = "NewCardWizard";
            this.Size = new System.Drawing.Size(575, 783);
            this.Load += new System.EventHandler(this.NewCardWizard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox currencyComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox wifInputBox;
        private System.Windows.Forms.Button generatePKBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox cardIdTextbox;
        private System.Windows.Forms.ComboBox cardTypeSelection;
        private System.Windows.Forms.Label label4;
    }
}
