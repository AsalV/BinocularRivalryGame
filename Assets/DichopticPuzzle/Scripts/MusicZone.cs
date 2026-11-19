using UnityEngine;

/// <summary>
/// 房间音乐区域。挂在一个空物体上，用 Box Collider 的大小框住整个房间。
/// 玩家的头（Main Camera）进入这个区域，就切换到这里的音乐。
/// Box Collider 只用来在场景里画范围，运行时会自动关掉，不会挡手、不影响抓取。
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class MusicZone : MonoBehaviour
{
    public AudioClip music;
    [Tooltip("切换到这首歌的淡入淡出时间（秒）")]
    public float fadeTime = 2f;

    static MusicZone active;

    Vector3 center, halfSize;
    Transform head;

    void Awake()
    {
        var box = GetComponent<BoxCollider>();
        center = box.center;
        halfSize = box.size * 0.5f;
        box.enabled = false;   // 只当范围用，不参与物理
    }

    void Update()
    {
        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        if (active == this || !Contains(head.position)) return;
        active = this;
        MusicManager.Get().Play(music, fadeTime);
    }

    bool Contains(Vector3 worldPos)
    {
        Vector3 p = transform.InverseTransformPoint(worldPos) - center;
        return Mathf.Abs(p.x) <= halfSize.x && Mathf.Abs(p.y) <= halfSize.y && Mathf.Abs(p.z) <= halfSize.z;
    }

    void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider>();
        if (box == null) return;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(1f, 0.4f, 1f, 0.12f);
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = new Color(1f, 0.4f, 1f, 0.8f);
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
