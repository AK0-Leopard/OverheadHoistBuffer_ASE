using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Resources;
using System.Diagnostics;
using System.Reflection;
using NLog;

namespace com.mirle.ibg3k0.bc.winform
{
    public static class Localization
    {
        public static string Lang { get; private set; }
        public static string Font { get; private set; }
        private static List<Dictionary<string, string>> translateDictionaryList = new List<Dictionary<string, string>>();
        private static Dictionary<string, string> namespaceDictionary = new Dictionary<string, string>();
        public static LanguageConfigHandler LCH = new LanguageConfigHandler();
        static ResXResourceWriter resx;
        static string containerName;
        static string resourcefileName;
        static string alResources;
        static string dir = Directory.GetCurrentDirectory();
        static List<string> resourcefileNameList = new List<string>();
        public static int index = 0;
        public static string formNamespace;
        public static bool buildResource = LCH.getBuildFlag();
        public static bool changeFont = LCH.getFontFlag();
        public static bool genTranslateKeys = LCH.getGenKeysFlag();
        static List<string> KeyList = new List<string>();//存放需翻譯的key值
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Load(string path)
        {
            readLanguage(path);
        }
        private static void readLanguage(string path)
        {
            // Read the language file
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read Language File Failed.");
                logger.Error(ex, "Exception");
                return;
            }
            // Begin to parase
            try
            {
                reader.ReadToFollowing("language");
                Localization.Lang = reader.GetAttribute("language");
                Localization.Font = reader.GetAttribute("Font");
                translateDictionaryList.Add(new Dictionary<string, string>());
                paraseTranslataXml(reader, "content", translateDictionaryList[index]);
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read Language File Failed.");
                logger.Error(ex, "Exception");
                return;
            }
            return;
        }
        private static void createTranslateXML(string fileName, List<string> keyList)
        {
            // 建立一個 XmlDocument 物件並加入 Declaration
            XmlDocument xdoc = new XmlDocument();
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            XmlComment comment1 = xdoc.CreateComment("language為語系(如:zh-TW,zh-CN),Font為字型(如Arial,microsoft sans serif)");
            xdoc.AppendChild(comment1);
            // 建立根節點物件並加入 XmlDocument 中 (第 0 層)
            XmlElement rootElement = xdoc.CreateElement("language");
            XmlAttribute rootAttChild1 = xdoc.CreateAttribute("language");
            if (Lang == null)
            {
                rootAttChild1.Value = "zh-TW";//預設繁中
            }
            else
            {
                rootAttChild1.Value = Lang;
            }
            XmlAttribute rootAttChild2 = xdoc.CreateAttribute("Font");
            if (Font == null)
            {
                rootAttChild2.Value = "Arial";//預設給這個字型
            }
            else
            {
                rootAttChild2.Value = Font;
            }
            rootElement.Attributes.Append(rootAttChild1);
            rootElement.Attributes.Append(rootAttChild2);
            xdoc.AppendChild(rootElement);

            // 建立一個子元素, 並在這個子元素裡加上一個 attribute (第 1 層)
            XmlElement eleChild1 = xdoc.CreateElement("content");
            rootElement.AppendChild(eleChild1);
            XmlComment comment2 = xdoc.CreateComment("請對照key值翻譯後填入value");
            eleChild1.AppendChild(comment2);
            // 再為這個子元素加入一個孫元素 (第 2 層)
            foreach (string key in keyList)
            {
                XmlElement eleGrandChild1 = xdoc.CreateElement("Item");
                XmlAttribute grandAttChild1 = xdoc.CreateAttribute("key");
                grandAttChild1.Value = key;
                XmlAttribute grandAttChild2 = xdoc.CreateAttribute("value");
                if (translateDictionaryList.Count != 0 && translateDictionaryList[index].ContainsKey(key.ToLower().Replace(" ", "")))
                {
                    string text = "";
                    translateDictionaryList[index].TryGetValue(key.ToLower().Replace(" ", ""), out text);
                    grandAttChild2.Value = text;
                }
                else
                {
                    grandAttChild2.Value = key;
                }
                eleGrandChild1.Attributes.Append(grandAttChild1);
                eleGrandChild1.Attributes.Append(grandAttChild2);
                eleChild1.AppendChild(eleGrandChild1);
            }
            // 將建立的 XML 節點儲存為檔案
            if (File.Exists(fileName))
            {
                DialogResult result = MessageBox.Show(
                    "File " + fileName + " already exist,do you want to overwrite the file?", "File exists!"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                // Process message box results
                switch (result)
                {
                    case DialogResult.Yes:
                        xdoc.Save(fileName);
                        break;
                    case DialogResult.No:
                        // do nothing
                        break;
                }
            }
            else
            {
                xdoc.Save(fileName);
            }
        }
        private static void getTranlateKeyFromResx(string fileName, out List<string> keyList)
        {
            keyList = KeyList;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("root").ChildNodes;//查找
            foreach (XmlNode node in nodeList)
            {
                XmlElement element;
                if (node.NodeType == XmlNodeType.Element)//此node為element
                {
                    element = (XmlElement)node;
                    if (element.Name == "data")//名稱為data
                    {
                        if (element.GetAttribute("name").EndsWith(".Text") || element.GetAttribute("name").EndsWith(".HeaderText"))//
                        {
                            foreach (XmlNode childNode in element.ChildNodes)
                            {
                                if (childNode.Name == "value")
                                {
                                    string text = null;
                                    XmlElement childElement = (XmlElement)childNode;
                                    text = childElement.InnerText;
                                    if (!keyList.Exists(x => x.ToLower().Replace(" ", "") == text.ToLower().Replace(" ", "")))
                                    {
                                        keyList.Add(text.Trim());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void changeResxText(string fileName, Dictionary<string, string> obj)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("root").ChildNodes;//查找
            foreach (XmlNode node in nodeList)
            {
                XmlElement element;

                if (node.NodeType == XmlNodeType.Element)//此node為element
                {
                    element = (XmlElement)node;
                    if (element.Name == "data")//名稱為data
                    {
                        if (changeFont == true && element.GetAttribute("name").EndsWith(".Font"))//更改Font
                        {
                            foreach (XmlNode childNode in element.ChildNodes)
                            {
                                if (childNode.Name == "value")
                                {
                                    XmlElement childElement = (XmlElement)childNode;
                                    char[] separators = new char[] { ',' };
                                    childElement.InnerText = Font + "," + childElement.InnerText.Split(separators, 2)[1];
                                    break;
                                }
                            }
                        }
                        else if (element.GetAttribute("name").EndsWith(".Text") || element.GetAttribute("name").EndsWith(".HeaderText"))
                        {
                            foreach (XmlNode childNode in element.ChildNodes)
                            {
                                if (childNode.Name == "value")
                                {
                                    string text = null;
                                    XmlElement childElement = (XmlElement)childNode;
                                    text = childElement.InnerText;
                                    int headSpaceCount = countHeadSpace(text);
                                    int tailSpaceCount = countTailSpace(text);
                                    text = text.Replace(" ", "");
                                    if (obj.TryGetValue(text.ToLower(), out text))
                                    {
                                        text = addSpaceAtHeadAndtail(headSpaceCount, tailSpaceCount, text);
                                        childElement.InnerText = text;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (element.GetAttribute("name") == ">>$this.Name")
                        {
                            foreach (XmlNode childNode in element.ChildNodes)
                            {
                                if (childNode.Name == "value")
                                {
                                    XmlElement childElement = (XmlElement)childNode;
                                    containerName = childElement.InnerText;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            xmlDoc.Save(fileName);
        }
        private static void paraseTranslataXml(XmlReader reader, string item, Dictionary<string, string> obj)
        {
            // Get the attribute key & value 
            reader.ReadToFollowing(item);
            XmlReader subreader = reader.ReadSubtree();
            while (subreader.Read())
            {
                if (subreader.NodeType == XmlNodeType.Element && subreader.Name == "Item")
                    obj.Add((subreader.GetAttribute("key")).Replace(" ", "").ToLower(), subreader.GetAttribute("value"));
            }
        }
        public static void BuildMultiLanguageResources()
        {
            if (genTranslateKeys)
            {
                DirectoryInfo resxParentDir = Directory.GetParent(Directory.GetParent(dir).FullName);
                FileInfo[] resxFileArray = resxParentDir.GetFiles("*.resx", SearchOption.AllDirectories);//取得所有.resx檔
                foreach (FileInfo resxFile in resxFileArray)
                {
                    getTranlateKeyFromResx(resxFile.FullName, out KeyList);//建立keyList
                }
                KeyList.Sort();//內容排序
                if (!Directory.Exists(dir + @"\Language"))
                {
                    Directory.CreateDirectory(dir + @"\Language");
                }
                DirectoryInfo langDirInfo = new DirectoryInfo(dir + @"\Language");
                FileInfo[] langFiles = langDirInfo.GetFiles();
                if (langFiles.Length > 0)
                {
                    foreach (FileInfo file in langFiles)
                    {
                        Load(file.FullName);//建立翻譯用的辭典
                        createTranslateXML(dir + @"\Language\" + Lang + @".xml", KeyList);
                        index++;
                    }
                }
                else
                {
                    createTranslateXML(dir + @"\Language\zh-TW.xml", KeyList);//都沒有的話預設給一個繁中的範例
                }
                Lang = null;
                Font = null;
                translateDictionaryList.Clear();
                index = 0;
                //清乾淨避免影響到後面buildResource出問題
            }
            if (buildResource)//根據config buildResource決定是否建立資源檔
            {

                Assembly a = Assembly.GetExecutingAssembly();       //取得目前組件
                foreach (Type t in a.GetTypes())                    //找尋組件內所有類別型態
                {
                    //自動取得所有Form來建立資源檔
                    if (t.IsSubclassOf(typeof(ContainerControl)))           //如果父類別是繼承自ContainerControl的話
                    {
                        namespaceDictionary.Add(t.Name, t.Namespace);
                    }
                }
                System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-US");//以英文為翻譯基準
                System.Threading.Thread.CurrentThread.CurrentUICulture = CI;
                DirectoryInfo resxParentDir = Directory.GetParent(Directory.GetParent(dir).FullName);

                FileInfo[] resxFileArray = resxParentDir.GetFiles("*.resx", SearchOption.AllDirectories);
                if (!Directory.Exists(dir + @"\Language"))
                {
                    Directory.CreateDirectory(dir + @"\Language");
                }
                foreach (string filePath in System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + @"\Language"))//對每個在Language資料夾裡的語言檔案件進行處理
                {
                    Load(filePath);//建立翻譯用的辭典
                    foreach (FileInfo resxFile in resxFileArray)
                    {
                        string newResxFileName = dir + @"\" + Path.GetFileNameWithoutExtension(resxFile.FullName) + "." + Lang + ".resx";
                        FileInfo newResxFile = resxFile.CopyTo(newResxFileName, true);
                        newResxFile.IsReadOnly = false;
                        //newResxFile.Refresh();
                        changeResxText(newResxFileName, translateDictionaryList[index]);
                        if (containerName != null)
                        {
                            namespaceDictionary.TryGetValue(containerName, out formNamespace);
                            ResGen(newResxFileName);
                            alResources += (@" /embed:" + resourcefileName);
                        }
                        //resourcefileName = dir + @"\" + formNameSpace + "." + controlName + "." + Lang + ".resources";
                    }
                    //Localization.generateResource(new BCMainForm());
                    //Localization.generateResource(new com.mirle.ibg3k0.bc.winform.UI.BCMain(bcapp));
                    //Localization.generateResource(new uiPort());
                    generateResourceDll();

                    index++;
                }
            }
            if (!LCH.failOrNot())
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(LCH.getLanguage());
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;//更換當前語系
            }
            FileInfo[] resxFileArrayhaha2 = new DirectoryInfo(dir).GetFiles("*.resx", SearchOption.AllDirectories);
            foreach (FileInfo fi in resxFileArrayhaha2)
            {
                File.Delete(fi.FullName);
            }
        }
        public static void generateResourceDll()
        {
            Process al = new Process();
            if (!Directory.Exists(dir + @"\" + Lang))//若無對應語言目錄則建立一個
            {
                Directory.CreateDirectory(dir + @"\" + Lang);
            }

            al.StartInfo.FileName = "al.exe";
            string processName = Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
            File.WriteAllText(dir + @"\" + "al.txt", @"/t:lib" + alResources + @" /culture:" + Lang + @" /out:" + dir + @"\" + Lang + @"\" + processName + ".resources.dll");

            al.StartInfo.Arguments = "@al.txt";
            al.Start();
            al.WaitForExit();
            alResources = "";
            foreach (string fileName in resourcefileNameList)
            {
                File.Delete(fileName);
            }
            File.Delete(dir + @"\" + "al.txt");
            resourcefileNameList.Clear();
        }
        public static void ResGen(string sourceFilename)
        {
            resourcefileName = dir + @"\" + formNamespace + "." + containerName + "." + Lang + ".resources";
            Process resGen = new Process();
            resGen.StartInfo.FileName = "ResGen.exe";
            resGen.StartInfo.Arguments = sourceFilename + " " + resourcefileName;
            resGen.Start();
            resGen.WaitForExit();
            resourcefileNameList.Add(resourcefileName);
            File.Delete(sourceFilename);
            formNamespace = null;
            containerName = null;

        }

        private static string addSpaceAtHeadAndtail(int headSpaceCount, int tailSpaceCount, string str)
        {
            string headSqace = "";
            string tailSqace = "";
            for (int i = 0; i < headSpaceCount; i++)
            {
                headSqace += " ";
            }
            for (int i = 0; i < tailSpaceCount; i++)
            {
                tailSqace += " ";
            }
            str = headSqace + str + tailSqace;
            return str;
        }
        private static int countHeadSpace(string str)//找出字串開頭有多少空白
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c != ' ')
                {
                    return count;
                }
                else
                {
                    count++;
                }
            }
            return count;
        }
        private static int countTailSpace(string str)//找出字串結尾有多少空白
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c != ' ')
                {
                    count = 0;
                }
                else
                {
                    count++;
                }
            }
            return count;
        }
    }
}
