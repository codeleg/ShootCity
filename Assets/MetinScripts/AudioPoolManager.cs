using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

public class AudioPoolManager : MonoBehaviour
{
    public static AudioPoolManager Instance;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 16;
    [SerializeField] private int maxSimultaneousSounds = 32; // aynı anda çalabilecek maksimum ses

    [Header("Mixer Settings")]
    [SerializeField] public AudioMixerGroup defaultSFXMixerGroup;

    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>(32);
    private readonly List<AudioSource> _activeSources = new List<AudioSource>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("AudioSrc_" + i);
            go.transform.SetParent(transform);

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = defaultSFXMixerGroup;

            go.SetActive(false);
            _pool.Enqueue(src);
        }
    }

    /// <summary>
    /// Ses çal (3D veya 2D)
    /// </summary>
    public void PlayAt(AudioClip clip, Vector3 pos, float volume = 1f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        if (clip == null) return;

        // Ses limiti kontrolü
        if (_activeSources.Count >= maxSimultaneousSounds)
        {
            Debug.LogWarning("[AudioPoolManager] Max ses limiti aşıldı, yeni ses oynatılmadı.");
            return;
        }

        if (_pool.Count == 0)
        {
            Debug.LogWarning("[AudioPoolManager] Havuz boş, yeni ses oynatılamıyor.");
            return;
        }

        var src = _pool.Dequeue();

        src.transform.position = pos;
        src.clip = clip;
        src.volume = volume;
        src.spatialBlend = Mathf.Clamp01(spatialBlend); // 0 = 2D, 1 = 3D
        src.outputAudioMixerGroup = mixer != null ? mixer : defaultSFXMixerGroup;

        src.gameObject.SetActive(true);
        src.Play();

        _activeSources.Add(src);

        StartCoroutine(ReturnAfter(src, clip.length));
    }

    private IEnumerator ReturnAfter(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);

        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);

        _activeSources.Remove(src);
        _pool.Enqueue(src);
    }
}

// örnek kullanım : AudioPoolManager.Instance.PlayAt(gunClip, gunEnd.position, 1f, 1f); // 
// sesi kullanacagım objenin altında   //  public UnityEngine.Audio.AudioMixerGroup CevreSfxMixerGroup; DİYEREK  Inspectörde atayarak //Farklı ses tipleri için ayrı mixer group’lar yapabilirsin://SFX_Weapons → silah sesleri//SFX_Environment → objelere vurma, patlama//SFX_UI → coin, exp, menu


//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//public class AudioPoolManager : MonoBehaviour
//{
//    public static AudioPoolManager Instance;
//    [SerializeField] private int poolSize = 16;

//    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>(32);

//    void Awake()
//    {
//        Instance = this;
//        for (int i = 0; i < poolSize; i++)
//        {
//            var go = new GameObject("AudioSrc_" + i);
//            go.transform.SetParent(transform);
//            var src = go.AddComponent<AudioSource>();
//            src.playOnAwake = false;
//            go.SetActive(false);
//            _pool.Enqueue(src);
//        }
//    }

//    public void PlayAt(AudioClip clip, Vector3 pos, float volume = 1f, float spatialBlend = 1f)
//    {
//        if (clip == null || _pool.Count == 0) return;

//        var src = _pool.Dequeue();
//        src.transform.position = pos;
//        src.clip = clip;
//        src.volume = volume;
//        src.spatialBlend = spatialBlend;
//        src.gameObject.SetActive(true);
//        src.Play();
//        StartCoroutine(ReturnAfter(src, clip.length));
//    }

//    private IEnumerator ReturnAfter(AudioSource src, float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        src.Stop();
//        src.clip = null;
//        src.gameObject.SetActive(false);
//        _pool.Enqueue(src);
//    }
//}
