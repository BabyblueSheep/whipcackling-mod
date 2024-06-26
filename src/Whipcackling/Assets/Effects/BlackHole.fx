sampler2D uImage0 : register(s0);

float2 uResolution;
float uTime;
float uRadius; // Radius of the smoke
float uHoleRadius; // Radius of the hole

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
sampler2D texturepalhole = sampler_state
{
    texture = <uTexturePalette0>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};
texture uTexturePalette1;
sampler2D texturepal1 = sampler_state
{
    texture = <uTexturePalette1>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};
texture uTextureDither;
sampler2D texturedither = sampler_state
{
    texture = <uTextureDither>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

static const float EDGE_RADIUS = 0.3;


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
    float4 brightest_color = tex2D(texturepal1, float2(1, 0));
    float4 almost_brightest_color = tex2D(texturepal1, float2(0.66, 0));
    float4 almost_darkest_color = tex2D(texturepal1, float2(0.33, 0));
    float4 darkest_color = tex2D(texturepal1, float2(0, 0));
    
    float4 dither = tex2D(texturedither, coords * uResolution * 0.5);
    
    //Pixelate
    coords.x -= coords.x % (1 / uResolution.x);
    coords.y -= coords.y % (1 / uResolution.y);
    
    float angle, radius;
    cartesianToPolar(coords, angle, radius); // Convert coordinates to angle and radius
    
    float holeRadius = radius;
    float wobble = tex2D(texture1, float2(radius + uTime - uRadius, angle)) + 1;
    holeRadius *= wobble * 5;
    holeRadius -= wobble * 2;
    holeRadius -= uRadius;
    
    float gasRadius = radius;
    gasRadius /= wobble * 0.6;
    gasRadius -= wobble * 0.2;
    gasRadius -= uRadius;
    
    float4 outergas =
    tex2D(texture0, float2(angle + uTime * 0.9, gasRadius * 0.7 + uTime))
    * tex2D(texture0, float2(angle - uTime * 0.7, gasRadius * 0.5 + uTime * 1.2)) * 2;
    
    outergas *= 1 - gasRadius * 1.2;
    
    // Kinda naive dithering for gas for a smoother gradient
    if (outergas.r > 0.9)
        outergas = brightest_color;
    else
    {
        if (outergas.r > 0.85)
            outergas = dither.r == 0 ? almost_brightest_color : brightest_color;
        else
        {
            if (outergas.r > 0.8)
                outergas = almost_brightest_color;
            else
            {
                if (outergas.r > 0.75)
                    outergas = dither.r == 0 ? 0 : almost_brightest_color;
                else
                    outergas = 0;
            }
        }
    }
    
    float4 innergas =
    tex2D(texture0, float2(angle + uTime * 0.8, gasRadius * 0.7 + uTime * 0.9))
    * tex2D(texture0, float2(angle - uTime * 0.6, gasRadius * 0.5 + uTime)) * 2;
    innergas *= 1 - gasRadius * 2;

    // Kinda naive dithering for gas for a smoother gradient
    if (innergas.r > 0.5)
        innergas = almost_darkest_color;
    else
    {
        if (innergas.r > 0.3)
            innergas = dither.r == 0 ? darkest_color : almost_darkest_color;
        else
        {
            if (innergas.r > 0.1)
                innergas = darkest_color;
            else
            {
                if (innergas.r > 0)
                    innergas = dither.r == 0 ? 0 : darkest_color;
                else
                    innergas = 0;
            }
        }
    }
    
    float4 result = outergas.r > 0.2 ? outergas : innergas;
    result.a = 0;
    
    if (holeRadius < uHoleRadius) // The hole itself
    {
        if (holeRadius > uHoleRadius - EDGE_RADIUS)
        {
            result = tex2D(texturepalhole, float2(inverselerp(uHoleRadius, uHoleRadius - EDGE_RADIUS, holeRadius), 0));
        }
        else
        {
            result = float4(0, 0, 0, 1);
        }
    }
    
    return result;
}

technique Technique1
{
    pass BlackHolePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}