using System.Collections;
using UnityEngine;

/// <summary>
/// 一个可按的按键。两种触发方式：
///  1) 带 KeypadPresser 的物体（手柄 / 指尖）碰到它
///  2) 外部调用 Press()（比如 Interaction SDK 的 Poke 事件）
/// </summary>
public class KeypadButton : MonoBehaviour
{
    public Keypad keypad;
    public string key;
    [Tooltip("按下时移动的物体（一般是按键的父物体）")]
    public Transform moveTarget;
    public float pressDepth = 0.006f;
    [Tooltip("防止一次碰触触发多次")]
    public float repeatDelay = 0.35f;

    float lastPress = -10f;
    Vector3 rest;
    bool restSaved;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<KeypadPresser>() != null) Press();
    }

    public void Press()
    {
        if (Time.time - lastPress < repeatDelay) return;
        lastPress = Time.time;

        if (keypad) keypad.OnKey(key);

        if (moveTarget)
        {
            StopAllCoroutines();
            StartCoroutine(Bump());
        }
    }

    IEnumerator Bump()
    {
        if (!restSaved) { rest = moveTarget.localPosition; restSaved = true; }
        moveTarget.localPosition = rest + Vector3.forward * pressDepth;
        yield return new WaitForSeconds(0.12f);
        moveTarget.localPosition = rest;
    }
}
