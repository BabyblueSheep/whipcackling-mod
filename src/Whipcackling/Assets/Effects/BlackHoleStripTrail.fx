sampler2D uImage0 : register(s0);

matrix uTransformMatrix;
float uTime;

texture uTexturePalette0;
sampler2D texturepal0 = sampler_state
{
    texture = <uTexturePalette0>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

texture uTexture0;
sampler2D texture0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderInput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Color = input.Color;
    output.Coord = input.Coord;
    output.Position = mul(input.Position, uTransformMatrix);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.Coord;
    coords.y *= 0.3;
    
    float4 result = tex2D(texture0, float2(coords.y * 2 - 0.2, coords.x + uTime * 0.2))
    * tex2D(texture0, float2(coords.y + 0.2, coords.x * 1.5 + uTime + 0.2)) * 4;
    result *= saturate(coords.x * 1.8);
    result *= saturate(1.8 - 1.8 * coords.x);
    
    result = tex2D(texturepal0, float2(result.r, 0));
    return result;
}

technique Technique1
{
    pass BlackHoleStripTrailPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}