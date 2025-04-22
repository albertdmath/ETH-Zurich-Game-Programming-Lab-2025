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
	float2 Position : POSITION;
	float2 UV : TEXCOORD;	
};

struct VertexOut
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD;
	float4 FrustumVector : FRUSTUM_VECTOR;
};

	// contains the view-space vector pointing to the frustum corner in the same order corresponding to the vertices of the quad being rendered,
	// scaled so that z == 1 (used to reconstruct view space positions in the fragment shader)
	// ie: viewFrustumVectors[0] = nearCorner[0] / nearCorner[0].z
	float4 viewFrustumVectors[4];


VertexOut VS(VertexIn vin)
{
	VertexOut vertex;
	vertex.Position = float4(vin.Position, 0.0f, 1.0f);
	vertex.UV = vin.UV;
	vertex.FrustumVector = viewFrustumVectors[vertexID];
	return vertex;
}

technique HBAO {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}