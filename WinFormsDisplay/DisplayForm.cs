using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MusicData;
using Ninject;

namespace WinFormsDisplay
{
    public partial class DisplayForm : Form
    {
        
        public DisplayForm()
        {
            InitializeComponent();
        }

        public void Display(Lyric lyric)
        {
            lblAlbum.Text = lyric.OriginalMetadata.Album;
            lblArtist.Text = lyric.OriginalMetadata.Artist;
            lblTitle.Text = lyric.OriginalMetadata.Title;
            lblLyrics.Text = lyric.Text.Replace("\n","\r\n");
        }
        public void Display(AlbumArt art)
        {
            try
            {
                pbAlbumArt.Image = new Bitmap(new MemoryStream(art.ImageData));
            }
            catch { }
        }

        private Point? grabPoint;
        private int grabScroll;
        private void lblLyrics_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
                grabPoint = e.Location;
                grabScroll = pnlScroll.VerticalScroll.Value;
                ((Control)sender).Capture = true;
            }
        }

        private void lblLyrics_MouseUp(object sender, MouseEventArgs e)
        {
            grabPoint = null;
            ((Control)sender).Capture = false;
        }

        private void lblLyrics_MouseMove(object sender, MouseEventArgs e)
        {
            if (grabPoint.HasValue)
            {
                var newScroll=grabScroll-(e.Location.Y - grabPoint.Value.Y);
                pnlScroll.VerticalScroll.Value = Math.Max(Math.Min(pnlScroll.VerticalScroll.Maximum, newScroll), pnlScroll.VerticalScroll.Minimum);
            }
            pnlScroll.Focus();
        }
    }
}
