Shader "Destructible 2D/Diffuse"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		
		// D2D
		[PerRendererData] _D2dAlpha ("D2D Alpha", 2D) = "white" {}
		[PerRendererData] _D2dScale ("D2D Scale", Vector) = (1,1,0,0)
		[PerRendererData] _D2dOffset ("D2D Offset", Vector) = (0,0,0,0)
		[PerRendererData] _D2dSharpness ("D2D Sharpness", Float) = 1.0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog keepalpha
		#pragma multi_compile _ PIXELSNAP_ON

		sampler2D _MainTex;
		float4    _Color;
		
		// D2D
		sampler2D _D2dAlpha;
		float     _D2dSharpness;
		float2    _D2dScale;
		float2    _D2dOffset;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
			// D2D
			float2 alphaUv;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			v.normal = float3(0,0,-1);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
			
			// D2D
			o.alphaUv = (v.vertex.xy - _D2dOffset) / _D2dScale;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 mainTex  = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			float4 alphaTex = tex2D(_D2dAlpha, IN.alphaUv);
			
			// Apply alpha tex
			o.Alpha = mainTex.a * saturate(0.5f + (alphaTex.a - 0.5f) * _D2dSharpness);
			
			// Premultiply alpha
			o.Albedo = mainTex.xyz * o.Alpha;
		}
		ENDCG
	}
	
	Fallback "Transparent/VertexLit"
}