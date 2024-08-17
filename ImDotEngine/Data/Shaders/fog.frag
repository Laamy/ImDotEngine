uniform vec2 u_res; // screen dimensions
uniform float u_fog_width; // width of the fog effect
uniform vec3 u_fog_color;

void main()
{
    // calc dist to nearest edge
    float distX = min(gl_FragCoord.x, u_res.x - gl_FragCoord.x);
    float distY = min(gl_FragCoord.y, u_res.y - gl_FragCoord.y);

    // combine both axes
    float combinedDist = min(distX, distY) + 0.5 * abs(distX - distY);

    // normalize
    float fogFactor = smoothstep(0.0, u_fog_width, combinedDist);

    // calc alpha
    float alpha = 1.0 - fogFactor;

	gl_FragColor = vec4(u_fog_color, alpha);
}