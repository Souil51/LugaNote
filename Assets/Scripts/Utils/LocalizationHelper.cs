using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine.Localization.Settings;

namespace Assets.Scripts.Utils
{
    public class LocalizationHelper
    {
        public static string GetString(string tableAndEntryName)
        {
            return GetStringAsync(tableAndEntryName).Result;
        }

        public static string GetString(string tableName, string entryName)
        {
            return GetStringAsync(tableName, entryName).Result;
        }

        public static string GetStringConcatenation(string separator, params string[] tableAndEntryNames)
        {
            return GetStringConcatenationAsync(separator, tableAndEntryNames).Result;
        }

        public static async Task<string> GetStringAsync(string tableAndEntryName)
        {
            string[] split = tableAndEntryName.Split("/");

            if (split.Length >= 2)
                return await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(split[0], split[1]).Task;
            else
                return null;
        }

        public static async Task<string> GetStringAsync(string tableName, string entryName)
        {
            return await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, entryName).Task;
        }

        public static async Task<string> GetStringConcatenationAsync(string separator, params string[] tableAndEntryNames)
        {
            string result = "";
            foreach(string entry in tableAndEntryNames)
            {
                if (!String.IsNullOrEmpty(result))
                    result += separator;

                result += await GetStringAsync(entry);
            }

            return result;
        }
    }
}
