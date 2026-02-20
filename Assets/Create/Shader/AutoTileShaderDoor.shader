Shader "Custom/AutoTile3TexturesDoor"
{
    Properties
    {
        [HideInInspector] _MainTex ("Unused", 2D) = "white" {}
        _TopTex ("Top", 2D) = "white" {}
        _SideTex ("Sides", 2D) = "white" {}
        _BottomTex ("Bottom", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
        _UVScale ("UV Scale", Range(0.01, 5)) = 1
        _SideYOffset ("Side Y Offset", Range(0, 1)) = 0.5
        _SideBottomThreshold ("Side/Bottom Threshold", Range(-5, 5)) = 0
        _TopThreshold ("Top Face Threshold", Range(0, 1)) = 0.9
    }
        SubShader
        {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        ZWrite On

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _TopTex;
        sampler2D _SideTex;
        sampler2D _BottomTex;
        float _Alpha;
        float _UVScale;
        float _SideYOffset;
        float _SideBottomThreshold;
        float _TopThreshold;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 scale = float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
            );

            float3 normal = normalize(IN.worldNormal);
            float2 tiledUV = IN.uv_MainTex * float2(scale.x, scale.y) * _UVScale;
            float2 fractUV = frac(tiledUV);
            
            float3 localPos = mul(unity_WorldToObject, float4(IN.worldPos, 1)).xyz;
            
            fixed4 col;
            
            if (normal.y > _TopThreshold)
                col = tex2D(_TopTex, fractUV);
            else if (normal.y < -_TopThreshold)
                col = tex2D(_BottomTex, fractUV);
            else
            {
                if (localPos.y < _SideBottomThreshold)
                    col = tex2D(_BottomTex, fractUV);
                else
                    col = tex2D(_SideTex, float2(fractUV.x, fractUV.y + _SideYOffset));
            }
            
            o.Albedo = col.rgb;
            o.Alpha = col.a * _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}