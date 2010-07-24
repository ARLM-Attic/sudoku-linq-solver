using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SudokuLib
{
    public static class Directories
    {
        public static string Solutions
        {
            get
            {
                return new DirectoryInfo(System.Reflection.Assembly.GetAssembly(typeof(SudokuLib.SudokuSolutionNode)).Location).
                    Parent.Parent.Parent.Parent.CreateSubdirectory("SudokuLINQSolver").CreateSubdirectory("Solutions").FullName;
            }
        }

        public static string Examples
        {
            get
            {
                var di = new DirectoryInfo(new DirectoryInfo(System.Reflection.Assembly.GetAssembly(
                    typeof(SudokuLib.SudokuSolutionNode)).Location).Parent.FullName + 
                    Path.DirectorySeparatorChar + "Examples");

                if (di.Exists)
                    return di.FullName;

                return new DirectoryInfo(System.Reflection.Assembly.GetAssembly(typeof(SudokuLib.SudokuSolutionNode)).Location).
                    Parent.Parent.Parent.Parent.CreateSubdirectory("SudokuLINQSolver").CreateSubdirectory("Examples").FullName;
            }
        }

        public static string TestExamples
        {
            get
            {
                return new DirectoryInfo(System.Reflection.Assembly.GetAssembly(typeof(SudokuLib.SudokuSolutionNode)).Location).
                    Parent.Parent.Parent.Parent.CreateSubdirectory("SudokuTest").CreateSubdirectory("Examples").FullName;
            }
        }

        public static string SolutionTrees
        {
            get
            {
                return new DirectoryInfo(System.Reflection.Assembly.GetAssembly(typeof(SudokuLib.SudokuSolutionNode)).Location).
                    Parent.Parent.Parent.Parent.CreateSubdirectory("SudokuLINQSolver").CreateSubdirectory("SolutionTrees").FullName;
            }
        }

        public static string ConvertName(string a_name)
        {
            string str1 = Path.GetFileNameWithoutExtension(a_name);
            string str2 = Path.GetFileNameWithoutExtension(str1);
            string str3 = str1.Substring(0, str2.Length - "_rotate_0".Length);
            string str4 = str3 + Path.GetExtension(str1) + Path.GetExtension(a_name);
            return str4;
        }
    }
}
