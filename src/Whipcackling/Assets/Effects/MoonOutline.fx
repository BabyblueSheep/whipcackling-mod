sampler2D uImage0 : register(s0);

float4 uColor;
float uTime;
float uAmount;

float uSpeed;

float uAmplitude;
float uFrequency;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float time = uSpeed * uTime;
    // Normalize coords from 0 - 1 to -1 - 1
    coords = (coords - 0.5) * 2;
    
    // Apply triangular distortion
    float x = coords.x * uFrequency + time;
    float y = coords.y * uFrequency + time;
    coords.y += cos(x + y) * uAmplitude * cos(x);
    coords.x += sin(x - y) * uAmplitude * sin(y);
    
    // Normalize back
    coords = coords / 2 + 0.5;

    return lerp(tex2D(uImage0, coords), float4(uColor.r, uColor.g, uColor.b, tex2D(uImage0, coords).a * uColor.a), uAmount);
}

technique Technique1
{
    pass MoonOutlinePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}