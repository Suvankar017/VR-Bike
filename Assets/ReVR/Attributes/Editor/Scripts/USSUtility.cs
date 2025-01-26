using UnityEngine;
using UnityEngine.UIElements;

public static class USSUtility
{

    #region Background Image

    public static void SetBackgroundImageScaleMode(IStyle style, ScaleMode mode)
    {
        if (style == null)
            return;

        style.backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(mode);
        style.backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(mode);
        style.backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(mode);
        style.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(mode);
    }

    #endregion

    #region Margin Utils

    public static void SetMargin(IStyle style, float margin)
    {
        if (style == null)
            return;

        style.marginTop = margin;
        style.marginRight = margin;
        style.marginBottom = margin;
        style.marginLeft = margin;
    }

    public static void SetMargin(IStyle style, float topAndBottomMargin, float leftAndRightMargin)
    {
        if (style == null)
            return;

        style.marginTop = topAndBottomMargin;
        style.marginRight = leftAndRightMargin;
        style.marginBottom = topAndBottomMargin;
        style.marginLeft = leftAndRightMargin;
    }

    public static void SetMargin(IStyle style, float topMargin, float rightMargin, float bottomMargin, float leftMargin)
    {
        if (style == null)
            return;

        style.marginTop = topMargin;
        style.marginRight = rightMargin;
        style.marginBottom = bottomMargin;
        style.marginLeft = leftMargin;
    }

    public static void SetMargin(IStyle style, float topMargin, float rightMargin, float bottomMargin)
    {
        if (style == null)
            return;

        style.marginTop = topMargin;
        style.marginRight = rightMargin;
        style.marginBottom = bottomMargin;
        style.marginLeft = 0.0f;
    }

    #endregion

    #region Padding Utils

    public static void SetPadding(IStyle style, float padding)
    {
        if (style == null)
            return;

        style.paddingTop = padding;
        style.paddingRight = padding;
        style.paddingBottom = padding;
        style.paddingLeft = padding;
    }

    public static void SetPadding(IStyle style, float topAndBottomPadding, float leftAndRightPadding)
    {
        if (style == null)
            return;

        style.paddingTop = topAndBottomPadding;
        style.paddingRight = leftAndRightPadding;
        style.paddingBottom = topAndBottomPadding;
        style.paddingLeft = leftAndRightPadding;
    }

    public static void SetPadding(IStyle style, float topPadding, float rightPadding, float bottomPadding, float leftPadding)
    {
        if (style == null)
            return;

        style.paddingTop = topPadding;
        style.paddingRight = rightPadding;
        style.paddingBottom = bottomPadding;
        style.paddingLeft = leftPadding;
    }

    public static void SetPadding(IStyle style, float topPadding, float rightPadding, float bottomPadding)
    {
        if (style == null)
            return;

        style.paddingTop = topPadding;
        style.paddingRight = rightPadding;
        style.paddingBottom = bottomPadding;
        style.paddingLeft = 0.0f;
    }

    #endregion

}
