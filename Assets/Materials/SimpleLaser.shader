Shader "Harpmonics/SimpleLaser"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Blend SrcAlpha One
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 view : TEXCOORD0;
				float2 normal : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				o.view = normalize(WorldSpaceViewDir(v.vertex).xz);
				o.normal = normalize((v.normal).xz);
				//o.lightness = 1 - sqrt(1 - pow(max(dot(normalize(v.normal.xz), normalize(UnityObjectToViewPos(v.vertex).xz)), 0), 2));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color;
				col.a = 1 - exp(
					(1 - pow(
						sqrt(1 - pow(
							max(dot(i.view, i.normal), 0) // tangent
							, 2)) // linear position
						, -2)) * 0.07 // lightness attenuation on inverse proportion of position (pos^-n)
				); // soft clip on maximum value 1
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
