Shader "Hidden/Vignette"
{
    Properties
    {
        _MainTex ("-", 2D) = "" {}
        _RedIntensity ("Red Intensity", Range(0, 1)) = 0.3
        _BlueIntensity ("Blue Intensity", Range(0, 1)) = 0.3
        _SideWidth ("Side Width", Range(0, 0.5)) = 0.2
    }
    CGINCLUDE
    
    #include "UnityCG.cginc"
    
    sampler2D _MainTex;
    float _RedIntensity;
    float _BlueIntensity;
    float _SideWidth;
    
    half4 frag(v2f_img i) : SV_Target
    {
        half4 col = tex2D(_MainTex, i.uv);
        
        if (i.uv.x < _SideWidth)
        {
            float leftFade = 1 - (i.uv.x / _SideWidth);
            col.rgb += half3(_RedIntensity * leftFade, 0, 0);
        }
        
        if (i.uv.x > (1 - _SideWidth))
        {
            float rightFade = (i.uv.x - (1 - _SideWidth)) / _SideWidth;
            col.rgb += half3(0, 0, _BlueIntensity * rightFade);
        }
        
        return col;
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
            ENDCG
        }
    }
}