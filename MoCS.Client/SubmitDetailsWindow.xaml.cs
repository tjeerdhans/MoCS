using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MoCS.Client
{
    /// <summary>
    /// Interaction logic for SubmitDetailsWindow.xaml
    /// </summary>
    public partial class SubmitDetailsWindow : Window
    {
        public SubmitDetailsWindow()
        {
            InitializeComponent();
        }

        public SubmitDetailsWindow(string details)
        {
            InitializeComponent();
            this.richTextBox1.AppendText(details);
        }

        public SubmitDetailsWindow(string details, bool isCode)
        {
            InitializeComponent();
            RichTextBoxSyntaxHighlighter highlighter = new RichTextBoxSyntaxHighlighter();
            this.richTextBox1.TextChanged += new TextChangedEventHandler(highlighter.TextChangedEventHandler);
            this.richTextBox1.AppendText(details);
        }


                        

    }
}
