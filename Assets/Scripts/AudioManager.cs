using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private static AudioSource audioSourceMusic;
	private static float volumeRestore = 1f;

    private void Awake()
    {
        audioSourceMusic = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayMusic();
    }

    public static void PlayMusic()
    {
        audioSourceMusic.Play();
    }

    public static void MuteMusic()
    {
        audioSourceMusic.Pause();
    }

    public static void ReduceVolume()
    {
		volumeRestore = audioSourceMusic.volume;
		if(volumeRestore > 0.3f)
			audioSourceMusic.volume = 0.3f;
    }

    public static void RestoreVolume()
    {
        audioSourceMusic.volume = volumeRestore;
    }

    public static void ChangeVolume(float volume)
    {
        audioSourceMusic.volume = volume;
    }
}
