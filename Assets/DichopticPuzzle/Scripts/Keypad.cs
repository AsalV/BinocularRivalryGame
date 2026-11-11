using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 自动生成一个 3×4 的数字键盘（1–9, C, 0, OK）和上方的显示屏。
/// 键盘面朝本物体的 -Z 方向（蓝色 Z 轴要背对玩家）。
/// 在 Inspector 组件右上角 ⋮ 菜单里点 “Build Keypad Now” 可以在编辑模式下先生成出来。
/// </summary>
public class Keypad : MonoBehaviour
{
    [Header("密码来源")]
    [Tooltip("拖入密码箱上的 DichopticCode，密码自动同步")]
    public DichopticCode codeSource;
    [Tooltip("没有 codeSource 时用这个")]
    public string fallbackCode = "40718253";
    [Tooltip("输满位数后自动提交，不用按 OK")]
    public bool autoSubmit = false;

    [Header("外观")]
    public Material buttonMaterial;
    public float buttonSize = 0.045f;
    public float buttonGap = 0.012f;
    public float buttonDepth = 0.015f;
    public Color textColor = new Color(0.6f, 1f, 1f);
    [Tooltip("按键文字大小（TextMeshPro 3D 字号，0.3 ≈ 2 cm 高）")]
    public float labelFontSize = 0.3f;
    [Tooltip("显示屏文字大小")]
    public float displayFontSize = 0.26f;
    [Tooltip("提交键上显示的字，比如 # 或 E（用普通字符，特殊符号默认字体可能没有）。Icon Material 留空时才用它")]
    public string submitLabel = "#";
    [Tooltip("提交键上画对勾用的材质：拖入 DigitMat（EyeOnlyFade）。留空则显示上面的 Submit Label")]
    public Material iconMaterial;
    public Color okColor = new Color(0.3f, 1f, 0.4f);
    public Color errorColor = new Color(1f, 0.3f, 0.3f);

    [Header("声音（可选）")]
    public AudioClip keyClip;
    public AudioClip successClip;
    public AudioClip errorClip;

    [Header("事件")]
    public UnityEvent onUnlock;
    public UnityEvent onWrongCode;

