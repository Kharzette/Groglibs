//common functions used by most shaders
#ifndef _COMMONFUNCTIONSFXH
#define _COMMONFUNCTIONSFXH

//constants
#define	MAX_BONES		55
#define	PI_OVER_FOUR	0.7853981634f
#define	PI_OVER_TWO		1.5707963268f

//matrii
shared float4x4	mWorld;
shared float4x4 mView;
shared float4x4 mProjection;
shared float4x4	mLightViewProj;	//for shadowing
shared float3	mEyePos;

//for dangly shaders
float3	mDanglyForce;

//outline / cell related
shared Texture	mCellTable;

//for shadowmaps
shared Texture	mShadowTexture;
shared float3	mShadowLightPos;
shared bool		mbDirectional;

//sky gradient
shared float3	mSkyGradient0;	//horizon colour
shared float3	mSkyGradient1;	//peak colour

//specular stuff
float4	mSpecColor;
float	mSpecPower;

#include "Types.fxh"


sampler CellSampler = sampler_state
{
	Texture	=(mCellTable);

	MinFilter	=Point;
	MagFilter	=Point;
	MipFilter	=Point;

	AddressU	=Clamp;
};

sampler	ShadowSampler2D	=sampler_state
{
	Texture	=(mShadowTexture);
	MinFilter	=Point;
	MagFilter	=Point;
	MipFilter	=Point;
	AddressU	=Clamp;
	AddressV	=Clamp;
};

sampler	ShadowSampler3D	=sampler_state
{
	Texture	=(mShadowTexture);
	MinFilter	=Point;
	MagFilter	=Point;
	MipFilter	=Point;
	AddressU	=Clamp;
	AddressV	=Clamp;
	AddressW	=Clamp;
};

//does the math to get a normal from a sampled
//normal map to a proper normal useful for lighting
float3 ComputeNormalFromMap(float4 sampleNorm, float3 tan, float3 biTan, float3 surfNorm)
{
	//convert normal from 0 to 1 to -1 to 1
	sampleNorm	=2.0 * sampleNorm - float4(1.0, 1.0, 1.0, 1.0);

	float3x3	tbn	=float3x3(
					normalize(tan),
					normalize(biTan),
					normalize(surfNorm));
	
	//I borrowed a bunch of my math from GL samples thus
	//this is needed to get things back into XNA Land
	tbn	=transpose(tbn);

	//rotate normal into worldspace
	sampleNorm.xyz	=mul(tbn, sampleNorm.xyz);
	sampleNorm.xyz	=normalize(sampleNorm.xyz);

	return	sampleNorm.xyz;
}


//look up the skin transform
float4x4 GetSkinXForm(float4 bnIdxs, float4 bnWeights, float4x4 bones[MAX_BONES])
{
	float4x4 skinTransform	=bones[bnIdxs.x] * bnWeights.x;
	skinTransform	+=bones[bnIdxs.y] * bnWeights.y;
	skinTransform	+=bones[bnIdxs.z] * bnWeights.z;
	skinTransform	+=bones[bnIdxs.w] * bnWeights.w;
	
	return	skinTransform;
}


//compute the 3 light effects on the vert
//see http://home.comcast.net/~tom_forsyth/blog.wiki.html
float3 ComputeTrilight(float3 normal, float3 lightDir, float3 c0, float3 c1, float3 c2)
{
    float3	totalLight;
	float	LdotN	=dot(normal, lightDir);
	
	//trilight
	totalLight	=(c0 * max(0, LdotN))
		+ (c1 * (1 - abs(LdotN)))
		+ (c2 * max(0, -LdotN));
		
	return	totalLight;
}


VPosNorm ComputeSkin(VPosNormBone input, float4x4 bones[MAX_BONES])
{
	VPosNorm	output;
	
	float4	vertPos	=input.Position;
	
	//generate the world-view-proj matrix
	float4x4	wvp	=mul(mul(mWorld, mView), mProjection);
	
	//do the bone influences
	float4x4 skinTransform	=GetSkinXForm(input.Blend0, input.Weight0, bones);
	
	//xform the vert to the character's boney pos
	vertPos	=mul(vertPos, skinTransform);
	
	//transform the input position to the output
	output.Position	=mul(vertPos, wvp);

	//skin transform the normal
	float3	worldNormal	=mul(input.Normal, skinTransform);
	
	//world transform the normal
	output.Normal	=mul(worldNormal, mWorld);

	return	output;
}


VPosTex03Tex13 ComputeSkinWorld(VPosNormBone input, float4x4 bones[MAX_BONES])
{
	VPosTex03Tex13	output;
	
	float4	vertPos	=input.Position;
	
	//generate view-proj matrix
	float4x4	vp	=mul(mView, mProjection);
	
	//do the bone influences
	float4x4 skinTransform	=GetSkinXForm(input.Blend0, input.Weight0, bones);
	
	//xform the vert to the character's boney pos
	vertPos	=mul(vertPos, skinTransform);
	
	//transform to world
	float4	worldPos	=mul(vertPos, mWorld);
	output.TexCoord1	=worldPos.xyz;

	//viewproj
	output.Position	=mul(worldPos, vp);

	//skin transform the normal
	float3	worldNormal	=mul(input.Normal, skinTransform);
	
	//world transform the normal
	output.TexCoord0	=mul(worldNormal, mWorld);

	return	output;
}


