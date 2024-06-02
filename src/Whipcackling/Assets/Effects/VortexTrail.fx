sampler2D uImage0 : register(s0);

matrix uTransformMatrix;
float uTime;

texture uTextureNoise0;
sampler2D texture0 = sampler_state
{
    texture = <uTextureNoise0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTextureNoise1;
sampler2D texture1 = sampler_state
{
    texture = <uTextureNoise1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
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

float easeInOutSine(float x)
{
    return -(cos(3.14159265359 * x) - 1) / 2;
}

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
    
    float4 base = tex2D(texture0, coords);
    float4 baseRotated = tex2D(texture0, float2(coords.y, coords.x));
    float4 fullBase = pow(base, 10) * pow(baseRotated, 0.5) * 1.5;
    
    float2 distortedCoords1 = coords;
    distortedCoords1.x += cos(distortedCoords1.y * 5.2 + uTime * 1.4) / 50.0;
    distortedCoords1.y += sin(distortedCoords1.x * 5.1 + uTime * 1.4) / 50.0;
    distortedCoords1.x -= cos(distortedCoords1.y * 5.2 + uTime * 1.4) / 50.0;
    distortedCoords1.x -= cos(distortedCoords1.x * 5.2 + uTime * 1.4) / 50.0;
    float2 distortedCoords2 = coords;
    distortedCoords2.x += sin(distortedCoords2.y * 5.2 + uTime * 1.4) / 20.0;
    distortedCoords2.y -= cos(distortedCoords2.x * 5.1 + uTime * 1.4) / 20.0;
    distortedCoords2.x -= cos(distortedCoords2.y * 5.2 + uTime * 1.4) / 20.0;
    distortedCoords2.x += sin(distortedCoords2.x * 5.2 + uTime * 1.4) / 520.0;
    
    float4 energy1 = tex2D(texture1, distortedCoords1 + float2(easeInOutSine((uTime) % 1), 0));
    float4 energy2 = tex2D(texture1, distortedCoords2 + float2(easeInOutSine((uTime + 0.5) % 1), 0));
    float4 energy = energy1 * energy2;
    energy *= 50;

    
    float4 result = fullBase * energy + pow(base, 35);
    result = tex2D(texturepal0, float2(result.r, 0));
    return result;
}

technique Technique1
{
    pass VortexTrailPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}