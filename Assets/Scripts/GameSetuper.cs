using UnityEngine;

namespace BubbleShooter
{
    public static class GameSetuper
    {
        private const float WALL_THIÑKNESS = 1f;

        public static void Setup(Vector2Int gridSize, Vector2 gridCellSize, Camera camera)
        {
            var gridWidth = gridSize.x * gridCellSize.x + gridCellSize.x / 2f;
            var gridHeight = gridSize.y * (gridCellSize.y * 0.75f);

            var gridMinX = -gridCellSize.x / 2f;
            var gridMinY = gridCellSize.y / 2f - gridHeight;

            var gridRect = new Rect(gridMinX, gridMinY, gridWidth, gridHeight);

            SetupCamera(gridRect, camera);
            SetupWalls(gridRect);
        }

        private static void SetupCamera(Rect gridRect, Camera camera)
        {
            camera.orthographic = true;
            camera.orthographicSize = gridRect.width;
            camera.transform.position = gridRect.center;
        }

        private static void SetupWalls(Rect gridRect)
        {
            var thicknessDoubled = WALL_THIÑKNESS * 2f;
            var thicknessHalf = WALL_THIÑKNESS / 2f;

            var topWall = CreateWall(gridRect.width + thicknessDoubled, WALL_THIÑKNESS);
            var leftWall = CreateWall(WALL_THIÑKNESS, gridRect.height + thicknessDoubled);
            var rightWall = CreateWall(WALL_THIÑKNESS, gridRect.height + thicknessDoubled);

            topWall.position = new Vector3(gridRect.center.x, gridRect.yMax + thicknessHalf);
            leftWall.position = new Vector3(gridRect.xMin - thicknessHalf, gridRect.center.y);
            rightWall.position = new Vector3(gridRect.xMax + thicknessHalf, gridRect.center.y);

            var wallLayer = LayerMask.NameToLayer("Wall");
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            topWall.gameObject.layer = bubbleLayer;
            leftWall.gameObject.layer = wallLayer;
            rightWall.gameObject.layer = wallLayer;

            var wallsContainer = new GameObject("Walls").transform;
            topWall.SetParent(wallsContainer);
            leftWall.SetParent(wallsContainer);
            rightWall.SetParent(wallsContainer);
        }

        private static Transform CreateWall(float width, float height)
        {
            var wall = new GameObject("Wall");
            var boxCollider = wall.AddComponent<BoxCollider2D>();

            boxCollider.size = new Vector2(width, height);
            return wall.transform;
        }
    }
}
