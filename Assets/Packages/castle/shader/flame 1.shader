// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "TripleBrick/flame" {
    Properties {
        _Utils_map ("Utils_map", 2D) = "white" {}
        _flame_basecolor ("flame_basecolor", 2D) = "white" {}
        _emission ("emission", Float ) = 3
        _flicker ("flicker", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _Utils_map; uniform float4 _Utils_map_ST;
            uniform sampler2D _flame_basecolor; uniform float4 _flame_basecolor_ST;
            uniform float _emission;
            uniform float _flicker;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_7068 = _Time + _TimeEditor;
                float2 node_3573 = (o.uv0+node_7068.g*float2(0.5,0.3));
                float4 _Utils_map_var = tex2Dlod(_Utils_map,float4(TRANSFORM_TEX(node_3573, _Utils_map),0.0,0));
                v.vertex.xyz += ((o.uv0.g*1.5)*(sin(((_flicker*_Utils_map_var.rgb)*1.0))*float3(1,3,1)));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float4 _flame_basecolor_var = tex2D(_flame_basecolor,TRANSFORM_TEX(i.uv0, _flame_basecolor));
                float3 emissive = (_flame_basecolor_var.rgb*(_emission*_flame_basecolor_var.a));
                float3 finalColor = emissive;
                return fixed4(finalColor,0.75);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _Utils_map; uniform float4 _Utils_map_ST;
            uniform float _flicker;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4 node_8061 = _Time + _TimeEditor;
                float2 node_3573 = (o.uv0+node_8061.g*float2(0.5,0.3));
                float4 _Utils_map_var = tex2Dlod(_Utils_map,float4(TRANSFORM_TEX(node_3573, _Utils_map),0.0,0));
                v.vertex.xyz += ((o.uv0.g*1.5)*(sin(((_flicker*_Utils_map_var.rgb)*1.0))*float3(1,3,1)));
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    
}
