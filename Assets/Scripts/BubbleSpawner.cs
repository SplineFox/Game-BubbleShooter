using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BubbleShooter
{
    public class BubbleSpawner : BubblePool
    {
        [SerializeField] private List<BubbleSetting> _bubbleSettings;

        private int _previousTypeId;
        private int _nextTypeId;

        public int NextTypeId => _nextTypeId;
        public BubbleSetting NextBubble => _bubbleSettings[NextTypeId];


        protected override void Awake()
        {
            base.Awake();
            GenerateNextTypeId();
        }

        protected override void SetupItem(Bubble bubble)
        {
            bubble.Setup(_nextTypeId, _bubbleSettings[_nextTypeId]);
        }

        public Bubble GetRandomItem()
        {
            var bubble = GetItem();
            GenerateNextTypeId();

            return bubble;
        }

        public Bubble GetItemForExisting(List<Bubble> existingBubbles)
        {
            if (existingBubbles.Count == 0)
                throw new ArgumentException();

            var bubble = GetItem();
            GenerateNextTypeIdForExisting(existingBubbles);

            return bubble;
        }

        private void GenerateNextTypeId()
        {
            _previousTypeId = _nextTypeId;
            _nextTypeId = GenerateRandomTypeId();

            while (_previousTypeId == _nextTypeId && _bubbleSettings.Count > 1)
                _nextTypeId = GenerateRandomTypeId();
        }

        private void GenerateNextTypeIdForExisting(List<Bubble> existingBubbles)
        {
            _previousTypeId = _nextTypeId;
            _nextTypeId = GenerateRandomTypeId();

            while (
                !existingBubbles.Any(x => x.TypeId == _nextTypeId)
                || (_previousTypeId == _nextTypeId && existingBubbles.Any(x => x.TypeId != _nextTypeId)))
            {
                _nextTypeId = GenerateRandomTypeId();
            }
        }

        private int GenerateRandomTypeId()
        {
            return Random.Range(0, _bubbleSettings.Count);
        }
    }
}