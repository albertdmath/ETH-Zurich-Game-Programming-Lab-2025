#if OPENGL
 #define SV_POSITION POSITION
 #define VS_SHADERMODEL vs_3_0
 #define PS_SHADERMODEL ps_3_0
#else
 #define VS_SHADERMODEL vs_4_0_level_9_1
 #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix Projection;
matrix View;
matrix World;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_Position;
    float Depth : TEXCOORD0;
};

VSOutput VS(VertexShaderInput input)
{
    VSOutput output;

    // Calculate position as usual- the ViewProjection Matrix already takes
    // the new light frustum into account.
    float4 viewPos = mul(input.Position, mul(World, View));
    output.Position = mul(viewPos, Projection);
    // Calculate depth. The division by the W component brings the component
    // into homogenous space- which means the depth is normalized on a 0-1 scale.
    output.Depth = -viewPos.z; 
    //output.Depth = output.Position.z;

    return output;
}

float4 PS(VSOutput input) : COLOR
{   

    // Store depth in the red component.
    return float4(input.Depth,input.Depth,input.Depth,1); 
}

technique DepthMap
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
};
