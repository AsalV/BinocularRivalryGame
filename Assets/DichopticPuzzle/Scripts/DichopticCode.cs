using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

/// <summary>
/// 挂在密码箱上。把一串密码拆给左右眼：
///  - 标 L 的位：只给左眼看，一直显示
///  - 标 R 的位：只给右眼看，平时隐藏；调用 Activate() 后浮现几秒再淡出
/// 两只眼同时睁开时，大脑把两组数字融合成完整的密码。
/// 数字排成一行，面朝本物体的 -Z 方向（蓝色 Z 轴要背对玩家）。
/// </summary>
public class DichopticCode : MonoBehaviour
{
    [Header("密码")]
    [Tooltip("只能是数字")]
    public string code = "40718253";
    [Tooltip("每一位给哪只眼：L = 左眼（常驻），R = 右眼（启用能力后才出现）。长度必须和密码一样")]
    public string eyeMask = "LRLRLRLR";

    [Header("外观")]
    [Tooltip("用 Custom/EyeOnlyFade shader 的材质。左右眼会自动各复制一份")]
    public Material digitMaterial;
    public Color leftColor = new Color(0.3f, 1f, 1f, 1f);
    public Color rightColor = new Color(0.3f, 1f, 1f, 1f);
    [Tooltip("数字放在哪里，留空就放在本物体上")]
    public Transform digitAnchor;
    public float digitHeight = 0.10f;
    public float digitWidth = 0.055f;
    public float segmentThickness = 0.010f;
    public float spacing = 0.025f;
    [Tooltip("右眼数字从多低的地方浮上来（米）")]
    public float riseDistance = 0.03f;

    [Header("能力时间（秒）")]
    public float fadeInTime = 0.4f;
    public float holdTime = 3f;
    public float fadeOutTime = 1.2f;
    public float cooldown = 2f;

    [Header("调试")]
    [Tooltip("没戴头显时（编辑器 Game 视图只渲染一只眼），让两组数字都显示出来，方便看效果。戴头显 / Quest Link 时自动失效")]
    public bool previewBothEyesWithoutHeadset = true;

    [Header("声音（可选）")]
    public AudioClip revealClip;

    [Header("事件")]
    public UnityEvent onRevealStart;
    public UnityEvent onRevealEnd;

    public bool IsRevealing { get; private set; }
    public bool IsReady => !IsRevealing && !coolingDown;

    static readonly int AlphaID = Shader.PropertyToID("_Alpha");
    static readonly int EyeID = Shader.PropertyToID("_ShowEye");
    static readonly int ColorID = Shader.PropertyToID("_BaseColor");

    bool coolingDown;
    Material leftMat, rightMat;
    Transform rightRoot;

    void Start()
    {
        Build();
    }

    void Build()
    {
        if (digitMaterial == null) { Debug.LogError("[DichopticCode] 没有指定 Digit Material", this); return; }
        if (string.IsNullOrEmpty(code) || eyeMask.Length != code.Length)
        {
            Debug.LogError("[DichopticCode] Eye Mask 的长度必须和密码一样", this);
            return;
        }

        leftMat = new Material(digitMaterial);
        leftMat.SetFloat(EyeID, 0);
        leftMat.SetColor(ColorID, leftColor);
        leftMat.SetFloat(AlphaID, 1);

        rightMat = new Material(digitMaterial);
        rightMat.SetFloat(EyeID, 1);
        rightMat.SetColor(ColorID, rightColor);
        rightMat.SetFloat(AlphaID, 0);

        if (previewBothEyesWithoutHeadset && !XRSettings.isDeviceActive)
        {
            // 非 VR 渲染时 shader 永远认为是“左眼”，右眼数字会被丢掉，所以这里两组都改成双眼可见
            leftMat.SetFloat(EyeID, 2);
            rightMat.SetFloat(EyeID, 2);
            Debug.Log("[DichopticCode] 没检测到头显：两组数字在编辑器里都显示（预览模式）", this);
        }

        Transform anchor = digitAnchor != null ? digitAnchor : transform;

        var leftRoot = new GameObject("LeftEyeDigits").transform;
        leftRoot.SetParent(anchor, false);
        rightRoot = new GameObject("RightEyeDigits").transform;
        rightRoot.SetParent(anchor, false);
        rightRoot.localPosition = Vector3.down * riseDistance;

        int n = code.Length;
        float total = n * digitWidth + (n - 1) * spacing;
        float x0 = -total / 2 + digitWidth / 2;

        for (int i = 0; i < n; i++)
        {
            bool isLeft = char.ToUpper(eyeMask[i]) == 'L';
            Transform parent = isLeft ? leftRoot : rightRoot;
            Material mat = isLeft ? leftMat : rightMat;

            var d = SevenSegment.Build(code[i], parent, mat, digitWidth, digitHeight, segmentThickness);
            d.transform.localPosition = new Vector3(x0 + i * (digitWidth + spacing), 0, 0);
        }
    }

    /// <summary>给按钮 / UnityEvent 用</summary>
    public void Activate() => TryActivate();

    /// <summary>成功启用返回 true；正在显示或冷却中返回 false</summary>
    public bool TryActivate()
    {
        if (!Application.isPlaying) { Debug.LogWarning("[DichopticCode] 要在 Play 模式下才能触发", this); return false; }
        if (rightMat == null) { Debug.LogWarning("[DichopticCode] 数字没有生成，先看 Console 里的红色报错（材质没拖、Eye Mask 长度不对等）", this); return false; }
        if (!IsReady) { Debug.Log("[DichopticCode] 正在显示或冷却中，稍后再试", this); return false; }
        StartCoroutine(RevealRoutine());
        return true;
    }

    [ContextMenu("Test Reveal (Play 模式下)")]
    void TestReveal() => TryActivate();

    IEnumerator RevealRoutine()
    {
        IsRevealing = true;
        onRevealStart?.Invoke();
        if (revealClip) AudioSource.PlayClipAtPoint(revealClip, transform.position);

        yield return Fade(0f, 1f, fadeInTime, true);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(1f, 0f, fadeOutTime, false);

        rightRoot.localPosition = Vector3.down * riseDistance;
        IsRevealing = false;
        onRevealEnd?.Invoke();

        coolingDown = true;
        yield return new WaitForSeconds(cooldown);
        coolingDown = false;
    }

    IEnumerator Fade(float from, float to, float duration, bool rise)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            rightMat.SetFloat(AlphaID, Mathf.Lerp(from, to, k));
            if (rise) rightRoot.localPosition = Vector3.Lerp(Vector3.down * riseDistance, Vector3.zero, k);
            yield return null;
        }
        rightMat.SetFloat(AlphaID, to);
        if (rise) rightRoot.localPosition = Vector3.zero;
    }

    void OnDestroy()
    {
        if (leftMat) Destroy(leftMat);
        if (rightMat) Destroy(rightMat);
    }
}
