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

        float _TileScale;
        float _TopThreshold;

        struct Input
        {
            float2 uv_MainTex;
            float3 localNormal;
            float3 localPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex.xyz;
            o.localNormal = v.normal.xyz;

            // Bake object scale into localPos so tiling is scale-aware
            float3 scale = float3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22)
            );
            o.localPos *= scale;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
{
    float3 normal = normalize(IN.localNormal);
    fixed4 col;

    if (normal.y > _TopThreshold)
    {
        float2 uv = frac(IN.localPos.xz * _TileScale);
        col = tex2D(_TopTex, uv);
    }
    else if (normal.y < -_TopThreshold)
    {
        float2 uv = frac(IN.localPos.xz * _TileScale);
        col = tex2D(_BottomTex, uv);
    }
    else if (normal.z > 0.5)
    {
        float2 uv = float2(frac(IN.localPos.x * _TileScale), saturate((IN.localPos.y + 0.5)));
        col = tex2D(_FrontTex, uv);
    }
    else if (normal.z < -0.5)
    {
        float2 uv = float2(frac(IN.localPos.x * _TileScale), saturate((IN.localPos.y + 0.5)));
        col = tex2D(_BackTex, uv);
    }
    else if (normal.x > 0.5)
    {
        float2 uv = float2(frac(IN.localPos.z * _TileScale), saturate((IN.localPos.y + 0.5)));
        col = tex2D(_RightTex, uv);
    }
    else
    {
        float2 uv = float2(frac(IN.localPos.z * _TileScale), saturate((IN.localPos.y + 0.5)));
        col = tex2D(_LeftTex, uv);
    }

    o.Albedo = col.rgb;
    o.Alpha = col.a;
}
        ENDCG
    }
    FallBack "Diffuse"
}