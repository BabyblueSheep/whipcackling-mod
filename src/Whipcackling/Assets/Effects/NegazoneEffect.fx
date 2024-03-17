sampler uImage0 : register(s0); 

float2 uScreenResolution; // The resolution of the screen.
float2 uScreenPosition; // The in-world position of the screen.
float2 uZoom;

float2 uPositions[10]; // The in-world positions of the negative zones.
float uRadiuses[10]; // The maximum radiuses of the negative zones. The area inside this circle will be partially inverted.
float uInnerRadiuses[10]; // The radiuses of the negative zones. The area inside this circle will be fully inverted.

float inverselerp(float from, float to, float value)
{
    return saturate((value - from) / (to - from));
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 screenRes = uScreenResolution;
    
    // Aspect ratio to convert from elipsis to cirlce.
    float aspect = screenRes.x / screenRes.y;
    
    float4 base_color = tex2D(uImage0, coords);
    // Saturate...
    float4 final_color = float4(base_color.rgb * 3, base_color.a);
    // ...grayscale...
    final_color.rgb = (final_color.r + final_color.g + final_color.b) / 3;
    // ...and invert the final result.
    final_color.rgb = 1 - final_color.rgb;
    
    float zoom = uZoom.x;
    
    float negaLevel = 0;
    
    for (int i = 0; i < 10; i++)
    {     
        float2 position = (uPositions[i] - uScreenPosition) / uScreenResolution; // Position from in-world to in-screen.
        float radius = uRadiuses[i] * zoom; 
        float innerRadius = uInnerRadiuses[i] * zoom;
        
        float dist = distance(float2(coords.x, coords.y / aspect), float2(position.x, position.y / aspect)); // Calculate the distance from any coordinate to position.
        
        if (dist < radius) // Depending on distance from center, increase negative level.
        {
            negaLevel += inverselerp(radius, innerRadius, dist);
            if (negaLevel >= 1)
            {
                negaLevel = 1;
                break;
            }
        }
        else if (dist < innerRadius)
        {
            negaLevel = 1;
            break;
        }

    }
    return lerp(base_color, final_color, saturate(negaLevel));
}

technique Technique1
{
    pass NegazoneEffectPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

// Unused parameters, left here in case removing them shits the runtime.
// update im pretty sure it does
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float2 uTargetPosition;
float3 uSecondaryColor;
float2 uDirection;
float uOpacity;
float3 uColor;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;