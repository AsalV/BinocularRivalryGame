using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 用 7 个面片拼出一个数码管数字（0–9）。不依赖字体。
/// 数字面朝物体的 -Z 方向（玩家应站在 -Z 一侧看）。
/// </summary>
public static class SevenSegment
{
    // 段顺序：a(上) b(右上) c(右下) d(下) e(左下) f(左上) g(中)
    static readonly bool[][] Map =
    {
        new[] { true,  true,  true,  true,  true,  true,  false }, // 0
        new[] { false, true,  true,  false, false, false, false }, // 1
        new[] { true,  true,  false, true,  true,  false, true  }, // 2
        new[] { true,  true,  true,  true,  false, false, true  }, // 3
        new[] { false, true,  true,  false, false, true,  true  }, // 4
        new[] { true,  false, true,  true,  false, true,  true  }, // 5
        new[] { true,  false, true,  true,  true,  true,  true  }, // 6
        new[] { true,  true,  true,  false, false, false, false }, // 7
        new[] { true,  true,  true,  true,  true,  true,  true  }, // 8
        new[] { true,  true,  true,  true,  false, true,  true  }, // 9
    };

    static readonly bool[] Horizontal = { true, false, false, true, false, false, true };

    public static GameObject Build(char ch, Transform parent, Material mat, float width, float height, float thickness)
    {
        int n = ch - '0';
        var root = new GameObject("Digit_" + ch);
        root.transform.SetParent(parent, false);
        if (n < 0 || n > 9) return root;

        float w = width, h = height, t = thickness;
        Vector2[] pos =
        {
            new Vector2(0,      h / 2),   // a
            new Vector2(w / 2,  h / 4),   // b
            new Vector2(w / 2, -h / 4),   // c
            new Vector2(0,     -h / 2),   // d
            new Vector2(-w / 2,-h / 4),   // e
            new Vector2(-w / 2, h / 4),   // f
            new Vector2(0,      0),       // g
        };

        for (int i = 0; i < 7; i++)
        {
            if (!Map[n][i]) continue;

            var seg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            seg.name = "Seg_" + "abcdefg"[i];
            seg.transform.SetParent(root.transform, false);
            seg.transform.localPosition = new Vector3(pos[i].x, pos[i].y, 0);
            seg.transform.localScale = Horizontal[i]
                ? new Vector3(w - t, t, 1)
                : new Vector3(t, h / 2 - t, 1);

            var col = seg.GetComponent<Collider>();
            if (Application.isPlaying) Object.Destroy(col); else Object.DestroyImmediate(col);

            var r = seg.GetComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
        }
        return root;
    }
}
