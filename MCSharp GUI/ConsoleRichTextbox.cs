using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSharp_GUI
{
    public class ConsoleRichTextbox : RichTextBox
    {
        const short WM_PAINT = 0x00f;
        public ConsoleRichTextbox()
        {
        }

        public bool _Paint = true;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // Code courtesy of Mark Mihevc
            // sometimes we want to eat the paint message so we don't have to see all the
            // flicker from when we select the text to change the color.
            if (m.Msg == WM_PAINT)
            {
                if (_Paint)
                    base.WndProc(ref m); // if we decided to paint this control, just call the RichTextBox WndProc
                else
                    m.Result = IntPtr.Zero; // not painting, must set this to IntPtr.Zero if not painting therwise serious problems.
            }
            else
                base.WndProc(ref m); // message other than WM_PAINT, jsut do what you normally do.
        }
    }


    public static class RichTextBoxExtensions
    {
        public static void AppendText(this ConsoleRichTextbox box, string text, Color color)
        {
            box._Paint = false;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box._Paint = true;
        }
    }
}