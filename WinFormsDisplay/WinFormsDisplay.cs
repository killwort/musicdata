using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LyricsCore;
using LyricsCore.Configuration;
using Ninject;

namespace WinFormsDisplay
{
    public class WinFormsDisplay : Display
    {
        private DisplayForm _form;

        [Inject]
        public InjectableSetting<float> FontSize { get; set; }
        [Inject]
        public InjectableSetting<string> FontFace { get; set; }

        public WinFormsDisplay()
        {
            new Thread(AppThread) { IsBackground = true }.Start();
        }


        void AppThread()
        {
            _form = new DisplayForm(); //_kernel.Get<DisplayForm>();
            _form.Font = new Font(FontFace.ValueOrDefault(_form.Font.Name), FontSize.ValueOrDefault(_form.Font.Size));

            Application.Run(_form);
            Environment.Exit(0);
        }

        public override void DoDisplay(Lyric lyric)
        {
            if (_form.InvokeRequired)
            {
                _form.Invoke(new DisplayDelegate(DoDisplay), lyric);
                return;
            }
            _form.Display(lyric);
        }

        private delegate void DisplayDelegate(Lyric lyric);
    }
}
