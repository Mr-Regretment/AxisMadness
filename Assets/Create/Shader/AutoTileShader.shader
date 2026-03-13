Shader "Custom/AutoTile3Textures"
{
    Properties
    {
        [HideInInspector] _MainTex ("Unused", 2D) = "white" {}
        _TopTex ("Top", 2D) = "white" {}
        _SideTex ("Sides", 2D) = "white" {}
        _BottomTex ("Bottom", 2D) = "white" {}
        
        _TileScale ("Tile Scale", Range(0.1, 5)) = 1
        _MaxSideTiles ("Max Side Tiles", Range(1, 10)) = 3
        _SideYOffset ("Side Y Offset", Range(-5, 5)) = 0
        _TopThreshold ("Top Face Threshold", Range(0, 1)) = 0.9
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _TopTex;
        sampler2D _SideTex;
        sampler2D _BottomTex;
        
        float _TileScale;
        float _MaxSideTiles;
        float _SideYOffset;
        float _TopThreshold;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 normal = normalize(IN.worldNormal);
            fixed4 col;
            
            if (abs(normal.y) > _TopThreshold)
            {
                float2 uv = frac(IN.worldPos.xz * _TileScale);
                col = (normal.y > 0) ? tex2D(_TopTex, uv) : tex2D(_BottomTex, uv);
            }
            else
            {
                float h = (abs(normal.z) > abs(normal.x)) ? IN.worldPos.x : IN.worldPos.z;
                float tiledY = (IN.worldPos.y + _SideYOffset) * _TileScale;
                
                if (tiledY > _MaxSideTiles)
                {
                    float2 uv = frac(float2(h * _TileScale, tiledY));
                    col = tex2D(_TopTex, uv);
                }
                else
                {
                    tiledY = clamp(tiledY, 0, _MaxSideTiles);
                    float2 uv = float2(frac(h * _TileScale), frac(tiledY));
                    col = tex2D(_SideTex, uv);
                }
            }
            
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}