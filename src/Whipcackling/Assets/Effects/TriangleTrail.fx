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
    
    float4 result = smoothstep(0, 1, coords.y / (coords.x * 0.7)) * smoothstep(0, 1, coords.y / (coords.x * 0.9));
    result *= smoothstep(0, 1, (1 - coords.y) / (coords.x * 0.7)) * smoothstep(0, 1, (1 - coords.y) / (coords.x * 0.9));
    result = tex2D(texturepal0, float2(result.r, 0));
    return result;
}

technique Technique1
{
    pass TriangleTrailPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}