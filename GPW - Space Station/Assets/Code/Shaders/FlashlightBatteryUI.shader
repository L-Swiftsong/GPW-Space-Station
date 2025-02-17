Shader "Custom/FlashlightBatteryUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurveStrength ("Curve Strength", Range(-1, 1))  = 0.1
        _CurveRadius ("Curve Radius", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _CurveStrength;
            float _CurveRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                
                float2 center = float2(0.5, 0.5);
                float2 uvOffset = v.uv - center;

                float dist = length(uvOffset);
                float curveFactor = pow(dist, 2.0) * _CurveStrength;

                uvOffset = normalize(uvOffset) * (1.0 + curveFactor * _CurveRadius);

                o.uv = center + uvOffset;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}

