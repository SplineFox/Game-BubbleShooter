using BubbleShooter.HexGrids;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public class BubbleSpawner : BubblePool
    {
        [SerializeField] private List<Sprite> _sprites;

        private int _nextTypeId;

        public int NextTypeId => _nextTypeId;

        protected override void Awake()
        {
            base.Awake();
            _nextTypeId = GenerateNextTypeId();
        }

        protected override void SetupItem(Bubble bubble)
        {
            bubble.Setup(_nextTypeId, _sprites[_nextTypeId]);
            _nextTypeId = GenerateNextTypeId();
        }

        private int GenerateNextTypeId()
        {
            return Random.Range(0, _sprites.Count);
        }
    }
}