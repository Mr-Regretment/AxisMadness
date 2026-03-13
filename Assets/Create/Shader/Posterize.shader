Shader "Custom/Posterize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Steps ("Colour Steps", Range(1, 32)) = 8
        _Strength ("Strength", Range(0, 1)) = 0.5
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
            float _Steps;
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
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float posterized = floor(grey * _Steps) / _Steps;
                float ratio = posterized / max(grey, 0.0001);
                fixed3 posterizedCol = col.rgb * ratio;
                
                col.rgb = lerp(col.rgb, posterizedCol, _Strength);
                
                return col;
            }
            ENDCG
        }
    }
}