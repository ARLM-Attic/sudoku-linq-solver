using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing.Printing;

namespace SudokuLINQSolver.Configurations
{
    public class PageSettings : ConfigurationSection
    {
        private static String SECTION_NAME = "pegeSettings";
        private static PageSettings s_instance;

        static PageSettings()
        {
            s_instance = Config.Instance.GetSection(SECTION_NAME) as PageSettings;

            if (s_instance == null)
            {
                s_instance = new PageSettings();
                s_instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_instance);
            }
        }

        public static PageSettings Instance
        {
            get
            {
                return s_instance;
            }
        }

        public void Save(System.Drawing.Printing.PageSettings a_settings)
        {
            Margins margins = PrinterUnitConvert.Convert(a_settings.Margins, 
                PrinterUnit.ThousandthsOfAnInch, PrinterUnit.TenthsOfAMillimeter);
            MarginLeft = margins.Left;
            MarginRight = margins.Right;
            MarginTop = margins.Top;
            MarginBottom = margins.Bottom;

            Landscape = a_settings.Landscape;
            PaperSizeName = a_settings.PaperSize.PaperName;
            PaperSourceName = a_settings.PaperSource.SourceName;

            Config.Instance.Save();
        }

        public void Restore(System.Drawing.Printing.PageSettings a_settings)
        {
            Margins margins = new Margins(MarginLeft, MarginRight, MarginTop, MarginBottom);
            margins = PrinterUnitConvert.Convert(margins, 
                PrinterUnit.TenthsOfAMillimeter, PrinterUnit.HundredthsOfAMillimeter);
            a_settings.Margins = margins;

            a_settings.Landscape = Landscape;

            foreach (PaperSize paper_size in a_settings.PrinterSettings.PaperSizes)
            {
                if (paper_size.PaperName == PaperSizeName)
                    a_settings.PaperSize = paper_size;
            }

            foreach (PaperSource printer_source in a_settings.PrinterSettings.PaperSources)
            {
                if (printer_source.SourceName == PaperSourceName)
                    a_settings.PaperSource = printer_source;
            }
        }

        [ConfigurationProperty("marginLeft", DefaultValue = 10)]
        public int MarginLeft
        {
            get
            {
                return (int)base["marginLeft"];
            }
            set
            {
                base["marginLeft"] = value;
            }
        }

        [ConfigurationProperty("marginRight", DefaultValue = 10)]
        public int MarginRight
        {
            get
            {
                return (int)base["marginRight"];
            }
            set
            {
                base["marginRight"] = value;
            }
        }

        [ConfigurationProperty("marginTop", DefaultValue = 10)]
        public int MarginTop
        {
            get
            {
                return (int)base["marginTop"];
            }
            set
            {
                base["marginTop"] = value;
            }
        }

        [ConfigurationProperty("marginBottom", DefaultValue = 10)]
        public int MarginBottom
        {
            get
            {
                return (int)base["marginBottom"];
            }
            set
            {
                base["marginBottom"] = value;
            }
        }

        [ConfigurationProperty("paperSourceName", DefaultValue = "")]
        public string PaperSourceName
        {
            get
            {
                return (string)base["paperSourceName"];
            }
            set
            {
                base["paperSourceName"] = value;
            }
        }

        [ConfigurationProperty("paperSizeName", DefaultValue = "")]
        public string PaperSizeName
        {
            get
            {
                return (string)base["paperSizeName"];
            }
            set
            {
                base["paperSizeName"] = value;
            }
        }

        [ConfigurationProperty("landscape", DefaultValue = false)]
        public bool Landscape
        {
            get
            {
                return (bool)base["landscape"];
            }
            set
            {
                base["landscape"] = value;
            }
        }
    }
}
