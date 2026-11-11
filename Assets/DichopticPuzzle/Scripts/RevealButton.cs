using System.Collections;
using UnityEngine;

/// <summary>
/// 按下去就启用“双眼能力”的按钮。挂在按钮帽上（帽子要有 Collider 并勾 Is Trigger）。
/// 带 KeypadPresser 的手 / 手柄碰到它就触发；也可以用 Interaction SDK 的 Poke 事件调用 Press()。
/// </summary>
public class RevealButton : MonoBehaviour
{
    public DichopticCode target;
    [Tooltip("按下时移动的物体，一般就是按钮帽自己")]
    public Transform moveTarget;
    [Tooltip("按下的方向和距离（世界坐标）。按钮朝上放就是向下")]
    public Vector3 pressOffset = new Vector3(0, -0.015f, 0);

    Vector3 rest;
    bool restSaved;

    void Reset()
    {
        moveTarget = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<KeypadPresser>() != null) Press();
    }

    public void Press()
    {
        if (target == null) return;
        if (target.TryActivate() && moveTarget) StartCoroutine(Bump());
    }

    IEnumerator Bump()
    {
        if (!restSaved) { rest = moveTarget.position; restSaved = true; }
        moveTarget.position = rest + pressOffset;
        yield return new WaitForSeconds(0.25f);
        moveTarget.position = rest;
    }
}
