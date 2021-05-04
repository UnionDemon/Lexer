using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    class CodeHandler
    {
        private RichTextBox richTextBox;

        private Regex integer;

        public CodeHandler(RichTextBox textBox)
        {
            richTextBox = textBox;
            integer = new Regex(@"\bint\b");
        }

        public void HandleText()
        {
            MatchCollection matchCollection = integer.Matches(richTextBox.Text);
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;
            richTextBox.SelectionStart = 0;
            richTextBox.SelectionLength = richTextBox.Text.Length;
            richTextBox.SelectionColor = Color.Black;
            foreach (Match match in matchCollection)
            {
                richTextBox.SelectionStart = match.Index;
                richTextBox.SelectionLength = match.Length;
                richTextBox.SelectionColor = Color.Red;
            }
            richTextBox.SelectionStart = selectionStart;
            richTextBox.SelectionLength = selectionLength;
        }
    }
}
