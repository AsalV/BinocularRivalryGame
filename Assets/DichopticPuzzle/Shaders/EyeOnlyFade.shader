// 只给某一只眼看的半透明材质，可以用 _Alpha 淡入淡出
// _ShowEye: 0 = 只给左眼, 1 = 只给右眼, 2 = 两眼都看到
// 适用：URP + Quest（Multiview / Single Pass Instanced）
Shader "Custom/EyeOnlyFade"
{
    Properties
    {
        _BaseColor ("Color", Color) = (0.3, 1, 1, 1)
        _Alpha ("Alpha", Range(0, 1)) = 1
        [Enum(Left,0,Right,1,Both,2)] _ShowEye ("Show To Eye", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull [_Cull]

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Alpha;
                float _ShowEye;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                ZERO_INITIALIZE(Varyings, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                int showEye = (int)_ShowEye;
                if (showEye != 2 && (int)unity_StereoEyeIndex != showEye)
                    discard;              // 不是目标眼睛就不画
                if (_Alpha <= 0.001)
                    discard;              // 完全透明时直接跳过

                return half4(_BaseColor.rgb, _BaseColor.a * _Alpha);
            }
            ENDHLSL
        }
    }
}
