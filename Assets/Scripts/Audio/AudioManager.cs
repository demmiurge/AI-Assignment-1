using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Foldout("General sound settings", styled = true)]
    [SerializeField] private AudioSource _sfxSource, _musicSource;
    [SerializeField][Range(0f, 1f)] private float _fadeIntensity;
    [SerializeField] private float _maxVolume;
    private readonly float _volumeSteps = 0.001f;
    [SerializeField] private AudioClip _menuHoverClip;

    [Foldout("Sliders references", styled = true)]
    [SerializeField] private GameObject _sliderSFXSource;
    [SerializeField] private string _nameSFXSource = "SFX";
    [SerializeField] private GameObject _sliderMusicSource;
    [SerializeField] private string _nameMusicSource = "Music";

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

    void WriteBasicInfo(GameObject slider, string text, float minValue, float maxValue)
    {
        TextMeshProUGUI _minText;
        TextMeshProUGUI _maxText;
        TextMeshProUGUI _labelText;

        _minText = slider.transform.Find("MinNumber").GetComponent<TextMeshProUGUI>();
        _maxText = slider.transform.Find("MaxNumber").GetComponent<TextMeshProUGUI>();
        _labelText = slider.transform.Find("Title").GetComponent<TextMeshProUGUI>();

        _labelText.text = text;
        _minText.text = minValue.ToString();
        _maxText.text = maxValue.ToString();
    }

    private void Start()
    {
        WriteBasicInfo(_sliderSFXSource, _nameSFXSource, 0, _maxVolume);
        WriteBasicInfo(_sliderMusicSource, _nameMusicSource, 0, _maxVolume);

        _musicSource.Play();
    }

    public void SFXChanged(float newValueVolumen)
    {
        _sfxSource.volume = newValueVolumen;
    }

    public void MusicChanged(float newValueVolumen)
    {
        _musicSource.volume = newValueVolumen;
    }

    public void ToggleEffects()
    {
        _sfxSource.mute = !_sfxSource.mute;
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
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
        if (_musicSource.volume == _fadeIntensity)
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
