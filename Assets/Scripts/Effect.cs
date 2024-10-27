using UnityEngine;

namespace BubbleShooter
{
    public class Effect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private EffectSpawner _effectSpawner;

        public void Setup(EffectSpawner effectSpawner, Color color)
        {
            _effectSpawner = effectSpawner;
            
            var settinigs = _particleSystem.main;
            settinigs.startColor = color;
        }

        public void Play()
        {
            _particleSystem.Play();
        }

        /// <summary>
        /// Called by ParticleSystem when
        /// StopAction.Callback is used.
        /// </summary>
        private void OnParticleSystemStopped()
        {
            _effectSpawner.Despawn(this);
        }

    }
}