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



Texture2D DitherTexture;
SamplerState DitherSampler {
	AddressU = Wrap;
	AddressV = Wrap;
};


Texture2D NormalTexture;
SamplerState NormalPosSampler;



float2 renderTargetResolution;
float strengthPerRay;
float fallOff;
float ditherScale;
float bias;


struct VertexShaderInput
{
	float4 Position : POSITION0;	
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VS(VertexShaderInput input) {
    	
    	VertexShaderOutput output = (VertexShaderOutput)0;
    	
    	output.Position = input.Position;
    	
    	output.TexCoord = input.TexCoord.xy;
    	return output;
}



float2 SnapToTexel(float2 uv, float2 maxScreenCoords)
{
	return round(uv * maxScreenCoords) * rcp(maxScreenCoords);
}



float4 PS(VertexShaderOutput input) : SV_Target
{   
        float3 normalVS = normalize(NormalTexture.Sample(NormalPosSampler, input.TexCoord).xyz);
        
        float3 positionVS = FragPosTexture.Sample(FragPosSampler, input.TexCoord).xyz;
		float depth = positionVS.z;
        
        float globalOcclusion = 0.0;
		float step_size = 0.3f;
		int number_of_directions = 8;
		int number_of_steps = 8;

        float angleStep = 2.0 * PI / number_of_directions;
        for(int i = 0; i < number_of_directions; ++i) {
           
            float theta = i * angleStep;
            theta += DitherTexture.Sample(DitherSampler,input.TexCoord*ditherScale) * PI * 2.0;
            
            float2 stepDir = float2(sin(theta), cos(theta));
            float2 texelSizedStep = stepDir * rcp(renderTargetResolution);
            float2 stepSize = step_size * texelSizedStep;
            
            float2 test = SnapToTexel(input.TexCoord + texelSizedStep, renderTargetResolution);
            
            
            float topOcclusion = bias;
            float occlusion = 0.0;
           
            for(int j = 0; j < number_of_steps; ++j) {
                
                float2 offset = j * stepSize;
                offset += 2.0 * DitherTexture.Sample(FragPosSampler,(input.TexCoord + offset)*ditherScale).xy * rcp(renderTargetResolution);
                float2 uv = SnapToTexel(input.TexCoord + offset, renderTargetResolution);
                
                float3 samplePos = FragPosTexture.Sample(FragPosSampler, uv).xyz;
                
                float3 horizonVector = samplePos - positionVS;
                float horizonVectorLength = length(horizonVector);
				float localOcclusion = dot(normalVS, horizonVector) / horizonVectorLength;
                float diff = max(localOcclusion - topOcclusion, 0.0);
                topOcclusion = max(localOcclusion, topOcclusion);
                                     
                float distanceFactor = saturate(horizonVectorLength / fallOff);
                distanceFactor = 1.0 - distanceFactor * distanceFactor;
                occlusion += diff * distanceFactor;

                
                
            }
            
            globalOcclusion += occlusion;
        }
        
        float a = 1.0 - saturate(strengthPerRay * globalOcclusion);
    	return float4(a,a,a, 1.0);
}

technique HBAO
{
	pass P0
	{
	    VertexShader = compile VS_SHADERMODEL VS();
		PixelShader = compile PS_SHADERMODEL PS();
	}
};