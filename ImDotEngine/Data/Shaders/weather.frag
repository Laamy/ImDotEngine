uniform vec2 u_res; // screen dimensions
uniform float u_time; // elapsed time

// Function to generate a pseudo-random value based on position
float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void main()
{
    vec2 uv = gl_FragCoord.xy / u_res; // Normalized screen coordinates

    // Parameters for the rain effect
    float dropWidth = 3.0 / u_res.x; // Width of raindrop in normalized coordinates
    float dropHeight = 1.0; // Height of raindrop in normalized coordinates
    float dropSpeed = 0.5; // Speed of the raindrops
    float dropSpawnRate = 0.05; // Probability of a raindrop appearing at a given fragment

    // Determine the vertical position of the raindrop
    float dropPos = mod(uv.y + u_time * dropSpeed, 1.0);

    // Generate random positions for raindrop spawning
    float randomX = random(floor(uv.y * 100.0));
    float randomY = random(floor(uv.x * 100.0));

    // Check if current fragment is in the rain area
    float inDropX = smoothstep(0.0, dropWidth, abs(fract(uv.x * 1.0) - randomX));
    float inDropY = smoothstep(0.0, dropHeight, abs(fract(uv.y * 1.0) - (dropPos - dropHeight * 0.5)));

    // Combine effects to create raindrop
    float inDrop = inDropX * inDropY;

    // Create the final alpha value with a random chance of drop appearance
    float alpha = inDrop * (random(floor(uv.y * 10.0)) < dropSpawnRate ? 1.0 : 0.0);

    // Set the color of the raindrops
    vec3 color = vec3(0.5, 0.5, 1.0); // Light blue color for drops

    // Set the final color with alpha
    gl_FragColor = vec4(color, alpha);
}