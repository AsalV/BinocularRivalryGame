using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在钥匙上（钥匙先用 Interaction SDK 的 Add Grab Interaction 做成可抓取）。
/// 负责：隐藏/显示、开关抓取、插进锁孔。
/// Grab Components 留空时，会自动找钥匙身上和子物体里名字带 “Grab” 的组件
/// （Grabbable、GrabInteractable、HandGrabInteractable 等）。
/// </summary>
public class DoorKey : MonoBehaviour
{
    [Tooltip("控制抓取的组件，留空自动查找")]
    public Behaviour[] grabComponents;

    Renderer[] renderers;
    public Renderer[] Renderers
    {
        get
        {
            if (renderers == null) renderers = GetComponentsInChildren<Renderer>(true);
            return renderers;
        }
    }

    void Awake()
    {
        if (grabComponents == null || grabComponents.Length == 0)
        {
            var found = new List<Behaviour>();
            foreach (var b in GetComponentsInChildren<Behaviour>(true))
            {
                if (b == null || b == this) continue;
                if (b.GetType().Name.Contains("Grab")) found.Add(b);
            }
            grabComponents = found.ToArray();
            if (found.Count == 0)
                Debug.LogWarning("[DoorKey] 没找到抓取组件，先对钥匙运行 Interaction SDK → Add Grab Interaction", this);
        }
    }

    public void SetHidden(bool hidden)
    {
        foreach (var r in Renderers) if (r) r.enabled = !hidden;
    }

    public void SetGrabbable(bool grabbable)
    {
        if (grabComponents == null) return;
        foreach (var b in grabComponents) if (b) b.enabled = grabbable;
    }

    /// <summary>插进锁孔：松手、固定在锁孔位置、不能再抓</summary>
    public void PlaceIn(Transform slot)
    {
        SetGrabbable(false);
        transform.SetParent(slot, true);
        transform.SetPositionAndRotation(slot.position, slot.rotation);
    }
}
