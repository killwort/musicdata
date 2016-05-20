namespace WinFormsDisplay
{
    partial class DisplayForm
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
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblArtist = new System.Windows.Forms.Label();
            this.lblAlbum = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.pnlScroll = new System.Windows.Forms.Panel();
            this.lblLyrics = new System.Windows.Forms.Label();
            this.pbAlbumArt = new System.Windows.Forms.PictureBox();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.pnlScroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlbumArt)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(this.lblTitle);
            flowLayoutPanel1.Controls.Add(label3);
            flowLayoutPanel1.Controls.Add(this.lblArtist);
            flowLayoutPanel1.Controls.Add(label5);
            flowLayoutPanel1.Controls.Add(this.lblAlbum);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(776, 13);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(51, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Song title";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(60, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(18, 13);
            label3.TabIndex = 1;
            label3.Text = "by";
            // 
            // lblArtist
            // 
            this.lblArtist.AutoSize = true;
            this.lblArtist.Location = new System.Drawing.Point(84, 0);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(57, 13);
            this.lblArtist.TabIndex = 2;
            this.lblArtist.Text = "Song artist";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(147, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(27, 13);
            label5.TabIndex = 3;
            label5.Text = "from";
            // 
            // lblAlbum
            // 
            this.lblAlbum.AutoSize = true;
            this.lblAlbum.Location = new System.Drawing.Point(180, 0);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(63, 13);
            this.lblAlbum.TabIndex = 4;
            this.lblAlbum.Text = "Song album";
            // 
            // pnlScroll
            // 
            this.pnlScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.Controls.Add(this.lblLyrics);
            this.pnlScroll.Location = new System.Drawing.Point(0, 19);
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Size = new System.Drawing.Size(375, 372);
            this.pnlScroll.TabIndex = 4;
            this.pnlScroll.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblLyrics_MouseDown);
            this.pnlScroll.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLyrics_MouseMove);
            this.pnlScroll.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblLyrics_MouseUp);
            // 
            // lblLyrics
            // 
            this.lblLyrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLyrics.AutoSize = true;
            this.lblLyrics.Location = new System.Drawing.Point(8, 8);
            this.lblLyrics.Name = "lblLyrics";
            this.lblLyrics.Size = new System.Drawing.Size(0, 13);
            this.lblLyrics.TabIndex = 5;
            this.lblLyrics.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblLyrics_MouseDown);
            this.lblLyrics.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLyrics_MouseMove);
            this.lblLyrics.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblLyrics_MouseUp);
            // 
            // pbAlbumArt
            // 
            this.pbAlbumArt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbAlbumArt.Location = new System.Drawing.Point(381, 19);
            this.pbAlbumArt.Name = "pbAlbumArt";
            this.pbAlbumArt.Size = new System.Drawing.Size(395, 372);
            this.pbAlbumArt.TabIndex = 5;
            this.pbAlbumArt.TabStop = false;
            // 
            // DisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 393);
            this.Controls.Add(this.pbAlbumArt);
            this.Controls.Add(this.pnlScroll);
            this.Controls.Add(flowLayoutPanel1);
            this.Name = "DisplayForm";
            this.Text = "DisplayForm";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.pnlScroll.ResumeLayout(false);
            this.pnlScroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlbumArt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.Label lblAlbum;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Panel pnlScroll;
        private System.Windows.Forms.Label lblLyrics;
        private System.Windows.Forms.PictureBox pbAlbumArt;
    }
}