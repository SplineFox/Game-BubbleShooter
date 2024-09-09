using BubbleShooter.HexGrids;
using DG.Tweening;
using UnityEngine;

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
        private Bubble _bubble;

        private void Start()
        {
            _hexGrid = new HexGrid(_rowsCount, _columnsCount);

            for (int row = 0; row < _rowsCount; row++)
                for (int column = 0; column < _columnsCount; column++)
                {
                    var offsetPosition = new Vector3Int(column, row);
                    var worldPosition = _hexGridLayout.OffsetToWorld(offsetPosition);

                    var bubble = Instantiate(_bubblePrefab, worldPosition, Quaternion.identity);
                    bubble.ResetVelocity();
                    _hexGrid[row, column].Bubble = bubble;

                }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (_bubble != null)
                    return;

                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;

                var direction = (mouseWorldPosition - _launchPosition.position).normalized;
                
                _bubble = Instantiate(_bubblePrefab, _launchPosition.position, Quaternion.identity);
                _bubble.Collided += OnBubbleCollided;
                _bubble.SetVelocity(direction * 10f);
            }
        }

        private void OnBubbleCollided()
        {
            var offsetPosition = _hexGridLayout.WorldToOffset(_bubble.transform.position);
            var worldPosition = _hexGridLayout.OffsetToWorld(offsetPosition);

            _bubble.ResetVelocity();
            _bubble.Collided -= OnBubbleCollided;
            _bubble.transform.DOMove(worldPosition, 0.2f).SetEase(Ease.OutBack);
            _bubble = null;
        }
    }
}