using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using BubbleShooter.HexGrids;
using System;

namespace BubbleShooter
{
    public class Game : MonoBehaviour
    {
        public event Action Finished;

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
        [SerializeField] private SoundManager _soundManager;

        [SerializeField] private InputArea _inputArea;
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private GameSetuper _gameSetuper;

        private HexGrid _hexGrid;
        private int _foulsAllowedCount;
        private int _foulsCount;
        private int _score;
        private List<Bubble> _existingBubbles;

        private Bubble _bubbleToLaunch;
        private BubbleTrajectory _bubbleTrajectory;
        private BubbleSequenceDetector _bubbleSequenceDetector;
        private BubbleFloatersDetector _bubbleFloatersDetector;

        public int Score => _score;

        private void Awake()
        {
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            _gameSetuper.Setup(new Vector2Int(_columnsCount, _rowsCount), _hexGridLayout.CellSize, 0.4f);
            _hexGrid = new HexGrid(_rowsCount, _columnsCount);

            _bubblePhysics.Setup(0.4f, bubbleLayer);
            _bubbleSequenceDetector = new BubbleSequenceDetector(_hexGrid);
            _bubbleFloatersDetector = new BubbleFloatersDetector(_hexGrid);

            _existingBubbles = new List<Bubble>();
        }

        private void OnEnable()
        {
            _inputArea.Clicked += OnAreaClicked;
        }

        private void OnDisable()
        {
            _inputArea.Clicked -= OnAreaClicked;
        }

        private void EnableInput()
        {
            _inputArea.enabled = true;
        }

        private void DisableInput()
        {
            _inputArea.enabled = false;
        }

        public void Restart()
        {
            _score = 0;
            _scoreView.SetValue(_score);

            _foulsAllowedCount = _maxFoulsAllowed;
            _foulsCount = _maxFoulsAllowed;
            _foulsView.SetValue(_foulsCount);

            ClearGrid();
            ReloadGrid();
            ReloadBubble();
            EnableInput();
        }

        private void ReloadGrid()
        {
            for (int row = 0; row < _rowsCount / 2; row++)
            {
                SpawnBubblesLineAsync(row);
            }
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

            _existingBubbles.Clear();
        }

        private void OnAreaClicked(Vector3 clickPosition)
        {
            var direction = (clickPosition - _launchPosition.position).normalized;

            LaunchBubble(direction);
        }

        private void ReloadBubble()
        {
            if (_bubbleToLaunch != null)
                _bubbleSpawner.ReleaseItem(_bubbleToLaunch);

            _bubbleToLaunch = _bubbleSpawner.GetItemForExisting(_existingBubbles);
            _bubbleToLaunch.transform.position = _launchPosition.position;

            _nextBubbleImage.sprite = _bubbleSpawner.NextSprite;
        }

        private async void LaunchBubble(Vector3 direction)
        {
            DisableInput();
            
            _bubbleToLaunch.SetColliderEnable(false);

            var trajectory = _bubblePhysics.FindTrajectory(_launchPosition.position, direction);
            var hexPoint = _hexGridLayout.WorldToHex(trajectory.LastPoint.Position);
            
            if (!_hexGrid.IsPointInBounds(hexPoint))
            {
                Finished?.Invoke();
                return;
            }

            var worldPoint = _hexGridLayout.HexToWorld(hexPoint);
            var sequence = _bubbleAnimator.CreateBubbleFlightSequence(_bubbleToLaunch, trajectory, worldPoint);

            _soundManager.PlayBubbleLauch();
            await PlaySequenceAsync(sequence);

            _hexGrid[hexPoint].Bubble = _bubbleToLaunch;
            _existingBubbles.Add(_bubbleToLaunch);
            
            _bubbleToLaunch.SetColliderEnable(true);
            _bubbleToLaunch = null;

            var result = await ProcessBubbleAsync(hexPoint);

            switch (result)
            {
                case ProcessResult.Continue:
                    {
                        ReloadBubble();
                        EnableInput();
                    }
                    break;
                case ProcessResult.Lose:
                case ProcessResult.Win:
                    {
                        Finished?.Invoke();
                    }
                    break;
            }
        }

