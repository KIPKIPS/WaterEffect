// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//UV动画Shader
Shader "Shader/Wave"
{
    Properties{
        _MainTex("MainTex",2D)=""{}
    }
    
    SubShader
    {
        Pass{
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex Vert
            #pragma fragment Frag 

            sampler2D _MainTex;
            sampler2D _WaveTex;
            //通信结构体
            struct VertToFrag{
                float4 pos:POSITION; 
                float2 uv:TEXCOORD0;
            };
            //顶点着色器
            VertToFrag Vert(appdata_full v){
                VertToFrag vtf;
                vtf.pos=UnityObjectToClipPos(v.vertex);
                vtf.uv=v.texcoord.xy;

                return vtf;
            }
            //片元着色器
            float4 Frag(VertToFrag IN):COLOR{
            	float2 uv=tex2D(_WaveTex,IN.uv).xy;
            	uv=uv*2-1;
            	uv*=0.025;
            	IN.uv+=uv;
            	float4 color=tex2D(_MainTex,IN.uv);
            	return color;
            }
            ENDCG
        }
        
    }
}
