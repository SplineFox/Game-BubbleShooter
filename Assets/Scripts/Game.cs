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
        [SerializeField] private EffectSpawner _effectSpawner;
        [SerializeField] private BubbleSpawner _bubbleSpawner;
        [SerializeField] private BubbleAnimator _bubbleAnimator;
        [SerializeField] private BubblePhysics _bubblePhysics;

        [SerializeField] private InputArea _inputArea;
        [SerializeField] private GameSetuper _gameSetuper;

        private HexGrid _hexGrid;

        private BubbleTrajectory _bubbleTrajectory;
        private BubbleSequenceDetector _bubbleSequenceDetector;
        private BubbleFloatersDetector _bubbleFloatersDetector;

        private void Start()
        {
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            _gameSetuper.Setup(new Vector2Int(_columnsCount, _rowsCount), _hexGridLayout.CellSize, 0.4f);

            _hexGrid = new HexGrid(_rowsCount, _columnsCount);
            SetupGrid();

            _bubblePhysics.Setup(0.4f, bubbleLayer);
            _bubbleSequenceDetector = new BubbleSequenceDetector(_hexGrid);
            _bubbleFloatersDetector = new BubbleFloatersDetector(_hexGrid);
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
                SpawnBubblesLine(row);
            }
        }

        private void OnAreaClicked(Vector3 clickPosition)
        {
            var direction = (clickPosition - _launchPosition.position).normalized;

            _bubbleTrajectory = _bubblePhysics.FindTrajectory(_launchPosition.position, direction);
            var bubble = _bubbleSpawner.GetItem();

            LaunchBubble(bubble, _bubbleTrajectory);
        }

        private void LaunchBubble(Bubble bubble, BubbleTrajectory trajectory)
        {
            _inputArea.enabled = false;
            var hexPoint = _hexGridLayout.WorldToHex(trajectory.LastPoint.Position);

            _bubbleAnimator.AnimateBubbleMove(bubble, trajectory, delegate
            {
                _hexGrid[hexPoint].Bubble = bubble;
                ProcessBubble(hexPoint);
                _inputArea.enabled = true;
            });
        }

        private void ProcessBubble(HexPoint hexPoint)
        {
            if (_bubbleSequenceDetector.TryGetSequence(hexPoint, out var sequence))
            {
                PopBubbles(sequence);

                if (_bubbleFloatersDetector.TryGetFloaters(out var floaters))
                    DropBubbles(floaters);
            }

            return;
            if (IsMovePossible())
            {
                MoveBubblesDown();
            }
            else
            {
                Debug.Log("Move not possible");
            }
        }

        private void PopBubbles(IEnumerable<(HexPoint, Bubble)> sequence)
        {
            foreach (var element in sequence)
            {
                var bubble = element.Item2;
                bubble.SetColliderEnable(false);

                _effectSpawner.Spawn(bubble.transform.position, bubble.TypeId).Play();
                _hexGrid[element.Item1].Bubble = null;
                _bubbleSpawner.ReleaseItem(bubble);
            }
        }

        private void DropBubbles(IEnumerable<(HexPoint, Bubble)> floaters)
        {
            foreach (var element in floaters)
            {
                var bubble = element.Item2;
                bubble.SetColliderEnable(false);

                var offsetPoint = HexPoints.HexToOffset(element.Item1, HexCellLayoutOffset.OddRows);
                var offsetDropPoint = new OffsetPoint(offsetPoint.column, _hexGrid.RowCount);
                var worldPoint = _hexGridLayout.OffsetToWorld(offsetDropPoint);

                _hexGrid[element.Item1].Bubble = null;
                _bubbleAnimator.AnimaterBubbleDrop(bubble, worldPoint, () =>
                {
                    _effectSpawner.Spawn(bubble.transform.position, bubble.TypeId).Play();
                    _bubbleSpawner.ReleaseItem(bubble);
                });
            }
        }

        private bool IsMovePossible()
        {
            var lastRowIndex = _hexGrid.RowCount - 1;
            for (int column = 0; column < _hexGrid.ColumnCount; column++)
            {
                if (_hexGrid[lastRowIndex, column].Bubble != null)
                    return false;
            }

            return true;
        }

        private void MoveBubblesDown()
        {
            for (int row = _rowsCount - 2; row >= 0; row--)
            {
                for (int column = 0; column < _columnsCount; column++)
                {
                    var bubble = _hexGrid[row, column].Bubble;
                    if (bubble != null)
                    {
                        var nextRow = row + 1;
                        var nextOffsetPoint = new OffsetPoint(column, nextRow);
                        var worldPoint = _hexGridLayout.OffsetToWorld(nextOffsetPoint);

                        _hexGrid[nextRow, column].Bubble = bubble;
                        _hexGrid[row, column].Bubble = null;
                        _bubbleAnimator.AnimaterBubbleMove(bubble, worldPoint);
                    }
                }
            }
            SpawnBubblesLine(0);
        }

        private void SpawnBubblesLine(int row)
        {
            for (int column = 0; column < _columnsCount; column++)
            {
                var offsetPoint = new OffsetPoint(column, row);
                var worldPoint = _hexGridLayout.OffsetToWorld(offsetPoint);

                var bubble = _bubbleSpawner.GetItem();
                bubble.transform.position = worldPoint;

                _hexGrid[row, column].Bubble = bubble;
                _bubbleAnimator.AnimateBubbleSpawn(bubble);
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