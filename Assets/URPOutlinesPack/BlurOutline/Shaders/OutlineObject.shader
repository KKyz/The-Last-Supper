Shader "OutlinesPack/OutlineObject"
{
   HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);

		TEXTURE2D(_SceneMask_Blur);
		SAMPLER(sampler_SceneMask_Blur);

        float _DrawBehind;

		struct Attributes
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
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
                output.screenPos = positionInputs.positionNDC;
                return output;
            }

		 half4 frag(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                //We calculate blur from B channel - combine
			    //Behind R channel
			    //Front G channel

                float4 mask = SAMPLE_TEXTURE2D(_SceneMask_Blur, sampler_SceneMask_Blur, input.screenPos.xy / input.screenPos.w);

                if(_DrawBehind)
                {
                    if (mask.r > input.positionCS.z)
			        {
			    	    return float4(1,0,0,input.positionCS.z);//Front 0 0 0 0
			        }
                }
                else
                {
                    if (mask.r > input.positionCS.z)
			        {
			    	    return float4(1,0,1,input.positionCS.z);//Front 0 0 0 0
			        }
                }
                
                return float4(0,1,1,input.positionCS.z);
             }

	ENDHLSL

	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }

		Cull Off
		//ZWrite On
		ZTest LEqual
		Lighting Off

		Pass
		{
			Name "ObjetsRender"

			 HLSLPROGRAM
	        #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
		}
	}
}
