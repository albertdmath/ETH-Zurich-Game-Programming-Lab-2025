#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif



// Shader Inputs
float3 LightDirection;
float3 LightColor; 

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 LightViewProjection;

float3 CameraPosition;


Texture2D ShadowTexture;
sampler2D ShadowSampler = sampler_state
{
    Texture = (ShadowTexture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};



Texture2D ModelTexture;
SamplerState TextureSampler;



struct VertexInput 
{
    float4 Position: SV_POSITION; 
    float3 Normal: NORMAL; 
    float2 TexCoord: TEXCOORD0; 
    float4 Color: COLOR0;
};

struct VertexOutput {
    float4 Position: SV_POSITION; 
    float3 Normal: TEXCOORD1;
    float4 Color: COLOR0; 
    float2 TexCoord: TEXCOORD0; 
    float4 FragLightPosSpace: TEXCOORD3;
    float3 WorldPos: TEXCOORD2; 
};


float ShadowCalc(float4 FragLightPosSpace){
    float3 projCoords = FragLightPosSpace.xyz/FragLightPosSpace.w;
    projCoords = projCoords * 0.5 + 0.5; 
    projCoords.y = 1.0 - projCoords.y;


    float closestDepth = tex2D(ShadowSampler,projCoords.xy).r;
    float currentDepth = projCoords.z;

   float bias = 0.005;
float shadow = currentDepth - bias < closestDepth  ? 1.0 : 0.0;  

    return shadow;
}

VertexOutput VS(VertexInput input) {
    VertexOutput output; 
    float4 worldPos = mul(input.Position, World);
    output.Position = mul(worldPos,mul(View,Projection)); 
    output.Normal = mul(input.Normal,(float3x3)World);
    output.TexCoord = input.TexCoord; 
    output.Color = input.Color;
    output.WorldPos = worldPos.xyz;
    output.FragLightPosSpace = mul(worldPos,LightViewProjection);
    return output; 

}


float4 PS(VertexOutput input) : SV_Target 
{   

    float3 norm = normalize(input.Normal);
    float3 texCol = ModelTexture.Sample(TextureSampler,input.TexCoord.xy);
    float3 ambient = float3(0.3f,0.3f,0.3f);
    float diff = max(dot(norm, LightDirection), 0.0);
    float3 diffuse = diff * LightColor;

    float3 viewDir = normalize(CameraPosition-input.WorldPos); 
    float3 reflectDir = reflect(-LightDirection, norm);

    float shadow = ShadowCalc(input.FragLightPosSpace);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    float3 specular = 0.5 * spec * LightColor;  

   float4 light = float4(texCol * (ambient + (1.0 - shadow) *(diffuse + spec)),1);

    //return light;
    return light;
}

technique BlinnPhongTec {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}