using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSrc; // audio source component
    [SerializeField] private List<AudioClip> SFXs; // list of general sound effects

    public void PlaySound(string SFX) {
        foreach (AudioClip sfx in SFXs) {
            if (sfx.name == SFX) {
                audioSrc.PlayOneShot(sfx);
                return;
            }
        }
    }

    public void PlaySound(AudioClip SFX) {
        audioSrc.PlayOneShot(SFX);
    }
}
