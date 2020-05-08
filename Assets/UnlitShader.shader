Shader "HE-Arc/Chap6_VertexFragmentShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NbSlices("Nb_Slices", Range(1,30)) = 10
		_Width("Width", Range(0, 1)) = 0.025
		_Color("Color", Color) = (0,0,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			// Tags { "RenderType"="Transparent" "Queue"="Transparent"}

			LOD 100

			Pass
			{
			// Cull Off
			// ZWrite Off
			// Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog


			#include "UnityCG.cginc"

			// *Semantics* => tells how it is supposed to be interpreted in the pipeline
			// - POSITION is the vertex position, typically a float3 or float4.
			// - NORMAL is the vertex normal, typically a float3.
			// - TEXCOORD0 is the first UV coordinate, typically float2, float3 or float4.
			// - TEXCOORD1, TEXCOORD2 and TEXCOORD3 are the 2nd, 3rd and 4th UV coordinates, respectively.
			// - TANGENT is the tangent vector (used for normal mapping), typically a float4.
			// - COLOR is the per-vertex color, typically a float4.
			// SEE ALSO https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION; // Vertex final clip space position
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			int _NbSlices;
			float _Width;
			float4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 defaultColor = fixed4(0,0,0,0.); // Transparent

				// int nbSlices = 10;
				int nbSlices = _NbSlices;

				// float wireframeWidth = 0.025;
				float wireframeWidth = _Width;
				// fixed4 wireframeColor = fixed4(0,0,1,1);
				float4 wireframeColor = _Color;

				// By default, make it transparent
				// NOTE: we must changed Tags
				float4 col = defaultColor;
				float sliceWidth = 1. / nbSlices;

				float posInSliceX = fmod(i.uv.x, sliceWidth);
				if (posInSliceX < wireframeWidth / 2. || posInSliceX >(sliceWidth - wireframeWidth / 2.))
				{
					col = wireframeColor;
				}
				float posInSliceY = fmod(i.uv.y, sliceWidth);
				if (posInSliceY < wireframeWidth / 2. || posInSliceY >(sliceWidth - wireframeWidth / 2.))
				{
					col = wireframeColor;
				}

				return col;
			}
			ENDCG
		}
		}
}