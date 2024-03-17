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
texture uTextureNoise2;
sampler2D texture2 = sampler_state
{
    texture = <uTextureNoise2>;
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
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp;
};
texture uTexturePalette1;
sampler2D texturepal1 = sampler_state
{
    texture = <uTexturePalette1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
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

float inverselerp(float from, float to, float value)
{
    return saturate((value - from) / (to - from));
}

float4 inverselerp(float4 from, float4 to, float4 value)
{
    return (value - from) / (to - from);
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
    
    // Stretch the flame to make it look less jank
    coords.x = coords.x * 3;
    
    //Triangular distortion
    coords.y += 0.1 * sin(coords.x * 10 + uTime * 5);
    coords.x += 0.2 * sin(coords.y * 5 + uTime * 5);
    
    //Create line masks for a more curvy flame
    //Sample perlin noise at their center line for a shifting noise line...
    float x = coords.x - uTime;
    float4 blurnoise0 = tex2D(texture0, x * 0.4);
    blurnoise0 = lerp(-0.3, 0.3, blurnoise0);
    float4 blurnoise1 = tex2D(texture0, x * 0.05);
    blurnoise1 = lerp(-0.1, 0.1, blurnoise1);
    
    //....and convert it to a y coordinate.
    float4 mask = 1 - abs(coords.y - 0.5 + blurnoise0); //Bigger mask
    mask = saturate(inverselerp(0.55, 1, mask));
    float4 maskinner = 1 - abs(coords.y - 0.5 + blurnoise1); //Smaller mask
    maskinner = saturate(inverselerp(0.8, 1, maskinner));
    
    
    //The actual noises
    float4 cellnoise0 = tex2D(texture1, float2(coords.x * 0.5 - uTime * 0.9, coords.y)); //Cellular noise for a more hole-ish base
    float4 cellnoise1 = tex2D(texture1, float2(coords.x * 0.7 - uTime * 1.2 + 0.2, coords.y + 0.3));
    float4 circlenoise0 = tex2D(texture2, float2(coords.x * 0.6 - uTime * 2.5, coords.y * 0.6)); //More empty cellular noise for more sharp edges
    float4 circlenoise1 = tex2D(texture2, float2(coords.x * 0.4 - uTime + 0.2, coords.y * 0.4 + 0.2));
    
    //Apply base noise, subtract emptier noise for less straight edges
    float4 result = mask * cellnoise0 * 0.6 + cellnoise1 * mask * 0.5;
    result -= circlenoise0 * circlenoise1 * 3 * (1 - maskinner * 2);
    result -= (circlenoise0 * 0.6 + circlenoise1 * 0.5) * (1 - mask * 1.2);
    
    //Apply final toon shading
    result = tex2D(texturepal0, float2(result.r, 0));
    
    //Apply final color
    return result * tex2D(texturepal1, float2(coords.x / 3, coords.y));
}

technique Technique1
{
    pass FlameTrailPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}