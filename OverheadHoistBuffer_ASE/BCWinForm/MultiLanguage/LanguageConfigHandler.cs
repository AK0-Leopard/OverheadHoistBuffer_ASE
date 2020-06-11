using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.bc.winform
{
    public class LanguageConfigHandler
    {
        private readonly XmlDocument xmlDocument = new XmlDocument();
        private string language;
        private bool buildResource;
        private bool changeFont;
        private bool genTranslateKeys;
        private bool fail = false;
        private string xmlPath;
        private string SettingPath = @"\Language\Setting\setting.xml";
        public LanguageConfigHandler()
        {
            //xmlPath = Directory.GetCurrentDirectory() + @"\App.config";
            xmlPath = Directory.GetCurrentDirectory() + SettingPath; 
            if (!File.Exists(xmlPath))
            {
                System.Windows.Forms.MessageBox.Show("Change language fail.");
                fail = true;
                return;
            }
            xmlDocument.Load(xmlPath);
            loadConfig();
        }
        private void loadConfig()
        {
            XmlNode baseAddr = xmlDocument.SelectSingleNode("//Language");
            //XmlNode baseAddr = xmlDocument.SelectSingleNode("//userSettings"); 
            if (baseAddr == null)
            {
                throw new Exception("The Language Node Does Not Exist!");
            }
            language = Paser(baseAddr);
        }
        private string Paser(XmlNode section)
        {
            XmlAttributeCollection attributes;

            string languageStr = null;
            attributes = section.Attributes;
            foreach (XmlAttribute attribute in attributes)
            {
                if (attribute.Name == "language")
                {
                    languageStr = attribute.Value;
                    //break;
                }
                else if (attribute.Name == "buildResource")
                {
                    if (attribute.Value == "Y" || attribute.Value == "y")
                    {
                        buildResource = true;
                    }
                    else
                    {
                        buildResource = false;
                    }
                }
                else if (attribute.Name == "changeFont")
                {
                    if (attribute.Value == "Y" || attribute.Value == "y")
                    {
                        changeFont = true;
                    }
                    else
                    {
                        changeFont = false;
                    }
                }
                else if (attribute.Name == "genTranslateKeys")
                {
                    if (attribute.Value == "Y" || attribute.Value == "y")
                    {
                        genTranslateKeys = true;
                    }
                    else
                    {
                        genTranslateKeys = false;
                    }
                }
            }
            return languageStr;
        }
        public void changeConfigLanguage(string language)
        {
            XmlNode baseAddr = xmlDocument.SelectSingleNode("//Language");
            if (baseAddr == null)
            {
                throw new Exception("The Language Node Does Not Exist!");
            }
            XmlAttributeCollection attributes;

            attributes = baseAddr.Attributes;
            foreach (XmlAttribute attribute in attributes)
            {
                if (attribute.Name == "language")
                {
                    attribute.Value = language;
                    xmlDocument.Save(xmlPath);
                    this.language = language;
                    break;
                }
            }
        }
        public string getLanguage()
        {
            return language;
        }
        public bool getBuildFlag()
        {
            return buildResource;
        }
        public bool getFontFlag()
        {
            return changeFont;
        }
        public bool getGenKeysFlag()
        {
            return genTranslateKeys;
        }
        public bool failOrNot()
        {
            return fail;
        }
    }
}
