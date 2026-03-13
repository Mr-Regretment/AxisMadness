Shader "Custom/ColourGrading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Saturation ("Saturation", Range(0, 3)) = 1.5
        _Contrast ("Contrast", Range(0, 3)) = 1.2
        _Brightness ("Brightness", Range(0, 2)) = 1.0
        _Strength ("Strength", Range(0, 1)) = 1.0
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
            float _Saturation;
            float _Contrast;
            float _Brightness;
            float _Strength;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv);
                fixed4 col = original;

                col.rgb *= _Brightness;

                col.rgb = (col.rgb - 0.5) * _Contrast + 0.5;

                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(float3(grey, grey, grey), col.rgb, _Saturation);

                col.rgb = lerp(original.rgb, col.rgb, _Strength);

                return col;
            }
            ENDCG
        }
    }
}