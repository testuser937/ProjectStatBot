using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulBot
{
    public static class UserState
    {
        public static string statName { get; set; } = "";
        public static string statQuery { get; set; } = "";
        public static string statMessage { get; set; } = "";
        public static bool statIsActive { get; set; }

    }
}
