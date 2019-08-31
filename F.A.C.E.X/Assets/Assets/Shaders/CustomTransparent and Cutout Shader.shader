Shader "Custom/Transparent and Cutout Shader"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}

		SubShader{

			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
			LOD 200

			// Non-lightmapped
			Pass
			{
				Tags { "LightMode" = "Vertex" }
				Alphatest Greater[_Cutoff]
				AlphaToMask True
				ColorMask RGB
				Material {
					Diffuse[_Color]
					Ambient[_Color]
				}
				Lighting On
				SetTexture[_MainTex] {
					Combine texture * primary DOUBLE, texture * primary
				}
			}

			// Lightmapped, encoded as dLDR
			Pass {
				Tags { "LightMode" = "VertexLM" }
				Alphatest Greater[_Cutoff]
				AlphaToMask True
				ColorMask RGB
				Material {
					Diffuse[_Color]
					Ambient[_Color]
				}
				Lighting On
				SetTexture[unity_Lightmap] {
					matrix[unity_LightmapMatrix]
					combine texture * texture alpha DOUBLE
				}

				SetTexture[_MainTex] {
					combine texture * previous DOUBLE, texture * primary
				}
			}

			// Lightmapped, encoded as RGBM
			Pass
			{
				Tags { "LightMode" = "VertexLMRGBM" }
				Alphatest Greater[_Cutoff]
				AlphaToMask True
				ColorMask RGB
				Material {
					Diffuse[_Color]
					Ambient[_Color]
				}
				Lighting On
				SetTexture[unity_Lightmap] {
					matrix[unity_LightmapMatrix]
					combine texture * texture alpha DOUBLE
				}

				SetTexture[_MainTex] {
					combine texture * previous DOUBLE, texture * primary
				}
			}

			// Pass to render object as a shadow caster
			Pass
			{
				Name "Caster"
				Tags { "LightMode" = "ShadowCaster" }
			}
		}
}