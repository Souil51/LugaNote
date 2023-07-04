using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenManager
{
    public static float ScreenWidth => Screen.width;
    public static float ScreenHeight => Screen.height;
    public static float ScreenRatio => (float)Screen.width / (float)Screen.height;
}
