// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
Shader "Unlit/ActiveCamoUnlitSimple"
{
    Properties
    {
        _DistortTex("Distortion", 2D) = "grey" {}
        [Tooltip(X and Y control the scaling of the _DistortTex. Z and W control the speed at which we scroll along the X and Y respectively.)]
            _DistortTexTiling("Distortion Tiling", Vector) = (2,2,0.05,0.05)
        [Tooltip(How strong is the distortion. 0 is no distortion 1 applies the full _DistortTex.)]
            _DistortAmount ("Distortion Amount", Range(0,1)) = 0.05
        [Tooltip(How extreme is the effect of the distortion (The pull of the text on the surrounding environment). Inverted from what you would expect (1 is weak while 0 is strong).)]
            _VertDistortAmount ("Vert Distortion Amount", Range(0,1)) = 0.025 // 

        _PassiveMimicryRamp("Passive Mimicry Ramp", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Offset -1,-1
            Blend One OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _DistortTex;
            float4 _DistortTexTiling;
            float _DistortAmount;
            float _VertDistortAmount;

            // Per Instance Variables.
            float _PassiveMimicryRamp;

            // Global Variables.
            sampler2D _LastFrame;
            float _GlobalPassiveMimicry;


            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float2 screenNormal : TEXCOORD2;
            };


            v2f vert (appdata_full v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.screenPos = ComputeScreenPos(o.vertex);
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.screenNormal = mul((float3x3)UNITY_MATRIX_V, worldNormal).xy;

                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Get the distortion for the previous frame coords.
                half2 distortion = tex2D(_DistortTex, IN.uv.xy * _DistortTexTiling.xy + _Time.yy * _DistortTexTiling.zw).xy;
                distortion -= tex2D(_DistortTex, IN.uv.xy * _DistortTexTiling.xy + _Time.yy * _DistortTexTiling.zw).yz;

                // Get the last frame to use for the mimicry effect.
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                screenUV += distortion * _DistortAmount * 0.1;
                screenUV += IN.screenNormal * _VertDistortAmount * 0.1;
                half3 lastFrame = tex2D(_LastFrame, screenUV).xyz;

                // The strength of mimicry to apply.
                half passiveMimicryStrength = _PassiveMimicryRamp * _GlobalPassiveMimicry;

                // Premultiplied Passive Mimicry (Premultiplied = Alpha included in colour).
                half4 final = half4(lastFrame * passiveMimicryStrength, passiveMimicryStrength);
                final.w = saturate(final.w);

                return final;
            }
            ENDCG
        }
    }
}
