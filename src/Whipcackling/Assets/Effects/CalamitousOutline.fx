sampler2D uImage0 : register(s0);

float4 uColor;
float uTime;
float uAmount;

float uSpeed;
float uAmplitude;
float uFrequency;
float uGamerChange;
float uTriangle;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float time = uSpeed * uTime;
    // Normalize coords from 0 - 1 to -1 - 1
    coords = (coords - 0.5) * 2;
    // Apply triangular distortion
    coords.x += uAmplitude * sin(coords.y * uFrequency + time);
    coords.y += uAmplitude * sin(coords.x * uFrequency + time);
    
    // Add some epic gamer change
    coords.x += uGamerChange * sin(time);
    coords.y += uGamerChange * cos(time);
   
    // Apply triangular distortion again
    coords.x += uTriangle * uAmplitude * sin(coords.y * uFrequency + time);
    coords.y += uTriangle * uAmplitude * sin(coords.x * uFrequency + time);
    
    // Normalize back
    coords = coords / 2 + 0.5;
    
    return lerp(tex2D(uImage0, coords), float4(uColor.r, uColor.g, uColor.b, tex2D(uImage0, coords).a * uColor.a), uAmount);
}

technique Technique1
{
    pass CalamitousOutlinePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}