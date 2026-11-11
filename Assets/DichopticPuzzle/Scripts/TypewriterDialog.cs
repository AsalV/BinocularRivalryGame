using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 3D 空间里的对话框：游戏开始后，文字一个字一个字打出来，最后一页打完停住。
/// 挂在一个空物体上即可，背景板、霓虹边框、文字都会自动生成。
/// 文字面朝本物体的 -Z 方向（蓝色 Z 轴背对玩家）。
/// 在组件 ⋮ 菜单里点 “Rebuild Dialog” 可以在编辑模式下先生成出来看大小和位置。
/// </summary>
public class TypewriterDialog : MonoBehaviour
{
    [Header("内容（每一项是一页，按顺序打出来）")]
    [TextArea(2, 6)]
    public string[] pages =
    {
        "Welcome aboard.",
        "Your eyes no longer agree with each other.\nFind the code. Open the door."
    };

    [Header("节奏（秒）")]
    [Tooltip("游戏开始后等多久才出现")]
    public float startDelay = 1.5f;
    [Tooltip("每个字之间的间隔")]
    public float charInterval = 0.045f;
    [Tooltip("遇到标点额外停顿")]
    public float punctuationPause = 0.25f;
    [Tooltip("一页打完后停多久再换下一页（最后一页不受影响）")]
    public float pageHold = 2.5f;
    [Tooltip("最后一页打完后多少秒隐藏；小于 0 = 一直停在那里")]
    public float hideAfter = -1f;

    [Header("位置")]
    [Tooltip("出现时自动放到玩家正前方。关掉则停在你在场景里摆的位置")]
    public bool placeInFrontOfPlayer = false;
    public float distance = 1.6f;
    [Tooltip("相对玩家眼睛的高度（米），负数 = 略低于视线")]
    public float heightOffset = -0.1f;
    [Tooltip("始终转向玩家（只转水平方向）")]
    public bool facePlayer = false;

    [Header("外观")]
    public Vector2 size = new Vector2(1.0f, 0.32f);
    [Tooltip("TextMeshPro 3D 字号，0.45 ≈ 3 cm 高")]
    public float fontSize = 0.45f;
    public Color textColor = new Color(0.75f, 1f, 1f);
    [Tooltip("要显示中文必须指定一个含中文的字体资源，否则会变成方框")]
    public TMP_FontAsset font;
    [Tooltip("背景和边框用的材质：拖入 DigitMat（EyeOnlyFade）")]
    public Material panelMaterial;
    public Color panelColor = new Color(0.02f, 0.04f, 0.08f, 0.85f);
    public Color frameColor = new Color(0.2f, 1f, 1f, 1f);
    public float frameWidth = 0.006f;
    public float padding = 0.05f;
    [Tooltip("文字放不下时自动把对话框加高（顶边不动，往下长）。按所有页里最长的那页来算，换页时框不会跳")]
    public bool autoFitHeight = true;

    [Header("声音（可选）")]
    public AudioSource audioSource;
    [Tooltip("每打一个字播放一次的短音效")]
    public AudioClip typeClip;

    [Header("事件")]
    public UnityEvent onFinished;

    public bool IsTyping { get; private set; }

    Transform visual;
    TextMeshPro tmp;
    Transform cam;

    const string Punctuation = ".,!?;:。，！？；：…、";

    void Start()
    {
        BuildIfNeeded();
        visual.gameObject.SetActive(false);
        StartCoroutine(Run());
    }

