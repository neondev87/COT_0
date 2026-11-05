// Lo oscuro se vuelve transparente: el brillo de cada píxel es su alfa (Low..High) y Strength atenúa todo
sampler2D input : register(s0);
float low : register(c0);
float high : register(c1);
float strength : register(c2);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float level = max(color.r, max(color.g, color.b));
    return color * (smoothstep(low, high, level) * strength);
}
