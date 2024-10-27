using UnityEngine;

namespace BubbleShooter.HexGrids
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private CircleCollider2D _collider;
        
        private int _typeId;

        public int TypeId => _typeId;

        public void Setup(int typeId, Sprite sprite)
        {
            _typeId = typeId;
            _renderer.sprite = sprite;
            SetColliderEnable(true);
        }

        public void SetColliderEnable(bool isEnabled)
        {
            _collider.enabled = isEnabled;
        }
    }
}