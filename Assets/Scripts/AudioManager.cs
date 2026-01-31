using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerenciador de áudio da cena principal do jogo.
///
/// Responsabilidades:
/// - Tocar música de fundo durante o gameplay
/// - Controlar volume e estados de áudio
///
/// IMPORTANTE - WebGL:
/// Navegadores bloqueiam áudio automático. O som só inicia
/// após interação do usuário. Este script aguarda o primeiro
/// clique antes de tocar música.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    #region Campos

    /// <summary>
    /// Fonte de áudio para música de fundo.
    /// </summary>
    private static AudioSource audioSourceMusic;

    /// <summary>
    /// Volume a restaurar após redução temporária.
    /// </summary>
    private static float volumeRestore = 1f;

    /// <summary>
    /// Indica se o áudio já foi iniciado após interação do usuário.
    /// </summary>
    private static bool audioStarted = false;

    /// <summary>
    /// Indica se deve tocar música quando possível.
    /// </summary>
    private static bool shouldPlayMusic = true;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização - obtém referência ao AudioSource.
    /// </summary>
    private void Awake()
    {
        audioSourceMusic = GetComponent<AudioSource>();

        // Configura para não tocar automaticamente (WebGL)
        if (audioSourceMusic != null)
        {
            audioSourceMusic.playOnAwake = false;
        }
    }

    /// <summary>
    /// Inicialização tardia - prepara para tocar música.
    /// </summary>
    private void Start()
    {
        shouldPlayMusic = true;
    }

    /// <summary>
    /// Atualização - detecta interação do usuário para iniciar áudio (WebGL).
    /// </summary>
    private void Update()
    {
        // WebGL: Aguarda interação do usuário para iniciar áudio
        if (!audioStarted && shouldPlayMusic)
        {
            if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            {
                audioStarted = true;
                PlayMusic();
            }
        }
    }

    #endregion

    #region Controle de Música

    /// <summary>
    /// Inicia a reprodução da música de fundo.
    /// </summary>
    public static void PlayMusic()
    {
        shouldPlayMusic = true;

        if (audioSourceMusic != null && audioStarted)
        {
            if (!audioSourceMusic.isPlaying)
                audioSourceMusic.Play();
        }
    }

    /// <summary>
    /// Pausa a música de fundo.
    /// </summary>
    public static void MuteMusic()
    {
        if (audioSourceMusic != null)
            audioSourceMusic.Pause();
    }

    /// <summary>
    /// Reduz o volume temporariamente (para diálogos, etc).
    /// </summary>
    public static void ReduceVolume()
    {
        if (audioSourceMusic == null) return;

        volumeRestore = audioSourceMusic.volume;
        if (volumeRestore > 0.3f)
            audioSourceMusic.volume = 0.3f;
    }

    /// <summary>
    /// Restaura o volume ao valor anterior.
    /// </summary>
    public static void RestoreVolume()
    {
        if (audioSourceMusic != null)
            audioSourceMusic.volume = volumeRestore;
    }

    /// <summary>
    /// Define um volume específico.
    /// </summary>
    public static void ChangeVolume(float volume)
    {
        if (audioSourceMusic != null)
            audioSourceMusic.volume = Mathf.Clamp01(volume);
    }

    #endregion
}
