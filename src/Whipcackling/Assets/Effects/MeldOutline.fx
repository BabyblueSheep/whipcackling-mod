sampler2D uImage0 : register(s0);

float4 uColor;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    
    return float4(uColor.r, uColor.g, uColor.b, tex2D(uImage0, coords).a);
}

technique Technique1
{
    pass MeldOutlinePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}