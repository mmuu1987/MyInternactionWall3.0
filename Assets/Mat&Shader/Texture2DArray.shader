Shader "Unlit/Texture2DArray"
{
   Properties
	{
		  _Convert("Convert",range(0,1))=0
		 _MainTex ("Texture", 2D) = "white" {}
		
	}

	SubShader
	{
		Tags { "RenderType"="Transparent"  "LightMode" = "ForwardBase" "Queue"="Transparent" }
		LOD 100

		Pass
		{

		    // 关闭深度写入
           ZWrite Off
            // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
           Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			  
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_instancing
			#pragma multi_compile_fwdbase  
			
			//#pragma multi_compile
		    #include "AutoLight.cginc"
            #include "Lighting.cginc"
          

			UNITY_DECLARE_TEX2DARRAY(_TexArr);
			float4 _Color;
			float _Convert;
			
		    fixed4 _Specular;
		    float _Gloss;
	     	float _AlphaScale;
			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
				float3 normal:NORMAL;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
				float4 uv_project:TEXCOORD2;
				SHADOW_COORDS(1) //只产生阴影
				float3 worldNormal:TEXCOORD3;//使用第二个插值寄存器
			   float3 worldPos:TEXCOORD4;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(int, _Index)
				UNITY_DEFINE_INSTANCED_PROP(float, _Flag)
				 UNITY_DEFINE_INSTANCED_PROP(int, _Width)
				UNITY_DEFINE_INSTANCED_PROP(float, _Height)
            UNITY_INSTANCING_BUFFER_END(Props)
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv_project = ComputeScreenPos(o.pos);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);//世界空间下顶点法线
			    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
			    UNITY_SETUP_INSTANCE_ID(i);
				

			    //return _Color;
				fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv.xy, UNITY_ACCESS_INSTANCED_PROP(Props, _Index)));
				float2 uv = i.uv_project.xy/i.uv_project.w;
				fixed4  col2 =  tex2D(_MainTex, uv);
				fixed shadow =SHADOW_ATTENUATION(i); 


				//float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));

				//fixed4 col2;
				//col2.rgb = grey;
				//col2.a = 0.5;

				fixed4 col3 = lerp(col,col2,_Convert);


				
				//fixed4 col3 = lerp(col,col2,UNITY_ACCESS_INSTANCED_PROP(Props, _Flag));

				//col3.a = lerp(1,0.5,UNITY_ACCESS_INSTANCED_PROP(Props, _Flag));
				return col*shadow; 
			}
			ENDCG
		}
	}

	Fallback "VertexLit"

}
