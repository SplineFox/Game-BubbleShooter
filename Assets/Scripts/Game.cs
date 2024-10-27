using DG.Tweening;
using UnityEngine;
using BubbleShooter.HexGrids;
using System.Collections.Generic;

namespace BubbleShooter
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private int _rowsCount;
        [SerializeField] private int _columnsCount;

        [SerializeField] private Transform _launchPosition;
        [SerializeField] private HexGridLayout _hexGridLayout;
        [SerializeField] private BubbleSpawner _bubbleSpawner;
        [SerializeField] private BubblePhysics _bubblePhysics;

        [SerializeField] private InputArea _inputArea;
        [SerializeField] private GameSetuper _gameSetuper;

        private HexGrid _hexGrid;
        private Sequence _sequence;

        private BubbleTrajectory _bubbleTrajectory;
        private BubbleSequenceDetector _bubbleSequenceDetector;

        private void Start()
        {
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            _gameSetuper.Setup(new Vector2Int(_columnsCount, _rowsCount), _hexGridLayout.CellSize, 0.4f);

            _hexGrid = new HexGrid(_rowsCount, _columnsCount);
            SetupGrid();

            _bubblePhysics.Setup(0.4f, bubbleLayer);
            _bubbleSequenceDetector = new BubbleSequenceDetector(_hexGrid);
        }

        private void OnEnable()
        {
            _inputArea.Clicked += OnAreaClicked;
        }

        private void OnDisable()
        {
            _inputArea.Clicked -= OnAreaClicked;
        }

        private void SetupGrid()
        {
            for (int row = 0; row < _rowsCount / 2; row++)
            {
                for (int column = 0; column < _columnsCount; column++)
                {
                    var offsetPoint = new OffsetPoint(column, row);
                    var worldPoint = _hexGridLayout.OffsetToWorld(offsetPoint);

                    var bubble = _bubbleSpawner.GetItem();
                    bubble.transform.position = worldPoint;

                    _hexGrid[row, column].Bubble = bubble;
                }
            }
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

            var hexPoint = _hexGridLayout.WorldToHex(trajectory.LastPoint.Position);
            var worldPoint = _hexGridLayout.HexToWorld(hexPoint);

            _sequence.Append(bubble.transform.DOMove(worldPoint, 0.2f));
            _sequence.OnComplete(delegate
            {
                _hexGrid[hexPoint].Bubble = bubble;
                if (_bubbleSequenceDetector.TryGetSequence(hexPoint, out var bubbleSequence))
                    PopBubbles(bubbleSequence);

                _inputArea.enabled = true; 
            });
        }

        private void PopBubbles(IEnumerable<(HexPoint, Bubble)> bubbleSequence)
        {
            foreach (var element in bubbleSequence)
            {
                _hexGrid[element.Item1].Bubble = null;
                _bubbleSpawner.ReleaseItem(element.Item2);
            }
        }

        private void OnDrawGizmos()
        {
            if (_bubbleTrajectory == null)
                return;

            var previousPosition = _launchPosition.position;
            foreach (var point in _bubbleTrajectory)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(point.Position, 0.32f);
                Gizmos.DrawLine(previousPosition, point.Position);
                
                Gizmos.color = Color.blue;
                GizmosUtils.DrawArrow(point.Position, point.Position + point.Direction);
                previousPosition = point.Position;
            }
        }
    }
}