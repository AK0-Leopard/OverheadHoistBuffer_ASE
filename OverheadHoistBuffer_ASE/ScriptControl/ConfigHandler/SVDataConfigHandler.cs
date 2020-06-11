// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="SVDataConfigHandler.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ConfigHandler
{
    /// <summary>
    /// Class SVDataConfigHandler.
    /// </summary>
    public class SVDataConfigHandler
    {
        //;SVID,Name,"length(PLC LW)","type(BCD:2,INT:1,CHAR:0)",dot,"symbol(1:有,0:無)"
        /// <summary>
        /// The tpy e_ de f_ elemen t_ count
        /// </summary>
        private static readonly int TPYE_DEF_ELEMENT_COUNT = 6;
        /// <summary>
        /// The filename
        /// </summary>
        private string filename;
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get { return filename; } }
        /// <summary>
        /// Gets the sv data type list.
        /// </summary>
        /// <value>The sv data type list.</value>
        public List<SVDataType> SVDataTypeList { get; private set; }
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="SVDataConfigHandler"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public SVDataConfigHandler(string file) 
        {
            SVDataTypeList = new List<SVDataType>();
            reload(file);
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        public void reload()
        {
            reload(this.filename);
        }

        /// <summary>
        /// Reloads the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void reload(String filename)
        {
            this.filename = filename;
            SVDataTypeList.Clear();
            //Environment.CurrentDirectory+this.getString("CsvConfig", "") + tableName + ".csv"
            string real_file_name =
                Environment.CurrentDirectory + SCApplication.getInstance().getCsvConfigPath() + "\\" + filename;
            if (System.IO.File.Exists(real_file_name))
                loadFromFile(real_file_name);
            else
                System.IO.File.Create(real_file_name);
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="Exception">Process Data Config Has Error!</exception>
        private void loadFromFile(String file)
        {
            foreach (String line in System.IO.File.ReadAllLines(file))
            {
                if ((!String.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")))
                {
                    string[] valueAry = line.Split(new char[] { ',' },
                        TPYE_DEF_ELEMENT_COUNT, StringSplitOptions.None);

                    if (valueAry.Length < 6)
                    {
                        throw new Exception("Process Data Config Has Error!");
                    }
                    string svid = string.Empty;
                    if (valueAry.Length > 0)
                    {
                        try
                        {
                            svid = valueAry[0];
                        }
                        catch (Exception ex) { logger.Error(ex, "Exception:"); }
                    }
                    string name = string.Empty;
                    if (valueAry.Length > 1)
                    {
                        try
                        {
                            name = valueAry[1];
                        }
                        catch (Exception ex) { logger.Error(ex, "Exception:"); }
                    }
                    int wordCount = 1;
                    if (valueAry.Length > 2)
                    {
                        try
                        {
                            wordCount = Convert.ToInt32(valueAry[2]);
                        }
                        catch (Exception ex) { logger.Error(ex, "Exception:"); }
                    }
                    SVDataType.DataItemType itemType = SVDataType.DataItemType.ASCII;
                    if (valueAry.Length > 3)
                    {
                        try
                        {
                            itemType = (SVDataType.DataItemType)Convert.ToInt32(valueAry[3]);
                        }
                        catch (Exception ex) { logger.Error(ex, "Exception:"); }
                    }
                    double multiplier = 1;
                    if (valueAry.Length > 4)
                    {
                        try
                        {
                            //                            multiplier = Convert.ToDouble(valueAry[4]);
                            int dot = Convert.ToInt16(valueAry[4]);
                            multiplier = Math.Pow(0.1, dot);
                        }
                        catch (Exception ex) { logger.Error(ex, "Exception:"); }
                    }
                    bool isContainSign = false;
                    if (valueAry.Length > 5)
                    {
                        try
                        {
                            isContainSign = (Convert.ToInt32(valueAry[5]) == 1) ? true : false;
                        }
                        catch (Exception ex) { logger.Error(ex, "Exception:"); }
                    }
                    SVDataType dataType = new SVDataType(itemType, svid, name, wordCount, multiplier, isContainSign);
                    SVDataTypeList.Add(dataType);
                }
            }
        }

    }

    /// <summary>
    /// Class SVDataType.
    /// </summary>
    public class SVDataType
    {
        /// <summary>
        /// Enum DataItemType
        /// </summary>
        public enum DataItemType
        {
            /// <summary>
            /// The ASCII
            /// </summary>
            ASCII = 0,
            /// <summary>
            /// The int
            /// </summary>
            INT = 1,
            /// <summary>
            /// The BCD
            /// </summary>
            BCD = 2
        }
        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        public DataItemType ItemType { get; private set; }

        /// <summary>
        /// Gets the svid.
        /// </summary>
        /// <value>The svid.</value>
        public string SVID { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the word count.
        /// </summary>
        /// <value>The word count.</value>
        public int WordCount { get; private set; }  //PLC LW個數

        /// <summary>
        /// Gets the multiplier.
        /// </summary>
        /// <value>The multiplier.</value>
        public double Multiplier { get; private set; }  //乘數
        /// <summary>
        /// Gets the is contain sign.
        /// </summary>
        /// <value>The is contain sign.</value>
        public Boolean IsContainSign { get; private set; }  //是否為有號數

        /// <summary>
        /// Initializes a new instance of the <see cref="SVDataType"/> class.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="svid">The svid.</param>
        /// <param name="name">The name.</param>
        /// <param name="wordCound">The word cound.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="isContainSign">if set to <c>true</c> [is contain sign].</param>
        public SVDataType(DataItemType itemType, string svid, string name, int wordCound,
            double multiplier, bool isContainSign)
        {
            this.ItemType = itemType;
            this.SVID = svid;
            this.Name = name;
            this.WordCount = wordCound;
            this.Multiplier = multiplier;
            this.IsContainSign = isContainSign;
        }
    }
}
