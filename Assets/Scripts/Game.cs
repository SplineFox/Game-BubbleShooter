using DG.Tweening;
using UnityEngine;
using BubbleShooter.HexGrids;

namespace BubbleShooter
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private int _rowsCount;
        [SerializeField] private int _columnsCount;

        [SerializeField] private Bubble _bubblePrefab;
        [SerializeField] private Transform _launchPosition;
        [SerializeField] private HexGridLayout _hexGridLayout;

        private HexGrid _hexGrid;
        private BubblePhysics _bubblePhysics;
        private Bubble _bubble;
        private BubbleTrajectory _bubbleTrajectory;
        private Sequence _sequence;

        private void Start()
        {
            var bubbleLayer = LayerMask.NameToLayer("Bubble");

            _hexGrid = new HexGrid(_rowsCount, _columnsCount);
            _bubblePhysics = new BubblePhysics(0.4f, bubbleLayer);

            for (int row = 0; row < _rowsCount; row++)
                for (int column = 0; column < _columnsCount; column++)
                {
                    var offsetPosition = new Vector3Int(column, row);
                    var worldPosition = _hexGridLayout.OffsetToWorld(offsetPosition);

                    var bubble = Instantiate(_bubblePrefab, worldPosition, Quaternion.identity);
                    _hexGrid[row, column].Bubble = bubble;

                }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_bubble != null)
                    return;

                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;

                var direction = (mouseWorldPosition - _launchPosition.position).normalized;

                _bubbleTrajectory = _bubblePhysics.FindTrajectory(_launchPosition.position, direction);
                _bubble = Instantiate(_bubblePrefab, _bubbleTrajectory.FirstPoint.Position, Quaternion.identity);

                AnimateBubble(_bubble, _bubbleTrajectory);
                _bubble = null;
            }
        }

        private void AnimateBubble(Bubble bubble, BubbleTrajectory trajectory)
        {
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

                _sequence.Append(_bubble.transform.DOMove(point.Position, 0.2f));
            }

            var offsetPosition = _hexGridLayout.WorldToOffset(trajectory.LastPoint.Position);
            var worldPosition = _hexGridLayout.OffsetToWorld(offsetPosition);
            _sequence.Append(_bubble.transform.DOMove(worldPosition, 0.2f));
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