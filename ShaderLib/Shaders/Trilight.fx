//shaders using TomF's trilights for light
//constants
#define	MAX_BONES	50

//matrii for skinning
shared float4x4	mBindPose;
shared float4x4	mBones[MAX_BONES];

//texture layers used on the surface
texture	mTexture0;
texture mTexture1;

//These are considered directional (no falloff)
float4	mLightColor0;		//trilights need 3 colors
float4	mLightColor1;		//trilights need 3 colors
float4	mLightColor2;		//trilights need 3 colors
float3	mLightDirection;

float4	mSolidColour;	//for non textured

//specular stuff
float	mShiny;
float4	mSpecColor;
float	mSpecIntensity;

#include "Types.fxh"
#include "CommonFunctions.fxh"


sampler TexSampler0 = sampler_state
{
	Texture	=(mTexture0);

	MinFilter	=Linear;
	MagFilter	=Linear;
	MipFilter	=Linear;

	AddressU	=Wrap;
	AddressV	=Wrap;
};

sampler TexSampler1 = sampler_state
{
	Texture	=(mTexture1);

	MinFilter	=Linear;
	MagFilter	=Linear;
	MipFilter	=Linear;

	AddressU	=Wrap;
	AddressV	=Wrap;
};


VPosTex0Col0 TriTex0VS(VPosNormTex0 input)
{
	VPosTex0Col0	output;	
	
	//generate the world-view-proj matrix
	float4x4	wvp	=mul(mul(mWorld, mView), mProjection);
	
	//transform the input position to the output
	output.Position	=mul(input.Position, wvp);
	
	float3 worldNormal	=mul(input.Normal, mWorld);

	output.Color	=ComputeTrilight(worldNormal, mLightDirection,
						mLightColor0, mLightColor1, mLightColor2);
	
	//direct copy of texcoords
	output.TexCoord0	=input.TexCoord0;
	
	//return the output structure
	return	output;
}

//tangent stuff
VOutPosNormTanBiTanTex0 TriTanVS(VPosNormTanTex0 input)
{
	VOutPosNormTanBiTanTex0	output;
	
	//generate the world-view-proj matrix
	float4x4	wvp	=mul(mul(mWorld, mView), mProjection);
	
	output.Position		=mul(input.Position, wvp);
	output.Normal		=mul(input.Normal, mWorld);
	output.Tangent		=mul(input.Tangent.xyz, mWorld);
	output.TexCoord0	=input.TexCoord0;

	float3	biTan	=cross(input.Normal, input.Tangent) * input.Tangent.w;

	output.BiTangent	=normalize(biTan);

	//return the output structure
	return	output;
}

//packed tangents with worldspace pos
VPosTex04Tex14Tex24Tex34 TriTanWorldVS(VPosNormTanTex0 input)
{
	VPosTex04Tex14Tex24Tex34	output;
	
	//generate the world-view-proj matrix
	float4x4	wvp	=mul(mul(mWorld, mView), mProjection);
	
	output.Position			=mul(input.Position, wvp);
	output.TexCoord0.xyz	=mul(input.Normal, mWorld);
	output.TexCoord0.w		=input.TexCoord0.x;
	output.TexCoord1.xyz	=mul(input.Tangent.xyz, mWorld);
	output.TexCoord1.w		=input.TexCoord0.y;

	float3	biTan	=cross(input.Normal, input.Tangent) * input.Tangent.w;

	output.TexCoord2		=float4(normalize(biTan), 0);
	output.TexCoord3		=mul(input.Position, mWorld);

	//return the output structure
	return	output;
}

VPosTex0Col0 TriSkinTex0VS(VPosNormBoneTex0 input)
{
	VPosNormBone	skVert;
	skVert.Position	=input.Position;
	skVert.Normal	=input.Normal;
	skVert.Blend0	=input.Blend0;
	skVert.Weight0	=input.Weight0;
	
	VPosCol0	singleOut	=ComputeSkinTrilight(skVert, mBones, mBindPose,
								mLightDirection, mLightColor0, mLightColor1, mLightColor2);
	
	VPosTex0Col0		output;
	output.Position		=singleOut.Position;
	output.TexCoord0	=input.TexCoord0;
	output.Color		=singleOut.Color;
	
	return	output;
}

//skinned dual texcoord
VPosTex0Tex1Col0 TriSkinTex0Tex1VS(VPosNormBoneTex0Tex1 input)
{
	VPosNormBone	skVert;
	skVert.Position	=input.Position;
	skVert.Normal	=input.Normal;
	skVert.Blend0	=input.Blend0;
	skVert.Weight0	=input.Weight0;
	
	VPosCol0	singleOut	=ComputeSkinTrilight(skVert, mBones, mBindPose,
								mLightDirection, mLightColor0, mLightColor1, mLightColor2);
	
	VPosTex0Tex1Col0	output;
	output.Position		=singleOut.Position;
	output.TexCoord0	=input.TexCoord0;
	output.TexCoord1	=input.TexCoord1;
	output.Color		=singleOut.Color;
	
	return	output;
}

//normal mapped from tex0
float4 NormalMapTriTex0PS(VNormTanBiTanTex0 input) : COLOR
{
	float4	norm	=tex2D(TexSampler0, input.TexCoord0);

	float3	goodNorm	=ComputeNormalFromMap(
		norm, input.Tangent, input.BiTangent, input.Normal);
	
	float4	texLitColor	=ComputeTrilight(goodNorm, mLightDirection,
							mLightColor0, mLightColor1, mLightColor2);
	
	return	texLitColor;
}

