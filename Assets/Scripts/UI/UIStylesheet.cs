using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIStylesheet", menuName = "UI/Stylesheet")]
public class UIStylesheet : ScriptableObject
{
    public List<TextStyle> TextStyles;

    [System.Serializable]
    public class TextStyle
    {
        public string Name;
        public Font Font;
        public int FontSize;
        public Color Color;
        public FontStyle FontStyle;
        public TextAnchor Alignment;
    }

    public TextStyle GetStyle(string styleName)
    {
        return TextStyles.Find(style => style.Name == styleName);
    }
}
