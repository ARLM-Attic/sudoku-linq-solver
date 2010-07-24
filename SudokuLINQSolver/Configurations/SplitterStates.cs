using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using SudokuLib;
using System.IO;
using System.Diagnostics;

namespace SudokuLINQSolver.Configurations
{
    #region SplitterElement
    public class SplitterElement : ConfigurationElement
    {
        public SplitterElement()
        {
        }

        public SplitterElement(string a_splitterContainerName)
        {
            SplitContainerName = a_splitterContainerName;
        }

        [ConfigurationProperty("splitContainerName", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string SplitContainerName
        {
            get
            {
                return (string)this["splitContainerName"];
            }
            set
            {
                this["splitContainerName"] = value;
                Config.Instance.Save();
            }

        }

        [ConfigurationProperty("distance", DefaultValue = (int)0, IsRequired = true)]
        public int Distance
        {
            get
            {
                return (int)this["distance"];
            }
            set
            {
                this["distance"] = value;
                Config.Instance.Save();
            }

        }
    }
    #endregion

    # region SplittersCollections
    public class SplittersCollections : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SplitterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as SplitterElement).SplitContainerName;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public new SplitterElement this[string a_splitterContainerName]
        {
            get
            {
                if (BaseGet(a_splitterContainerName) == null)
                    Add(a_splitterContainerName);

                return BaseGet(a_splitterContainerName) as SplitterElement; ;
            }
        }

        protected void Add(string a_splitterContainerName)
        {
            BaseRemove(a_splitterContainerName);
            base.BaseAdd(new SplitterElement(a_splitterContainerName));
            Config.Instance.Save();
        }
    }
    #endregion

    #region Splitters
    public class Splitters : ConfigurationSection
    {
        private static String SECTION_NAME = "splitters";
        private static Splitters s_instance;

        static Splitters()
        {
            s_instance = Config.Instance.GetSection(SECTION_NAME) as Splitters;

            if (s_instance == null)
            {
                s_instance = new Splitters();
                s_instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_instance);
            }  
        }

        public static Splitters Instance 
        {
            get
            {
                return s_instance;
            }
        }

        public new SplitterElement this[string a_splitterContainerName]
        {
            get
            {
                return SplittersCollection[a_splitterContainerName];
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        internal SplittersCollections SplittersCollection
        {
            get
            {
                return base[""] as SplittersCollections;
            }
        }
    }
    #endregion
}
