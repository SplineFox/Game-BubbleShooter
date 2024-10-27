using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public class EffectSpawner : EffectPool
    {
        [SerializeField] private List<Color> _colors;

        protected override void SetupItem(Effect effect, int effectId)
        {
            var color = _colors[effectId];
            effect.Setup(this, color);
        }
    }
}