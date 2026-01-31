using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerenciador de som global (Singleton).
///
/// Responsabilidades:
/// - Tocar música de fundo nas cenas
/// - Persistir entre cenas (DontDestroyOnLoad)
/// - Lidar com restrições de áudio do WebGL
///
/// IMPORTANTE - WebGL:
/// Navegadores bloqueiam áudio automático. O som só inicia
/// após interação do usuário (clique). Este script aguarda
/// o primeiro clique antes de tocar música.
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    #region Configuração

    /// <summary>
    /// Música da tela de splash/menu.
    /// </summary>
    [SerializeField]
    private AudioClip musicSplash = null;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Componente de áudio para reprodução.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Indica se o áudio já foi iniciado (após interação do usuário).
    /// </summary>
    private bool audioStarted = false;

    /// <summary>
    /// Indica se deve tocar música quando possível.
    /// </summary>
    private bool shouldPlayMusic = true;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização - configura persistência entre cenas.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();

        // Configura AudioSource para loop
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Atualização - detecta clique do usuário para iniciar áudio (WebGL).
    /// </summary>
    private void Update()
    {
        // WebGL: Aguarda interação do usuário para iniciar áudio
        if (!audioStarted && shouldPlayMusic)
        {
            if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            {
                StartAudio();
            }
        }
    }

    #endregion

    #region Controle de Áudio

    /// <summary>
    /// Inicia a reprodução de áudio após interação do usuário.
    /// </summary>
    private void StartAudio()
    {
        if (audioStarted) return;

        audioStarted = true;

        if (audioSource != null && musicSplash != null && shouldPlayMusic)
        {
            audioSource.clip = musicSplash;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Toca a música (chamado externamente).
    /// No WebGL, só funciona após interação do usuário.
    /// </summary>
    public void Play()
    {
        shouldPlayMusic = true;

        if (audioStarted && audioSource != null)
        {
            if (musicSplash != null)
                audioSource.clip = musicSplash;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    /// <summary>
    /// Para a música.
    /// </summary>
    public void Stop()
    {
        shouldPlayMusic = false;

        if (audioSource != null)
            audioSource.Stop();
    }

    /// <summary>
    /// Pausa a música.
    /// </summary>
    public void Pause()
    {
        if (audioSource != null)
            audioSource.Pause();
    }

    /// <summary>
    /// Retoma a música pausada.
    /// </summary>
    public void Resume()
    {
        shouldPlayMusic = true;

        if (audioSource != null && audioStarted)
            audioSource.UnPause();
    }

    /// <summary>
    /// Define o volume da música (0 a 1).
    /// </summary>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = Mathf.Clamp01(volume);
    }

    #endregion
}
