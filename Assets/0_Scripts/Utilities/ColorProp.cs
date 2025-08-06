using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public enum ObjColor { Red, Blue, Green, Yellow, Purple, Orange }

    public static class ColorUtils
    {
        private static readonly Dictionary<ObjColor, Color> colorMap = new()
        {
            { ObjColor.Red, Color.red },
            { ObjColor.Blue, Color.blue },
            { ObjColor.Green, Color.green },
            { ObjColor.Yellow, Color.yellow },
            { ObjColor.Purple, new Color(0.5f, 0f, 0.5f) },
            { ObjColor.Orange, new Color(1f, 0.5f, 0f) }
        };
        
        public static Color FromObjColor(ObjColor color)
        {
            return colorMap.TryGetValue(color, out var result) ? result : Color.white;
        }
    }
}
