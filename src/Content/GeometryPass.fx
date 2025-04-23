#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif




float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 NormalMatrix;

float OpacityVal;
float metallic;
float roughness;

Texture2D ModelTexture;
SamplerState TextureSampler;



struct VertexInput 
{
    float3 Position: POSITION; 
    float3 Normal: NORMAL0; 
    float2 TexCoord: TEXCOORD0; 
};

struct VertexOutput {
    float4 Position: SV_POSITION; 
    float3 Normal: TEXCOORD1;
    float2 TexCoord: TEXCOORD0; 
    float4 ViewPos: TEXCOORD3; 
};

VertexOutput VS(VertexInput input) {
    VertexOutput output; 
    float4 worldPos = mul(float4(input.Position,1.0f),World);
    float4 viewPos =  mul(worldPos, View);
    output.Position = mul(viewPos,Projection); 
    output.Normal = mul(input.Normal,(float3x3)NormalMatrix);
    output.TexCoord = input.TexCoord; 
    output.ViewPos = viewPos;
    return output; 

}

struct PSOutput {
    float4 FragPos: SV_Target0;
    float4 Normal: SV_Target1; 
    float4 Albedo: SV_Target2; 
    float4 RoughnessMetallic: SV_Target3;
};





PSOutput PS(VertexOutput input) 
{   
    PSOutput output;
    output.FragPos = input.ViewPos;
    output.Normal = float4(normalize(input.Normal),1.0f);
    output.Albedo = float4(ModelTexture.Sample(TextureSampler,input.TexCoord.xy).rgb, OpacityVal);
    output.RoughnessMetallic = float4(roughness,metallic,0.0f,1.0f);
    return output;

}

technique Geometry {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}