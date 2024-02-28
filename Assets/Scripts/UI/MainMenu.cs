using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    public void Awake()
    {
        audioMixer.SetFloat("mainVol", Mathf.Log10(PlayerPrefs.GetFloat("mainVol", 0.75f)) * 20);
        audioMixer.SetFloat("musicVol", Mathf.Log10(PlayerPrefs.GetFloat("musicVol", 0.75f)) * 20);
        audioMixer.SetFloat("soundVol", Mathf.Log10(PlayerPrefs.GetFloat("soundVol", 0.75f)) * 20);
        audioMixer.SetFloat("ambientVol", Mathf.Log10(PlayerPrefs.GetFloat("ambientVol", 0.75f)) * 20);
        audioMixer.SetFloat("interfaceVol", Mathf.Log10(PlayerPrefs.GetFloat("interfaceVol", 0.75f)) * 20);
    }
    
}
