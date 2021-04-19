Shader "Destructible 2D/Default"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		// D2D
		[PerRendererData] _D2dAlpha("D2D Alpha", 2D) = "white" {}
		[PerRendererData] _D2dScale("D2D Scale", Vector) = (1,1,0,0)
		[PerRendererData] _D2dOffset("D2D Offset", Vector) = (0,0,0,0)
		[PerRendererData] _D2dSharpness("D2D Sharpness", Float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert_d2d
			#pragma fragment SpriteFrag_d2d
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			// D2D
			sampler2D _D2dAlpha;
			float     _D2dSharpness;
			float2    _D2dScale;
			float2    _D2dOffset;

			struct v2f_d2d
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_d2d SpriteVert_d2d(appdata_t IN)
			{
				v2f_d2d OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.texcoord1 = (IN.vertex.xy - _D2dOffset) / _D2dScale; // D2D
				OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

				return OUT;
			}

			fixed4 SpriteFrag_d2d(v2f_d2d IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.a *= saturate(0.5f + (tex2D(_D2dAlpha, IN.texcoord1).a - 0.5f) * _D2dSharpness); // D2D
				c.rgb *= c.a;
				//c = tex2D(_D2dAlpha, IN.texcoord1).a;
				return c;
			}
		ENDCG
		}
	}
}