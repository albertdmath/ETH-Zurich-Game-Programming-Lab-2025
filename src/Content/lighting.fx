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
float3 CameraPosition;

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
    float3 WorldPos: TEXCOORD2; 
};

VertexOutput VS(VertexInput input) {
    VertexOutput output; 
    float4 worldPos = mul(input.Position, World);
    output.Position = mul(worldPos,mul(View,Projection)); 
    output.Normal = mul(input.Normal,(float3x3)World);
    output.TexCoord = input.TexCoord; 
    output.Color = input.Color;
    output.WorldPos = worldPos.xyz;
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

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    float3 specular = 0.5 * spec * LightColor;  

    return float4(texCol * (ambient + diffuse + spec),1); 

}

technique BlinnPhongTec {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}