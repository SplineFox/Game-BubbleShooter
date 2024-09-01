using UnityEditor;
using UnityEngine;

public class GridCellVisualizer : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;

    private void OnDrawGizmos()
    {
        if (_grid == null)
            return;

        var mousePosition = Event.current.mousePosition;
        var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        var worldPosition = new Vector3(ray.origin.x, ray.origin.y, _grid.transform.position.z);

        var cellPosition = _grid.WorldToCell(worldPosition);
        var cellCenterPosition = _grid.GetCellCenterWorld(cellPosition);

        Gizmos.color = Color.yellow;
        DrawHexagon(cellCenterPosition, _grid.cellSize / 2f, true);
        Handles.Label(cellCenterPosition, cellPosition.ToString());
    }

    private void DrawHexagon(Vector3 hexCenter, Vector3 hexSize, bool isPointy)
    {
        for (int index = 0; index < 6; index++)
        {
            var currentCorner = GetHexCornerPosition(hexCenter, hexSize, index, isPointy);
            var nextCorner = GetHexCornerPosition(hexCenter, hexSize, index + 1, isPointy);
            Gizmos.DrawLine(currentCorner, nextCorner);
        }
    }

    private Vector3 GetHexCornerPosition(Vector3 center, Vector3 size, int index, bool isPointy)
    {
        var offset = isPointy ? -30f : 0f;
        var angleDegrees = 60 * index + offset;
        var angleRadians = angleDegrees * Mathf.Deg2Rad;

        return new Vector3(
            center.x + size.x * Mathf.Cos(angleRadians),
            center.y + size.y * Mathf.Sin(angleRadians),
            center.z);
    }
}
