using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace MoCS.Client
{
    public class RichTextBoxSyntaxHighlighter
    {
        private List<Tag> _tags = new List<Tag>();

        private struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;

        }

        public void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.RichTextBox rtb = (System.Windows.Controls.RichTextBox)sender;

            if (rtb.Document == null)
                return;

            TextRange documentRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = rtb.Document.ContentStart;
            while (navigator.CompareTo(rtb.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);

                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            Format(rtb);
        }

        void Format(System.Windows.Controls.RichTextBox rtb)
        {
            rtb.TextChanged -= this.TextChangedEventHandler;

            for (int i = 0; i < _tags.Count; i++)
            {
                TextRange range = new TextRange(_tags[i].StartPosition, _tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            _tags.Clear();

            rtb.TextChanged += this.TextChangedEventHandler;
        }

        void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | CSSyntaxProvider.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | CSSyntaxProvider.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        if (CSSyntaxProvider.IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            _tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (CSSyntaxProvider.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                _tags.Add(t);
            }
        }
    }
}
