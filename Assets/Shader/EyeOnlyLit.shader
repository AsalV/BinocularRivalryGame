// 有明暗、但不受阴影影响的“只给某一只眼看”材质
// 明暗 = 环境光 + 场景主光源（不算阴影）+ 跟着视线的“头灯”，保证褶皱、面片在任何角度都看得清
// _ShowEye: 0 = 只给左眼, 1 = 只给右眼, 2 = 两眼都看到
// 适用：URP + Quest（Multiview / Single Pass Instanced）
// 不投射阴影、不接收阴影（投影子会让另一只眼通过影子“看到”它）
Shader "Custom/EyeOnlyLit"
{
    Properties
    {
        _BaseMap ("Albedo", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1,1,1,1)
        _AmbientStrength ("Ambient Strength", Range(0,3)) = 1
        _LightStrength ("Main Light Strength", Range(0,3)) = 0.8
        _HeadlightStrength ("Headlight Strength", Range(0,2)) = 0.5
        [HDR] _EmissionColor ("Emission", Color) = (0,0,0,1)
        [Enum(Left,0,Right,1,Both,2)] _ShowEye ("Show To Eye", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Cull [_Cull]

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4  _BaseColor;
            half   _AmbientStrength;
            half   _LightStrength;
            half   _HeadlightStrength;
            half4  _EmissionColor;
            float  _ShowEye;
            float  _Cull;
        CBUFFER_END

        void EyeClip()
        {
            int showEye = (int)_ShowEye;
            if (showEye != 2 && (int)unity_StereoEyeIndex != showEye)
                discard;
        }
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                half   fogFactor  : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                ZERO_INITIALIZE(Varyings, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                VertexPositionInputs pos = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = pos.positionCS;
                o.positionWS = pos.positionWS;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.fogFactor = ComputeFogFactor(pos.positionCS.z);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                EyeClip();

                half3 albedo = (SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _BaseColor).rgb;
                half3 n = normalize(i.normalWS);

                // 1) 环境光（Lighting 窗口里的 Environment Lighting）
                half3 lighting = SampleSH(n) * _AmbientStrength;

                // 2) 场景主光源（方向光），不算阴影
                Light mainLight = GetMainLight();
                lighting += mainLight.color * saturate(dot(n, mainLight.direction)) * _LightStrength;

                // 3) 头灯：正对视线的面更亮，保证褶皱从哪个角度都看得出来
                half3 viewDir = GetWorldSpaceNormalizeViewDir(i.positionWS);
                lighting += saturate(dot(n, viewDir)) * _HeadlightStrength;

                half3 color = albedo * lighting + _EmissionColor.rgb;
                color = MixFog(color, i.fogFactor);
                return half4(color, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            ZWrite On
            ColorMask R

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

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

            half frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                EyeClip();
                return 0;
            }
            ENDHLSL
        }
    }
}
