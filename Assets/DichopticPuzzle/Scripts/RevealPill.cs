using System.Collections;
using UnityEngine;

/// <summary>
/// 挂在“药丸”上（药丸本身用 Interaction SDK 做成可抓取）。
/// 拿到嘴边（碰到 MouthTrigger）就会被吃掉，启用双眼能力，过几秒在原处重新出现。
/// 能力在冷却中时吃不下去，药丸不会消失。
/// </summary>
public class RevealPill : MonoBehaviour
{
    public DichopticCode target;
    [Tooltip("吃掉后多久在原位置重新出现；设为负数则不再出现")]
    public float respawnDelay = 3f;
    public AudioClip swallowClip;

    Vector3 startPos;
    Quaternion startRot;

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public bool TryConsume(MonoBehaviour runner)
    {
        if (target == null || !target.TryActivate()) return false;

        if (swallowClip) AudioSource.PlayClipAtPoint(swallowClip, transform.position);
        gameObject.SetActive(false);
        if (respawnDelay >= 0 && runner != null) runner.StartCoroutine(Respawn());
        return true;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        transform.SetPositionAndRotation(startPos, startRot);
        gameObject.SetActive(true);
    }
}
