sampler2D uImage0 : register(s0);

float2 uResolution;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords.x -= coords.x % (1 / uResolution.x);
    coords.y -= coords.y % (1 / uResolution.y);
    
    float4 color = tex2D(uImage0, coords);
    return color * baseColor;
}

technique Technique1
{
    pass PixelisePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}