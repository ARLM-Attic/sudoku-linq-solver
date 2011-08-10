using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using SudokuLib;
using System.IO;
using System.Diagnostics;
using TomanuExtensions;

namespace SudokuLINQSolver.Configurations
{
    #region RecentFileElement
    internal class RecentFileElement : ConfigurationElement
    {
        public RecentFileElement()
        {
        }

        public RecentFileElement(string a_fileName)
        {
            FileName = a_fileName;
        }

        [ConfigurationProperty("fileName", DefaultValue = "", IsRequired = false, IsKey = true)]
        public string FileName
        {
            get
            {
                return (string)this["fileName"];
            }
            set
            {
                this["fileName"] = value;
            }

        }
    }
    #endregion

    # region RecentFileCollection
    public class RecentFileCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RecentFileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as RecentFileElement).FileName;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        internal RecentFileElement this[int index]
        {
            get
            {
                return (RecentFileElement)BaseGet(index);
            }
        }

        internal void Add(string a_lastOpenFile)
        {
            BaseRemove(a_lastOpenFile);
            base.BaseAdd(new RecentFileElement(a_lastOpenFile));
        }

        public void Opened(string a_lastOpenFile)
        {
            BaseRemove(a_lastOpenFile);
            base.BaseAdd(0, new RecentFileElement(a_lastOpenFile));
        }

        public string[] FileNames
        {
            get
            {
                object[] ar = BaseGetAllKeys();

                if (ar == null)
                    return new string[0];
                else
                    return (from o in ar select o as string).ToArray();
            }
        }

        public void Clear()
        {
            base.BaseClear();
        }
    }
    #endregion

    #region LastOpenFiles
    public class LastOpenFiles : ConfigurationSection
    {
        private static String SECTION_NAME = "lastOpenFiles";
        private static LastOpenFiles s_instance;

        static LastOpenFiles()
        {
            s_instance = Config.Instance.GetSection(SECTION_NAME) as LastOpenFiles;
   
            if (s_instance == null)
            {
                s_instance = new LastOpenFiles();
                s_instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_instance);
            }  
        }

        public static LastOpenFiles Instance 
        {
            get
            {
                return s_instance;
            }
        }

        public void Opened(string a_lastOpenFile)
        {
            RecentFiles.Opened(a_lastOpenFile);
            LastOpenFile = a_lastOpenFile;
            try
            {
                LastOpenDir = Path.GetDirectoryName(a_lastOpenFile);
            }
            catch (ArgumentException)
            {
            }
            Check();
            Config.Instance.Save();
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public RecentFileCollection RecentFiles
        {
            get
            {
                return base[""] as RecentFileCollection;
            }
        }

        [ConfigurationProperty("count", DefaultValue = "6")]
        [IntegerValidator(MinValue = 1, MaxValue = 10, ExcludeRange = false)]
        public Int32 Count
        {
            get
            {
                return (Int32)base["count"];
            }
            set
            {
                base["count"] = value;
            }
        }

        [ConfigurationProperty("lastopenfile", DefaultValue = "")]
        public string LastOpenFile
        {
            get
            {
                return (string)base["lastopenfile"];
            }
            set
            {
                base["lastopenfile"] = value;
                try
                {
                    if (value != "")
                        LastOpenDir = new System.IO.FileInfo(value).DirectoryName;
                }
                catch (Exception)
                {
                    LastOpenDir = "";
                }
            }
        }

        [ConfigurationProperty("lastopendir", DefaultValue = "")]
        public string LastOpenDir
        {
            get
            {
                return (string)base["lastopendir"];
            }
            set
            {
                base["lastopendir"] = value;
            }
        }

        private string CheckFileName(string a_fileName)
        {
            try
            {
                if (!new System.IO.FileInfo(a_fileName).Exists)
                    return string.Empty;

                return a_fileName;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private string CheckDirectory(string a_fileName)
        {
            try
            {
               String str = new System.IO.DirectoryInfo(a_fileName).FindExistingDirectory();

               if (str.Equals(""))
                   return Directories.Examples;

               return str;
            }
            catch (Exception)
            {
                return Directories.Examples;
            }
        }

        public void Check()
        {
            string[] ar = RecentFiles.FileNames;

            for (int i=0; i<ar.Length; i++)
            {
                try
                {
                    if (!new System.IO.FileInfo(ar[i]).Exists)
                        ar[i] = "";
                }
                catch (Exception)
                {
                    ar[i] = "";
                }
            }

            var v1 = ar.Where((s) => !s.Equals("")).Select((s, index) => new { s, index } );
            var v2 = v1.Distinct();
            var v3 = v2.OrderBy((index) => index);
            var v4 = v3.Take(Count);

            RecentFiles.Clear();
            v2.Take(20).ForEach((v) => RecentFiles.Add(v.s));

            LastOpenFile = CheckFileName(LastOpenFile);
            LastOpenDir = CheckDirectory(LastOpenDir);
        }
    }
    #endregion
}
