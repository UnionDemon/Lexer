using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Compiler
{
    class LocalisationController
    {
        public struct LocalisationElement
        {
            public string Name;
            public string Text;

            public LocalisationElement(string Name, string Text)
            {
                this.Name = Name;
                this.Text = Text;
            }
        }
        private class NameComparer
        {
            string _s;

            public NameComparer(string s)
            {
                _s = s;
            }

            public bool cmp(LocalisationElement elem)
            {
                return elem.Name == _s;
            }
        }
        private struct Localisation
        {
            public string name;
            public string path;
            public List<LocalisationElement> localisationElements;

            public Localisation(string Name, string Path)
            {
                name = Name;
                path = Path;
                localisationElements = new List<LocalisationElement>();

                StreamReader file;
                try
                {
                    file = new StreamReader(Path);
                }
                catch (Exception e)
                {
                    throw new Exception("Can't open file");
                }
                localisationElements.Clear();
                while (!file.EndOfStream)
                    localisationElements.Add(new LocalisationElement(file.ReadLine(), file.ReadLine()));
            }
        }



        List<Localisation> localisations;
        int currentLocalisationIndex;

        public string CurrentLocalisation
        {
            get => localisations[currentLocalisationIndex].name;
            set
            {
                int index = 0;
                foreach (Localisation t in localisations)
                {
                    if (t.name == value)
                    {
                        currentLocalisationIndex = index;
                        return;
                    }
                    index++;
                }
            }
        }
        public List<string> Localisations
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (Localisation t in localisations)
                    ret.Add(t.name);
                return ret;
            }
        }

        public LocalisationController(string localisationsFileName)
        {
            StreamReader file;
            try
            {
                file = new StreamReader(localisationsFileName, Encoding.Default);
            }
            catch (Exception e)
            {
                throw new Exception("Не получается открыть файл " + localisationsFileName);
            }
            Regex pathReg = new Regex(@"[\w|/\.]+(?=\W*=)", RegexOptions.Multiline);
            Regex nameReg = new Regex(@"(?<==\W*)(\w)+", RegexOptions.Multiline);
            string buffer = file.ReadToEnd();
            MatchCollection paths = pathReg.Matches(buffer);
            MatchCollection names = nameReg.Matches(buffer);

            if (paths.Count != names.Count)
                throw new Exception("Файл локализации имеет неверный формат");
            if (paths.Count == 0)
                throw new Exception("Не найдено ни одной локализации");

            localisations = new List<Localisation>();
            for (int i = 0; i < paths.Count; i++)
                localisations.Add(new Localisation(names[i].Value, paths[i].Value));

            currentLocalisationIndex = 0;
        }

        public string this[string Name]
        {
            get
            {
                NameComparer nc = new NameComparer(Name);
                int index = localisations[currentLocalisationIndex].localisationElements.FindIndex(nc.cmp);
                if (index == -1)
                    return "NOT FOUND";
                else
                    return localisations[currentLocalisationIndex].localisationElements[index].Text;
            }
        }
    }
}