        private async Task<ProcessResult> ProcessBubbleAsync(HexPoint hexPoint)
        {
            if (_bubbleSequenceDetector.TryGetSequence(hexPoint, out var sequence))
            {
                await PopBubblesAsync(sequence);
                _score += CalculateScorePoints(sequence.Count(), false);
        
                if (_bubbleFloatersDetector.TryGetFloaters(out var floaters))
                {
                    DropBubbles(floaters);
                    _score += CalculateScorePoints(floaters.Count(), true);
                }
        
                _scoreView.SetValueWithAnimation(_score);

                if (_existingBubbles.Count == 0)
                    return ProcessResult.Win;

                return ProcessResult.Continue;
            }

            _soundManager.PlayBubbleStop();
            if (!TryAddFoul())
            {
                if (!IsMovePossible())
                    return ProcessResult.Lose;

                await MoveBubblesDownAsync();

                if (_bubbleFloatersDetector.TryGetFloaters(out var floaters))
                    DropBubbles(floaters);
            }

            return ProcessResult.Continue;
        }

        private async Task PopBubblesAsync(IEnumerable<(HexPoint, Bubble)> collection)
        {
            var sequence = DOTween.Sequence();

            foreach (var element in collection)
            {
                sequence.AppendInterval(0.025f);
                sequence.AppendCallback(() =>
                {
                    var bubble = element.Item2;
                    bubble.SetColliderEnable(false);

                    _soundManager.PlayBubblePop();
                    _effectSpawner.Spawn(bubble.transform.position, bubble.TypeId).Play();
                    
                    _bubbleSpawner.ReleaseItem(bubble);
                    _hexGrid[element.Item1].Bubble = null;
                    _existingBubbles.Remove(bubble);
                });
            }

            await PlaySequenceAsync(sequence);
        }

        private void DropBubbles(IEnumerable<(HexPoint, Bubble)> collection)
        {
            var sequence = DOTween.Sequence();

            var interval = 0f;
            foreach (var element in collection)
            {
                var bubble = element.Item2;
                bubble.SetColliderEnable(false);

                var offsetPoint = HexPoints.HexToOffset(element.Item1, HexCellLayoutOffset.OddRows);
                var offsetDropPoint = new OffsetPoint(offsetPoint.column, _hexGrid.RowCount);
                var worldPoint = _hexGridLayout.OffsetToWorld(offsetDropPoint);

                var tween = _bubbleAnimator.CreateBubbleDropTween(bubble, worldPoint).OnComplete(() =>
                {
                    _soundManager.PlayBubblePop();
                    _effectSpawner.Spawn(bubble.transform.position, bubble.TypeId).Play();
                    
                    _bubbleSpawner.ReleaseItem(bubble);
                    _hexGrid[element.Item1].Bubble = null;
                    _existingBubbles.Remove(bubble);
                });
                
                sequence.Insert(interval, tween);
                interval += 0.05f;
            }

            PlaySequenceAsync(sequence);
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

        private async Task MoveBubblesDownAsync()
        {
            var sequence = DOTween.Sequence();

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
                        var tween = _bubbleAnimator.CreateBubbleMoveTween(bubble, worldPoint);

                        sequence.Insert(0, tween);
                    }
                }
            }

            await PlaySequenceAsync(sequence);
            await SpawnBubblesLineAsync(0);
        }

        private async Task SpawnBubblesLineAsync(int row)
        {
            var sequence = DOTween.Sequence();

            for (int column = 0; column < _columnsCount; column++)
            {
                if (_hexGrid[row, column].Bubble != null)
                    continue;

                var offsetPoint = new OffsetPoint(column, row);
                var worldPoint = _hexGridLayout.OffsetToWorld(offsetPoint);

                var bubble = _bubbleSpawner.GetRandomItem();
                bubble.transform.position = worldPoint;
                _hexGrid[row, column].Bubble = bubble;
                _existingBubbles.Add(bubble);

                var tween = _bubbleAnimator.CreateBubbleSpawnTween(bubble);

                sequence.Insert(0f, tween);
            }

            await PlaySequenceAsync(sequence);
        }

        private async Task PlaySequenceAsync(Sequence sequence)
        {
            sequence.Play();

            while (sequence.IsActive() && sequence.IsPlaying())
                await Task.Yield();
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

    public enum ProcessResult
    {
        Continue,
        Lose,
        Win,
    }
}