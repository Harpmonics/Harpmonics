Shader "Harpmonics/RippleGlow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RippleStrength ("RippleStrength", Range(0,1)) = 0.5
		_RippleWidth ("RippleWidth", Range(0,1)) = 0.05
		_RippleCenter ("RippleCenter", Vector) = (0,0,0,0)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		float4 _Color;

		float _RippleStrength;
		float _RippleWidth;
		fixed2 _RippleCenter;

		float _RipplePositions[10];

		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float ripple_light_value(float render_pos, float ripple_pos) {
			float x = max(0, step(render_pos, ripple_pos) * ((render_pos - ripple_pos) / _RippleWidth + 1));
			return x;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			for (int i = 0; i < 10; ++i) {
				c.rgb = float3(1, 1, 1) - (float3(1, 1, 1) - c.rgb) * (1 - (ripple_light_value(distance(IN.uv_MainTex, _RippleCenter), _RipplePositions[i]) * _RippleStrength));
			}
			o.Albedo = c.rgb * 0.5;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
