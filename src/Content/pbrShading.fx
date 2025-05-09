#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


#define PI 3.1415926538
// Shader Inputs
float3 LightDirection;
float3 LightColor; 

//PBR
float4x4 ViewInverse;
float4x4 LightViewProjection;
float4x4 View;

float ShadowsEnabled;
float OcclusionEnabled;





sampler2D ShadowSampler;



Texture2D ModelTexture;
SamplerState TextureSampler;


Texture2D FragPosTexture;
SamplerState FragPosSampler;

Texture2D NormalTexture;
SamplerState NormalPosSampler;

Texture2D OcclusionTexture;
SamplerState OcclusionSampler;

Texture2D RoughnessTexture;
SamplerState RoughnessSampler;

Texture2D MetallicTexture;
SamplerState MetallicSampler;




struct VertexInput 
{
    float4 Position: SV_POSITION; 
    float2 TexCoord: TEXCOORD0; 
};

struct VertexOutput {
    float4 Position: SV_POSITION; 
    float2 TexCoord: TEXCOORD0; 

};



//F0 = Surface reflection at zero incidence or how much the surface reflects if looking directly at the surface. 
float3 fresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}  

float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
	
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
	
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
	
    return ggx1 * ggx2;
}


float ShadowCalc(float4 FragLightPosSpace){
    
    float shadow = 1.0f;
    float2 projCoords = FragLightPosSpace.xy/FragLightPosSpace.w;
    if (projCoords.x > -1.0 && projCoords.x<1.0 && projCoords.y>-1.0 && projCoords.y < 1.0){
    shadow = 20.0f;
    projCoords = mad(0.5f, projCoords, float2(0.5f,0.5f)); 

    projCoords.y = 1.0 - projCoords.y;

    float closestDepth = tex2D(ShadowSampler,projCoords.xy).r;
    float currentDepth = FragLightPosSpace.z/FragLightPosSpace.w;
    int samples = 0;
    float bias = 0.001;
    float2 texel = 1.0f/4096.0f; 
    for (int x = -3; x <= 3; ++x) {
        for (int y = -3; y <= 3; ++y) {
            float2 offset = float2(x, y) * texel;
            float2 sampleUV = projCoords + offset;

            if (sampleUV.x >= 0 && sampleUV.x <= 1 &&
                sampleUV.y >= 0 && sampleUV.y <= 1) {

                float pcfDepth = tex2D(ShadowSampler, sampleUV).r;
                shadow += (currentDepth - bias < pcfDepth) ? 1.0 : 0.0;
                samples += 1;
            }
        }
    }

    if (samples > 0) {
        shadow /= samples;
    }
    else
    {
        shadow = 1.0; // Default to fully lit if nothing sampled
    }
    }
    return shadow;
}




VertexOutput VS(VertexInput input) {
    VertexOutput output; 
    output.Position = input.Position;
    output.TexCoord = input.TexCoord; 
    return output; 

}


float4 PS(VertexOutput input) : SV_Target
{   

    float3 Normal = normalize(NormalTexture.Sample(NormalPosSampler,input.TexCoord.xy).xyz);
    float4 albedoColor = pow(ModelTexture.Sample(TextureSampler,input.TexCoord.xy),2.2f);
    float3 albedo = albedoColor.rgb;
    float OpacityVal = albedoColor.a;
    float4 FragPos = FragPosTexture.Sample(FragPosSampler,input.TexCoord.xy);
    float2 roughmet = RoughnessTexture.Sample(RoughnessSampler,input.TexCoord.xy).rg;
    float roughness = roughmet.r;
    float metallic = roughmet.g;
    float3 LightDirView = mul(LightDirection,(float3x3)View).rgb;
    float4 WorldPos = mul(FragPos,ViewInverse);
    float4 LightViewPos = mul(WorldPos,LightViewProjection);
    float3 F0 = float3(0.04f,0.04f,0.04f); 
    F0 = lerp(F0,albedo,metallic); 
    float3 wi = normalize(LightDirView);
    float3 viewDir = normalize(-FragPos.xyz); 

    float3 halfWay = normalize(viewDir + wi); 
    float cosTheta = max(dot(Normal,viewDir),0.0); 
    float3 radiance = LightColor * 2.0f; 

    float NDF = DistributionGGX(Normal, halfWay, roughness); 
    float G = GeometrySmith(Normal,viewDir, wi,roughness); 
    float3 F = fresnelSchlick(max(dot(halfWay,viewDir),0.0f),F0);

    float3 numerator = NDF * G * F; 
    float deniminator = 4.0f * cosTheta * max(dot(Normal,wi),0.0f) + 0.0001;
    float3 specular = numerator / deniminator; 

    float3 kS = F; 
    float3 kD = float3(1.0f,1.0f,1.0f) - kS; 

    kD *= 1.0 - metallic; 

    float NdotL = max(dot(Normal, wi), 0.0);     
    float shadow = 1.0f; 
    float interShadow = ShadowCalc(LightViewPos); 
    shadow = lerp(1.0f, interShadow, ShadowsEnabled);

    float3 Lo =  (kD * albedo / PI + specular) * radiance * NdotL * shadow;
    float occlusion = 1.0f;
 float occlusionCheck = OcclusionTexture.Sample(OcclusionSampler, input.TexCoord.xy).r;
    
    occlusion = lerp(1.0f, occlusionCheck, OcclusionEnabled);


    float3 ambient = float3(0.2f,0.2f,0.2f) * albedo;
    float3 color   = ambient + Lo;
    //color = color / (color + float3(1.0f,1.0f,1.0f));
    float HDRnormalizer = 1.0f/2.2f; 
    color = pow(color, float3(HDRnormalizer,HDRnormalizer,HDRnormalizer)); 



    return float4(color * occlusion,OpacityVal); 

}

technique PBR {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}