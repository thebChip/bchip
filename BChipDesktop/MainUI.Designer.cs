using System.Windows.Forms;

namespace BChipDesktop
{
    partial class MainUI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUI));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ReceiveBtn = new System.Windows.Forms.Button();
            this.sendBtn = new System.Windows.Forms.Button();
            this.bchipBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.addBtn = new System.Windows.Forms.Button();
            this.wipeBchipBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.importKeyTextBox = new System.Windows.Forms.TextBox();
            this.importConfirmKeyButton = new System.Windows.Forms.Button();
            this.keyAddressLabel = new System.Windows.Forms.LinkLabel();
            this.copyKeyIcon = new System.Windows.Forms.PictureBox();
            this.bchipCrcFailLabel = new System.Windows.Forms.Label();
            this.notFormattedLabel = new System.Windows.Forms.Label();
            this.keyTypeLabel = new System.Windows.Forms.Label();
            this.confirmPassphraseBtn = new System.Windows.Forms.Button();
            this.savePassphraseBtn = new System.Windows.Forms.Button();
            this.reconfirmPassphraseBox = new System.Windows.Forms.TextBox();
            this.passphraseEntryBox = new System.Windows.Forms.TextBox();
            this.errorLoadingLbl = new System.Windows.Forms.Label();
            this.connectedIcon = new System.Windows.Forms.PictureBox();
            this.currencyComboBox = new System.Windows.Forms.ComboBox();
            this.saveToBchipBtn = new System.Windows.Forms.Button();
            this.setPinBtn = new System.Windows.Forms.Button();
            this.insertCardLabel = new System.Windows.Forms.Label();
            this.notConnectedIcon = new System.Windows.Forms.PictureBox();
            this.importKeyBtn = new System.Windows.Forms.Button();
            this.createKeyBtn = new System.Windows.Forms.Button();
            this.exportKeyBtn = new System.Windows.Forms.Button();
            this.qrCodeImage = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cardList = new System.Windows.Forms.DataGridView();
            this.Remove = new System.Windows.Forms.DataGridViewImageColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mlviDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.idLabelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cardTypeLabelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressCopyIconDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.publicAddressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connectionStringDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pkSourceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bChipMemoryLayoutBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copyKeyIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectedIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.notConnectedIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qrCodeImage)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cardList)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bChipMemoryLayoutBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.ReceiveBtn);
            this.panel2.Controls.Add(this.sendBtn);
            this.panel2.Controls.Add(this.bchipBtn);
            this.panel2.Location = new System.Drawing.Point(675, 16);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(639, 156);
            this.panel2.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(568, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(59, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Click += new System.EventHandler(this.PictureBox1_Click);
            // 
            // ReceiveBtn
            // 
            this.ReceiveBtn.Location = new System.Drawing.Point(395, 40);
            this.ReceiveBtn.Margin = new System.Windows.Forms.Padding(4);
            this.ReceiveBtn.Name = "ReceiveBtn";
            this.ReceiveBtn.Size = new System.Drawing.Size(144, 56);
            this.ReceiveBtn.TabIndex = 5;
            this.ReceiveBtn.Text = "Receive";
            this.ReceiveBtn.UseVisualStyleBackColor = true;
            this.ReceiveBtn.Visible = false;
            this.ReceiveBtn.Click += new System.EventHandler(this.ReceiveBtn_Click);
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(244, 40);
            this.sendBtn.Margin = new System.Windows.Forms.Padding(4);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(143, 56);
            this.sendBtn.TabIndex = 4;
            this.sendBtn.Text = "Send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Visible = false;
            this.sendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // bchipBtn
            // 
            this.bchipBtn.Location = new System.Drawing.Point(44, 40);
            this.bchipBtn.Margin = new System.Windows.Forms.Padding(4);
            this.bchipBtn.Name = "bchipBtn";
            this.bchipBtn.Size = new System.Drawing.Size(172, 56);
            this.bchipBtn.TabIndex = 1;
            this.bchipBtn.Text = "Load BChip";
            this.bchipBtn.UseVisualStyleBackColor = true;
            this.bchipBtn.Click += new System.EventHandler(this.BchipBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.addBtn);
            this.panel1.Controls.Add(this.wipeBchipBtn);
            this.panel1.Location = new System.Drawing.Point(15, 16);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(657, 156);
            this.panel1.TabIndex = 0;
            // 
            // addBtn
            // 
            this.addBtn.Enabled = false;
            this.addBtn.Location = new System.Drawing.Point(267, 40);
            this.addBtn.Margin = new System.Windows.Forms.Padding(4);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(103, 56);
            this.addBtn.TabIndex = 0;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Visible = false;
            this.addBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // wipeBchipBtn
            // 
            this.wipeBchipBtn.Location = new System.Drawing.Point(225, 40);
            this.wipeBchipBtn.Margin = new System.Windows.Forms.Padding(4);
            this.wipeBchipBtn.Name = "wipeBchipBtn";
            this.wipeBchipBtn.Size = new System.Drawing.Size(184, 56);
            this.wipeBchipBtn.TabIndex = 14;
            this.wipeBchipBtn.Text = "Erase/Initialize";
            this.wipeBchipBtn.UseVisualStyleBackColor = true;
            this.wipeBchipBtn.Click += new System.EventHandler(this.wipeBchipBtn_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.importKeyTextBox);
            this.panel3.Controls.Add(this.importConfirmKeyButton);
            this.panel3.Controls.Add(this.keyAddressLabel);
            this.panel3.Controls.Add(this.copyKeyIcon);
            this.panel3.Controls.Add(this.bchipCrcFailLabel);
            this.panel3.Controls.Add(this.notFormattedLabel);
            this.panel3.Controls.Add(this.keyTypeLabel);
            this.panel3.Controls.Add(this.confirmPassphraseBtn);
            this.panel3.Controls.Add(this.savePassphraseBtn);
            this.panel3.Controls.Add(this.reconfirmPassphraseBox);
            this.panel3.Controls.Add(this.passphraseEntryBox);
            this.panel3.Controls.Add(this.errorLoadingLbl);
            this.panel3.Controls.Add(this.connectedIcon);
            this.panel3.Controls.Add(this.currencyComboBox);
            this.panel3.Controls.Add(this.saveToBchipBtn);
            this.panel3.Controls.Add(this.setPinBtn);
            this.panel3.Controls.Add(this.insertCardLabel);
            this.panel3.Controls.Add(this.notConnectedIcon);
            this.panel3.Controls.Add(this.importKeyBtn);
            this.panel3.Controls.Add(this.createKeyBtn);
            this.panel3.Controls.Add(this.exportKeyBtn);
            this.panel3.Controls.Add(this.qrCodeImage);
            this.panel3.Location = new System.Drawing.Point(675, 179);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(639, 951);
            this.panel3.TabIndex = 3;
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // importKeyTextBox
            // 
            this.importKeyTextBox.Location = new System.Drawing.Point(9, 725);
            this.importKeyTextBox.Multiline = true;
            this.importKeyTextBox.Name = "importKeyTextBox";
            this.importKeyTextBox.Size = new System.Drawing.Size(488, 56);
            this.importKeyTextBox.TabIndex = 27;
            // 
            // importConfirmKeyButton
            // 
            this.importConfirmKeyButton.Location = new System.Drawing.Point(504, 725);
            this.importConfirmKeyButton.Margin = new System.Windows.Forms.Padding(4);
            this.importConfirmKeyButton.Name = "importConfirmKeyButton";
            this.importConfirmKeyButton.Size = new System.Drawing.Size(123, 56);
            this.importConfirmKeyButton.TabIndex = 26;
            this.importConfirmKeyButton.Text = "Import";
            this.importConfirmKeyButton.UseVisualStyleBackColor = true;
            this.importConfirmKeyButton.Click += new System.EventHandler(this.importConfirmKeyButton_Click);
            // 
            // keyAddressLabel
            // 
            this.keyAddressLabel.AutoSize = true;
            this.keyAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyAddressLabel.Location = new System.Drawing.Point(4, 343);
            this.keyAddressLabel.Name = "keyAddressLabel";
            this.keyAddressLabel.Size = new System.Drawing.Size(123, 25);
            this.keyAddressLabel.TabIndex = 25;
            this.keyAddressLabel.TabStop = true;
            this.keyAddressLabel.Text = "(remove me)";
            this.keyAddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.keyAddressLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.keyAddressLabel_LinkClicked);
            // 
            // copyKeyIcon
            // 
            this.copyKeyIcon.Image = global::BChipDesktop.Properties.Resources.Editing_Copy_icon;
            this.copyKeyIcon.Location = new System.Drawing.Point(295, 384);
            this.copyKeyIcon.Name = "copyKeyIcon";
            this.copyKeyIcon.Size = new System.Drawing.Size(45, 45);
            this.copyKeyIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.copyKeyIcon.TabIndex = 24;
            this.copyKeyIcon.TabStop = false;
            this.copyKeyIcon.Click += new System.EventHandler(this.copyKeyIcon_Click);
            // 
            // bchipCrcFailLabel
            // 
            this.bchipCrcFailLabel.AutoSize = true;
            this.bchipCrcFailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bchipCrcFailLabel.ForeColor = System.Drawing.Color.Red;
            this.bchipCrcFailLabel.Location = new System.Drawing.Point(114, 40);
            this.bchipCrcFailLabel.Name = "bchipCrcFailLabel";
            this.bchipCrcFailLabel.Size = new System.Drawing.Size(465, 32);
            this.bchipCrcFailLabel.TabIndex = 22;
            this.bchipCrcFailLabel.Text = "bChip checksum data was invalid";
            // 
            // notFormattedLabel
            // 
            this.notFormattedLabel.AutoSize = true;
            this.notFormattedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notFormattedLabel.ForeColor = System.Drawing.Color.Red;
            this.notFormattedLabel.Location = new System.Drawing.Point(123, 40);
            this.notFormattedLabel.Name = "notFormattedLabel";
            this.notFormattedLabel.Size = new System.Drawing.Size(409, 32);
            this.notFormattedLabel.TabIndex = 21;
            this.notFormattedLabel.Text = "bChip must be initialized first";
            // 
            // keyTypeLabel
            // 
            this.keyTypeLabel.AutoSize = true;
            this.keyTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyTypeLabel.Location = new System.Drawing.Point(259, 444);
            this.keyTypeLabel.Name = "keyTypeLabel";
            this.keyTypeLabel.Size = new System.Drawing.Size(142, 32);
            this.keyTypeLabel.TabIndex = 20;
            this.keyTypeLabel.Text = "Key Type";
            // 
            // confirmPassphraseBtn
            // 
            this.confirmPassphraseBtn.Location = new System.Drawing.Point(244, 347);
            this.confirmPassphraseBtn.Margin = new System.Windows.Forms.Padding(4);
            this.confirmPassphraseBtn.Name = "confirmPassphraseBtn";
            this.confirmPassphraseBtn.Size = new System.Drawing.Size(168, 56);
            this.confirmPassphraseBtn.TabIndex = 19;
            this.confirmPassphraseBtn.Text = "Confirm Passphrase";
            this.confirmPassphraseBtn.UseVisualStyleBackColor = true;
            this.confirmPassphraseBtn.Click += new System.EventHandler(this.confirmPassphraseBtn_Click);
            // 
            // savePassphraseBtn
            // 
            this.savePassphraseBtn.Location = new System.Drawing.Point(260, 347);
            this.savePassphraseBtn.Margin = new System.Windows.Forms.Padding(4);
            this.savePassphraseBtn.Name = "savePassphraseBtn";
            this.savePassphraseBtn.Size = new System.Drawing.Size(143, 56);
            this.savePassphraseBtn.TabIndex = 18;
            this.savePassphraseBtn.Text = "Set Passphrase";
            this.savePassphraseBtn.UseVisualStyleBackColor = true;
            this.savePassphraseBtn.Click += new System.EventHandler(this.savePassphraseBtn_Click);
            // 
            // reconfirmPassphraseBox
            // 
            this.reconfirmPassphraseBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reconfirmPassphraseBox.Location = new System.Drawing.Point(232, 301);
            this.reconfirmPassphraseBox.Name = "reconfirmPassphraseBox";
            this.reconfirmPassphraseBox.PasswordChar = '*';
            this.reconfirmPassphraseBox.Size = new System.Drawing.Size(196, 39);
            this.reconfirmPassphraseBox.TabIndex = 17;
            // 
            // passphraseEntryBox
            // 
            this.passphraseEntryBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passphraseEntryBox.Location = new System.Drawing.Point(232, 301);
            this.passphraseEntryBox.Name = "passphraseEntryBox";
            this.passphraseEntryBox.PasswordChar = '*';
            this.passphraseEntryBox.Size = new System.Drawing.Size(196, 39);
            this.passphraseEntryBox.TabIndex = 16;
            // 
            // errorLoadingLbl
            // 
            this.errorLoadingLbl.AutoSize = true;
            this.errorLoadingLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorLoadingLbl.ForeColor = System.Drawing.Color.Firebrick;
            this.errorLoadingLbl.Location = new System.Drawing.Point(123, 40);
            this.errorLoadingLbl.Name = "errorLoadingLbl";
            this.errorLoadingLbl.Size = new System.Drawing.Size(428, 32);
            this.errorLoadingLbl.TabIndex = 13;
            this.errorLoadingLbl.Text = "Failed to load data from BChip";
            // 
            // connectedIcon
            // 
            this.connectedIcon.Image = ((System.Drawing.Image)(resources.GetObject("connectedIcon.Image")));
            this.connectedIcon.Location = new System.Drawing.Point(273, 104);
            this.connectedIcon.Name = "connectedIcon";
            this.connectedIcon.Size = new System.Drawing.Size(130, 127);
            this.connectedIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.connectedIcon.TabIndex = 12;
            this.connectedIcon.TabStop = false;
            // 
            // currencyComboBox
            // 
            this.currencyComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currencyComboBox.ItemHeight = 32;
            this.currencyComboBox.Items.AddRange(new object[] {
            "BTC",
            "ETH",
            "CUSTOM"});
            this.currencyComboBox.Location = new System.Drawing.Point(244, 484);
            this.currencyComboBox.Name = "currencyComboBox";
            this.currencyComboBox.Size = new System.Drawing.Size(170, 40);
            this.currencyComboBox.TabIndex = 11;
            this.currencyComboBox.SelectedIndexChanged += new System.EventHandler(this.CurrencyComboBox_SelectionChangeCommitted);
            // 
            // saveToBchipBtn
            // 
            this.saveToBchipBtn.Location = new System.Drawing.Point(273, 845);
            this.saveToBchipBtn.Margin = new System.Windows.Forms.Padding(4);
            this.saveToBchipBtn.Name = "saveToBchipBtn";
            this.saveToBchipBtn.Size = new System.Drawing.Size(123, 56);
            this.saveToBchipBtn.TabIndex = 10;
            this.saveToBchipBtn.Text = "Save to BChip";
            this.saveToBchipBtn.UseVisualStyleBackColor = true;
            this.saveToBchipBtn.Click += new System.EventHandler(this.saveToBchipBtn_Click);
            // 
            // setPinBtn
            // 
            this.setPinBtn.Location = new System.Drawing.Point(273, 312);
            this.setPinBtn.Margin = new System.Windows.Forms.Padding(4);
            this.setPinBtn.Name = "setPinBtn";
            this.setPinBtn.Size = new System.Drawing.Size(123, 56);
            this.setPinBtn.TabIndex = 6;
            this.setPinBtn.Text = "Key Phrase";
            this.setPinBtn.UseVisualStyleBackColor = true;
            this.setPinBtn.Click += new System.EventHandler(this.setPinBtn_Click);
            // 
            // insertCardLabel
            // 
            this.insertCardLabel.AutoSize = true;
            this.insertCardLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.insertCardLabel.Location = new System.Drawing.Point(29, 40);
            this.insertCardLabel.Name = "insertCardLabel";
            this.insertCardLabel.Size = new System.Drawing.Size(589, 32);
            this.insertCardLabel.TabIndex = 7;
            this.insertCardLabel.Text = "Please insert BChip into smart card reader";
            // 
            // notConnectedIcon
            // 
            this.notConnectedIcon.Image = ((System.Drawing.Image)(resources.GetObject("notConnectedIcon.Image")));
            this.notConnectedIcon.Location = new System.Drawing.Point(273, 104);
            this.notConnectedIcon.Name = "notConnectedIcon";
            this.notConnectedIcon.Size = new System.Drawing.Size(130, 127);
            this.notConnectedIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.notConnectedIcon.TabIndex = 6;
            this.notConnectedIcon.TabStop = false;
            // 
            // importKeyBtn
            // 
            this.importKeyBtn.Location = new System.Drawing.Point(273, 653);
            this.importKeyBtn.Margin = new System.Windows.Forms.Padding(4);
            this.importKeyBtn.Name = "importKeyBtn";
            this.importKeyBtn.Size = new System.Drawing.Size(123, 56);
            this.importKeyBtn.TabIndex = 9;
            this.importKeyBtn.Text = "Import Key";
            this.importKeyBtn.UseVisualStyleBackColor = true;
            this.importKeyBtn.Click += new System.EventHandler(this.importKeyBtn_Click);
            // 
            // createKeyBtn
            // 
            this.createKeyBtn.Location = new System.Drawing.Point(273, 589);
            this.createKeyBtn.Margin = new System.Windows.Forms.Padding(4);
            this.createKeyBtn.Name = "createKeyBtn";
            this.createKeyBtn.Size = new System.Drawing.Size(123, 56);
            this.createKeyBtn.TabIndex = 8;
            this.createKeyBtn.Text = "Create Key";
            this.createKeyBtn.UseVisualStyleBackColor = true;
            this.createKeyBtn.Click += new System.EventHandler(this.createKeyBtn_Click);
            // 
            // exportKeyBtn
            // 
            this.exportKeyBtn.Location = new System.Drawing.Point(273, 589);
            this.exportKeyBtn.Margin = new System.Windows.Forms.Padding(4);
            this.exportKeyBtn.Name = "exportKeyBtn";
            this.exportKeyBtn.Size = new System.Drawing.Size(123, 56);
            this.exportKeyBtn.TabIndex = 15;
            this.exportKeyBtn.Text = "Export Key";
            this.exportKeyBtn.UseVisualStyleBackColor = true;
            this.exportKeyBtn.Click += new System.EventHandler(this.exportKeyBtn_Click);
            // 
            // qrCodeImage
            // 
            this.qrCodeImage.Location = new System.Drawing.Point(197, 75);
            this.qrCodeImage.Name = "qrCodeImage";
            this.qrCodeImage.Size = new System.Drawing.Size(260, 260);
            this.qrCodeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.qrCodeImage.TabIndex = 23;
            this.qrCodeImage.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.cardList);
            this.panel4.Location = new System.Drawing.Point(15, 179);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(657, 952);
            this.panel4.TabIndex = 2;
            // 
            // cardList
            // 
            this.cardList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cardList.AutoGenerateColumns = false;
            this.cardList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.cardList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.cardList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mlviDataGridViewImageColumn,
            this.idLabelDataGridViewTextBoxColumn,
            this.cardTypeLabelDataGridViewTextBoxColumn,
            this.addressCopyIconDataGridViewTextBoxColumn,
            this.publicAddressDataGridViewTextBoxColumn,
            this.connectionStringDataGridViewTextBoxColumn,
            this.pkSourceDataGridViewTextBoxColumn,
            this.Remove});
            this.cardList.DataSource = this.bChipMemoryLayoutBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.cardList.DefaultCellStyle = dataGridViewCellStyle3;
            this.cardList.Location = new System.Drawing.Point(0, 0);
            this.cardList.Margin = new System.Windows.Forms.Padding(4);
            this.cardList.Name = "cardList";
            this.cardList.ReadOnly = true;
            this.cardList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.cardList.Size = new System.Drawing.Size(652, 948);
            this.cardList.TabIndex = 0;
            this.cardList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CardList_CellClick);
            this.cardList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cardList_CellContentClick);
            this.cardList.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.cardList_RowsAdded);
            this.cardList.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.CardList_RowsRemoved);
            this.cardList.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.cardList_RowStateChanged);
            // 
            // Remove
            // 
            this.Remove.HeaderText = "Remove";
            this.Remove.Image = ((System.Drawing.Image)(resources.GetObject("Remove.Image")));
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1133);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1327, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // mlviDataGridViewImageColumn
            // 
            this.mlviDataGridViewImageColumn.DataPropertyName = "mlvi";
            this.mlviDataGridViewImageColumn.HeaderText = "mlvi";
            this.mlviDataGridViewImageColumn.Name = "mlviDataGridViewImageColumn";
            this.mlviDataGridViewImageColumn.ReadOnly = true;
            this.mlviDataGridViewImageColumn.Visible = false;
            // 
            // idLabelDataGridViewTextBoxColumn
            // 
            this.idLabelDataGridViewTextBoxColumn.DataPropertyName = "IdLabel";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.idLabelDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.idLabelDataGridViewTextBoxColumn.HeaderText = "ID";
            this.idLabelDataGridViewTextBoxColumn.MinimumWidth = 80;
            this.idLabelDataGridViewTextBoxColumn.Name = "idLabelDataGridViewTextBoxColumn";
            this.idLabelDataGridViewTextBoxColumn.ReadOnly = true;
            this.idLabelDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.idLabelDataGridViewTextBoxColumn.Width = 80;
            // 
            // cardTypeLabelDataGridViewTextBoxColumn
            // 
            this.cardTypeLabelDataGridViewTextBoxColumn.DataPropertyName = "CardTypeLabel";
            this.cardTypeLabelDataGridViewTextBoxColumn.HeaderText = "Type";
            this.cardTypeLabelDataGridViewTextBoxColumn.MinimumWidth = 80;
            this.cardTypeLabelDataGridViewTextBoxColumn.Name = "cardTypeLabelDataGridViewTextBoxColumn";
            this.cardTypeLabelDataGridViewTextBoxColumn.ReadOnly = true;
            this.cardTypeLabelDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cardTypeLabelDataGridViewTextBoxColumn.Width = 80;
            // 
            // addressCopyIconDataGridViewTextBoxColumn
            // 
            this.addressCopyIconDataGridViewTextBoxColumn.DataPropertyName = "AddressCopyIcon";
            this.addressCopyIconDataGridViewTextBoxColumn.HeaderText = "Addr";
            this.addressCopyIconDataGridViewTextBoxColumn.MinimumWidth = 80;
            this.addressCopyIconDataGridViewTextBoxColumn.Name = "addressCopyIconDataGridViewTextBoxColumn";
            this.addressCopyIconDataGridViewTextBoxColumn.ReadOnly = true;
            this.addressCopyIconDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.addressCopyIconDataGridViewTextBoxColumn.Width = 80;
            // 
            // publicAddressDataGridViewTextBoxColumn
            // 
            this.publicAddressDataGridViewTextBoxColumn.DataPropertyName = "PublicAddress";
            this.publicAddressDataGridViewTextBoxColumn.HeaderText = "PublicAddress";
            this.publicAddressDataGridViewTextBoxColumn.Name = "publicAddressDataGridViewTextBoxColumn";
            this.publicAddressDataGridViewTextBoxColumn.ReadOnly = true;
            this.publicAddressDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.publicAddressDataGridViewTextBoxColumn.Visible = false;
            // 
            // connectionStringDataGridViewTextBoxColumn
            // 
            this.connectionStringDataGridViewTextBoxColumn.DataPropertyName = "ConnectionString";
            this.connectionStringDataGridViewTextBoxColumn.HeaderText = "Status";
            this.connectionStringDataGridViewTextBoxColumn.Name = "connectionStringDataGridViewTextBoxColumn";
            this.connectionStringDataGridViewTextBoxColumn.ReadOnly = true;
            this.connectionStringDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pkSourceDataGridViewTextBoxColumn
            // 
            this.pkSourceDataGridViewTextBoxColumn.DataPropertyName = "PkSource";
            this.pkSourceDataGridViewTextBoxColumn.HeaderText = "PK Status";
            this.pkSourceDataGridViewTextBoxColumn.Name = "pkSourceDataGridViewTextBoxColumn";
            this.pkSourceDataGridViewTextBoxColumn.ReadOnly = true;
            this.pkSourceDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.pkSourceDataGridViewTextBoxColumn.Visible = false;
            // 
            // bChipMemoryLayoutBindingSource
            // 
            this.bChipMemoryLayoutBindingSource.DataSource = typeof(BChipDesktop.BChipMemoryLayout);
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1327, 1155);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainUI";
            this.Text = "BChip BETA Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUI_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copyKeyIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectedIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.notConnectedIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qrCodeImage)).EndInit();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cardList)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bChipMemoryLayoutBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button ReceiveBtn;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.Button bchipBtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;

        private DataGridView cardList;
        private PictureBox pictureBox1;
        private ComboBox currencyComboBox;
        private Button saveToBchipBtn;
        private Button importKeyBtn;
        private Button createKeyBtn;
        private Button setPinBtn;
        private Label insertCardLabel;
        private PictureBox notConnectedIcon;
        private PictureBox connectedIcon;
        private Label errorLoadingLbl;
        private Button wipeBchipBtn;
        private Button exportKeyBtn;
        private TextBox passphraseEntryBox;
        private TextBox reconfirmPassphraseBox;
        private Button confirmPassphraseBtn;
        private Button savePassphraseBtn;
        private BindingSource bChipMemoryLayoutBindingSource;
        private Label keyTypeLabel;
        private Label notFormattedLabel;
        private Label bchipCrcFailLabel;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private LinkLabel keyAddressLabel;
        private PictureBox copyKeyIcon;
        private PictureBox qrCodeImage;
        private DataGridViewImageColumn mlviDataGridViewImageColumn;
        private DataGridViewTextBoxColumn idLabelDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cardTypeLabelDataGridViewTextBoxColumn;
        private DataGridViewImageColumn addressCopyIconDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn publicAddressDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn connectionStringDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pkSourceDataGridViewTextBoxColumn;
        private DataGridViewImageColumn Remove;
        private TextBox importKeyTextBox;
        private Button importConfirmKeyButton;
    }
}

