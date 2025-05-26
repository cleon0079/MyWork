#version 410

uniform sampler2D u_texture; // Texture sampler for sand
uniform sampler2D u_roadTexture; // Texture sampler for road

const float GAMMA = 2.2; // Gamma correction value

in vec3 v_normal; // Interpolated normal vector
in vec2 v_texcoord; // UV coordinates

layout(location = 0) out vec4 o_colour; // Output color

const float rotationAngle = radians(-20.0); // Angle that the road rotate

void main() {
    // Normalize interpolated normal vector
    vec3 normal = normalize(v_normal);

    // Define light direction (assuming light is from top)
    vec3 lightDirection = normalize(vec3(0.0, 1.0, 0.0));

    // Calculate light intensity using dot product of normal and light direction
    float intensity = dot(normal, lightDirection);

    // Rotate vertex position
    vec2 rotatedTexcoord = vec2(
        cos(rotationAngle) * v_texcoord.x - sin(rotationAngle) * v_texcoord.y,
        sin(rotationAngle) * v_texcoord.x + cos(rotationAngle) * v_texcoord.y
    );

    // Map UV coordinates for road texture
    vec2 mappedUVForRoad = rotatedTexcoord / vec2(40, 20);
    
    // Map UV coordinates for sand texture
    vec2 mappedUVForSand = vec2(v_texcoord.x / 20, v_texcoord.y / 20);

    // Define top and bottom boundaries of road texture coordinates
    float topBoundary = 280;  // Top boundary texture coordinate
    float bottomBoundary = 300; // Bottom boundary texture coordinate

    // Sample sand color from texture
    vec4 sandColor = texture(u_texture, mappedUVForSand);
    
    // Sample road color from texture
    vec4 roadColor = texture(u_roadTexture, mappedUVForRoad);
    
    // Calculate blend factor based on distance to road
    float distanceToRoad = abs(rotatedTexcoord.y - (topBoundary + bottomBoundary) / 2.0);
    float blendFactor = smoothstep(0.0, 20.0, distanceToRoad); // Closer to 1 as distance decreases

    // Blend road and sand colors using blend factor
    vec4 blendedColor = mix(roadColor, sandColor, blendFactor);

    // Apply gamma correction
    vec3 gammaCorrectedColor = pow(blendedColor.rgb, vec3(GAMMA));

    // Set final color output by multiplying with intensity
    o_colour = vec4(gammaCorrectedColor, blendedColor.a) * intensity;
}