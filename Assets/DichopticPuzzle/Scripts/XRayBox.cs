using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

/// <summary>
/// “透视黑盒子”。挂在盒子上。
///  - 开始：盒子两眼都看得到，钥匙隐藏、不能抓
///  - Activate()（吃下 X 光药丸后）：盒子的面只给一只眼（默认左眼），钥匙出现在另一只眼（默认右眼），可以伸手抓
///  - 钥匙离开盒子内部区域：钥匙变成两眼都看得到，盒子恢复正常（可关）
/// 盒子的面和钥匙的材质都要用带 _ShowEye 的 shader（EyeOnlyLit / EyeOnly / EyeOnlyFade）。
/// </summary>
public class XRayBox : MonoBehaviour
{
    public enum Eye { Left = 0, Right = 1, Both = 2 }

    [Header("盒子")]
    [Tooltip("吃药后变成只给一只眼看的部分（盒子的几个面）。盒子的边框不要放进来，让它一直两眼可见")]
    public Renderer[] boxFaces;
    [Tooltip("盒子内部区域：一个勾了 Is Trigger 的 Box Collider。钥匙离开它就算被拿出来")]
    public Collider insideZone;

    [Header("钥匙")]
    public DoorKey key;

    [Header("眼睛")]
    public Eye boxEye = Eye.Left;
    public Eye keyEye = Eye.Right;
    [Tooltip("钥匙拿出来后：钥匙两眼都看得到，盒子恢复正常")]
    public bool restoreWhenKeyTaken = true;
    [Tooltip("没戴头显时（编辑器只渲染一只眼）：吃药后隐藏盒子面、钥匙双眼可见，方便在编辑器里看效果")]
    public bool previewWithoutHeadset = true;

    [Header("声音（可选）")]
    public AudioClip activateClip;
    public AudioClip keyTakenClip;

    [Header("事件")]
    public UnityEvent onActivated;
    public UnityEvent onKeyTaken;

    public bool Activated { get; private set; }
    public bool KeyTaken { get; private set; }

    static readonly int EyeID = Shader.PropertyToID("_ShowEye");

    void Start()
    {
        SetEye(boxFaces, Eye.Both);
        if (key != null)
        {
            key.SetHidden(true);
            key.SetGrabbable(false);
        }
        if (insideZone != null) insideZone.isTrigger = true;
    }

    [ContextMenu("Test Activate (Play 模式下)")]
    void TestActivate() => Activate();

    public void Activate()
    {
        if (Activated) return;
        Activated = true;

        bool preview = previewWithoutHeadset && !XRSettings.isDeviceActive;
        if (preview) Debug.Log("[XRayBox] 没检测到头显：预览模式（盒子面隐藏，钥匙双眼可见）", this);

        // 非 VR 渲染时 shader 认为是左眼：把盒子设成右眼 = 在编辑器里隐藏
        SetEye(boxFaces, preview ? Eye.Right : boxEye);

        if (key != null)
        {
            key.SetHidden(false);
            SetEye(key.Renderers, preview ? Eye.Both : keyEye);
            key.SetGrabbable(true);
        }

        Play(activateClip);
        onActivated?.Invoke();
    }

    void Update()
    {
        if (!Activated || KeyTaken || key == null || insideZone == null) return;
        if (!insideZone.bounds.Contains(key.transform.position)) OnKeyTakenOut();
    }

    void OnKeyTakenOut()
    {
        KeyTaken = true;
        if (restoreWhenKeyTaken)
        {
            SetEye(key.Renderers, Eye.Both);
            SetEye(boxFaces, Eye.Both);
        }
        Play(keyTakenClip);
        onKeyTaken?.Invoke();
    }

    void SetEye(Renderer[] renderers, Eye eye)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
        {
            if (r == null) continue;
            foreach (var m in r.materials)   // 自动复制成实例，不会改到材质资源本身
            {
                if (m.HasProperty(EyeID)) m.SetFloat(EyeID, (int)eye);
                else Debug.LogWarning($"[XRayBox] {r.name} 的材质 {m.name} 没有 _ShowEye，换成 EyeOnlyLit 材质", r);
            }
        }
    }

    void Play(AudioClip clip)
    {
        if (clip) AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
