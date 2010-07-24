using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    public static class FileExtensions
    {
        public static string ZipMask
        {
            get
            {
                return "*.zip";
            }
        }

        public static string XmlZipMask
        {
            get
            {
                return "*.xml.zip";
            }
        }

        public static string XmlMask
        {
            get
            {
                return "*.xml";
            }
        }

        public static string TxtMask
        {
            get
            {
                return "*.txt";
            }
        }

        public static string ZipExt
        {
            get
            {
                return ".zip";
            }
        }

        public static string XmlZipExt
        {
            get
            {
                return ".xml.zip";
            }
        }

        public static string XmlExt
        {
            get
            {
                return ".xml";
            }
        }

        public static string TxtExt
        {
            get
            {
                return ".txt";
            }
        }
    }
}
