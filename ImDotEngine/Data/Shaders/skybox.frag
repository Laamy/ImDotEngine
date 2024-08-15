uniform float u_res_y;

void main()
{
    // bk colour
    vec3 baseColor = vec3(0.0, 72.0 / 255.0, 105.0 / 255.0);

    // vertical gradient
    float gradient = (gl_FragCoord.y / u_res_y);
    vec3 gradientColor = mix(baseColor, vec3(1.0, 1.0, 1.0), gradient * 0.5);

    // adjust brightness
    float brightnessAdjust = (gl_FragCoord.y / u_res_y - 0.5) * 0.4;

    // calculate final & clamp
    vec3 finalColor = clamp(gradientColor + brightnessAdjust, 0.0, 1.0);

    gl_FragColor = vec4(finalColor, 1.0);
}