using UnityEngine;

/// <summary>
/// Gestor centralizado de audio - Simple y fácil de usar
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("Para música de fondo (loop)")]
    public AudioSource musicSource;
    
    [Tooltip("Para efectos de sonido (SFX)")]
    public AudioSource sfxSource;

    [Header("Música")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;
    public AudioClip musicaVictoria;
    public AudioClip musicaDerrota;

    [Header("SFX - UI")]
    public AudioClip sonidoBoton;
    public AudioClip sonidoBotonHover;
    public AudioClip sonidoMenuAbrir;
    public AudioClip sonidoMenuCerrar;

    [Header("SFX - Jugador")]
    public AudioClip sonidoPaso;
    public AudioClip sonidoAtaque;
    public AudioClip sonidoDañoRecibido;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoRecogerItem;
    public AudioClip sonidoUsarItem;

    [Header("SFX - Enemigos")]
    public AudioClip sonidoEnemigoAtaque;
    public AudioClip sonidoEnemigoDaño;
    public AudioClip sonidoEnemigoMuerte;

    [Header("SFX - Estabilizador")]
    public AudioClip sonidoComponenteColocado;
    public AudioClip sonidoEstabilizadorCompleto;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float volumenMusica = 0.5f;
    
    [Range(0f, 1f)]
    public float volumenSFX = 0.7f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InicializarAudioSources()
    {
        // Crear AudioSources si no existen
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        ActualizarVolumenes();
    }

    #region Música

    public void ReproducirMusicaMenu()
    {
        CambiarMusica(musicaMenu);
    }

    public void ReproducirMusicaJuego()
    {
        CambiarMusica(musicaJuego);
    }

    public void ReproducirMusicaVictoria()
    {
        CambiarMusica(musicaVictoria);
    }

    public void ReproducirMusicaDerrota()
    {
        CambiarMusica(musicaDerrota);
    }

    void CambiarMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica == null) return;

        if (musicSource.clip == nuevaMusica && musicSource.isPlaying)
            return; // Ya está sonando

        musicSource.clip = nuevaMusica;
        musicSource.Play();
    }

    public void PausarMusica()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ReanudarMusica()
    {
        if (!musicSource.isPlaying && musicSource.clip != null)
            musicSource.UnPause();
    }

    public void DetenerMusica()
    {
        musicSource.Stop();
    }

    #endregion

    #region Efectos de Sonido

    /// <summary>
    /// Reproduce un efecto de sonido
    /// </summary>
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volumenSFX);
    }

    // UI
    public void SonidoBoton() => ReproducirSFX(sonidoBoton);
    public void SonidoBotonHover() => ReproducirSFX(sonidoBotonHover);
    public void SonidoMenuAbrir() => ReproducirSFX(sonidoMenuAbrir);
    public void SonidoMenuCerrar() => ReproducirSFX(sonidoMenuCerrar);

    // Jugador
    public void SonidoPaso() => ReproducirSFX(sonidoPaso);
    public void SonidoAtaque() => ReproducirSFX(sonidoAtaque);
    public void SonidoDañoRecibido() => ReproducirSFX(sonidoDañoRecibido);
    public void SonidoMuerte() => ReproducirSFX(sonidoMuerte);
    public void SonidoRecogerItem() => ReproducirSFX(sonidoRecogerItem);
    public void SonidoUsarItem() => ReproducirSFX(sonidoUsarItem);

    // Enemigos
    public void SonidoEnemigoAtaque() => ReproducirSFX(sonidoEnemigoAtaque);
    public void SonidoEnemigoDaño() => ReproducirSFX(sonidoEnemigoDaño);
    public void SonidoEnemigoMuerte() => ReproducirSFX(sonidoEnemigoMuerte);

    // Estabilizador
    public void SonidoComponenteColocado() => ReproducirSFX(sonidoComponenteColocado);
    public void SonidoEstabilizadorCompleto() => ReproducirSFX(sonidoEstabilizadorCompleto);

    #endregion

    #region Configuración de Volumen

    public void ActualizarVolumenes()
    {
        if (musicSource != null)
            musicSource.volume = volumenMusica;
        
        // sfxSource usa volumenSFX en PlayOneShot
    }

    public void CambiarVolumenMusica(float nuevoVolumen)
    {
        volumenMusica = Mathf.Clamp01(nuevoVolumen);
        if (musicSource != null)
            musicSource.volume = volumenMusica;
    }

    public void CambiarVolumenSFX(float nuevoVolumen)
    {
        volumenSFX = Mathf.Clamp01(nuevoVolumen);
    }

    #endregion
}
