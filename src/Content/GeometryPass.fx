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

float4x4 FinalBoneMatrices[100];

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
    float4 BoneIds: TEXCOORD1;
    float4 BoneWeights: TEXCOORD2;
};

struct VertexOutput {
    float4 Position: SV_POSITION; 
    float3 Normal: TEXCOORD4;
    float2 TexCoord: TEXCOORD0; 
    float4 ViewPos: TEXCOORD3; 
};
float4x4 MulMatrixScalar(float4x4 m, float s)
{
    return float4x4(
        m[0] * s,
        m[1] * s,
        m[2] * s,
        m[3] * s
    );
}
// for(int i = 0; i < 4; i++){
//     if(BoneIndices[i] < 0 ){
//         count++;
//         continue;
//     } 
//     if(BoneIndices[i] >= 50){
//         break;
//     }
//     boneTransform += MulMatrixScalar(FinalBoneMatrices[(int)BoneIndices[i]],BoneWeights[i]);
// }

bool MatricesEqual(float4x4 a, float4x4 b)
{
    return all(
        (a[0] == b[0]) &&
        (a[1] == b[1]) &&
        (a[2] == b[2]) &&
        (a[3] == b[3])
    );
}
VertexOutput VS(VertexInput input) {


    float4x4 boneTransform = float4x4(
    0.0, 0.0, 0.0, 0.0,
    0.0, 0.0, 0.0, 0.0,
    0.0, 0.0, 0.0, 0.0,
    0.0, 0.0, 0.0, 0.0
);
    float4 BoneIndices = input.BoneIds;
    float4 BoneWeights = input.BoneWeights;
int count = 0;


if((int)BoneIndices.x > -1 && BoneIndices.x < 100){
    boneTransform += mul(FinalBoneMatrices[(int)BoneIndices.x], BoneWeights.x);
} else {
    count++;
}
if((int)BoneIndices.y > -1 && BoneIndices.y < 100){
    boneTransform += mul(FinalBoneMatrices[(int)BoneIndices.y], BoneWeights.y);
} else {
    count++;
}
if((int)BoneIndices.z > -1 && BoneIndices.z < 100){
    boneTransform += mul(FinalBoneMatrices[(int)BoneIndices.z], BoneWeights.z);
} else {
    count++;
}
if((int)BoneIndices.w > -1 && BoneIndices.w < 100){
    boneTransform += mul(FinalBoneMatrices[(int)BoneIndices.w], BoneWeights.w);
} else {
    count++;
}

    if(count >= 4) {
 boneTransform = float4x4(
    1.0, 0.0, 0.0, 0.0,
     0.0, 1.0, 0.0, 0.0,
     0.0, 0.0, 1.0, 0.0,
     0.0, 0.0, 0.0, 1.0
);
    }


    
    float4 BonePos = mul(float4(input.Position,1.0f),boneTransform);
    float4 BoneNormal = mul(float4(input.Normal,0.0f),boneTransform);
    float4 worldPos = mul(float4(BonePos.xyz,1.0f),World);
    float4 viewPos =  mul(worldPos, View);
    VertexOutput output; 
    output.Position = mul(viewPos,Projection); 
    output.Normal = mul(BoneNormal.xyz,(float3x3)NormalMatrix);
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