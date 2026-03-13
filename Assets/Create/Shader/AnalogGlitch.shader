Shader "Kino/Glitch/Analog"
{
    Properties
    {
        _MainTex ("-", 2D) = "" {}
    }
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    float2 _ScanLineJitter;
    float2 _VerticalJump;
    float _HorizontalShake;
    float2 _ColorDrift;

    float nrand(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }

    half4 frag(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;

        float jitter = nrand(uv.y, _Time.x) * 2 - 1;
        jitter *= step(_ScanLineJitter.y, abs(jitter)) * _ScanLineJitter.x;

        float jump = lerp(uv.y, frac(uv.y + _VerticalJump.y), _VerticalJump.x);
        float shake = (nrand(_Time.x, 2) - 0.5) * _HorizontalShake;
        float drift = sin(jump + _ColorDrift.y) * _ColorDrift.x;

        half4 src1 = tex2D(_MainTex, frac(float2(uv.x + jitter + shake, jump)));
        half4 src2 = tex2D(_MainTex, frac(float2(uv.x + jitter + shake + drift, jump)));

        return half4(src1.r, src2.g, src1.b, 1);
    }

    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}