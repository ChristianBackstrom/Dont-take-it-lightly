using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundPage : SettingsPage
{
    [SerializeField] Slider main, mus, sfx, amb, itf;
    [SerializeField] AudioMixer audioMixer;
    void OnEnable()
    {
        main.value = PlayerPrefs.GetFloat("mainVol", 0.75f);
        mus.value = PlayerPrefs.GetFloat("musicVol", 0.75f);
        sfx.value = PlayerPrefs.GetFloat("soundVol", 0.75f);
        amb.value = PlayerPrefs.GetFloat("ambientVol", 0.75f);
        itf.value = PlayerPrefs.GetFloat("interfaceVol", 0.75f);
    }

    public void SetMainLevel (float sliderValue)
    {
	    audioMixer.SetFloat("mainVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("mainVol", sliderValue);
    }

    public void SetMusicLevel (float sliderValue)
    {
	    audioMixer.SetFloat("musicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("musicVol", sliderValue);
    }

    public void SetSfxLevel (float sliderValue)
    {
	    audioMixer.SetFloat("soundVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("soundVol", sliderValue);
    }

    public void SetAmbLevel (float sliderValue)
    {
	    audioMixer.SetFloat("ambientVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("ambientVol", sliderValue);
    }

    public void SetInterfaceLevel (float sliderValue)
    {
	    audioMixer.SetFloat("interfaceVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("interfaceVol", sliderValue);
    }
}
