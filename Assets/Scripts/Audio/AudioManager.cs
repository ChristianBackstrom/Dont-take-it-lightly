using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    private Queue<AudioSource> audioSources = new Queue<AudioSource>();

    public void PlaySoundEffect(SimpleAudioEvent audio)
    {
        if (audioSources.Count == 0)
        {
            audioSources.Enqueue(gameObject.AddComponent<AudioSource>());
        }
        var source = audioSources.Dequeue();
        int index = audio.Play(source);

        StartCoroutine(ReturnToQueue(source, audio.Clips[index].length));
    }

    private IEnumerator ReturnToQueue(AudioSource source, float length)
    {
        yield return new WaitForSeconds(length);

        audioSources.Enqueue(source);
    }
}
