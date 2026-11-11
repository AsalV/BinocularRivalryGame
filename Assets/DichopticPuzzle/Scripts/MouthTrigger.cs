using UnityEngine;

/// <summary>
/// “嘴”的位置。挂在 CenterEyeAnchor 下面的一个空物体上，
/// 本地位置大约 (0, -0.07, 0.03)。药丸碰到这里就算吃下去。
/// 支持两种药丸：RevealPill（第一个房间，短暂）和 XRayPill（X 光，永久）。
/// </summary>
public class MouthTrigger : MonoBehaviour
{
    public float radius = 0.07f;

    void Awake()
    {
        var col = GetComponent<SphereCollider>();
        if (col == null) col = gameObject.AddComponent<SphereCollider>();
        col.radius = radius;
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        var pill = other.GetComponentInParent<RevealPill>();
        if (pill != null) { pill.TryConsume(this); return; }

        var xray = other.GetComponentInParent<XRayPill>();
        if (xray != null) xray.TryConsume();
    }
}
