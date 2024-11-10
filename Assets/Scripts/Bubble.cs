using UnityEngine;

namespace BubbleShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private CircleCollider2D _collider;
        
        private int _typeId;
        private BubbleSetting _setting;

        public int TypeId => _typeId;
        public BubbleSetting Setting => _setting;

        public void Setup(int typeId, BubbleSetting setting)
        {
            _typeId = typeId;
            _setting = setting;

            var startColor = setting.Color;
            var endColor = setting.Color;

            startColor.a = 1f;
            endColor.a = 0.2f;

            _renderer.sprite = setting.Sprite;
            _trailRenderer.startColor = startColor;
            _trailRenderer.endColor = endColor;

            SetColliderEnable(true);
            SetTrailEnable(true);
        }

        public void SetColliderEnable(bool isEnabled)
        {
            _collider.enabled = isEnabled;
        }

        public void SetTrailEnable(bool isEnabled)
        {
            _trailRenderer.enabled = isEnabled;
        }
    }
}