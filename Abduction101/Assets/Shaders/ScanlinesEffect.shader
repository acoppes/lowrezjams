Shader "Custom/Scanlines" {

	// scanlines from Downloaded from https://github.com/aaaleee/UnityScanlinesEffect 
	// vignette and crt from https://luka712.github.io/2018/07/21/CRT-effect-Shadertoy-Unity/
	
     Properties {
         _MainTex("_Color", 2D) = "white" {}
         _LineWidth("Line Width", Float) = 4
         _Hardness("Hardness", Float) = 0.9
         _Speed("Displacement Speed", Range(0,1)) = 0.1
     	 _VignetteSize("Vignette Size", Float) = 1
     	 _VignetteSmoothness("Vignette Smoothness", Float) = 0.6
     	 _VignetteEdge("Vignette Edge", Float) = 8
     	 _CRTBend("CRT Bend", Float) = 4
     }
 
     SubShader {

         Tags {"IgnoreProjector" = "True" "Queue" = "Overlay"} 

         Pass {
         	ZTest Always 
         	Cull Off 
         	ZWrite Off 

         	Fog{ Mode off }
 
         CGPROGRAM
 
 		 #pragma vertex vert
 		 #pragma fragment frag
 		 #pragma fragmentoption ARB_precision_hint_fastest
 		 #include "UnityCG.cginc"
 		 #pragma target 3.0
 
	     struct v2f {
	         float4 pos      : POSITION;
	         float2 uv       : TEXCOORD0;
	         float4 scr_pos : TEXCOORD1;
	     };
	 
	     uniform sampler2D _MainTex;
	     uniform float _LineWidth;
	     uniform float _Hardness;
	     uniform float _Speed;
 		 uniform float _VignetteSize;
 		 uniform float _VignetteSmoothness;
 		 uniform float _VignetteEdge;
 		 uniform float _CRTBend;
	 
	     v2f vert(appdata_img v) {
	         v2f o;
	         o.pos = UnityObjectToClipPos(v.vertex);
	         o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
	         o.scr_pos = ComputeScreenPos(o.pos);
	         
	         return o;
	     }

 		 float2 crt_coords(float2 uv, float bend)
		{
			uv -= 0.5;
		    uv *= 2.;
		    uv.x *= 1. + pow(abs(uv.y)/bend, 2.);
		    uv.y *= 1. + pow(abs(uv.x)/bend, 2.);
		    
		    uv /= 2;
		    return uv + .5;
		}

		float vignette(float2 uv, float size, float smoothness, float edgeRounding)
		{
 			uv -= .5;
		    uv *= size;
		    float amount = sqrt(pow(abs(uv.x), edgeRounding) + pow(abs(uv.y), edgeRounding));
		    amount = 1. - amount;
		    return smoothstep(0., smoothness, amount);
		}
	 
	     half4 frag(v2f i) : COLOR {

	     	 float vignetteFactor = vignette(i.uv, _VignetteSize, _VignetteSmoothness, _VignetteEdge);
	     	 float2 crt_uv = crt_coords(i.uv, _CRTBend);
	     	
	         half4 color = tex2D(_MainTex, crt_uv) * vignetteFactor;
	         fixed lineSize = _ScreenParams.y*0.005;
	         float displacement = ((_Time.y*1000)*_Speed)%_ScreenParams.y;
	         float ps = displacement+(i.scr_pos.y * _ScreenParams.y / i.scr_pos.w);

	         return ((int)(ps / floor(_LineWidth*lineSize)) % 2 == 0) ? color : color * float4(_Hardness,_Hardness,_Hardness,1);
	     }
	 
	     ENDCG
	     }
     }
     FallBack "Diffuse"
 }