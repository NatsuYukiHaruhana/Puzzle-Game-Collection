using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;
    [SerializeField] private bool forBGM, forSFX;

    private void Start()
    {
        if (forBGM) { 
            slider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
            mixer.SetFloat("BGMVolume", Mathf.Log10(PlayerPrefs.GetFloat("BGMVolume", 1f)) * 20);
        } else {
            slider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1f)) * 20);
        }
    }

    public void SetLevel(float sliderValue)
    {
        if (forBGM) { 
            mixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("BGMVolume", sliderValue);
        } else {
            mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        }
    }
}