using DG.Tweening;
using UnityEngine;
using BubbleShooter.HexGrids;

namespace BubbleShooter
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private int _rowsCount;
        [SerializeField] private int _columnsCount;

        [SerializeField] private Transform _launchPosition;
        [SerializeField] private HexGridLayout _hexGridLayout;
        [SerializeField] private BubbleSpawner _bubbleSpawner;

        [SerializeField] private InputArea _inputArea;
        [SerializeField] private GameSetuper _gameSetuper;

        private HexGrid _hexGrid;
        private Sequence _sequence;

        private BubblePhysics _bubblePhysics;
        private BubbleTrajectory _bubbleTrajectory;

        private void Start()
        {
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            _gameSetuper.Setup(new Vector2Int(_columnsCount, _rowsCount), _hexGridLayout.CellSize);

            _hexGrid = new HexGrid(_rowsCount, _columnsCount);
            _bubblePhysics = new BubblePhysics(0.4f, bubbleLayer);

            for (int row = 0; row < _rowsCount/2; row++)
                for (int column = 0; column < _columnsCount; column++)
                {
                    var offsetPosition = new Vector3Int(column, row);
                    var worldPosition = _hexGridLayout.OffsetToWorld(offsetPosition);

                    var bubble = _bubbleSpawner.GetItem();
                    bubble.transform.position = worldPosition;

                    _hexGrid[row, column].Bubble = bubble;

                }
        }

        private void OnEnable()
        {
            _inputArea.Clicked += OnAreaClicked;
        }

        private void OnDisable()
        {
            _inputArea.Clicked -= OnAreaClicked;
        }

        private void OnAreaClicked(Vector3 clickPosition)
        {
            _inputArea.enabled = false;
            var direction = (clickPosition - _launchPosition.position).normalized;

            _bubbleTrajectory = _bubblePhysics.FindTrajectory(_launchPosition.position, direction);
            var bubble = _bubbleSpawner.GetItem();

            AnimateBubble(bubble, _bubbleTrajectory);
        }

        private void AnimateBubble(Bubble bubble, BubbleTrajectory trajectory)
        {
            bubble.transform.position = trajectory.FirstPoint.Position;

            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            var isFirst = true;
            foreach (var point in trajectory)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                _sequence.Append(bubble.transform.DOMove(point.Position, 0.2f));
            }

            var offsetPosition = _hexGridLayout.WorldToOffset(trajectory.LastPoint.Position);
            var worldPosition = _hexGridLayout.OffsetToWorld(offsetPosition);
            _sequence.Append(bubble.transform.DOMove(worldPosition, 0.2f));
            _sequence.OnComplete(delegate { _inputArea.enabled = true; });
        }

        private void OnDrawGizmos()
        {
            if (_bubbleTrajectory == null)
                return;

            var previousPosition = _launchPosition.position;
            foreach (var point in _bubbleTrajectory)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(point.Position, 0.4f);
                Gizmos.DrawLine(previousPosition, point.Position);
                
                Gizmos.color = Color.blue;
                GizmosUtils.DrawArrow(point.Position, point.Position + point.Direction);
                previousPosition = point.Position;
            }
        }
    }
}