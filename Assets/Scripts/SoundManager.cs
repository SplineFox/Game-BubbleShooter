using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _uiAudioSource;
    [SerializeField] private AudioSource _fxAudioSource;
    [Space]
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private AudioClip _bubbleLaunchClip;
    [SerializeField] private AudioClip _bubbleStopClip;
    [SerializeField] private List<AudioClip> _bubblePopClips;

    private bool _soundsEnabled = true;
    private bool _musicEnabled;

    private float _musicStopTime;

    public bool SoundsEnabled
    {
        get => _soundsEnabled;
        set
        {
            if (value == _soundsEnabled)
                return;

            _soundsEnabled = value;
            if (!value)
                StopSounds();
        }
    }

    public bool MusicEnabled
    {
        get => _musicEnabled;
        set
        {
            if (value == _musicEnabled)
                return;

            _musicEnabled = value;
            if (value)
            {
                StartMusic();
            }
            else
            {
                StopMusic();
            }
        }
    }

    public void PlayBubbleLauch()
    {
        PlayOneShot(AudioChannel.FxSounds, _bubbleLaunchClip);
    }

    public void PlayBubbleStop()
    {
        PlayOneShot(AudioChannel.FxSounds, _bubbleStopClip);
    }

    public void PlayBubblePop()
    {
        var randomIndex = UnityEngine.Random.Range(0, _bubblePopClips.Count);
        var randomClip = _bubblePopClips[randomIndex];

        PlayOneShot(AudioChannel.FxSounds, randomClip);
    }

    private void Play(AudioChannel audioChannel, AudioClip audioClip, bool interrupt = false, bool loop = false)
    {
        if (!SoundsEnabled)
            return;

        var audioSource = GetAudioChannelSource(audioChannel);
        if (audioSource.isPlaying && !interrupt)
            return;

        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    private void PlayOneShot(AudioChannel audioChannel, AudioClip audioClip)
    {
        if (!SoundsEnabled)
            return;

        var audioSource = GetAudioChannelSource(audioChannel);
        audioSource.PlayOneShot(audioClip);
    }

    public void Stop(AudioChannel audioChannel)
    {
        var audioSource = GetAudioChannelSource(audioChannel);
        audioSource.Stop();
    }

    private void StartMusic()
    {
        _musicAudioSource.loop = true;
        _musicAudioSource.clip = _musicClip;
        _musicAudioSource.time = _musicStopTime;
        _musicAudioSource.Play();
    }

    private void StopMusic()
    {
        _musicStopTime = _musicAudioSource.time;
        _musicAudioSource.Stop();
    }

    private void StopSounds()
    {
        _uiAudioSource.Stop();
        _fxAudioSource.Stop();
    }

    private AudioSource GetAudioChannelSource(AudioChannel audioChannel)
    {
        switch (audioChannel)
        {
            case AudioChannel.Music:
                return _musicAudioSource;
            case AudioChannel.UISounds:
                return _uiAudioSource;
            case AudioChannel.FxSounds:
                return _fxAudioSource;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum AudioChannel
{
    Music,
    UISounds,
    FxSounds,
}