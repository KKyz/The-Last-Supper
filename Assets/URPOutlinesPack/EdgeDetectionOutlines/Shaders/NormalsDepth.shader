Shader "OutlinesPack/NormalsDepth"
{
    Properties
    {
        //_UseDepthMask("_UseDepthMask",float) =1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"  }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
             
             TEXTURE2D(_SceneMask_Edge);
		     SAMPLER(sampler_SceneMask_Edge);

             float _UseDepthMask;

            struct Attributes
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
                float3 normal       : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
                float3 viewNormal   : NORMAL;
                float4 screenPos    : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.position.xyz);

                output.uv = input.texcoord;
                output.positionCS = positionInputs.positionCS;
                output.viewNormal = input.normal;
                output.screenPos = positionInputs.positionNDC;
                return output;
            }

            float4 frag(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                if(_UseDepthMask)
                {
                    float4 mask = SAMPLE_TEXTURE2D(_SceneMask_Edge, sampler_SceneMask_Edge, input.screenPos.xy / input.screenPos.w);

                    if (mask.r > input.positionCS.z)
			        {
			    	    return float4(0,0,0,1);
			        }
                }

                return float4(normalize(input.viewNormal) , input.positionCS.z);
             }

        ENDHLSL

        Pass
        {
            Name "DepthNormals"

            ZWrite Off
	        ZTest Never

            HLSLPROGRAM
	        #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}
