Shader "Custom/WaterRippleShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale ("Scale", float) = 1
		_Speed("Speed", float) = 1
		_Frequency("Frequency", float) = 1
	}
	SubShader 
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass 
		{
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			/*#pragma surface surf Standard fullforwardshadows vertex:vert*/
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Scale, _Speed, _Frequency;
			half4 _Color;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal: NORMAL;
			};

			half _Glossiness;
			half _Metallic;

			//// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			//// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			//// #pragma instancing_options assumeuniformscaling
			//UNITY_INSTANCING_BUFFER_START(Props)
			//	// put more per-instance properties here
			//UNITY_INSTANCING_BUFFER_END(Props)


			v2f vert(appdata v)
			{
				v2f o;
				half offsetvert = (v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z);

				half value = _Scale * sin(_Time.w * _Speed + offsetvert * _Frequency);


				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.vertex.y += value;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal.y -= value;
				//v.normal.y += value;
				//o.customValue = value;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET {
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
