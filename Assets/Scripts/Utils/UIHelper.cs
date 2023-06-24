using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class UIHelper
    {
        public static Color GetColorFromHEX(string hex)
        {
            if(String.IsNullOrEmpty(hex))
                return Color.white;

            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length == 6)
                hex += "FF";

            uint hexInt = Convert.ToUInt32(hex, 16);
            var r = ((hexInt & 0xff000000) >> 0x18) / 255f;
            var g = ((hexInt & 0xff0000) >> 0x10) / 255f;
            var b = ((hexInt & 0xff00) >> 8) / 255f;
            var a = ((hexInt & 0xff)) / 255f;

            return new Color(r, g, b, a);
        }
    }
}
