using UnityEngine;
using UnityEngine.Pool;

namespace BubbleShooter
{
    public abstract class EffectPool : MonoBehaviour
    {
        [SerializeField] private int _defaultCapacity;
        [SerializeField] private int _maxCapacity;
        [SerializeField] private Effect _prefab;

        private ObjectPool<Effect> _pool;

        protected virtual void Awake()
        {
            _pool = new ObjectPool<Effect>(OnCreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect, true, _defaultCapacity, _maxCapacity);
        }

        protected abstract void SetupItem(Effect bubble, Color color);

        public Effect Spawn(Vector3 worldPoint, Color color)
        {
            var item = _pool.Get();
            item.transform.position = worldPoint;
            
            SetupItem(item, color);
            return item;
        }

        public void Despawn(Effect effect)
        {
            _pool.Release(effect);
        }

        private Effect OnCreateEffect()
        {
            return Instantiate(_prefab, transform);
        }

        private void OnGetEffect(Effect effect)
        {
            effect.gameObject.SetActive(true);
        }

        private void OnReleaseEffect(Effect effect)
        {
            effect.gameObject.SetActive(false);
        }

        private void OnDestroyEffect(Effect effect)
        {
            Destroy(effect.gameObject);
        }
    }
}