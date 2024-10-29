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
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            //Offset -1,-1
            //Blend One OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            sampler2D _DistortTex;
            float4 _DistortTexTiling;
            float _DistortAmount;
            float _VertDistortAmount;

            // Per Instance Variables.
            float _PassiveMimicryRamp;
            CBUFFER_END

            CBUFFER_START(GlobalMimicryMaterials)
            // Global Variables.
            sampler2D _LastFrame;
            float _GlobalPassiveMimicry;
            CBUFFER_END


            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float2 screenNormal : TEXCOORD2;
            };

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 tangentOS : TANGENT;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;

                //float4 texcoord1 : TEXCOORD1;
                //float4 texcoord2 : TEXCOORD2;
                //float4 texcoord3 : TEXCOORD3;
                //fixed4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                //OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                //fixed3 worldNormal = UnityObjectToWorldNormal(IN.normal);
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                float3 worldNormal = vertexNormalInput.normalWS;
                OUT.screenNormal = mul((float3x3)UNITY_MATRIX_V, worldNormal).xy;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
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
            ENDHLSL
        }
    }
}
