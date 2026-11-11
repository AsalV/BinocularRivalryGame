using UnityEngine;

/// <summary>
/// X 光药丸：只能吃一次，效果永久。挂在药丸上（药丸先做成可抓取）。
/// 拿到嘴边（碰到 MouthTrigger）就吃下，调用 XRayBox.Activate()，药丸消失、不再出现。
/// </summary>
public class XRayPill : MonoBehaviour
{
    public XRayBox target;
    public AudioClip swallowClip;

    bool used;

    public bool TryConsume()
    {
        if (used || target == null) return false;
        used = true;
        if (swallowClip) AudioSource.PlayClipAtPoint(swallowClip, transform.position);
        target.Activate();
        gameObject.SetActive(false);
        return true;
    }

    [ContextMenu("Test Eat (Play 模式下)")]
    void TestEat() => TryConsume();
}