VPosTex03Tex13 ComputeSkinWorldDangly(VPosNormBoneCol0 input, float4x4 bones[MAX_BONES])
{
	VPosTex03Tex13	output;
	
	float4	vertPos	=input.Position;
	
	//generate view-proj matrix
	float4x4	vp	=mul(mView, mProjection);
	
	//do the bone influences
	float4x4 skinTransform	=GetSkinXForm(input.Blend0, input.Weight0, bones);
	
	//xform the vert to the character's boney pos
	vertPos	=mul(vertPos, skinTransform);
	
	//transform to world
	float4	worldPos	=mul(vertPos, mWorld);

	//dangliness
	worldPos.xyz	-=input.Color.x * mDanglyForce;

	output.TexCoord1	=worldPos.xyz;

	//viewproj
	output.Position	=mul(worldPos, vp);

	//skin transform the normal
	float3	worldNormal	=mul(input.Normal, skinTransform);
	
	//world transform the normal
	output.TexCoord0	=mul(worldNormal, mWorld);

	return	output;
}


//compute the position and color of a skinned vert
VPosCol0 ComputeSkinTrilight(VPosNormBone input, float4x4 bones[MAX_BONES],
							 float3 lightDir, float4 c0, float4 c1, float4 c2)
{
	VPosCol0	output;
	VPosNorm	skinny	=ComputeSkin(input, bones);

	output.Position		=skinny.Position;	
	output.Color.xyz	=ComputeTrilight(skinny.Normal, lightDir, c0, c1, c2);
	output.Color.w		=1.0;
	
	return	output;
}


float3 ComputeGoodSpecular(float3 wpos, float3 lightDir, float3 pnorm, float3 lightVal, float4 fillLight)
{
	float3	eyeVec	=normalize(mEyePos - wpos);
	float3	halfVec	=normalize(eyeVec + lightDir);
	float	ndotv	=saturate(dot(eyeVec, pnorm));
	float	ndoth	=saturate(dot(halfVec, pnorm));

	float	normalizationTerm	=(mSpecPower + 2.0f) / 8.0f;
	float	blinnPhong			=pow(ndoth, mSpecPower);
	float	specTerm			=normalizationTerm * blinnPhong;
	
	//fresnel stuff
	float	base		=1.0f - dot(halfVec, lightDir);
	float	exponential	=pow(base, 5.0f);
	float	fresTerm	=mSpecColor + (1.0f - mSpecColor) * exponential;

	//vis stuff
	float	alpha	=1.0f / (sqrt(PI_OVER_FOUR * mSpecPower + PI_OVER_TWO));
	float	visTerm	=(lightVal * (1.0f - alpha) + alpha) *
				(ndotv * (1.0f - alpha) + alpha);

	visTerm	=1.0f / visTerm;

	float3	specular	=specTerm * lightVal * fresTerm * visTerm * fillLight;

	return	specular;
}

//snaps a color to a cellish range
float3 CalcCellColor(float3 colVal)
{
	float3	ret;

	ret.x	=tex1D(CellSampler, colVal.x);
	ret.y	=tex1D(CellSampler, colVal.y);
	ret.z	=tex1D(CellSampler, colVal.z);

	return	ret;
}

float3 CalcSkyColorGradient(float3 worldPos)
{
	float3	upVec	=float3(0.0f, 1.0f, 0.0f);

	//texcoord has world pos
	float3	skyVec	=(worldPos - mEyePos);

	skyVec	=normalize(skyVec);

	float	skyDot	=abs(dot(skyVec, upVec));

	return	lerp(mSkyGradient0, mSkyGradient1, skyDot);
}

float3 ComputeShadowCoord(float4 worldPos)
{
	float3	shadCoord;

	//powerup near shadow calculation
	float4	lightPos	=mul(worldPos, mLightViewProj);

	//texCoord xy, world depth in z
	shadCoord.xy	=0.5f * lightPos.xy / lightPos.w + float2(0.5f, 0.5f);
	shadCoord.z		=saturate((lightPos.z / lightPos.w) - 0.00001f);

	//flip y
	shadCoord.y	=1.0f - shadCoord.y;

	return	shadCoord;
}

float3 ApplyShadow2D(float3 shadCoord, float3 texLitColor)
{
	float	depth0	=tex2D(ShadowSampler2D, shadCoord).r;

	if(depth0 < shadCoord.z)
	{
		texLitColor	*=0.2f;
	}
	return	texLitColor;
}

float3 ApplyShadow3D(float3 shadDir, float depth, float3 texLitColor)
{
	float	depth0	=texCUBE(ShadowSampler3D, shadDir).r;

	if(depth0 < (depth - 0.00001f))
	{
		texLitColor	*=0.2f;
	}
	return	texLitColor;
}

float3	ShadowColor(bool bDirectional, float4 worldPos, float3 color)
{
	if(bDirectional)
	{
		float3	shadCoord	=ComputeShadowCoord(worldPos);
		color				=ApplyShadow2D(shadCoord, color.xyz);
	}
	else
	{
		float3	shadDir	=normalize(worldPos.xyz - mShadowLightPos);
		color			=ApplyShadow3D(shadDir, worldPos.w, color.xyz);
	}
	return	color;
}
#endif	//_COMMONFUNCTIONSFXH