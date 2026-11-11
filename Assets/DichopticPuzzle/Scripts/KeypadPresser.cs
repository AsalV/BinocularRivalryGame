using UnityEngine;

/// <summary>
/// 挂在“用来按按钮的东西”上：手柄锚点下的一个小空物体，或者指尖。
/// 会自动加一个小球形触发器和运动学刚体。
/// KeypadButton 和 RevealButton 都认它。
/// </summary>
public class KeypadPresser : MonoBehaviour
{
    public float radius = 0.012f;

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
}
