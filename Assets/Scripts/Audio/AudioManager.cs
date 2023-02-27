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

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void FadeOutMusic()
    {
        if (_musicSource.volume == _maxVolume)
        {
            StartCoroutine(FadeOutVolume());
        }
    }

    public void FadeInMusic()
    {
        if (_musicSource.volume != _maxVolume)
        {
            StartCoroutine(FadeInVolume());
        }        
    }

    private IEnumerator FadeOutVolume()
    {
        while (_musicSource.volume >= _fadeIntensity)
        {
            _musicSource.volume -= _volumeSteps;
            yield return new WaitForEndOfFrame();
        }

        _musicSource.volume = _fadeIntensity;
    }

    private IEnumerator FadeInVolume()
    {
        while (_musicSource.volume <= _maxVolume)
        {
            _musicSource.volume += _volumeSteps;
            yield return new WaitForEndOfFrame();
        }

        _musicSource.volume = _maxVolume;
    }
}
