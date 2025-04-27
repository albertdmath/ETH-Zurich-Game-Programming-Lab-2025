#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
#define PI 3.14159265359

struct VertexIn
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;	
};

struct VertexOut
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
};


Texture2D FragPosTexture;
SamplerState FragPosSampler;

Texture2D AlbedoTexture;
SamplerState AlbedoSampler;

Texture2D NormalTexture;
SamplerState NormalPosSampler;

int filterSize;
float2 renderTargetResolution;


struct VertexShaderInput
{
	float4 Position : POSITION0;	
	float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VSOutput VS(VertexShaderInput input) {
    VSOutput output = (VSOutput)0;
    
    output.Position = input.Position;
    
    output.TexCoord = input.TexCoord.xy;
    return output;
}


float4 PS(VSOutput input) : SV_Target
{   
        float2 center = float2(filterSize, filterSize) * .5;
       float3 normal = NormalTexture.Sample(NormalPosSampler,input.TexCoord).xyz;    
       float3 FragPos = FragPosTexture.Sample(FragPosSampler, input.TexCoord).xyz;
       float3 albedo = AlbedoTexture.Sample(AlbedoSampler, input.TexCoord);
      
    float3 sampleSum = 0.0;
    float weightSum = 0.0;  

    float3 normalPower = 10.0f;
    float3 zPower = 0.5f;

       for(int i = 0; i < filterSize; i++){
        for(int j = 0; j < filterSize; j++){
            float2 offset = (float2(i, j) - center) * rcp(renderTargetResolution);
            float2 uv = input.TexCoord + offset;
            if(uv.x < 0.0 || uv.y < 0.0 || uv.x > 1.0 || uv.y > 1.0) {
                uv = input.TexCoord;
                weightSum += 1;
            }
              float3 sampleAlbedo = AlbedoTexture.Sample(AlbedoSampler, uv);
            float3 samplePosVS = FragPosTexture.Sample(FragPosSampler, uv).xyz;
            float3 sampleNormal = NormalTexture.Sample(NormalPosSampler, uv).xyz;

            float normalWeight = pow(dot(sampleNormal, normal) * 0.5 + 0.5, normalPower);
            float zWeight = 1.0 / pow(1.0 + abs(FragPos.z - samplePosVS.z), zPower);

            float weight = normalWeight * zWeight;

            sampleSum += sampleAlbedo * weight;
            weightSum += weight;
            // Do something with the samples
        }
       }
    return float4(sampleSum / weightSum, 1.0);
}

technique BilateralBlur
{
	pass P0
	{
	    VertexShader = compile VS_SHADERMODEL VS();
		PixelShader = compile PS_SHADERMODEL PS();
	}
};