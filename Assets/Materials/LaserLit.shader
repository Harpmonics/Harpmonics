// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Harpmonics/LaserLit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Laser Color", Color) = (1,1,1,0.1)
		_LitRange ("Lit Range", Float) = 1
		_Lightness ("Lightness", Float) = 2
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
				float4 col : COLOR;
			};

			struct v2f
			{
				float4 col : COLOR0;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 posOnLaser : TEXCOORD2;
				float3 dirToCamera : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
			float _LitRange;
			float _Lightness;

			float3 LaserPos;
			float3 LaserDir;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.col = v.col;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				o.posOnLaser = (v.vertex).xyz - LaserPos;
				o.dirToCamera = normalize(WorldSpaceViewDir(v.vertex));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//half3 dist = i.posOnLaser - dot(i.posOnLaser, LaserDir) * LaserDir;
				half3 dist_dir = normalize(cross(i.dirToCamera, LaserDir));
				half3 dist = abs(dot(i.posOnLaser, dist_dir));
				half strength = 1.0 / cosh(length(dist) / _LitRange);
				float4 col = tex2D(_MainTex, i.uv) * lerp(float4(i.col.rgb, 0), float4(_Color.rgb, i.col.a * _Lightness), strength);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
