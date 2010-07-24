using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing.Printing;
using SudokuLib;

namespace SudokuLINQSolver.Configurations
{
    public class Options : ConfigurationSection
    {
        private static String SECTION_NAME = "options";
        private static Options s_instance;

        static Options()
        {
            s_instance = Config.Instance.GetSection(SECTION_NAME) as Options;

            if (s_instance == null)
            {
                s_instance = new Options();
                s_instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_instance);
            }
        }

        public static Options Instance
        {
            get
            {
                return s_instance;
            }
        }

        private void OnChanged()
        {
            if (Changed != null)
                Changed();
        }

        public event Action Changed;

        [ConfigurationProperty("showAllSolutions", DefaultValue = false)]
        public bool ShowAllSolutions
        {
            get
            {
                return (bool)base["showAllSolutions"];
            }
            set
            {
                base["showAllSolutions"] = value;
                OnChanged();
            }
        }

        [ConfigurationProperty("includeBoxes", DefaultValue = false)]
        public bool IncludeBoxes
        {
            get
            {
                return (bool)base["includeBoxes"];
            }
            set
            {
                base["includeBoxes"] = value;
                OnChanged();
            }
        }

        public SudokuOptions SudokuOptions
        {
            get
            {
                return new SudokuOptions() 
                { 
                    IncludeBoxes = IncludeBoxes, 
                    ShowAllSolutions = ShowAllSolutions 
                };
            }
        }
    }
}
