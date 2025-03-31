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


sampler2D ShadowSampler;



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
        float shadow = 1.0f;

    float2 projCoords = FragLightPosSpace.xy/FragLightPosSpace.w;
    if (projCoords.x > -1.0 && projCoords.x<1.0 && projCoords.y>-1.0 && projCoords.y < 1.0){


    projCoords = mad(0.5f, projCoords, float2(0.5f,0.5f)); 

    projCoords.y = 1.0 - projCoords.y;




    float closestDepth = tex2D(ShadowSampler,projCoords.xy).r;
    float currentDepth = FragLightPosSpace.z/FragLightPosSpace.w;

   float bias = 0.005;
    float2 texel = 1.0f/2048.0f; 
    for(int x = -1; x<=1; ++x){
        for(int y=-1; y <= 1; ++y){
            float pcfDepth = tex2D(ShadowSampler,projCoords.xy + float2(x,y) * texel).r;
            shadow += currentDepth - bias < pcfDepth ? 1.0 : 0.0; 
        }
    }
    shadow /= 9.0f;

      
     }
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

   float4 light = float4(texCol * (ambient + (shadow) *(diffuse + spec)),1);

    return light;
    //return float4(shadow,shadow,shadow,1.0f);
}

technique BlinnPhongTec {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}