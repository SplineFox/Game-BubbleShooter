using BubbleShooter.HexGrids;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public class BubbleSpawner : BubblePool
    {
        [SerializeField] private List<Sprite> _sprites;

        private int _previousTypeId;
        private int _nextTypeId;

        public int NextTypeId => _nextTypeId;
        public Sprite NextSprite => _sprites[NextTypeId];


        protected override void Awake()
        {
            base.Awake();
            GenerateNextTypeId();
        }

        protected override void SetupItem(Bubble bubble)
        {
            bubble.Setup(_nextTypeId, _sprites[_nextTypeId]);
            GenerateNextTypeId();
        }

        private void GenerateNextTypeId()
        {
            _previousTypeId = _nextTypeId;
            _nextTypeId = GenerateRandomTypeId();

            while (_previousTypeId == _nextTypeId && _sprites.Count > 1)
                _nextTypeId = GenerateRandomTypeId();
        }

        private int GenerateRandomTypeId()
        {
            return Random.Range(0, _sprites.Count);
        }
    }
}