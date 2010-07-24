using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndoRedoLib
{
    // TODO: dodac do testowej ofrmy
    // dodac mozliwosc zapisu w konfiguracji aplikacji
    // zmiana opcji odswieza stan wizualny
    public static class UndoRedoConfiguration
    {
        public static int UndoRedo_MaxSubMenuItems = 10;

        // Dodac kontrole wiekosci, dodac mozliwosc kontroli w oparciu o zuzycie pamieci
        public static int UndoRedo_MaxDeep = 100;
        public static int Navigation_MaxDeep = 100;

        public static bool UndoRedo_ModifyTooltips = true;
        public static bool Navigation_ModifyTooltips = true;

        public static bool UndoRedo_SubMenu = true;
        public static bool Navigation_SubMenu = true;

        public static string Navigation_InitialStateDescription = "Initial";

        public static bool Logging = true;
    }
}
