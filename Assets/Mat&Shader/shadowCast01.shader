﻿Shader "Unlit/shadowCast01"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 加上此定义，才能正确的计算阴影，非常重要
            #pragma multi_compile_fwdbase  
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                SHADOW_COORDS(1) //只产生阴影
                //AuntoLight.cginc中的定义：  #define SHADOW_COORDS(idx1) unityShadowCoord4 _ShadowCoord : TEXCOORD##idx1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW(o);
                // 屏幕空间的阴影： #define TRANSFER_SHADOW(a) a._ShadowCoord = ComputeScreenPos(a.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //计算阴影
                fixed shadow =SHADOW_ATTENUATION(i); 
                //    #define SHADOW_ATTENUATION(a) unitySampleShadow(a._ShadowCoord)
                //    inline fixed unitySampleShadow (unityShadowCoord4 shadowCoord)
                // {
                //     fixed shadow = UNITY_SAMPLE_SCREEN_SHADOW(_ShadowMapTexture, shadowCoord);
                //     return shadow;
                // }
                col = col * shadow;
                return col*shadow;
            }
            ENDCG
        }
  //      Pass 
		//{
  //          //此pass就是 从默认的fallBack中找到的 "LightMode" = "ShadowCaster" 产生阴影的Pass
		//	Tags { "LightMode" = "ShadowCaster" }

		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	#pragma target 2.0
		//	#pragma multi_compile_shadowcaster
		//	#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
		//	#include "UnityCG.cginc"

		//	struct v2f {
		//		V2F_SHADOW_CASTER;
		//		UNITY_VERTEX_OUTPUT_STEREO
		//	};

		//	v2f vert( appdata_base v )
		//	{
		//		v2f o;
		//		UNITY_SETUP_INSTANCE_ID(v);
		//		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		//		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
		//		return o;
		//	}

		//	float4 frag( v2f i ) : SV_Target
		//	{
		//		SHADOW_CASTER_FRAGMENT(i)
		//	}
		//	ENDCG

		//}
    }
   // Fallback "Diffuse"
}

