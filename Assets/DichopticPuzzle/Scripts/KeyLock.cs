using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 锁孔。挂在门旁的锁上，物体要有一个勾了 Is Trigger 的 Collider。
/// 带 DoorKey 的钥匙碰到它：钥匙吸附到 Key Slot，触发 On Unlock（连到 DoorOpener.Open）。
/// </summary>
public class KeyLock : MonoBehaviour
{
    [Tooltip("钥匙插进去后的位置和朝向（一个空物体）。留空则不吸附")]
    public Transform keySlot;
    [Tooltip("开锁后让钥匙消失")]
    public bool hideKeyAfterUnlock = false;
    public AudioClip unlockClip;
    public UnityEvent onUnlock;

    public bool Unlocked { get; private set; }

    void Reset()
    {
        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Unlocked) return;
        var key = other.GetComponentInParent<DoorKey>();
        if (key != null) Unlock(key);
    }

    [ContextMenu("Test Unlock (Play 模式下)")]
    void TestUnlock()
    {
        if (Unlocked) return;
        Unlocked = true;
        onUnlock?.Invoke();
    }

    void Unlock(DoorKey key)
    {
        Unlocked = true;
        if (keySlot) key.PlaceIn(keySlot);
        if (hideKeyAfterUnlock) key.gameObject.SetActive(false);
        if (unlockClip) AudioSource.PlayClipAtPoint(unlockClip, transform.position);
        onUnlock?.Invoke();
    }
}