    static readonly string[] Layout = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "C", "0", "OK" };

    TextMeshPro display;
    string input = "";
    bool unlocked, busy;

    string Code => codeSource != null ? codeSource.code : fallbackCode;

    void Start()
    {
        BuildIfNeeded();
        Refresh();
    }

    [ContextMenu("Rebuild Keypad (删掉旧的重新生成)")]
    public void Rebuild()
    {
        foreach (var n in new[] { "Display", "Buttons" })
        {
            var t = transform.Find(n);
            if (t == null) continue;
            if (Application.isPlaying) Destroy(t.gameObject); else DestroyImmediate(t.gameObject);
        }
        BuildIfNeeded();
    }

    [ContextMenu("Build Keypad Now")]
    public void BuildIfNeeded()
    {
        var existing = transform.Find("Display");
        if (existing != null && transform.Find("Buttons") != null)
        {
            display = existing.GetComponent<TextMeshPro>();
            return;
        }

        float step = buttonSize + buttonGap;
        float gridW = 3 * buttonSize + 2 * buttonGap;
        float front = -buttonDepth / 2 - 0.001f;

        // 显示屏
        var dGo = new GameObject("Display");
        dGo.transform.SetParent(transform, false);
        dGo.transform.localPosition = new Vector3(0, buttonSize + buttonGap, front);
        display = dGo.AddComponent<TextMeshPro>();
        display.rectTransform.sizeDelta = new Vector2(gridW, buttonSize * 0.8f);
        display.alignment = TextAlignmentOptions.Center;
        display.enableAutoSizing = false;
        display.fontSize = displayFontSize;
        display.color = textColor;

        // 按键
        var buttons = new GameObject("Buttons").transform;
        buttons.SetParent(transform, false);

        for (int i = 0; i < Layout.Length; i++)
        {
            int col = i % 3, row = i / 3;
            string key = Layout[i];

            var holder = new GameObject("Key_" + key).transform;
            holder.SetParent(buttons, false);
            holder.localPosition = new Vector3(-gridW / 2 + buttonSize / 2 + col * step, -row * step, 0);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cap";
            cube.transform.SetParent(holder, false);
            cube.transform.localScale = new Vector3(buttonSize, buttonSize, buttonDepth);
            cube.GetComponent<BoxCollider>().isTrigger = true;
            if (buttonMaterial) cube.GetComponent<MeshRenderer>().sharedMaterial = buttonMaterial;

            var kb = cube.AddComponent<KeypadButton>();
            kb.keypad = this;
            kb.key = key;
            kb.moveTarget = holder;

            var label = new GameObject("Label");
            label.transform.SetParent(holder, false);
            label.transform.localPosition = new Vector3(0, 0, front);

            if (key == "OK" && iconMaterial != null)
            {
                BuildCheckMark(label.transform);
                continue;
            }

            var tmp = label.AddComponent<TextMeshPro>();
            string shown = key == "OK" ? submitLabel : key;
            tmp.text = shown;
            tmp.rectTransform.sizeDelta = new Vector2(buttonSize * 1.5f, buttonSize);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = false;
            tmp.fontSize = shown.Length > 1 ? labelFontSize * 0.75f : labelFontSize;
            tmp.color = key == "OK" ? okColor : textColor;
        }
    }

    // 用两根细面片拼一个对勾，不依赖字体
    void BuildCheckMark(Transform parent)
    {
        var mat = new Material(iconMaterial) { name = "CheckMark_Mat" };
        mat.SetFloat("_ShowEye", 2);   // 两只眼都看得到
        mat.SetFloat("_Alpha", 1);
        mat.SetColor("_BaseColor", okColor);

        float s = buttonSize;
        Vector2 a = new Vector2(-0.24f * s, 0.02f * s);
        Vector2 b = new Vector2(-0.07f * s, -0.17f * s);
        Vector2 c = new Vector2(0.26f * s, 0.20f * s);
        float thick = 0.09f * s;

        Stroke(parent, a, b, thick, mat);
        Stroke(parent, b, c, thick, mat);
    }

    static void Stroke(Transform parent, Vector2 from, Vector2 to, float thick, Material mat)
    {
        var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
        q.name = "CheckStroke";
        q.transform.SetParent(parent, false);
        Vector2 d = to - from;
        q.transform.localPosition = (Vector3)((from + to) * 0.5f);
        q.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
        q.transform.localScale = new Vector3(d.magnitude + thick * 0.5f, thick, 1);

        var col = q.GetComponent<Collider>();
        if (Application.isPlaying) Destroy(col); else DestroyImmediate(col);

        var r = q.GetComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void OnKey(string key)
    {
        if (unlocked || busy) return;
        Play(keyClip);

        if (key == "C") input = "";
        else if (key == "OK") { Submit(); return; }
        else if (input.Length < Code.Length) input += key;

        Refresh();
        if (autoSubmit && input.Length == Code.Length) Submit();
    }

    void Submit()
    {
        if (input == Code)
        {
            unlocked = true;
            Play(successClip);
            Show("OPEN", okColor);
            onUnlock?.Invoke();
        }
        else
        {
            StartCoroutine(WrongRoutine());
        }
    }

    IEnumerator WrongRoutine()
    {
        busy = true;
        Play(errorClip);
        Show("ERROR", errorColor);
        onWrongCode?.Invoke();
        yield return new WaitForSeconds(1f);
        input = "";
        busy = false;
        Refresh();
    }

    void Refresh()
    {
        if (unlocked) return;
        Show(input + new string('_', Mathf.Max(0, Code.Length - input.Length)), textColor);
    }

    void Show(string text, Color c)
    {
        if (!display) return;
        display.text = text;
        display.color = c;
    }

    void Play(AudioClip clip)
    {
        if (clip) AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
