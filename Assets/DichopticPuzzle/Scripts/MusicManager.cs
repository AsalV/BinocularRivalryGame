using System.Collections;
using UnityEngine;

/// <summary>
/// 背景音乐播放器：循环播放，换歌时淡入淡出交叉过渡。
/// 场景里不用手动放——第一个 MusicZone 用到它时会自动创建。
/// 想调整音量，也可以自己建一个空物体挂上它。
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Range(0f, 1f)] public float volume = 0.6f;
    public float defaultFade = 2f;

    AudioSource srcA, srcB, current;
    AudioClip currentClip;
    Coroutine fading;

    public static MusicManager Get()
    {
        if (Instance == null)
        {
            var go = new GameObject("MusicManager");
            Instance = go.AddComponent<MusicManager>();
        }
        return Instance;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        srcA = MakeSource();
        srcB = MakeSource();
    }

    AudioSource MakeSource()
    {
        var s = gameObject.AddComponent<AudioSource>();
        s.loop = true;
        s.playOnAwake = false;
        s.spatialBlend = 0f;   // 2D 背景音乐，不随位置变化
        s.volume = 0f;
        return s;
    }

    /// <summary>切换到这首歌（传 null = 淡出到静音）</summary>
    public void Play(AudioClip clip, float fade = -1f)
    {
        if (clip == currentClip) return;
        if (fade < 0f) fade = defaultFade;
        currentClip = clip;

        var next = current == srcA ? srcB : srcA;
        var prev = current;
        next.clip = clip;
        next.volume = 0f;
        if (clip != null) next.Play();
        current = next;

        if (fading != null) StopCoroutine(fading);
        fading = StartCoroutine(CrossFade(prev, next, fade));
    }

    public void Stop(float fade = -1f) => Play(null, fade);

    IEnumerator CrossFade(AudioSource from, AudioSource to, float duration)
    {
        float fromStart = from != null ? from.volume : 0f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            if (from != null) from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, volume, k);
            yield return null;
        }
        if (from != null) { from.volume = 0f; from.Stop(); }
        to.volume = to.clip != null ? volume : 0f;
    }
}
