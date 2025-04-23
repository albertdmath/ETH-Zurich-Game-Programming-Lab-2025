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


Texture2D FragPosTexture;
SamplerState FragPosSampler;



Texture2D DitherTexture;
SamplerState DitherSampler {
	AddressU = Wrap;
	AddressV = Wrap;
};


Texture2D NormalTexture;
SamplerState NormalPosSampler;



float4x4 Projection;	  	// the local projection matrix	// same as in vertex shader
float2 renderTargetResolution;	// the size of the render target


float2 sampleDirections[8];
float strengthPerRay;	// strength / numRays


	// sampleRadius / 2
float fallOff;			// the maximum distance to count samples
float ditherScale;				// the ratio between the render target size and the dither texture size. Normally: renderTargetResolution / 4
float bias;				// minimum factor to start counting occluders



// snaps a uv coord to the nearest texel centre
float2 SnapToTexel(float2 uv, float2 maxScreenCoords)
{
	return round(uv * maxScreenCoords) * rcp(maxScreenCoords);
}

// rotates a sample direction according to the row-vectors of the rotation matrix
float2 Rotate(float2 vec, float2 rotationX, float2 rotationY)
{
	float2 rotated;
	// just so we can use dot product
	float3 expanded = float3(vec, 0.0);
	rotated.x = dot(expanded.xyz, rotationX.xyy);
	rotated.y = dot(expanded.xyz, rotationY.xyy);
	return rotated;
}



// Retrieves the occlusion factor for a particular sample
// uv: the centre coordinate of the kernel
// frustumVector: The frustum vector of the sample point
// centerViewPos: The view space position of the centre point
// centerNormal: The normal of the centre point
// tangent: The tangent vector in the sampling direction at the centre point
// topOcclusion: The maximum cos(angle) found so far, will be updated when a new occlusion segment has been found
float GetSampleOcclusion(float2 uv, float3 centerViewPos, float3 centerNormal, float3 tangent, inout float topOcclusion)
{
	// reconstruct sample's view space position based on depth buffer and interpolated frustum vector
	float3 sampleViewPos = FragPosTexture.Sample(FragPosSampler,uv).xyz;

	// get occlusion factor based on candidate horizon elevation
	float3 horizonVector = sampleViewPos - centerViewPos;
	float horizonVectorLength = length(horizonVector);
	
	float occlusion;

	// If the horizon vector points away from the tangent, make an estimate
	if (dot(tangent, horizonVector) < 0.0)
		return 0.5;
	else
		occlusion = dot(centerNormal, horizonVector) / horizonVectorLength;
	
	// this adds occlusion only if angle of the horizon vector is higher than the previous highest one without branching
	float diff = max(occlusion - topOcclusion, 0.0);
	topOcclusion = max(occlusion, topOcclusion);

	// attenuate occlusion contribution using distance function 1 - (d/f)^2
	float distanceFactor = saturate(horizonVectorLength / fallOff);
	distanceFactor = 1.0 - distanceFactor * distanceFactor;
	return diff * distanceFactor;
}

