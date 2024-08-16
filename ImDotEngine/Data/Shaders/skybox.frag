uniform float u_res_y; // screen height
uniform int u_pos_y; // worldpos y
//uniform float u_zoom; // camera zoom

void main()
{
    // Background color (deep blue)
    vec3 baseColor = vec3(0.0, 72.0 / 255.0, 105.0 / 255.0);

    // normalized (TODO: FIX ZOOM)
    float normalizedY = (gl_FragCoord.y / u_res_y) + u_pos_y / u_res_y;

    // gradient factor stuff
    float gradientFactor = clamp(normalizedY, -0.3, 0.6);

    // final colour
    vec3 finalColor = mix(baseColor, vec3(1.0), gradientFactor);

    gl_FragColor = vec4(finalColor, 1.0);
}