    void LateUpdate()
    {
        if (!facePlayer || cam == null || visual == null || !visual.gameObject.activeSelf) return;
        Vector3 dir = transform.position - cam.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir);
    }

    /// <summary>从外部重新播放（比如进入某个区域时触发）</summary>
    public void Play()
    {
        StopAllCoroutines();
        BuildIfNeeded();
        StartCoroutine(Run(skipDelay: true));
    }

    [ContextMenu("Replay (Play 模式下)")]
    void Replay() => Play();

    IEnumerator Run(bool skipDelay = false)
    {
        if (!skipDelay && startDelay > 0) yield return new WaitForSeconds(startDelay);

        if (Camera.main != null) cam = Camera.main.transform;
        if (placeInFrontOfPlayer && cam != null) PlaceInFront();

        FitToPages();
        visual.gameObject.SetActive(true);
        IsTyping = true;

        for (int p = 0; p < pages.Length; p++)
        {
            yield return TypePage(pages[p]);
            if (p < pages.Length - 1) yield return new WaitForSeconds(pageHold);
        }

        IsTyping = false;
        onFinished?.Invoke();

        if (hideAfter >= 0)
        {
            yield return new WaitForSeconds(hideAfter);
            visual.gameObject.SetActive(false);
        }
    }

    IEnumerator TypePage(string page)
    {
        tmp.text = page;
        tmp.maxVisibleCharacters = 0;
        tmp.ForceMeshUpdate();
        int total = tmp.textInfo.characterCount;

        for (int i = 1; i <= total; i++)
        {
            tmp.maxVisibleCharacters = i;
            char c = tmp.textInfo.characterInfo[i - 1].character;

            if (!char.IsWhiteSpace(c) && audioSource && typeClip)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(typeClip, 0.5f);
            }

            float wait = charInterval;
            if (Punctuation.IndexOf(c) >= 0) wait += punctuationPause;
            yield return new WaitForSeconds(wait);
        }
    }

    void PlaceInFront()
    {
        Vector3 f = cam.forward;
        f.y = 0;
        if (f.sqrMagnitude < 0.0001f) f = Vector3.forward;
        f.Normalize();
        transform.position = cam.position + f * distance + Vector3.up * heightOffset;
        transform.rotation = Quaternion.LookRotation(f);
    }

    // ---------- 生成背景、边框、文字 ----------

    [ContextMenu("Rebuild Dialog (删掉旧的重新生成)")]
    public void Rebuild()
    {
        var old = transform.Find("Visual");
        if (old != null)
        {
            if (Application.isPlaying) Destroy(old.gameObject); else DestroyImmediate(old.gameObject);
        }
        visual = null;
        BuildIfNeeded();
        FitToPages();
        tmp.text = pages.Length > 0 ? pages[pages.Length - 1] : "";
        tmp.maxVisibleCharacters = 99999;
    }

    // 按最长的一页算需要的高度，把背景、边框、文字区域一起加高
    void FitToPages()
    {
        if (tmp == null || visual == null) return;
        float h = size.y;
        if (autoFitHeight)
        {
            float textW = size.x - padding * 2;
            foreach (var p in pages)
            {
                if (string.IsNullOrEmpty(p)) continue;
                float need = tmp.GetPreferredValues(p, textW, float.PositiveInfinity).y;
                h = Mathf.Max(h, need + padding * 2);
            }
        }
        ApplyHeight(h);
    }

    void ApplyHeight(float h)
    {
        float w = size.x, t = frameWidth;
        float topY = size.y / 2;
        float cy = topY - h / 2;

        SetPart("Panel", new Vector3(0, cy, 0.004f), new Vector3(w, h, 1));
        SetPart("Frame_Top", new Vector3(0, topY, 0.002f), new Vector3(w + t, t, 1));
        SetPart("Frame_Bottom", new Vector3(0, cy - h / 2, 0.002f), new Vector3(w + t, t, 1));
        SetPart("Frame_Left", new Vector3(-w / 2, cy, 0.002f), new Vector3(t, h + t, 1));
        SetPart("Frame_Right", new Vector3(w / 2, cy, 0.002f), new Vector3(t, h + t, 1));

        tmp.rectTransform.sizeDelta = new Vector2(w - padding * 2, h - padding * 2);
        tmp.transform.localPosition = new Vector3(0, cy, 0);
    }

    void SetPart(string partName, Vector3 pos, Vector3 scale)
    {
        var part = visual.Find(partName);
        if (part == null) return;
        part.localPosition = pos;
        part.localScale = scale;
    }

    void BuildIfNeeded()
    {
        var existing = transform.Find("Visual");
        if (existing != null)
        {
            visual = existing;
            tmp = visual.GetComponentInChildren<TextMeshPro>(true);
            return;
        }

        visual = new GameObject("Visual").transform;
        visual.SetParent(transform, false);

        if (panelMaterial != null)
        {
            var bg = Quad("Panel", new Vector3(0, 0, 0.004f), new Vector3(size.x, size.y, 1), panelColor);
            float w = size.x, h = size.y, t = frameWidth;
            Quad("Frame_Top", new Vector3(0, h / 2, 0.002f), new Vector3(w + t, t, 1), frameColor);
            Quad("Frame_Bottom", new Vector3(0, -h / 2, 0.002f), new Vector3(w + t, t, 1), frameColor);
            Quad("Frame_Left", new Vector3(-w / 2, 0, 0.002f), new Vector3(t, h + t, 1), frameColor);
            Quad("Frame_Right", new Vector3(w / 2, 0, 0.002f), new Vector3(t, h + t, 1), frameColor);
        }
        else
        {
            Debug.LogWarning("[TypewriterDialog] 没有指定 Panel Material，只显示文字", this);
        }

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(visual, false);
        tmp = textGo.AddComponent<TextMeshPro>();
        tmp.rectTransform.sizeDelta = new Vector2(size.x - padding * 2, size.y - padding * 2);
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.enableAutoSizing = false;
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        if (font != null) tmp.font = font;
        tmp.text = "";
    }

    GameObject Quad(string name, Vector3 pos, Vector3 scale, Color color)
    {
        var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
        q.name = name;
        q.transform.SetParent(visual, false);
        q.transform.localPosition = pos;
        q.transform.localScale = scale;

        var col = q.GetComponent<Collider>();
        if (Application.isPlaying) Destroy(col); else DestroyImmediate(col);

        var mat = new Material(panelMaterial) { name = name + "_Mat" };
        mat.SetFloat("_ShowEye", 2);   // 两只眼都看得到
        mat.SetFloat("_Alpha", 1);
        mat.SetColor("_BaseColor", color);

        var r = q.GetComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        return q;
    }
}
