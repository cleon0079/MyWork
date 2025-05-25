#version 410

in vec2 v_texcoord;  // Texture coordinates
const float GAMMA = 2.2; // Gamma correction value
uniform sampler2D u_texture; // Texture sampler

layout(location = 0) out vec4 o_colour;

void main() {
        // Sample the texture color using the texture coordinates
    vec4 textureColor = texture(u_texture, v_texcoord);
    
    // Apply gamma correction
    vec3 gammaCorrectedColor = pow(textureColor.rgb, vec3(GAMMA)); // Gamma correction with a value of 2.2
    
    // Set the output color with gamma-corrected color and the original alpha, modulated by u_colour
    o_colour = vec4(gammaCorrectedColor, textureColor.a);
}

