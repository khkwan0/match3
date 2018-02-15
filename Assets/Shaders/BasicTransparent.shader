Shader "Custom/BasicTransparent"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
		_Frequency("Frequency", float) = 1
		_Scale("Scale", Range(0,1)) = 1
		_Speed("Speed", float) = 0.5
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
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float3 normal: NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _Color;
	float _Frequency;
	float _Scale;
	float _Speed;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float value;
	float offset;

	//offset = sqrt((i.uv.x - 0.5) * (i.uv.x - 0.5)) + sqrt((i.uv.y - 0.5) * (i.uv.y - 0.5));
	//offset = ((i.uv.x - 0.5) * (i.uv.x - 0.5)) + ((i.uv.y - 0.5) * (i.uv.y - 0.5));

	//i.uv[0] += offsetX;
	//i.uv[1] += offsetY;
	//float value = _Scale * (sin(_Time.w * _Speed + _Frequency));
	//value = (_Scale * sin(_Time.y * _Speed + offset * _Frequency)) + (1 - _Scale);


	fixed4 col = tex2D(_MainTex, i.uv) * _Color;
	//col.rgb *= value;


	return col;
	}
		ENDCG
	}
	}
}
