using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : GridLayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public FitType fitType;
    public int rows;
    public int columns;
    public bool fitX;
    public bool fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        // Filter out children with ignoreLayout enabled
        var validChildren = rectChildren.FindAll(child =>
        {
            var layoutElement = child.GetComponent<LayoutElement>();
            return layoutElement == null || !layoutElement.ignoreLayout;
        });

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            float sqrRt = Mathf.Sqrt(validChildren.Count);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt((float)validChildren.Count / columns);
        }
        else if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt((float)validChildren.Count / rows);
        }

        fitX = (fitType == FitType.Width || fitType == FitType.Uniform || fitType == FitType.FixedColumns);
        fitY = (fitType == FitType.Height || fitType == FitType.Uniform || fitType == FitType.FixedRows);

        float cellWidth = (parentWidth / (float)columns) - (spacing.x * ((float)columns - 1) / (float)columns) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / (float)rows) - (spacing.y * ((float)rows - 1) / (float)rows) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize = new Vector2(
            fitX ? cellWidth : cellSize.x,
            fitY ? cellHeight : cellSize.y
        );

        float totalWidth = (cellSize.x * columns) + (spacing.x * (columns - 1));
        float totalHeight = (cellSize.y * rows) + (spacing.y * (rows - 1));

        Vector2 startOffset = GetStartOffset(totalWidth, totalHeight);

        for (int i = 0; i < validChildren.Count; i++)
        {
            var item = validChildren[i];

            int rowCount = i / columns;
            int columnCount = i % columns;

            var xPos = startOffset.x + (cellSize.x * columnCount) + (spacing.x * columnCount);
            var yPos = startOffset.y + (cellSize.y * rowCount) + (spacing.y * rowCount);

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    private Vector2 GetStartOffset(float totalWidth, float totalHeight)
    {
        float xOffset = padding.left;
        float yOffset = padding.top;

        if (childAlignment == TextAnchor.UpperCenter || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.LowerCenter)
        {
            xOffset += (rectTransform.rect.width - totalWidth) / 2f;
        }
        else if (childAlignment == TextAnchor.UpperRight || childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.LowerRight)
        {
            xOffset += rectTransform.rect.width - totalWidth - padding.right;
        }

        if (childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.MiddleRight)
        {
            yOffset += (rectTransform.rect.height - totalHeight) / 2f;
        }
        else if (childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.LowerRight)
        {
            yOffset += rectTransform.rect.height - totalHeight - padding.bottom;
        }

        return new Vector2(xOffset, yOffset);
    }

    public override void CalculateLayoutInputVertical()
    {
        // Intentionally left empty as layout is calculated in CalculateLayoutInputHorizontal.
    }

    public override void SetLayoutHorizontal()
    {
        // Intentionally left empty as layout is calculated in CalculateLayoutInputHorizontal.
    }

    public override void SetLayoutVertical()
    {
        // Intentionally left empty as layout is calculated in CalculateLayoutInputHorizontal.
    }
}