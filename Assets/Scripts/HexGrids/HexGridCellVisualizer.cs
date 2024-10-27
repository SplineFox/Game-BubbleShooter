#if UNITY_EDITOR

using BubbleShooter.HexGrids;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class HexGridCellVisualizer : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private HexGridLayout _hexGridLayout;

    private void OnDrawGizmosSelected()
    {
        if (_grid == null || _grid.cellLayout != GridLayout.CellLayout.Hexagon)
            return;

        var mousePoint = Event.current.mousePosition;
        var ray = HandleUtility.GUIPointToWorldRay(mousePoint);
        var worldPoint = new Vector3(ray.origin.x, ray.origin.y, _grid.transform.position.z);

        var cellPoint = _grid.WorldToCell(worldPoint);
        var cellHexPoint = _hexGridLayout.WorldToHex(worldPoint);

        var cellSize = (_grid.cellSize + Vector3.right * _grid.cellSize.x * (1f - Mathf.Cos(30f * Mathf.Deg2Rad))) * 0.5f;
        var cellCenterPoint = _grid.GetCellCenterWorld(cellPoint);

        var guiStyle = new GUIStyle(GUI.skin.label);
        guiStyle.alignment = TextAnchor.MiddleCenter;

        Gizmos.color = Color.yellow;
        DrawHexagon(cellCenterPoint, cellSize, true);
        GUI.color = Color.yellow;
        Handles.Label(cellCenterPoint, cellHexPoint.ToString(), guiStyle);
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
#endif