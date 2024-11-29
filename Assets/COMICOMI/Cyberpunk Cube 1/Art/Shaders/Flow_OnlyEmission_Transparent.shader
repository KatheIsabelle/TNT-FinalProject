// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Xuqi/FlowEffect/Flow_OnlyEmission_Transparent"
{
	Properties
	{
		_TextureEmission("TextureEmission", 2D) = "white" {}
		_EmissionColorIntensity("EmissionColorIntensity", Range( 0 , 10)) = 1
		[HDR]_EmissionColor("EmissionColor", Color) = (0.7605247,0.658375,0.597202,1)
		_TillingXY("TillingXY", Vector) = (1,1,0,0)
		[Toggle]_UseAlphaOrRGB("UseAlphaOrRGB", Float) = 0
		_SpeedXY("SpeedXY", Vector) = (0,0,0,0)
		_TextureColorRamp("TextureColorRamp", 2D) = "white" {}
		[Toggle]_UseGradientColor("UseGradientColor", Float) = 0
		_GradientColorSpeed("GradientColorSpeed", Range( 0 , 10)) = 1
		_LeftRamp("LeftRamp", Range( 0 , 1)) = 0
		_RightRamp("RightRamp", Range( 0 , 1)) = 1
		[Toggle]_UseAlphaGradient("使用透明度变化", Float) = 1
		[Toggle]_UseSinOrTexture("勾选-时间透明变化，不勾-贴图采样透明变化", Float) = 0
		_AlphaGradientSpeed("AlphaGradientSpeed透明度变化速度", Range( 0 , 10)) = 1
		_AlphaMin("AlphaMin最小值", Range( 0 , 1)) = 0
		_AlphaMax("AlphaMax最大值", Range( 0 , 1)) = 1
		_TextureNoise1("TextureNoise1", 2D) = "white" {}
		_TextureNoise1ValueMin("TextureNoise1ValueMin", Range( 0 , 1)) = 0
		_TextureNoise1ValueMax("TextureNoise1ValueMax", Range( 0 , 1)) = 1
		_Noise1TillingXY("Noise1TillingXY", Vector) = (1,1,0,0)
		_Noise1SpeedXY("Noise1SpeedXY", Vector) = (0,0,0,0)
		[Toggle]_UseCustomNoise("勾选_自定义透明度流动图 不勾_默认噪声", Float) = 0
		_TextureNoiseValue1Scale("TextureNoiseValue1Scale", Range( 0.1 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _UseGradientColor;
		uniform float4 _EmissionColor;
		uniform sampler2D _TextureColorRamp;
		uniform float _GradientColorSpeed;
		uniform float _LeftRamp;
		uniform float _RightRamp;
		uniform sampler2D _TextureEmission;
		uniform float2 _SpeedXY;
		uniform float2 _TillingXY;
		uniform float _UseAlphaGradient;
		uniform float _UseSinOrTexture;
		uniform float _UseCustomNoise;
		uniform float2 _Noise1SpeedXY;
		uniform float2 _Noise1TillingXY;
		uniform sampler2D _TextureNoise1;
		uniform float _TextureNoise1ValueMin;
		uniform float _TextureNoise1ValueMax;
		uniform float _AlphaGradientSpeed;
		uniform float _AlphaMin;
		uniform float _AlphaMax;
		uniform float _TextureNoiseValue1Scale;
		uniform float _EmissionColorIntensity;
		uniform float _UseAlphaOrRGB;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime58 = _Time.y * _GradientColorSpeed;
			float2 appendResult61 = (float2((_LeftRamp + (sin( mulTime58 ) - -1.0) * (_RightRamp - _LeftRamp) / (1.0 - -1.0)) , 0.5));
			float2 uv_TexCoord36 = i.uv_texcoord * _TillingXY;
			float2 panner37 = ( 1.0 * _Time.y * _SpeedXY + uv_TexCoord36);
			float4 tex2DNode33 = tex2D( _TextureEmission, panner37 );
			float2 uv_TexCoord76 = i.uv_texcoord * _Noise1TillingXY;
			float2 panner78 = ( 1.0 * _Time.y * _Noise1SpeedXY + uv_TexCoord76);
			float simplePerlin2D80 = snoise( panner78 );
			simplePerlin2D80 = simplePerlin2D80*0.5 + 0.5;
			float mulTime68 = _Time.y * _AlphaGradientSpeed;
			o.Emission = ( (( _UseGradientColor )?( tex2D( _TextureColorRamp, appendResult61 ) ):( _EmissionColor )) * tex2DNode33 * (( _UseAlphaGradient )?( ( (( _UseSinOrTexture )?( (_AlphaMin + (sin( mulTime68 ) - -1.0) * (_AlphaMax - _AlphaMin) / (1.0 - -1.0)) ):( (( _UseCustomNoise )?( (_TextureNoise1ValueMin + (tex2D( _TextureNoise1, panner78 ).r - 0.0) * (_TextureNoise1ValueMax - _TextureNoise1ValueMin) / (1.0 - 0.0)) ):( simplePerlin2D80 )) )) * _TextureNoiseValue1Scale ) ):( 1.0 )) * _EmissionColorIntensity ).rgb;
			o.Alpha = ( (( _UseAlphaOrRGB )?( tex2DNode33.a ):( saturate( ( ( tex2DNode33.r + tex2DNode33.g + tex2DNode33.b ) * 0.3333 ) ) )) * (( _UseAlphaGradient )?( ( (( _UseSinOrTexture )?( (_AlphaMin + (sin( mulTime68 ) - -1.0) * (_AlphaMax - _AlphaMin) / (1.0 - -1.0)) ):( (( _UseCustomNoise )?( (_TextureNoise1ValueMin + (tex2D( _TextureNoise1, panner78 ).r - 0.0) * (_TextureNoise1ValueMax - _TextureNoise1ValueMin) / (1.0 - 0.0)) ):( simplePerlin2D80 )) )) * _TextureNoiseValue1Scale ) ):( 1.0 )) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows dithercrossfade 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18934
125;172;1033;649;2660.86;1125.531;4.38295;True;False
Node;AmplifyShaderEditor.Vector2Node;75;-1793.736,1067.511;Inherit;False;Property;_Noise1TillingXY;Noise1TillingXY;20;0;Create;True;0;0;0;False;0;False;1,1;0,0.58;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;48;-2109.292,478.1987;Inherit;False;Property;_TillingXY;TillingXY;4;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;-1554.68,1024.856;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;77;-1562.736,1248.511;Inherit;False;Property;_Noise1SpeedXY;Noise1SpeedXY;21;0;Create;True;0;0;0;False;0;False;0,0;0,-0.67;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;78;-1280.68,1161.857;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-1832.236,350.5446;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;49;-1998.292,605.1987;Inherit;False;Property;_SpeedXY;SpeedXY;6;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;67;-1330.543,754.6254;Inherit;False;Property;_AlphaGradientSpeed;AlphaGradientSpeed透明度变化速度;14;0;Create;False;0;0;0;False;0;False;1;6.51;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-2562.117,-347.4427;Inherit;False;Property;_GradientColorSpeed;GradientColorSpeed;9;0;Create;True;0;0;0;False;0;False;1;2.24;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;637.8514,1635.742;Inherit;False;Property;_TextureNoise1ValueMax;TextureNoise1ValueMax;19;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;68;-1021.476,813.8075;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;37;-1558.236,487.5446;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;58;-2257.05,-337.2608;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;79;-1058.606,1165.508;Inherit;True;Property;_TextureNoise1;TextureNoise1;17;0;Create;True;0;0;0;False;0;False;-1;None;b763ec8d18523cb42bc0c37b76e1e462;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;92;-1080.195,1395.472;Inherit;False;Property;_TextureNoise1ValueMin;TextureNoise1ValueMin;18;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;59;-2039.05,-333.2608;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-1298.3,463.5665;Inherit;True;Property;_TextureEmission;TextureEmission;0;0;Create;True;0;0;0;False;0;False;-1;None;19190a1de79d3ae418d879a705183388;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;69;-848.4755,830.8075;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;91;-736.4065,1269.526;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1050.075,1041.636;Inherit;False;Property;_AlphaMax;AlphaMax最大值;16;0;Create;False;0;0;0;False;0;False;1;0.541;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-2179.65,-8.432007;Inherit;False;Property;_RightRamp;RightRamp;11;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-2227.605,-111.475;Inherit;False;Property;_LeftRamp;LeftRamp;10;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1050.03,943.5933;Inherit;False;Property;_AlphaMin;AlphaMin最小值;15;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;80;-975.5908,1809.332;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;60;-1780.609,-159.016;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;82;-537.2333,1292.411;Inherit;True;Property;_UseCustomNoise;勾选_自定义透明度流动图 不勾_默认噪声;22;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-804.7478,438.7295;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;72;-641.4713,817.1461;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-245.3328,1400.198;Inherit;False;Property;_TextureNoiseValue1Scale;TextureNoiseValue1Scale;23;0;Create;True;0;0;0;False;0;False;1;2.9;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;61;-1624.878,-302.3355;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;74;-309.6047,888.609;Inherit;True;Property;_UseSinOrTexture;勾选-时间透明变化，不勾-贴图采样透明变化;13;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;52;-671.0046,437.5008;Inherit;False;0.3333;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;-1413.668,17.48432;Inherit;False;Property;_EmissionColor;EmissionColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0.7605247,0.658375,0.597202,1;0,0.4851389,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-26.05481,1159.747;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;54;-1234.353,-375.8601;Inherit;True;Property;_TextureColorRamp;TextureColorRamp;7;0;Create;True;0;0;0;False;0;False;-1;None;cd13e762a95fbe649be26e766a66ca79;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;43;-518.7161,463.0856;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1179.119,240.5132;Inherit;False;Property;_EmissionColorIntensity;EmissionColorIntensity;1;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;73;318.0761,925.6575;Inherit;True;Property;_UseAlphaGradient;使用透明度变化;12;0;Create;False;0;0;0;False;0;False;1;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;56;-1059.318,1.606276;Inherit;True;Property;_UseGradientColor;UseGradientColor;8;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;41;-374.3617,583.0669;Inherit;True;Property;_UseAlphaOrRGB;UseAlphaOrRGB;5;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-1542.384,1406.614;Inherit;False;Constant;_Float3;Float 3;21;0;Create;True;0;0;0;False;0;False;6.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;734.7002,670.1804;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-560.3639,1015.692;Inherit;False;Constant;_Float0;Float 0;21;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;95;-1396.066,-241.931;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-70.15083,191.4968;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-652.0638,1081.792;Inherit;False;Constant;_Float1;Float 1;21;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1450.105,369.8296;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Xuqi/FlowEffect/Flow_OnlyEmission_Transparent;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;76;0;75;0
WireConnection;78;0;76;0
WireConnection;78;2;77;0
WireConnection;36;0;48;0
WireConnection;68;0;67;0
WireConnection;37;0;36;0
WireConnection;37;2;49;0
WireConnection;58;0;57;0
WireConnection;79;1;78;0
WireConnection;59;0;58;0
WireConnection;33;1;37;0
WireConnection;69;0;68;0
WireConnection;91;0;79;1
WireConnection;91;3;92;0
WireConnection;91;4;94;0
WireConnection;80;0;78;0
WireConnection;60;0;59;0
WireConnection;60;3;62;0
WireConnection;60;4;63;0
WireConnection;82;0;80;0
WireConnection;82;1;91;0
WireConnection;42;0;33;1
WireConnection;42;1;33;2
WireConnection;42;2;33;3
WireConnection;72;0;69;0
WireConnection;72;3;70;0
WireConnection;72;4;71;0
WireConnection;61;0;60;0
WireConnection;74;0;82;0
WireConnection;74;1;72;0
WireConnection;52;0;42;0
WireConnection;84;0;74;0
WireConnection;84;1;83;0
WireConnection;54;1;61;0
WireConnection;43;0;52;0
WireConnection;73;1;84;0
WireConnection;56;0;30;0
WireConnection;56;1;54;0
WireConnection;41;0;43;0
WireConnection;41;1;33;4
WireConnection;64;0;41;0
WireConnection;64;1;73;0
WireConnection;95;0;61;0
WireConnection;35;0;56;0
WireConnection;35;1;33;0
WireConnection;35;2;73;0
WireConnection;35;3;96;0
WireConnection;0;2;35;0
WireConnection;0;9;64;0
ASEEND*/
//CHKSM=B92E1CF7A332E144B9011F53C695F864CE0BF4D5