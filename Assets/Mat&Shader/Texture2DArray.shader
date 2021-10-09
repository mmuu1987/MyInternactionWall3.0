Shader "Unlit/Texture2DArray"
{
   Properties
	{
		  _Convert("Convert",range(0,1))=0
		 _MainTex ("Texture", 2D) = "white" {}
		 _Color("Color Tint",Color) = (1,1,1,1)//控制整体颜色
		_Specular("Specular",Color) =(1,1,1,1)//控制高光反射颜色
		_Gloss("Gloss",Range(1,100))=10//控制高光区域大小
		_AlphaScale("Alpha Scale",Range(0,1)) = 0.65//透明度混合中的透明度系数
		
	}

	SubShader
	{
		Tags{"LightMode" = "ForwardBase" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
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
				
				fixed3 worldNormal = normalize(i.worldNormal);//世界空间下顶点法线
			    fixed3 worldLight = UnityWorldSpaceLightDir(i.worldPos);//世界空间下顶点处的入射光
			    fixed4 texColor = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv.xy, UNITY_ACCESS_INSTANCED_PROP(Props, _Index)));
				//fixed4 texColor = tex2D(_MainTex, i.uv);
			    fixed3 albedo = texColor.rgb*_Color.rgb;//纹理采样获取漫反射颜色
			    fixed3 diffuse = _LightColor0.rgb * albedo * (dot(worldLight, worldNormal)*0.5 + 0.5);//半兰伯特模型计算漫反射
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;//环境光

				//计算高光反射，Blinn-Phong模型
			    fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));//观察方向
			    fixed3 halfDir = normalize(worldLight + viewDir);
			    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(halfDir, worldNormal)), _Gloss);


				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);//获取光照衰减和阴影

				float2 uv = i.uv_project.xy/i.uv_project.w;
				fixed4  col2 =  tex2D(_MainTex, uv);
				fixed shadow =SHADOW_ATTENUATION(i); 


				fixed4 colEnd = fixed4(ambient + (diffuse + specular) * atten,texColor.a*_AlphaScale);


				fixed4 col3 = lerp(colEnd,col2,_Convert);
				return col3; 
			}
			ENDCG
		}
	}

	Fallback "VertexLit"

}
