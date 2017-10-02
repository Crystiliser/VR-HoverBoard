// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/BoundaryShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MaxDistance("DistanceTillView", Float) = 30.0
		_RatioDivideAmount("RatioDivideAmount", Float) = 1500.0
	}
	SubShader 
		{
		Tags
		{ 
			"Queue" = "Transparent"
			"RenderType"="Transparent"
		}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha
		

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _MaxDistance;
		float _RatioDivideAmount;

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}


		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
			//o.Alpha = IN.worldPos.y;
			float ratio = 1;

			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			if (dist > _MaxDistance)
			{
				ratio = 1;
			}
			else
			{
				ratio = (dist / _RatioDivideAmount);
			}
			if (ratio > 1)
			{
				ratio = 1;
			}
			else if (ratio < 0)
			{
				ratio = 0;
			}
			ratio = 1 - ratio;
			o.Alpha = ratio;
		}

		
		ENDCG
	}
	FallBack "Standard"
}
