using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CustomBA
{
    static class CustomAction
    {
        internal static void Backup(string installFolder)
        {
            Debug.WriteLine($"Backup. installFolder: {installFolder}");

        }

        internal static void RemoveBackup()
        {
            Debug.WriteLine($"RemoveBackup. ");
        }
    }
}
