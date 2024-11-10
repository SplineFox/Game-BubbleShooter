using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooter
{
    public class GameSetuper : MonoBehaviour
    {
        [SerializeField] private float _wallThickness = 1f;
        [SerializeField] private float _pixelsPerUnit = 256f;
        [SerializeField] private float _offset = 0.2f;

        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _circleDown;
        [SerializeField] private RectTransform _circle1;
        [SerializeField] private RectTransform _circle2;
        [SerializeField] private Transform _launchPosition;

        public void Setup(Vector2Int gridSize, Vector2 gridCellSize, float bubbleCastRadius)
        {
            var gridWidth = gridSize.x * gridCellSize.x + gridCellSize.x / 2f;
            var gridHeight = (gridSize.y - 1) * (gridCellSize.y * 0.75f) + gridCellSize.y;

            var gridMinX = -gridCellSize.x / 2f;
            var gridMinY = gridCellSize.y / 2f - gridHeight;

            var gridRect = new Rect(gridMinX, gridMinY, gridWidth, gridHeight);

            SetupLaunchPosition(gridRect, _launchPosition);
            SetupCamera(gridRect, _camera);
            SetupCanvas(gridRect, _canvas, _circleDown, _circle1, _circle2);
            SetupWalls(gridRect, bubbleCastRadius);
        }

        private void SetupCamera(Rect gridRect, Camera camera)
        {
            var screenRatio = (float)Screen.height / Screen.width;

            camera.orthographic = true;
            camera.orthographicSize = (gridRect.width + _offset * 2f) * screenRatio * 0.5f;
            camera.transform.position = new Vector3(gridRect.center.x, gridRect.center.y, -100);
        }

        public void SetupCanvas(Rect gridRect, Canvas canvas, RectTransform circleDown, RectTransform circle1, RectTransform circle2)
        {
            var heightToWidthRation = gridRect.height / gridRect.width;
            var canvasTransform = canvas.transform as RectTransform;
            var canvasScaler = canvas.GetComponent<CanvasScaler>();

            var canvasPPU = 256f;
            var canvasWidth = gridRect.width * canvasPPU;
            var canvasHeight = canvasWidth * heightToWidthRation;
            var canvasScale = gridRect.width / canvasWidth;

            canvasScaler.referencePixelsPerUnit = canvasPPU;
            canvasTransform.sizeDelta = new Vector2(canvasWidth + _offset * 2, canvasHeight + _offset * 2);
            canvasTransform.localScale = Vector3.one * canvasScale;
            canvasTransform.position = new Vector3(gridRect.center.x, gridRect.center.y, 1f);

            circleDown.sizeDelta = new Vector2(512f, 512f);
            circleDown.position = new Vector3(gridRect.center.x, gridRect.yMin, 1f);
            
            circle1.sizeDelta = new Vector2(256f, 256f);
            circle1.position = new Vector3(gridRect.center.x - 2f, gridRect.yMin - 1f, 1f);

            circle2.sizeDelta = new Vector2(256f, 256f);
            circle2.position = new Vector3(gridRect.center.x + 2f, gridRect.yMin - 1f, 1f);
        }

        private void SetupLaunchPosition(Rect gridSize, Transform launchPosition)
        {
            launchPosition.localPosition = new Vector3(gridSize.center.x, gridSize.yMin - 0.5f);
        }

        private void SetupWalls(Rect gridRect, float bubblePhysicalRadius)
        {
            var thiknessAdditional = bubblePhysicalRadius * 2f;
            var thicknessDoubled = _wallThickness * 2f;
            var thicknessHalf = _wallThickness / 2f;

            var topWall = CreateWall(gridRect.width + thicknessDoubled, _wallThickness + thiknessAdditional);
            var leftWall = CreateWall(_wallThickness + thiknessAdditional, gridRect.height + thicknessDoubled);
            var rightWall = CreateWall(_wallThickness + thiknessAdditional, gridRect.height + thicknessDoubled);

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
