Shader "OutlinesPack/EdgeDetectionOutlines"
{
	Properties
	{
		// [Toggle(DEBUG)]DEBUG("DEBUG", Float) = 0
			 [Header(Main Settings)]
			 [Space]
             _Thickness("_Thickness",Range(0,5)) = 0.83
			 _Color("_Color",Color) = (0,0,0,1)

			 [Header(Depth)]
			 [Space]
			 [Toggle(ONLY_DEPTH_VIEW)]ONLY_DEPTH_VIEW("See only depth edges", Float) = 0
			 _DepthThreshold("_DepthThreshold",Range(.2,40)) = 0.91

			 [Header(Normal)]
			 [Space]
			 [Toggle(ONLY_NORMAL_VIEW)]ONLY_NORMAL_VIEW("See only normal edges", Float) = 0
			 _DepthNormalThreshold("_DepthNormalThreshold",float) = 11
			 _NormalThreshold("_NormalThreshold",Range(0,3)) = 0.44

	}


	 SubShader
    {
		Cull Off
		ZWrite Off
		ZTest LEqual //Always
		Lighting Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

			#pragma shader_feature ONLY_DEPTH_VIEW
			#pragma shader_feature ONLY_NORMAL_VIEW

			TEXTURE2D(_NormalsDepthTex);
			SAMPLER(sampler_NormalsDepthTex);

			TEXTURE2D(_CameraColorTexture);
			SAMPLER(sampler_CameraColorTexture);

		CBUFFER_START(UnityPerMaterial)
			float4 _CameraColorTexture_TexelSize;
			
			float _Thickness;
			float4 _Color;

			float _DepthThreshold;
			float _DepthNormalThreshold;
			float _NormalThreshold;
		CBUFFER_END

			 struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 viewSpaceDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
				VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
				//output.positionCS = positionInputs.positionCS; // Clip Space
				//output.positionWS = positionInputs.positionWS; // World Space
				o.viewSpaceDir = positionInputs.positionVS; // View Space
                return o;
            }

			float invLerp(float from, float to, float value){
			  return (value - from) / (to - from);
			}

            float4 frag (v2f input) : SV_Target
            {
				float2 bottomLeftUV = input.uv - float2(_CameraColorTexture_TexelSize.x, _CameraColorTexture_TexelSize.y) * _Thickness;
				float2 topRightUV = input.uv + float2(_CameraColorTexture_TexelSize.x, _CameraColorTexture_TexelSize.y) * _Thickness;  
				float2 bottomRightUV = input.uv + float2(_CameraColorTexture_TexelSize.x * _Thickness, -_CameraColorTexture_TexelSize.y * _Thickness);
				float2 topLeftUV = input.uv + float2(-_CameraColorTexture_TexelSize.x * _Thickness, _CameraColorTexture_TexelSize.y * _Thickness);

				float3 normal0 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, bottomLeftUV).rgb;
				float3 normal1 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, topRightUV).rgb;
				float3 normal2 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, bottomRightUV).rgb;
				float3 normal3 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, topLeftUV).rgb;

				float depth0 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, bottomLeftUV).a;
				float depth1 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, topRightUV).a;
				float depth2 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, bottomRightUV).a;
				float depth3 = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, topLeftUV).a;

				// Transform the view normal from the 0...1 range to the -1...1 range.
				float3 viewNormal = normal0 * 2 - 1;
				float NdotV = 1 - dot(viewNormal, -input.viewSpaceDir);

				// Return a value in the 0...1 range depending on where NdotV lies 
				// between _DepthNormalThreshold and 1.
				float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
				// Scale the threshold, and add 1 so that it is in the range of 1..._NormalThresholdScale + 1.
				float normalThreshold = normalThreshold01 + 1;

				// Modulate the threshold by the existing depth value;
				// pixels further from the screen will require smaller differences
				// to draw an edge.
				float depthThreshold = _DepthThreshold * depth0 * normalThreshold;

				float depthFiniteDifference0 = depth1 - depth0;
				float depthFiniteDifference1 = depth3 - depth2;
				// edgeDepth is calculated using the Roberts cross operator.
				// The same operation is applied to the normal below.
				// https://en.wikipedia.org/wiki/Roberts_cross
				float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
				edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

				float3 normalFiniteDifference0 = normal1 - normal0;
				float3 normalFiniteDifference1 = normal3 - normal2;
				// Dot the finite differences with themselves to transform the 
				// three-dimensional values to scalars.
				float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
				edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;

				float edge = max(edgeDepth, edgeNormal);

				//final colors blend
				//float4 edgeColor = float4(_Color.rgb* edge, _Color.a * edge);
				float4 color = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, input.uv);

				#if ONLY_DEPTH_VIEW
				return edgeDepth;
				#elif ONLY_NORMAL_VIEW
				return edgeNormal;
				#endif

				return lerp(color,_Color,edge * _Color.a);

                //if (edgeColor.a > 0) {
                //    return edgeColor;
                //} else {
                //    return color;
                //}

            }
            ENDHLSL
        }
    }
}
