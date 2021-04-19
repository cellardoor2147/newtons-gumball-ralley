Shader "Destructible 2D/Keep Alpha + Color"
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
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY PIXELSNAP_ON

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4    _Color;

				// D2D
				sampler2D _D2dAlpha;
				float     _D2dSharpness;
				float2    _D2dScale;
				float2    _D2dOffset;

				struct a2v
				{
					float4 vertex    : POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};

				void Vert(a2v i, out v2f o)
				{
					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.color     = i.color * _Color;
					o.texcoord0 = i.texcoord0;
					o.texcoord1 = (i.vertex.xy - _D2dOffset) / _D2dScale;
#if PIXELSNAP_ON
					o.vertex = UnityPixelSnap(o.vertex);
#endif
				}

				void Frag(v2f i, out float4 o:COLOR0)
				{
					float4 mainTex  = tex2D(_MainTex, i.texcoord0);
					float4 alphaTex = tex2D(_D2dAlpha, i.texcoord1);

					alphaTex.a = saturate(0.5f + (alphaTex.a - 0.5f) * _D2dSharpness);

					// Multiply the color
					o.rgba = mainTex * i.color;

					// Apply alpha tex
					o *= alphaTex;

					// Premultiply alpha
					o.rgb *= o.a;
				}
			ENDCG
		}
	}
}
