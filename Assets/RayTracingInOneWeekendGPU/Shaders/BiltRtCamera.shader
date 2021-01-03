Shader "RayTracingInOneWeekendGPU/BiltRtCamera"
{
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "../Kernels/Structures.cginc"


            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            StructuredBuffer<Ray> _Rays;
            float2 _AccumulatedImageSize;

            
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;
                
                return output;
            }
            
            half4 frag (Varyings input) : SV_Target 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);


                uint rayCount, stride;
                _Rays.GetDimensions(rayCount, stride);


                float2 size = _AccumulatedImageSize;
                int2 xy = input.uv * size;
                float4 color = float4(0, 0, 0, 0);

                for (int z = 0; z < 8; z++) {
                    int rayIndex = xy.x * size.y
                                    + xy.y
                                    + size.x * size.y * (z);

                    color += _Rays[rayIndex % rayCount].accumColor;
                }

                // Note that the blur from the blog post is no longer applied, but could
                // be done here or in the transfer from accumulated image to screen.

                // Normalize by sample count.
                return color / color.a;


            }
            
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}
