using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioClip musicSplash = null;
    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Instance.Play();
    }

    private void Play()
    {
        audioSource.PlayOneShot(musicSplash);
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
