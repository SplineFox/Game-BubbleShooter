using UnityEngine;

namespace BubbleShooter
{
    [CreateAssetMenu(fileName = "BubbleSetting", menuName = "BubbleShooter/BubbleSetting")]
    public class BubbleSetting : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Color _color;

        public Sprite Sprite => _sprite;
        public Color Color => _color;
    }
}