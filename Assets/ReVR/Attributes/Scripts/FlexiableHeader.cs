using System;
using UnityEngine;
using UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class FlexiableHeader : PropertyAttribute
{
    public readonly Color color = defaultColor;
    public readonly string text;
    public readonly float size = defaultSize;
    public readonly TextAnchor anchor = defaultAnchor;
    public readonly FontStyle style = defaultStyle;
    public readonly WhiteSpace whiteSpace = defaultWhiteSpace;
    public readonly TextOverflow overflow = defaultOverflow;

    public readonly static Color defaultColor = new Color32(210, 210, 210, 255);
    public const float defaultSize = 12.0f;
    public const TextAnchor defaultAnchor = TextAnchor.MiddleLeft;
    public const FontStyle defaultStyle = FontStyle.Bold;
    public const WhiteSpace defaultWhiteSpace = WhiteSpace.Normal;
    public const TextOverflow defaultOverflow = TextOverflow.Clip;

    public FlexiableHeader(string text)
    {
        this.text = text;
    }

    public FlexiableHeader(string text, float size)
    {
        this.text = text;
        this.size = size;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, float r, float g, float b, float a)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = new Color(r, g, b, a);
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, uint hex)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = ColorHexToNormalizedColor(hex);
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, string hex)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = ColorHexStringToNormalizedColor(hex);
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, float r, float g, float b, float a, bool wrap)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = new Color(r, g, b, a);
        whiteSpace = wrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, uint hex, bool wrap)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = ColorHexToNormalizedColor(hex);
        whiteSpace = wrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, string hex, bool wrap)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = ColorHexStringToNormalizedColor(hex);
        whiteSpace = wrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, float r, float g, float b, float a, bool wrap, bool ifTextOverflowThenShowEllipsis)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = new Color(r, g, b, a);
        whiteSpace = wrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;
        overflow = ifTextOverflowThenShowEllipsis ? TextOverflow.Ellipsis : TextOverflow.Clip;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, uint hex, bool wrap, bool ifTextOverflowThenShowEllipsis)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = ColorHexToNormalizedColor(hex);
        whiteSpace = wrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;
        overflow = ifTextOverflowThenShowEllipsis ? TextOverflow.Ellipsis : TextOverflow.Clip;
    }

    public FlexiableHeader(string text, float size, TextAnchor anchor, FontStyle style, string hex, bool wrap, bool ifTextOverflowThenShowEllipsis)
    {
        this.text = text;
        this.size = size;
        this.anchor = anchor;
        this.style = style;
        color = ColorHexStringToNormalizedColor(hex);
        whiteSpace = wrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;
        overflow = ifTextOverflowThenShowEllipsis ? TextOverflow.Ellipsis : TextOverflow.Clip;
    }

    private static Color ColorHexStringToNormalizedColor(string hex)
    {
        if (!ValidateColorHexString(hex))
            return Color.clear;

        if (hex.Length == 3)
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}ff";

        if (hex.Length == 6)
            hex += "ff";

        uint hexCode = 0;
        foreach (char c in hex)
        {
            hexCode = (hexCode << 4) | HexCharToInt(c);
        }

        return ColorHexToNormalizedColor(hexCode);
    }

    private static Color ColorHexToNormalizedColor(uint hex)
    {
        uint r = (hex & 0xff000000) >> 24;
        uint g = (hex & 0xff0000) >> 16;
        uint b = (hex & 0xff00) >> 8;
        uint a = hex & 0xff;
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }

    private static bool ValidateColorHexString(string hex)
    {
        if (string.IsNullOrEmpty(hex) || string.IsNullOrWhiteSpace(hex))
            return false;

        if (hex.Length != 3 && hex.Length != 6 && hex.Length != 8)
            return false;

        foreach (char c in hex)
        {
            if (!IsValidColorHexChar(c))
                return false;
        }

        return true;
    }

    private static bool IsValidColorHexChar(char ch)
    {
        return ch switch
        {
            'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'A' or 'B' or 'C' or 'D' or 'E' or 'F' or '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => true,
            _ => false,
        };
    }

    private static uint HexCharToInt(char ch)
    {
        return ch switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            'a' => 10,
            'b' => 11,
            'c' => 12,
            'd' => 13,
            'e' => 14,
            'f' => 15,
            'A' => 10,
            'B' => 11,
            'C' => 12,
            'D' => 13,
            'E' => 14,
            'F' => 15,
            _ => uint.MaxValue,
        };
    }

}