//normal mapped from tex1, with tex0 texturing
float4 NormalMapTriTex0Tex1PS(VNormTanBiTanTex0Tex1 input) : COLOR
{
	float4	norm	=tex2D(TexSampler1, input.TexCoord1);
	float4	texel0	=tex2D(TexSampler0, input.TexCoord0);

	float3	goodNorm	=ComputeNormalFromMap(
		norm, input.Tangent, input.BiTangent, input.Normal);
	
	float4	texLitColor	=ComputeTrilight(goodNorm, mLightDirection,
							mLightColor0, mLightColor1, mLightColor2);
	
	return	texLitColor * texel0;
}

//normal mapped from tex0, with solid color
float4 NormalMapTriTex0SolidPS(VNormTanBiTanTex0Tex1 input) : COLOR
{
	float4	norm	=tex2D(TexSampler0, input.TexCoord0);

	float3	goodNorm	=ComputeNormalFromMap(
		norm, input.Tangent, input.BiTangent, input.Normal);
	
	float4	texLitColor	=ComputeTrilight(goodNorm, mLightDirection,
							mLightColor0, mLightColor1, mLightColor2);
	
	return	texLitColor * mSolidColour;
}

//normal mapped from tex0, with solid color and specular
float4 NormalMapTriTex0SolidSpecPS(VTex04Tex14Tex24Tex34 input) : COLOR
{
	float2	tex;

	tex.x	=input.TexCoord0.w;
	tex.y	=input.TexCoord1.w;

	float4	norm	=tex2D(TexSampler0, tex);

	float3	pnorm	=input.TexCoord0.xyz;
	float3	tan		=input.TexCoord1.xyz;
	float3	bitan	=input.TexCoord2.xyz;
	float3	wpos	=input.TexCoord3.xyz;

	float3	goodNorm	=ComputeNormalFromMap(norm, tan, bitan, pnorm);

	float3	eyeVec	=normalize(mEyePos - wpos);

	float3	r	=normalize(2 * dot(mLightDirection, goodNorm) * goodNorm - mLightDirection);

	float	specDot	=dot(r, eyeVec);

	float4	texLitColor	=ComputeTrilight(goodNorm, mLightDirection,
							mLightColor0, mLightColor1, mLightColor2);

	float4	spec	=mSpecIntensity * mSpecColor *
		max(pow(specDot, mShiny), 0) * length(texLitColor.xyz);

	texLitColor	*=mSolidColour;
	
	return	saturate(texLitColor + spec);
}

//single texture, single color modulated
float4 Tex0Col0PS(VTex0Col0 input) : COLOR
{
	float4	texel0	=tex2D(TexSampler0, input.TexCoord0);
	
	float4	inColor	=input.Color;
	
	float4	texLitColor	=inColor * texel0;
	
	return	texLitColor;
}

//two texture lookups, but one set of texcoords
//alphas tex0 over tex1
float4 Tex0Col0DecalPS(VTex0Col0 input) : COLOR
{
	float4	texel0, texel1;
	texel0	=tex2D(TexSampler0, input.TexCoord0);
	texel1	=tex2D(TexSampler1, input.TexCoord0);
	
	float4	inColor	=input.Color;
	
	float4	texLitColor	=(texel1.w * texel1) + ((1.0 - texel1.w) * texel0);

	texLitColor	*=inColor;

	texLitColor.w	=1.0f;
	
	return	texLitColor;
}

//two texture lookups, 2 texcoord, alpha tex0 over tex1
float4 Tex0Tex1Col0DecalPS(VTex0Tex1Col0 input) : COLOR
{
	float4	texel0, texel1;
	texel0	=tex2D(TexSampler0, input.TexCoord0);
	texel1	=tex2D(TexSampler1, input.TexCoord1);
	
	float4	inColor	=input.Color;
	
	float4	texLitColor	=(texel1.w * texel1) + ((1.0 - texel1.w) * texel0);

	texLitColor	*=inColor;

	texLitColor.w	=1.0f;
	
	return	texLitColor;
}


technique TriTex0
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriTex0VS();
		PixelShader		=compile ps_2_0 Tex0Col0PS();
	}
}

technique TriTex0NormalMap
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriTanVS();
		PixelShader		=compile ps_2_0 NormalMapTriTex0PS();
	}
}

technique TriTex0NormalMapSolid
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriTanVS();
		PixelShader		=compile ps_2_0 NormalMapTriTex0SolidPS();
	}
}

technique TriTex0NormalMapSolidSpec
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriTanWorldVS();
		PixelShader		=compile ps_2_0 NormalMapTriTex0SolidSpecPS();
	}
}

technique TriSkinTex0
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriSkinTex0VS();
		PixelShader		=compile ps_2_0 Tex0Col0PS();
	}
}

technique TriSkinDecalTex0
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriSkinTex0VS();
		PixelShader		=compile ps_2_0 Tex0Col0DecalPS();
	}
}

technique TriSkinDecalTex0Tex1
{     
	pass P0
	{
		VertexShader	=compile vs_2_0 TriSkinTex0Tex1VS();
		PixelShader		=compile ps_2_0 Tex0Tex1Col0DecalPS();
	}
}