//
//  OutlineFill.shader
//  QuickOutline
//
//  Created by Chris Nolet on 2/21/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

Shader "Custom/Outline Fill Texture" {
	Properties{
	  [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0

	  _MainTex("Texture", 2D) = "white" {}
	  _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
	  _OutlineWidth("Outline Width", Range(0, 10)) = 2
	}

		SubShader{
		  Tags {
			"Queue" = "Transparent+110"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		  }

		  Pass {
			Name "Fill"
			Cull Off
			ZTest[_ZTest]
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

			Stencil {
			  Ref 1
			  Comp NotEqual
			}

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
			  float4 vertex : POSITION;
			  float2 uv : TEXCOORD0;
			  float3 normal : NORMAL;
			  float3 smoothNormal : TEXCOORD3;
			  UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
			  float4 position : SV_POSITION;
			  float2 uv : TEXCOORD0;
			  fixed4 color : COLOR;
			  UNITY_FOG_COORDS(1)
			  UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform fixed4 _OutlineColor;
			uniform float _OutlineWidth;

			v2f vert(appdata input) {
			  v2f output;

			  UNITY_SETUP_INSTANCE_ID(input);
			  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			  float3 normal = any(input.smoothNormal) ? input.smoothNormal : input.normal;
			  float3 viewPosition = UnityObjectToViewPos(input.vertex);
			  float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal));

			  output.position = UnityViewToClipPos(viewPosition + viewNormal * -viewPosition.z * _OutlineWidth / 1000.0);
			  output.color = _OutlineColor;

			  output.uv = TRANSFORM_TEX(input.uv, _MainTex);
			  UNITY_TRANSFER_FOG(output, o.position);

			  return output;
			}

			fixed4 frag(v2f input) : SV_Target {
				//return input.color;
				fixed4 col = tex2D(_MainTex, input.uv);
				UNITY_APPLY_FOG(input.fogCoord, col);
				return col;
			  }
			  ENDCG
			}
	  }
}
