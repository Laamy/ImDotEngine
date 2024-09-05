uniform vec2 u_res; // screen dimensions

uniform float u_fog_width; // width of the fog effect
uniform float u_fog_strength; // strength of the fog
uniform vec3 u_fog_color_factor; // modify the colour of the fog

uniform sampler2D u_fog_noise; // fog noise/texture for the colours

void main()
{
    // create scale factors
    vec2 SCALE = vec2(u_res.x / 240.0, u_res.y / 240.0);

    // adjust value for fog width
    float adjustFactor = u_fog_width * max(SCALE.x, SCALE.y);

    // calc dist to nearest edge
    float distX = min(gl_FragCoord.x, u_res.x - gl_FragCoord.x);
    float distY = min(gl_FragCoord.y, u_res.y - gl_FragCoord.y);

    // combine both axes
    float combinedDist = min(distX, distY) + 0.5 * abs(distX - distY);

    // normalize
    float fogFactor = smoothstep(0.0, adjustFactor, combinedDist);
    
    // get noise value
    vec2 noiseCoords = gl_FragCoord.xy / u_res;
    vec4 noiseColor = texture2D(u_fog_noise, noiseCoords*(SCALE/6)); // TODO: tile the fog

    // perturb alpha with noise
    float alpha = (1.0 - fogFactor) * (0.8 + u_fog_strength * noiseColor.a);

    // make sure the alpa doesnt go below 0.1 or above 0.9
    alpha = min(alpha, 0.96);

    vec3 noiseColorFactored = noiseColor.rgb * u_fog_color_factor;
    
    gl_FragColor = vec4(noiseColorFactored, alpha);
}