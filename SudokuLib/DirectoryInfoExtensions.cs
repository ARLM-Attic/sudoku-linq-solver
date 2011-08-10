//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;

//namespace SudokuLib
//{
//    public static class DirectoryInfoExtensions
//    {
//        public static void DeleteAll(this DirectoryInfo a_dir_info)
//        {
//            if (!a_dir_info.Exists)
//                return;

//            foreach (FileInfo file_info in a_dir_info.GetFiles())
//                file_info.Delete();

//            foreach (DirectoryInfo dir_info in a_dir_info.GetDirectories())
//                dir_info.DeleteAll();

//            a_dir_info.Delete(false);
//        }

//        public static void DeleteContent(this DirectoryInfo a_dir_info)
//        {
//            if (!a_dir_info.Exists)
//                return;

//            foreach (FileInfo file_info in a_dir_info.GetFiles())
//                file_info.Delete();

//            foreach (DirectoryInfo dir_info in a_dir_info.GetDirectories())
//                dir_info.DeleteAll();
//        }

//        public static void CreateOrEmpty(this DirectoryInfo a_dir_info)
//        {
//            a_dir_info.DeleteContent();
//            a_dir_info.Create();
//        }
//    }
//}
