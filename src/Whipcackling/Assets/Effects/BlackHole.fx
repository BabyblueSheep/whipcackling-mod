sampler2D uImage0 : register(s0);

float2 uResolution;
float uTime;
float uRadius; // Radius of the smoke
float uHoleRadius; // Radius of the hole

float4 uColorOuter; // Color of foreground gas
float4 uColorInner; // Color of background gas

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
texture uTexturePalette2;
sampler2D texturepal2 = sampler_state
{
    texture = <uTexturePalette2>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


void cartesianToPolar(float2 coords, out float angle, out float radius)
{
    float2 normalizedCoords = (coords - float2(0.5, 0.5)) * 2;
    radius = length(normalizedCoords);
    angle = atan2(normalizedCoords.y, normalizedCoords.x) * 0.15915 + 3.141592;
}

float2 polarToCartesian(float angle, float radius)
{
    float x = radius * cos((angle - 3.141592) * 6.28318);
    float y = radius * sin((angle - 3.141592) * 6.28318);
    return float2(x, y) * 0.5 + float2(0.5, 0.5);
}

float inverselerp(float from, float to, float value)
{
    return saturate((value - from) / (to - from));
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //Pixelate
    coords.x -= coords.x % (1 / uResolution.x);
    coords.y -= coords.y % (1 / uResolution.y);
    
    float angle, radius;
    
    cartesianToPolar(coords, angle, radius); // Convert coordinates to angle and radius
    
    // Add wobbling to the gas and hole edges
    float wobble = tex2D(texture2, float2(radius + uTime - uRadius, angle));
    float origRadius = radius;
    radius *= wobble * 5;
    radius -= wobble * 2;
    radius -= uRadius;
    origRadius *= lerp(wobble, 1, 0.6);
    
    float2 outerCoords1 = float2(((radius * 0.4 + uTime * 0.3) % 1), -angle - uTime * 0.3 - radius * 0.1);
    float2 outerCoords2 = float2(((radius * 0.3 + uTime * 0.2) % 1), angle + uTime * 0.1 + radius * 0.3);
    float4 outerGas1 = tex2D(texture0, outerCoords1); // Outer, brighter gas
    float4 outerGas2 = tex2D(texture0, outerCoords2);
    
    float2 innerCoords1 = polarToCartesian(angle + uTime * 0.1 + radius * 0.2, radius + 0.1 + sin(uTime) * 0.2); // Inner, darker background gas
    float2 innerCoords2 = polarToCartesian(angle - uTime * 0.2 - radius * 0.3 + cos(uTime) * 0.01, radius);
    float4 innerGas1 = tex2D(texture1, innerCoords1);
    float4 innerGas2 = tex2D(texture1, innerCoords2);
    
    float4 innerResult = innerGas1 * innerGas2 * 5;
    innerResult *= inverselerp(1.05, 0.85, radius);
    innerResult *= inverselerp(0.8, 0.6, radius);
    innerResult = tex2D(texturepal1, float2(innerResult.x, 0)) * uColorInner;
    
    float4 outerResult = outerGas1 * outerGas2;
    outerResult *= inverselerp(1.2, 0.8, radius);
    outerResult *= inverselerp(0.8, 0.6, radius);
    outerResult = tex2D(texturepal0, float2(outerResult.x, 0)) * uColorOuter;
    
    float4 gas = outerResult + innerResult;

    if (origRadius < uHoleRadius) // The hole itself
    {
        if (origRadius > uHoleRadius - 0.015)
        {
            gas = tex2D(texturepal2, float2(angle + uTime, origRadius));
        }
        else
        {
            gas = float4(0, 0, 0, 1);
        }
    }
    
    return gas;
}

technique Technique1
{
    pass BlackHolePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}