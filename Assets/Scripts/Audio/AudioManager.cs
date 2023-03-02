using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource _sfxSource, _musicSource;
    [SerializeField][Range(0f, 1f)] private float _fadeIntensity;
    [SerializeField] private float _maxVolume;
    private readonly float _volumeSteps = 0.001f;
    [SerializeField] private AudioClip _menuHoverClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _musicSource.Play();
    }

    public void DisableMusicSource()
    {
        _musicSource.mute = true;
    }

    public void DisableEffectsSource()
    {
        _sfxSource.mute = true;
    }

    public void EnableMusicSource()
    {
        _musicSource.mute = false;
    }

    public void EnableEffectsSource()
    {
        _musicSource.mute = false;
    }

    public void UpdateMusicSourceVolume(float volume)
    {
        _musicSource.volume = volume;
    }

    public void UpdateEffectSourceVolume(float volume)
    {
        _sfxSource.volume = volume;
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayHoverSound()
    {
        _sfxSource.PlayOneShot(_menuHoverClip, 0.1f);
    }

    public void FadeOutMusic()
    {
        if (_musicSource.volume == _maxVolume)
        {
            StartCoroutine(FadeOutTrackWithSteps());
        }
    }

    public void ResetMusic()
    {
        _musicSource.Stop();
        _musicSource.volume = _maxVolume;
        _musicSource.Play();
    }

    public void FadeInMusic()
    {
        if (_musicSource.volume ==_fadeIntensity)
        {
            StartCoroutine(FadeInTrackWithSteps());
        }
    }

    private IEnumerator FadeOutTrackWithSteps()
    {
        while (_musicSource.volume > _fadeIntensity)
        {
            _musicSource.volume -= _volumeSteps;
            yield return new WaitForEndOfFrame();
        }

        _musicSource.volume = _fadeIntensity;
    }

    private IEnumerator FadeInTrackWithSteps()
    {
        while (_musicSource.volume < _maxVolume)
        {
            _musicSource.volume += _volumeSteps;
            yield return new WaitForEndOfFrame();
        }

        _musicSource.volume = _maxVolume;
    }
}
