﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ProjectShader"
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
           
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 uv_project:TEXCOORD1;
              
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
            
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_project = ComputeScreenPos(o.vertex);
           
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
               // fixed4 col = tex2D(_MainTex, i.uv);
             

                float2 uv = i.uv_project.xy/i.uv_project.w;
				fixed4 col =  tex2D(_MainTex, float2(uv.x,uv.y));

                return col;
            }
            ENDCG
        }
    }
}
