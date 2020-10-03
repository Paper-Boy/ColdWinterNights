// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "multiply2D"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
        _Stencil("Stencil ID", Float) = 0
        _ColorMask("Color Mask", Float) = 15
        _ClipAlpha("ClipAlpha", Float) = 0.2
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp NotEqual
                Pass Replace
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    half2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                };

                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                fixed _ClipAlpha;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.worldPosition = IN.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = IN.texcoord;

                    #ifdef UNITY_HALF_TEXEL_OFFSET
                    OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1,1);
                    #endif

                    OUT.color = IN.color * _Color;
                    return OUT;
                }

                sampler2D _MainTex;

                fixed4 frag(v2f IN) : SV_Target
                {
                    half4 color = (tex2D(_MainTex, IN.texcoord));
                    color.rgb = _Color.rgb;
                    color.a *= _Color.a;
                    clip(color.a - _ClipAlpha);
                    return color;
                }
            ENDCG
            }
        }
}