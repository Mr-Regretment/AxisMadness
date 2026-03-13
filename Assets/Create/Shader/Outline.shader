Shader "Custom/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _CameraDepthTexture;

            float4 _OutlineColor;
            float _OutlineThickness;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float SampleDepth(float2 uv)
            {
                return tex2D(_CameraDepthTexture, uv).r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy * _OutlineThickness;

                float d0 = SampleDepth(i.uv);
                float d1 = SampleDepth(i.uv + float2(texel.x, 0));
                float d2 = SampleDepth(i.uv - float2(texel.x, 0));
                float d3 = SampleDepth(i.uv + float2(0, texel.y));
                float d4 = SampleDepth(i.uv - float2(0, texel.y));

                float edge = abs(d1 - d0) + abs(d2 - d0) + abs(d3 - d0) + abs(d4 - d0);
                edge = step(0.001, edge);

                fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(col, _OutlineColor, edge);
            }
            ENDCG
        }
    }
}