Shader "Glass"
{ 
Properties
{
_MainTex ("Main Texture", 2D) = "white" {}
}

Subshader 
{
Tags { "Queue" = "Transparent" }

Pass 
{
Cull Off 
ColorMask RGB
Blend SrcAlpha OneMinusSrcAlpha 

CGPROGRAM
#pragma vertex vert 
#pragma fragment frag
#include "UnityCG.cginc" 
 
 
uniform sampler2D _MainTex;
uniform float _ReflectValue;


struct vertInput
{
float4 vertex : POSITION;
float3 normal : NORMAL;
float2 texcoord : TEXCOORD0;
};

struct vertOutput 
{
float4 pos : POSITION; 
float2 texcoord : TEXCOORD0;
float3 reflectVec : TEXCOORD1;
};  

  vertOutput vert(vertInput i)
{
vertOutput o;
//Set position.
o.pos = mul (UNITY_MATRIX_MVP, i.vertex);  
//Set texcoord.
o.texcoord = i.texcoord;
//Calculate normal and position in camera space.
float3 normWorld = normalize( mul((float3x3)UNITY_MATRIX_IT_MV, i.normal));
float3 posWorld = mul(UNITY_MATRIX_MV, i.vertex).xyz;
//Set vector used in cubeMap reflection.
o.reflectVec = reflect(posWorld, normWorld);

return o;
}

half4 frag( vertOutput i ) : COLOR
{
//Get colors of MainTexture and AlphaTexture.
half4 colorTex = tex2D(_MainTex, i.texcoord);
return colorTex;
}
ENDCG
}
}

SubShader {
Tags { "Queue" = "Transparent" }
Pass {
Cull Off 
ColorMask RGB
Blend SrcAlpha OneMinusSrcAlpha 
SetTexture [_MainTex] { combine texture }
}
}
}