using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static Color invertColor(Color prevColor, float alpha = 1f) =>
        new Color(1f - prevColor.r, 1f - prevColor.g, 1f - prevColor.b, prevColor.a);
    
    public static Color changeOpacity(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
   
}
