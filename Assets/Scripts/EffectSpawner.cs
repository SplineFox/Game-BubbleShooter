using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public class EffectSpawner : EffectPool
    {
        protected override void SetupItem(Effect effect, Color color)
        {
            effect.Setup(this, color);
        }
    }
}