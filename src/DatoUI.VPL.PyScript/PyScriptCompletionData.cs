using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DatoUI.VPL.PyScript
{
    public class PyScriptCompletionData :ICompletionData
    {
        public PyScriptCompletionData (string text)
        {
            Text = text;
        }
        public ImageSource Image
        {
            get => null;
        }

        public string Text { get; private set; }

        public object Content => this.Text;

        public object Description => $"Keyword for {Text}";

        public double Priority => 0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (textArea.Document.Text[completionSegment.Offset - 1] == Text[0])
            {
                textArea.Document.Replace(completionSegment, this.Text.Substring(1));
            }
            else
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }

        }
    }
}
