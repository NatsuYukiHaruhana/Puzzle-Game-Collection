using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSrc;      // audio source component
    [SerializeField] private List<AudioClip> BGMs;      // list of all songs for current scene
    [SerializeField] private bool loopBGM = false;      // loop first song in the list 
    [SerializeField] private bool loopList = false;     // when the last song ends, go back to the first song
    private int currentlyPlaying = 0;                   // number of the currently playing song in the list
    private bool keepPlaying = true;                    // should the songs keep playing?

    private void Awake()
    {
        if (loopBGM == true)
        {
            audioSrc.loop = true;
        }

        if (BGMs.Count > 0)
        {
            audioSrc.clip = BGMs[0];
            audioSrc.Play();
        } 
        else
        {
            keepPlaying = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (keepPlaying == true && audioSrc.isPlaying == false)
        {
            currentlyPlaying++;

            if (currentlyPlaying > BGMs.Count)
            {
                if (loopList == true) { 
                    currentlyPlaying = 0;
                }
                else
                {
                    keepPlaying = false;
                }
            }

            if (keepPlaying) { 
                audioSrc.clip = BGMs[currentlyPlaying];
                audioSrc.Play();
            } 
            else
            {
                audioSrc.Stop();
            }
        }
    }
}
