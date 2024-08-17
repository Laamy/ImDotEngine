uniform vec2 u_res; // screen dimensions
uniform vec2 u_pos; // worldpos (0,0)
uniform vec3 u_basecolor;

void main()
{
    // normalized (TODO: FIX ZOOM)
    float normalizedY = (gl_FragCoord.y / u_res.y) + u_pos.y / u_res.y;

    // gradient factor stuff
    float gradientFactor = clamp(normalizedY, -0.3, 0.6);

    // final colour
    vec3 finalColor = mix(u_basecolor, vec3(1.0), gradientFactor);

    gl_FragColor = vec4(finalColor, 1.0);
}