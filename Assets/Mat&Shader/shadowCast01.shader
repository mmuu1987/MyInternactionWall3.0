 
 
Shader "Custom/shadowCast01"
{
	Properties{
		_MainTex("MainTex",2D) = "white"{}//存放贴图
		_Color("Color Tint",Color) = (1,1,1,1)//控制整体颜色
		_Specular("Specular",Color) =(1,1,1,1)//控制高光反射颜色
		_Gloss("Gloss",Range(1,100))=10//控制高光区域大小
		_AlphaScale("Alpha Scale",Range(0,1)) = 0.65//透明度混合中的透明度系数
 
	}
 
		SubShader{	
 
		Pass{
		//指定前向渲染模式
		Tags{"LightMode" = "ForwardBase" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
 
		ZWrite Off//关闭深度读写
		Blend SrcAlpha OneMinusSrcAlpha//开启混合模式
		//Cull Off//关闭剔除功能
 
		CGPROGRAM
 
		#pragma vertex vert
		#pragma fragment frag
        #pragma multi_compile_fwdbase//保证在Shader中使用的光照衰减等光照变量可以被正确赋值
 
		#include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"//添加Unity的内置文件，包含计算阴影所用的宏
        
		//定义Properties中的变量
		sampler2D _MainTex;
		float4 _MainTex_ST;//纹理的缩放和偏移值,TRANSFORM_TEX会调用
		fixed4 _Color;
		fixed4 _Specular;
		float _Gloss;
		float _AlphaScale;
 
		struct a2v {
			float4 vertex:POSITION;
			float2 texcoord:TEXCOORD0;
			float3 normal:NORMAL;
		};
 
		struct v2f {
			float4 pos:SV_POSITION;
			float2 uv:TEXCOORD0;//使用第一个插值寄存器
			float3 worldNormal:TEXCOORD1;//使用第二个插值寄存器
			float3 worldPos:TEXCOORD2;
			SHADOW_COORDS(3)//声明一个用于对阴影纹理采样的坐标,表示使用第4个插值寄存器
		};
 
		v2f vert(a2v v) {
			v2f o;
 
			o.worldNormal = UnityObjectToWorldNormal(v.normal);//世界空间下顶点法线
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.pos = UnityObjectToClipPos(v.vertex );//顶点从模型空间到裁剪空间
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);//传递UV坐标
			TRANSFER_SHADOW(o);//计算声明阴影纹理坐标
			return o;
		}
 
		fixed4 frag(v2f i) :SV_Target{
 
			//计算漫反射
			fixed3 worldNormal = normalize(i.worldNormal);//世界空间下顶点法线
			fixed3 worldLight = UnityWorldSpaceLightDir(i.worldPos);//世界空间下顶点处的入射光
			fixed4 texColor = tex2D(_MainTex, i.uv);
			fixed3 albedo = texColor.rgb*_Color.rgb;//纹理采样获取漫反射颜色
			fixed3 diffuse = _LightColor0.rgb * albedo * (dot(worldLight, worldNormal)*0.5 + 0.5);//半兰伯特模型计算漫反射
 
			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;//环境光
 
			//计算高光反射，Blinn-Phong模型
			fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));//观察方向
			fixed3 halfDir = normalize(worldLight + viewDir);
			fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(halfDir, worldNormal)), _Gloss);
			
			UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);//获取光照衰减和阴影
			//fixed shadow = SHADOW_ATTENUATION(i);//获取阴影值

			
 
			return fixed4(ambient + (diffuse + specular) * atten,texColor.a*_AlphaScale);//改变透明通道的值
		}
 
		ENDCG
		    }
		 }
	FallBack "VertexLit"//最好不要更改Fallback
}
 