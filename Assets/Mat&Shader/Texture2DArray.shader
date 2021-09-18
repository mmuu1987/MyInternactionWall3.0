Shader "Unlit/Texture2DArray"
{
   Properties
	{
		  _LineWidth("LineWidth",range(0,0.5))=0.1
		 _MainTex ("Texture", 2D) = "white" {}
		
	}

	SubShader
	{
		Tags { "Queue"="Transparent"  "LightMode" = "ForwardBase" "RenderType"="Transparent" }
		LOD 100

		Pass
		{

		    // 关闭深度写入
            ZWrite Off
            // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_instancing
			#pragma multi_compile_fwdbase  nolightmap nodirlightmap nodynlightmap novertexlight
			//#pragma multi_compile
			
          

			UNITY_DECLARE_TEX2DARRAY(_TexArr);
			float4 _Color;
			float _LineWidth;
			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
				
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

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
			    UNITY_SETUP_INSTANCE_ID(i);
				//计算阴影

			    //return _Color;
				fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv.xy, UNITY_ACCESS_INSTANCED_PROP(Props, _Index)));
				//fixed4 col =  tex2D(_MainTex, i.uv);

				float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));

				fixed4 col2;
				col2.rgb = grey;
				col2.a = 0.5;

				
				
				fixed4 col3 = lerp(col,col2,UNITY_ACCESS_INSTANCED_PROP(Props, _Flag));

				col3.a = lerp(1,0.5,UNITY_ACCESS_INSTANCED_PROP(Props, _Flag));




				 //float width = _LineWidth*UNITY_ACCESS_INSTANCED_PROP(Props, _Width);//得到左右两边需要白边的像素，同样也是高低需要白边的像素

     //           //算出高低需要白边的比值
     //           float LineHeight = width/UNITY_ACCESS_INSTANCED_PROP(Props, _Height);


     //           if(i.uv.x<_LineWidth || i.uv.x>1-_LineWidth || i.uv.y<LineHeight || i.uv.y>1-LineHeight)
     //           {
     //               col3 = fixed4(1,1,1,1);

     //           }


				return col3;
			}
			ENDCG
		}
	}

	Fallback "Diffuse"

}
