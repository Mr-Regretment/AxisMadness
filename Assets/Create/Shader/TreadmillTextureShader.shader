Shader "Custom/AutoTile3Textures"
{
    Properties
    {
        [HideInInspector] _MainTex ("Unused", 2D) = "white" {}
        _TopTex ("Top", 2D) = "white" {}
        _BottomTex ("Bottom", 2D) = "white" {}
        _FrontTex ("Front", 2D) = "white" {}
        _BackTex ("Back", 2D) = "white" {}
        _LeftTex ("Left", 2D) = "white" {}
        _RightTex ("Right", 2D) = "white" {}
        
        _TileScale ("Tile Scale", Range(0.1, 5)) = 1
        _TopThreshold ("Top Face Threshold", Range(0, 1)) = 0.9
        
        _ScrollSpeed ("Scroll Speed", Float) = 0.5
        _ScrollTime ("Scroll Time", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _TopTex;
        sampler2D _BottomTex;
        sampler2D _FrontTex;
        sampler2D _BackTex;
        sampler2D _LeftTex;
        sampler2D _RightTex;

        float4 _TopTex_ST;
        float4 _BottomTex_ST;
        float4 _FrontTex_ST;
        float4 _BackTex_ST;
        float4 _LeftTex_ST;
        float4 _RightTex_ST;

        float _TileScale;
        float _TopThreshold;
        float _ScrollSpeed;
        float _ScrollTime;

        struct Input
        {
            float2 uv_MainTex;
            float3 localNormal;
            float3 localPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localNormal = v.normal.xyz;

            float3 scale = float3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22)
            );
            o.localPos = v.vertex.xyz * scale;
        }

    void surf (Input IN, inout SurfaceOutputStandard o)
    {
        float3 normal = normalize(IN.localNormal);
        float scroll = _ScrollTime * _ScrollSpeed;
        fixed4 col;

        if (normal.y > _TopThreshold)
        {
            float2 uv = IN.localPos.xz * _TileScale * _TopTex_ST.xy + _TopTex_ST.zw + float2(0, scroll);
            col = tex2D(_TopTex, uv);
        }
        else if (normal.y < -_TopThreshold)
        {
            float2 uv = IN.localPos.xz * _TileScale * _BottomTex_ST.xy + _BottomTex_ST.zw;
            col = tex2D(_BottomTex, uv);
        }
        else if (normal.z > 0.5)
        {
            float2 uv = float2(IN.localPos.x * _TileScale, saturate(IN.localPos.y + 0.5)) * _FrontTex_ST.xy + _FrontTex_ST.zw;
            col = tex2D(_FrontTex, uv);
        }
        else if (normal.z < -0.5)
        {
            float2 uv = float2(IN.localPos.x * _TileScale, saturate(IN.localPos.y + 0.5)) * _BackTex_ST.xy + _BackTex_ST.zw;
            col = tex2D(_BackTex, uv);
        }
        else if (normal.x > 0.5)
        {
            float2 uv = float2(IN.localPos.z * _TileScale + scroll, saturate(IN.localPos.y + 0.5)) * _RightTex_ST.xy + _RightTex_ST.zw;
            col = tex2D(_RightTex, uv);
        }
        else
        {
            float2 uv = float2(IN.localPos.z * _TileScale + scroll, saturate(IN.localPos.y + 0.5)) * _LeftTex_ST.xy + _LeftTex_ST.zw;
            col = tex2D(_LeftTex, uv);
        }

        o.Albedo = col.rgb;
        o.Alpha = col.a;
    }
        ENDCG
    }
    FallBack "Diffuse"
}