// Retrieves the occlusion for a given ray
// origin: The uv coordinates of the ray origin (the AO kernel centre)
// direction: The direction of the ray
// jitter: The random jitter factor by which to offset the start position
// maxScreenCoords: The maximum screen position (the texel that corresponds with uv = 1)
// projectedRadii: The sample radius in uv space
// numStepsPerRay: The amount of samples to take along the ray
// centerViewPos: The view space position of the centre point
// centerNormal: The normal of the centre point
// frustumDiff: The difference between frustum vectors horizontally and vertically, used for frustum vector interpolation
float GetRayOcclusion(float2 origin, float2 direction, float jitter, float2 maxScreenCoords, float2 projectedRadii, int numStepsPerRay, float3 centerViewPos, float3 centerNormal)
{	
	// calculate the nearest neighbour sample along the direction vector
	float2 texelSizedStep = direction * rcp(renderTargetResolution);
	direction *= projectedRadii;
	
	// gets the tangent for the current ray, this will be used to handle opposing horizon vectors
	// Tangent is corrected with respect to per-pixel normal by projecting it onto the tangent plane defined by the normal
	float2 sampleUVs = origin + texelSizedStep;
	float3 tangent = FragPosTexture.Sample(FragPosSampler,sampleUVs).xyz - centerViewPos;	
	tangent -= dot(centerNormal, tangent) * centerNormal;
	
	// calculate uv increments per marching step, snapped to texel centres to avoid depth discontinuity artefacts
	float2 stepUV = SnapToTexel(direction.xy / (numStepsPerRay - 1), maxScreenCoords);
	
	// jitter the starting position for ray marching between the nearest neighbour and the sample step size
	float2 jitteredOffset = lerp(texelSizedStep, stepUV, jitter);	
	float2 uv = SnapToTexel(origin + jitteredOffset, maxScreenCoords);

	// top occlusion keeps track of the occlusion contribution of the last found occluder.
	// set to bias value to avoid near-occluders
	float topOcclusion = bias;
	float occlusion = 0.0;

	// march!
	for (int step = 0; step < numStepsPerRay; ++step) {
		occlusion += GetSampleOcclusion(uv, centerViewPos, centerNormal, tangent, topOcclusion);

		uv += stepUV;
	}

	return occlusion;
}


VertexOut VS(VertexIn vin)
{
	VertexOut vertex;
	vertex.Position = vin.Position;
	vertex.UV = vin.UV;
	return vertex;
}


float4 PS(VertexOut pin) : SV_TARGET
{
	int numRays = 8;
	int maxStepsPerRay = 5;
	float halfSampleRadius = 0.015f;
	// The maximum screen position (the texel that corresponds with uv = 1), used to snap to texels
	// (normally, this would be passed in as a constant)
	float2 maxScreenCoords = renderTargetResolution - 1.0;
	// reconstruct view-space position from depth buffer
	float4 centerViewPos = FragPosTexture.Sample(FragPosSampler, pin.UV);
	
	// unpack normal
	float3 centerNormal = normalize(NormalTexture.SampleLevel(NormalPosSampler, pin.UV, 0).xyz);	
	
	// Get the random factors and construct the row vectors for the 2D matrix from cos(a) and -sin(a) to rotate the sample directions
	float3 randomFactors = DitherTexture.SampleLevel(DitherSampler, pin.UV * ditherScale, 0).xyz;
	float2 rotationX = normalize(randomFactors.xy);
	float2 rotationY = rotationX.yx * float2(-1.0f, 1.0f);
	
	float4 projPos = mul(centerViewPos,Projection);
	// scale the sample radius perspectively according to the given view depth (becomes ellipse)
		// scale the sample radius perspectively according to the given view depth (becomes ellipse)
	float w = centerViewPos.z * Projection[2][3] + Projection[3][3];
	float2 projectedRadii = halfSampleRadius * float2(Projection[0][0], Projection[1][1]) / w;	// half radius because projection ([-1, 1]) -> uv ([0, 1])
	float screenRadius = projectedRadii.x * renderTargetResolution.x;
	// bail out if there's nothing to march

	if (screenRadius < 1.0f)
		return float4((projPos.z/projPos.w), (projPos.z/projPos.w), (projPos.z/projPos.w), 1.0f)*0.1f;

	// do not take more steps than there are pixels		
	int numStepsPerRay = min(maxStepsPerRay, screenRadius);
	
	float totalOcclusion = 0.0;

	for (int i = 0; i < numRays; ++i) {
		float2 sampleDir = Rotate(sampleDirections[i].xy, rotationX, rotationY);
		totalOcclusion += GetRayOcclusion(pin.UV, sampleDir, randomFactors.z, maxScreenCoords, projectedRadii, numStepsPerRay, centerViewPos.xyz, centerNormal);
	}
	float finalOcclusion =1.0 - saturate(strengthPerRay * totalOcclusion);
	return float4(finalOcclusion,0.0f,0.0f,1.0f);
}


technique HBAO {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader  = compile PS_SHADERMODEL PS();
    }
}