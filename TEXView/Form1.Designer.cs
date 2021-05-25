namespace TEXView
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtDestFolder = new System.Windows.Forms.TextBox();
            this.btnDestBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rbImagesSeparateFolder = new System.Windows.Forms.RadioButton();
            this.rbImagesSingleFolder = new System.Windows.Forms.RadioButton();
            this.rbDumpDTX = new System.Windows.Forms.RadioButton();
            this.rbDumpDTXZipFiles = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(113, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Browse for TEX.DAT";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtDestFolder
            // 
            this.txtDestFolder.Location = new System.Drawing.Point(12, 145);
            this.txtDestFolder.Name = "txtDestFolder";
            this.txtDestFolder.Size = new System.Drawing.Size(249, 20);
            this.txtDestFolder.TabIndex = 1;
            // 
            // btnDestBrowse
            // 
            this.btnDestBrowse.Location = new System.Drawing.Point(267, 142);
            this.btnDestBrowse.Name = "btnDestBrowse";
            this.btnDestBrowse.Size = new System.Drawing.Size(54, 23);
            this.btnDestBrowse.TabIndex = 2;
            this.btnDestBrowse.Text = "Browse";
            this.btnDestBrowse.UseVisualStyleBackColor = true;
            this.btnDestBrowse.Click += new System.EventHandler(this.btnDestBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Output Destination";
            // 
            // rbImagesSeparateFolder
            // 
            this.rbImagesSeparateFolder.AutoSize = true;
            this.rbImagesSeparateFolder.Location = new System.Drawing.Point(16, 13);
            this.rbImagesSeparateFolder.Name = "rbImagesSeparateFolder";
            this.rbImagesSeparateFolder.Size = new System.Drawing.Size(174, 17);
            this.rbImagesSeparateFolder.TabIndex = 4;
            this.rbImagesSeparateFolder.TabStop = true;
            this.rbImagesSeparateFolder.Text = "Dump Images (separate folders)";
            this.rbImagesSeparateFolder.UseVisualStyleBackColor = true;
            this.rbImagesSeparateFolder.Click += new System.EventHandler(this.rbImagesSeparateFolder_Click);
            // 
            // rbImagesSingleFolder
            // 
            this.rbImagesSingleFolder.AutoSize = true;
            this.rbImagesSingleFolder.Location = new System.Drawing.Point(16, 37);
            this.rbImagesSingleFolder.Name = "rbImagesSingleFolder";
            this.rbImagesSingleFolder.Size = new System.Drawing.Size(155, 17);
            this.rbImagesSingleFolder.TabIndex = 5;
            this.rbImagesSingleFolder.TabStop = true;
            this.rbImagesSingleFolder.Text = "Dump Images (single folder)";
            this.rbImagesSingleFolder.UseVisualStyleBackColor = true;
            this.rbImagesSingleFolder.Click += new System.EventHandler(this.rbImagesSingleFolder_Click);
            // 
            // rbDumpDTX
            // 
            this.rbDumpDTX.AutoSize = true;
            this.rbDumpDTX.Location = new System.Drawing.Point(16, 61);
            this.rbDumpDTX.Name = "rbDumpDTX";
            this.rbDumpDTX.Size = new System.Drawing.Size(102, 17);
            this.rbDumpDTX.TabIndex = 6;
            this.rbDumpDTX.TabStop = true;
            this.rbDumpDTX.Text = "Dump DTX Files";
            this.rbDumpDTX.UseVisualStyleBackColor = true;
            this.rbDumpDTX.Click += new System.EventHandler(this.rbDumpDTX_Click);
            // 
            // rbDumpDTXZipFiles
            // 
            this.rbDumpDTXZipFiles.AutoSize = true;
            this.rbDumpDTXZipFiles.Location = new System.Drawing.Point(16, 85);
            this.rbDumpDTXZipFiles.Name = "rbDumpDTXZipFiles";
            this.rbDumpDTXZipFiles.Size = new System.Drawing.Size(120, 17);
            this.rbDumpDTXZipFiles.TabIndex = 7;
            this.rbDumpDTXZipFiles.TabStop = true;
            this.rbDumpDTXZipFiles.Text = "Dump DTX.Zip Files";
            this.rbDumpDTXZipFiles.UseVisualStyleBackColor = true;
            this.rbDumpDTXZipFiles.Click += new System.EventHandler(this.rbDumpDTXZipFiles_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 224);
            this.Controls.Add(this.rbDumpDTXZipFiles);
            this.Controls.Add(this.rbDumpDTX);
            this.Controls.Add(this.rbImagesSingleFolder);
            this.Controls.Add(this.rbImagesSeparateFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDestBrowse);
            this.Controls.Add(this.txtDestFolder);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "TEX(Dat) Extractor 0.1 (Rattajin)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtDestFolder;
        private System.Windows.Forms.Button btnDestBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbImagesSeparateFolder;
        private System.Windows.Forms.RadioButton rbImagesSingleFolder;
        private System.Windows.Forms.RadioButton rbDumpDTX;
        private System.Windows.Forms.RadioButton rbDumpDTXZipFiles;
    }
}

