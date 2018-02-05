using UnityEngine;
namespace BubbleShooter
{

    public class BubbleMatrixControllerHelper
    {

        public static Vector2 CellForPosition(Vector3 position, BubbleMatrixGeometry geometry, bool isBaselineAlignedLeft)
        {
            int row = geometry.rows - Mathf.FloorToInt(position.y) - 1;
            int column;
            bool rowIsEven = row % 2 == 0;
            if ((rowIsEven && isBaselineAlignedLeft) || (!rowIsEven && !isBaselineAlignedLeft))
            {
                column = Mathf.FloorToInt(position.x);
            }
            else
            {
                column = Mathf.FloorToInt(position.x - geometry.bubbleRadius);
            }
            Vector2 result = new Vector2(row, Utils.TruncateToInterval(column, 0, geometry.columns - 1));
            return result;
        }


        public static Vector3 PositionForCell(Vector2 cell, BubbleMatrixGeometry geometry, bool isBaselineAlignedLeft)
        {
            bool rowIsEven = cell.x % 2 == 0;
            float y = geometry.rows - cell.x - geometry.bubbleRadius;
            float x;
            if (isBaselineAlignedLeft)
            {
                if (rowIsEven)
                {
                    x = cell.y + geometry.bubbleRadius;
                }
                else
                {
                    x = cell.y + 2 * geometry.bubbleRadius;
                }
            }
            else
            {
                if (rowIsEven)
                {
                    x = cell.y + 2 * geometry.bubbleRadius;
                }
                else
                {
                    x = cell.y + geometry.bubbleRadius;
                }
            }
            return new Vector3(x, y, geometry.depth);
        }
    }

}