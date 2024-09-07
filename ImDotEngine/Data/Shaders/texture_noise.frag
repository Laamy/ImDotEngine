uniform vec2 u_res; // screen dimensions

uniform sampler2D u_texture_noise; // noise/texture to give it a bit more randomness

void main()
{
    // get noise value
    //vec2 noiseCoords = gl_FragCoord.xy / u_res;
    //vec4 noiseColor = texture2D(u_fog_noise, noiseCoords); // TODO: tile the fog
    
	gl_FragColor = vec4(1, 0, 0, 1);
}