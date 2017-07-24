Shader "Custom/BufferShader"
{
	SubShader
	{
		Pass
		{
			ZTest Less
			Cull Back
			ZWrite Off
			BlendOp Add
			Blend SrcAlpha OneMinusSrcAlpha

			Fog	{ Mode off }

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag

			StructuredBuffer<float4> gVertexBuffer;

			// ---

			struct VS_OUTPUT
			{
				float4 svPosition : SV_POSITION;
                float2 uv : UV;
			};

            VS_OUTPUT vert(uint vID : SV_VertexID)
			{
                VS_OUTPUT output;

                float3 vPosition = gVertexBuffer[vID];

                output.svPosition = UnityObjectToClipPos(float4(vPosition, 1));

                vID = vID % 3;
                float x = vID == 0 || vID == 1 || vID == 3;
                float y = vID == 0 || vID == 2 || vID == 5;
                output.uv = float2(x, 1.0f - y);

				return output;
			}

			// ---

			float4 frag(VS_OUTPUT input) : SV_TARGET0
			{
				float x = input.uv.x - 0.5f;
				float y = input.uv.y - 0.5f;
				float r = sqrt(x * x + y * y);
				float factor = max(1.f - r * 2.f, 0.f); //[1,0]

				float cosFactor = -cos(3.14159265f / 2.f * (factor + 1.f));

				return float4(0, 1, 0, cosFactor);
			}

			ENDCG
		}
	}
}
