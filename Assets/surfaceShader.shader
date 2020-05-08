Shader "Ciconia Studio/Double Sided/Emissive/Diffuse"
{
	Properties
	{
		_Color0("Color 0", Color) = (0.5, 0.5, 0.5, 1.0)
		_Color1("Color 1", Color) = (0.0, 0.0, 0.0, 1.0)
		_Size("Density", Range(2, 50)) = 8
	}
	SubShader
	{
		Pass
		{
			Name "FORWARD"
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color0;
			float4 _Color1;
			float _Size;

			struct v2f 
			{
				float2 uv:TEXCOORD0;
				float4 vertex:SV_POSITION;
			};
			v2f vert(float4 pos:POSITION, float2 uv : TEXCOORD0) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(pos);
				o.uv = _Size * uv;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				float2 c = floor(i.uv) / 2;
				float checker = 2 * frac(c.x + c.y);
				if (checker > 0)
				{
					return _Color1;
				}
				return _Color0;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
