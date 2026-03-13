Shader "Custom/AdaptiveText"
{
    Properties
    {
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _MainTex ("Font Atlas", 2D) = "white" {}
        _GrabTexture ("Background", 2D) = "white" {}
        _LuminanceThreshold ("Luminance Threshold", Range(0,1)) = 0.5
        _LightColor ("Light Background Color", Color) = (0,0,0,1)
        _DarkColor ("Dark Background Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        GrabPass { "_GrabTexture" }

        Pass
        {
            ZTest [unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            fixed4 _FaceColor;
            float _LuminanceThreshold;
            fixed4 _LightColor;
            fixed4 _DarkColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                fixed4 color : COLOR;
            };

            float GetLuminance(fixed3 c)
            {
                return dot(c, fixed3(0.2126, 0.7152, 0.0722));
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.color = v.color * _FaceColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texSample = tex2D(_MainTex, i.uv);
                float alpha = texSample.a * i.color.a;

                fixed4 background = tex2Dproj(_GrabTexture, i.grabPos);
                float lum = GetLuminance(background.rgb);

                fixed4 textColor = lum > _LuminanceThreshold ? _LightColor : _DarkColor;

                return fixed4(textColor.rgb, alpha);
            }
            ENDCG
        }
    }
}