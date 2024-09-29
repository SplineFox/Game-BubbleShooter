using UnityEngine;
using UnityEngine.Pool;
using BubbleShooter.HexGrids;

namespace BubbleShooter
{
    public abstract class BubblePool : MonoBehaviour
    {
        [SerializeField] private int _defaultCapacity;
        [SerializeField] private int _maxCapacity;
        [SerializeField] private Bubble _prefab;

        private ObjectPool<Bubble> _pool;

        protected virtual void Awake()
        {
            _pool = new ObjectPool<Bubble>(OnCreateBubble, OnGetBubble, OnReleaseBubble, OnDestroyBubble, true, _defaultCapacity, _maxCapacity);
        }
        protected abstract void SetupItem(Bubble bubble);

        public Bubble GetItem()
        {
            var item = _pool.Get();
            SetupItem(item);

            return item;
        }

        public void ReleaseItem(Bubble bubble)
        {
            _pool.Release(bubble);
        }

        private Bubble OnCreateBubble()
        {
            return Instantiate(_prefab, transform);
        }

        private void OnGetBubble(Bubble bubble)
        {
            bubble.gameObject.SetActive(true);
        }

        private void OnReleaseBubble(Bubble bubble)
        {
            bubble.gameObject.SetActive(false);
        }

        private void OnDestroyBubble(Bubble bubble)
        {
            Destroy(bubble.gameObject);
        }
    }
}