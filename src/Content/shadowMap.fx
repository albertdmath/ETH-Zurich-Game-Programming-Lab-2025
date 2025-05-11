#if OPENGL
 #define SV_POSITION POSITION
 #define VS_SHADERMODEL vs_3_0
 #define PS_SHADERMODEL ps_3_0
#else
 #define VS_SHADERMODEL vs_4_0_level_9_1
 #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix LightViewProjection;
matrix World;
float4x4 FinalBoneMatrices[100];

struct VertexShaderInput 
{
    float4 Position: POSITION; 
    float3 Normal: NORMAL0; 
    float2 TexCoord: TEXCOORD0; 
    float4 BoneIds: TEXCOORD1;
    float4 BoneWeights: TEXCOORD2;
};

struct ShadowMapVSOutput
{
    float4 Position : SV_Position;
    float Depth : TEXCOORD0;
};

ShadowMapVSOutput ShadowMapVS(VertexShaderInput input)
{

    
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

    float4 bonePos = mul(input.Position, boneTransform);

    ShadowMapVSOutput output;

    // Calculate position as usual- the ViewProjection Matrix already takes
    // the new light frustum into account.
    output.Position = mul(bonePos, mul(World, LightViewProjection));
    
    // Calculate depth. The division by the W component brings the component
    // into homogenous space- which means the depth is normalized on a 0-1 scale.
    output.Depth = output.Position.z / output.Position.w; 
    //output.Depth = output.Position.z;

    return output;
}

float4 ShadowMapPS(ShadowMapVSOutput input) : COLOR
{   

    // Store depth in the red component.
    return float4(input.Depth,input.Depth,input.Depth,1); 
}

technique ShadowMapDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL ShadowMapVS();
        PixelShader = compile PS_SHADERMODEL ShadowMapPS();
    }
};
