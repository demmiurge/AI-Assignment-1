using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource _sfxSource, _musicSource;
    [SerializeField] private float _timeToFade;

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
        StartCoroutine(DecreaseVolumeOverTime());
    }

    public void FadeInMusic()
    {
        StartCoroutine(IncreaseVolumeOverTime());
    }

    private IEnumerator IncreaseVolumeOverTime()
    {
        float time = 0f;

        while (time < _timeToFade)
        {
            _musicSource.volume = 0f + (time / _timeToFade);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DecreaseVolumeOverTime()
    {
        float time = 0f;

        while (time < _timeToFade)
        {
            _musicSource.volume = 1f - (time / _timeToFade);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
