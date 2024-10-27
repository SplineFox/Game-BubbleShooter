using UnityEngine;
using BubbleShooter.HexGrids;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BubbleShooter
{
    public class Game : MonoBehaviour
    {
        private const int _maxFoulsAllowed = 5;
        private const int _scorePerBubble = 10;
        private const int _scorePerFloatingBubble = 100;
        private const int _scoreGroupSize = 3;

        [Header("Grid")]
        [SerializeField] private int _rowsCount;
        [SerializeField] private int _columnsCount;
        [SerializeField] private HexGridLayout _hexGridLayout;

        [Header("UI")]
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private FoulsView _foulsView;

        [Header("Dependecies")]
        [SerializeField] private Transform _launchPosition;
        [SerializeField] private Image _nextBubbleImage;

        [SerializeField] private EffectSpawner _effectSpawner;
        [SerializeField] private BubbleSpawner _bubbleSpawner;
        [SerializeField] private BubbleAnimator _bubbleAnimator;
        [SerializeField] private BubblePhysics _bubblePhysics;

        [SerializeField] private InputArea _inputArea;
        [SerializeField] private GameSetuper _gameSetuper;

        private HexGrid _hexGrid;
        private int _foulsAllowedCount;
        private int _foulsCount;
        private int _score;

        private Bubble _currentBubble;
        private BubbleTrajectory _bubbleTrajectory;
        private BubbleSequenceDetector _bubbleSequenceDetector;
        private BubbleFloatersDetector _bubbleFloatersDetector;

        private void Start()
        {
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            _gameSetuper.Setup(new Vector2Int(_columnsCount, _rowsCount), _hexGridLayout.CellSize, 0.4f);
            _hexGrid = new HexGrid(_rowsCount, _columnsCount);

            _bubblePhysics.Setup(0.4f, bubbleLayer);
            _bubbleSequenceDetector = new BubbleSequenceDetector(_hexGrid);
            _bubbleFloatersDetector = new BubbleFloatersDetector(_hexGrid);

            Restart();
        }

        private void OnEnable()
        {
            _inputArea.Clicked += OnAreaClicked;
        }

        private void OnDisable()
        {
            _inputArea.Clicked -= OnAreaClicked;
        }

        public void Restart()
        {
            _score = 0;
            _scoreView.SetValue(_score);

            _foulsAllowedCount = _maxFoulsAllowed;
            _foulsCount = _maxFoulsAllowed;
            _foulsView.SetValue(_foulsCount);

            ClearGrid();
            SetupGrid();
            EnableInput();
        }

        private void ClearGrid()
        {
            for (int row = 0; row < _rowsCount; row++)
            {
                for (int column = 0; column < _columnsCount; column++)
                {
                    var cell = _hexGrid[row, column];
                    if (cell.Bubble != null)
                    {
                        _bubbleSpawner.ReleaseItem(cell.Bubble);
                        cell.Bubble = null;
                    }
                }
            }
        }

        private void SetupGrid()
        {
            for (int row = 0; row < _rowsCount / 2; row++)
            {
                SpawnBubblesLine(row);
            }
        }

        private void SetupBubble()
        {
            if (_currentBubble != null)
                _bubbleSpawner.ReleaseItem(_currentBubble);

            _currentBubble = _bubbleSpawner.GetItem();
            _currentBubble.transform.position = _launchPosition.position;
            _currentBubble.SetColliderEnable(false);

            _nextBubbleImage.sprite = _bubbleSpawner.NextSprite;
        }

        private void OnAreaClicked(Vector3 clickPosition)
        {
            var direction = (clickPosition - _launchPosition.position).normalized;
            _bubbleTrajectory = _bubblePhysics.FindTrajectory(_launchPosition.position, direction);

            DisableInput();
            LaunchBubble(_currentBubble, _bubbleTrajectory);
            _currentBubble.SetColliderEnable(true);
            _currentBubble = null;
        }

        private void EnableInput()
        {
            _inputArea.enabled = true;
            SetupBubble();
        }

        private void DisableInput()
        {
            _inputArea.enabled = false;
        }

        private void LaunchBubble(Bubble bubble, BubbleTrajectory trajectory)
        {
            var hexPoint = _hexGridLayout.WorldToHex(trajectory.LastPoint.Position);
            if (!_hexGrid.IsPointInBounds(hexPoint))
            {
                _bubbleSpawner.ReleaseItem(bubble);
                return;
            }

            _bubbleAnimator.AnimateBubbleMove(bubble, trajectory, () =>
            {
                _hexGrid[hexPoint].Bubble = bubble;
                ProcessBubble(hexPoint);
            });
        }

        private void ProcessBubble(HexPoint hexPoint)
        {
            if (_bubbleSequenceDetector.TryGetSequence(hexPoint, out var sequence))
            {
                PopBubbles(sequence);
                _score += CalculateScorePoints(sequence.Count(), false);

                if (_bubbleFloatersDetector.TryGetFloaters(out var floaters))
                {
                    DropBubbles(floaters);
                    _score += CalculateScorePoints(sequence.Count(), true);
                }

                _scoreView.SetValue(_score);
                EnableInput();
                return;
            }

            if (!TryAddFoul())
            {
                if (IsMovePossible())
                {
                    StartCoroutine(MoveBubblesDown(() => EnableInput()));

                    if (_bubbleFloatersDetector.TryGetFloaters(out var floaters))
                        DropBubbles(floaters);

                    return;
                }
                else
                {
                    Debug.Log("Move not possible");
                    return;
                }
            }
            EnableInput();
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

        private IEnumerator MoveBubblesDown(Action onComplete = null)
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
            yield return new WaitForSeconds(_bubbleAnimator.MoveDuration);
            onComplete?.Invoke();
        }

        private void SpawnBubblesLine(int row)
        {
            for (int column = 0; column < _columnsCount; column++)
            {
                if (_hexGrid[row, column].Bubble != null)
                    continue;

                var offsetPoint = new OffsetPoint(column, row);
                var worldPoint = _hexGridLayout.OffsetToWorld(offsetPoint);

                var bubble = _bubbleSpawner.GetItem();
                bubble.transform.position = worldPoint;

                _hexGrid[row, column].Bubble = bubble;
                _bubbleAnimator.AnimateBubbleSpawn(bubble);
            }
        }

        private int CalculateScorePoints(int count, bool isFloatingBubble)
        {
            var score = 0;
            var groupSize = _scoreGroupSize;
            var groupScore = isFloatingBubble
                ? _scorePerFloatingBubble
                : _scorePerBubble;

            for (int index = 0; index < count; index++)
            {
                score += groupScore;

                if ((index + 1) % groupSize == 0)
                {
                    groupScore += _scorePerBubble;
                }
            }

            return score;
        }

        private bool TryAddFoul()
        {
            _foulsCount--;
            if (_foulsCount > 0)
            {
                _foulsView.SetValue(_foulsCount);
                return true;
            }

            _foulsAllowedCount--;
            if (_foulsAllowedCount > 0)
            {
                _foulsCount = _foulsAllowedCount;
                _foulsView.SetValue(_foulsCount);
                return false;
            }

            _foulsAllowedCount = _maxFoulsAllowed;
            _foulsCount = _maxFoulsAllowed;
            _foulsView.SetValue(_foulsCount);
            return false;
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