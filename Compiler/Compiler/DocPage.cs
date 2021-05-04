using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    class DocPage
    {
        private static string defaultTitle = "Default Title Not Initialised";
        private string resultText;
        private string text;
        private string title;
        private string fileName;
        private bool saved;
        private Stack<string> States, CanceledStates;

        public string Text
        {
            get => text;
            set
            {
                saved = false;
                text = value;
                if (text != States.Peek())
                    SaveState();
            }
        }
        public string ResultText { get => resultText; }
        public string Title { get => title; }
        public bool Saved { get => saved; }
        public bool CanCancel { get => States.Count > 1; }
        public bool CanRepeat { get => CanceledStates.Count > 0; }
        public string FileName { get => fileName; set => fileName = value; }
        public static string DefaultTitle { get => defaultTitle; set => defaultTitle = value; }

        public static DocPage OpenFromFile(string fileName)
        {
            StreamReader file = new StreamReader(fileName);
            Regex regex = new Regex(@"[^\\]+$");
            MatchCollection matchCollection = regex.Matches(fileName);

            DocPage ret = new DocPage(matchCollection[matchCollection.Count - 1].Value);
            ret.text = file.ReadToEnd();
            file.Close();
            ret.States.Clear();
            ret.SaveState();
            ret.fileName = fileName;
            ret.saved = true;
            return ret;
        }
        public DocPage(string title)
        {
            text = "";
            resultText = "";
            fileName = null;
            this.title = title;
            saved = false;
            States = new Stack<string>();
            CanceledStates = new Stack<string>();
            SaveState();
        }
        public DocPage()
        {
            text = "";
            resultText = "";
            fileName = null;
            this.title = defaultTitle;
            saved = false;
            States = new Stack<string>();
            CanceledStates = new Stack<string>();
            SaveState();
        }

        public bool Close()
        {
            if (saved)
                return true;
            DialogResult res = MessageBox.Show("Сохранить файл " + title + "?", "Сохранение файла", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Yes)
                Save();
            if (res == DialogResult.Cancel)
                return false;
            return true;
        }

        public bool Save()
        {
            if (saved)
                return true;
            if (fileName == null)
                return SaveAs();
            StreamWriter file = new StreamWriter(fileName);
            file.Write(text);
            file.Close();
            saved = true;
            return true;
        }
        public bool SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = title;
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.Cancel)
                return false;
            fileName = saveFileDialog.FileName;
            Regex regex = new Regex(@"[^\\]+$");
            MatchCollection matchCollection = regex.Matches(fileName);
            title = matchCollection[matchCollection.Count - 1].Value;
            StreamWriter file = new StreamWriter(fileName);
            file.Write(text);
            file.Close();
            saved = true;
            return true;
        }
        private void SaveState()
        {
            CanceledStates.Clear();
            States.Push(text);
        }
        public void CancelState()
        {
            if (States.Count == 1)
                return;
            CanceledStates.Push(States.Pop());
            text = States.Peek();
        }
        public void RepeatState()
        {
            if (CanceledStates.Count == 0)
                return;
            States.Push(CanceledStates.Pop());
            text = States.Peek();
        }

    }
}
