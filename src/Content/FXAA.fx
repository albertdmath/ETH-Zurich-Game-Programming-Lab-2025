#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif





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


Texture2D AlbedoTexture;
SamplerState AlbedoSampler;



float2 renderTargetResolution;


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
    float FXAA_REDUCE_MIN = (1.0/ 128.0);
    float FXAA_REDUCE_MUL = (1.0 / 8.0);
    float FXAA_SPAN_MAX = 8.0;
    float4 color;
    float2 texelSize = rcp(renderTargetResolution);
    float3 luma = float3(0.299, 0.587, 0.114);
    float2 SE =clamp(input.TexCoord + float2(1,1)*texelSize, float2(0.0f,0.0f),float2(1.0f,1.0f));
    float2 NE = clamp(input.TexCoord + float2(1,-1)*texelSize ,float2(0.0f,0.0f),float2(1.0f,1.0f)) ;
    float2 NW = clamp( input.TexCoord + float2(-1,-1)*texelSize,float2(0.0f,0.0f),float2(1.0f,1.0f));
    float2 SW =clamp(input.TexCoord + float2(-1,1)*texelSize,float2(0.0f,0.0f),float2(1.0f,1.0f));
    float3 rgbSE = AlbedoTexture.Sample(AlbedoSampler,SE).xyz;
     float3 rgbNE = AlbedoTexture.Sample(AlbedoSampler,NE).xyz;
      float3 rgbNW = AlbedoTexture.Sample(AlbedoSampler,NW).xyz;
       float3 rgbSW = AlbedoTexture.Sample(AlbedoSampler,SW).xyz;
    float4 texColor = AlbedoTexture.Sample(AlbedoSampler,input.TexCoord);
     float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
    float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM  = dot(texColor.xyz,  luma);
    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));
    float2 dir;
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));
    float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) *
                          (0.25 * FXAA_REDUCE_MUL), FXAA_REDUCE_MIN);
     float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);
     dir = clamp(dir*rcpDirMin, -FXAA_SPAN_MAX, FXAA_SPAN_MAX);
    float3 rgbA = 0.5 * (
        AlbedoTexture.Sample(AlbedoSampler, input.TexCoord + dir * (1.0 / 3.0 - 0.5)).xyz +
        AlbedoTexture.Sample(AlbedoSampler, input.TexCoord  + dir * (2.0 / 3.0 - 0.5)).xyz);
    float3 rgbB = rgbA * 0.5 + 0.25 * (
         AlbedoTexture.Sample(AlbedoSampler,  input.TexCoord + dir * -0.5).xyz +
         AlbedoTexture.Sample(AlbedoSampler, input.TexCoord + dir * 0.5).xyz);
 float lumaB = dot(rgbB, luma);
    if ((lumaB < lumaMin) || (lumaB > lumaMax))
        color = float4(rgbA, texColor.a);
    else
        color = float4(rgbB, texColor.a);
    return color;
}
technique FXAA
{
	pass P0
	{
	    VertexShader = compile VS_SHADERMODEL VS();
		PixelShader = compile PS_SHADERMODEL PS();
	}
};