// 同一个面片：左眼显示 _LeftTex，右眼显示 _RightTex
// 适用：URP + Quest（Multiview / Single Pass Instanced），也兼容 Multi-pass
Shader "Custom/PerEyeTexture"
{
    Properties
    {
        _LeftTex  ("Left Eye Texture", 2D) = "white" {}
        _RightTex ("Right Eye Texture", 2D) = "white" {}
        _LeftColor  ("Left Eye Tint", Color) = (1,1,1,1)
        _RightColor ("Right Eye Tint", Color) = (1,1,1,1)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Cull [_Cull]

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_LeftTex);  SAMPLER(sampler_LeftTex);
            TEXTURE2D(_RightTex); SAMPLER(sampler_RightTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _LeftTex_ST;
                float4 _RightTex_ST;
                half4  _LeftColor;
                half4  _RightColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                ZERO_INITIALIZE(Varyings, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                if (unity_StereoEyeIndex == 0)
                {
                    float2 uv = i.uv * _LeftTex_ST.xy + _LeftTex_ST.zw;
                    return SAMPLE_TEXTURE2D(_LeftTex, sampler_LeftTex, uv) * _LeftColor;
                }
                else
                {
                    float2 uv = i.uv * _RightTex_ST.xy + _RightTex_ST.zw;
                    return SAMPLE_TEXTURE2D(_RightTex, sampler_RightTex, uv) * _RightColor;
                }
            }
            ENDHLSL
        }
    }
}
