using System.Collections;
using UnityEngine;

/// <summary>
/// 开门。挂在门上，把 Keypad 的 On Unlock 事件连到 Open()。
///  - Slide（滑动）：按世界坐标移动 slideOffset，适合左右推拉门（左右两扇各挂一个，方向相反）
///  - Rotate（旋转）：绕物体自身轴心旋转，轴心要在门轴处
/// Play 模式下可以在组件 ⋮ 菜单里点 “Test Open” 试一下。
/// </summary>
public class DoorOpener : MonoBehaviour
{
    public enum Mode { Slide, Rotate }

    public Mode mode = Mode.Slide;
    [Tooltip("滑动模式：世界坐标下移动多少（米）。左门填 (-0.6,0,0)，右门填 (0.6,0,0)，方向不对就把正负号反过来")]
    public Vector3 slideOffset = new Vector3(0.6f, 0, 0);
    [Tooltip("旋转模式：在本地坐标下转多少度")]
    public Vector3 rotateAngles = new Vector3(0, 90, 0);
    public float duration = 1.2f;
    public AudioClip openClip;

    bool opened;

    public void Open()
    {
        if (opened) return;
        opened = true;
        if (openClip) AudioSource.PlayClipAtPoint(openClip, transform.position);
        StartCoroutine(Run());
    }

    [ContextMenu("Test Open (Play 模式下)")]
    void TestOpen() => Open();

    IEnumerator Run()
    {
        Vector3 p0 = transform.position;
        Vector3 p1 = p0 + slideOffset;
        Quaternion r0 = transform.localRotation;
        Quaternion r1 = r0 * Quaternion.Euler(rotateAngles);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            if (mode == Mode.Slide) transform.position = Vector3.Lerp(p0, p1, k);
            else transform.localRotation = Quaternion.Slerp(r0, r1, k);
            yield return null;
        }
    }
}
