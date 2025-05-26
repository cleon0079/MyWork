#version 410

// Inputs from the vertex shader
in vec4 v_normal;    // Normal vector in world coordinates (not used in this shader)
in vec2 v_texcoord;  // Texture coordinates

// Constants
const float GAMMA = 2.2; // Gamma correction value

// Uniforms
uniform sampler2D u_texture; // Texture sampler
uniform vec4 u_colour;       // Color uniform (not used in this shader)

// Output
layout(location = 0) out vec4 o_colour; // Output color

void main() {
    // Sample the texture color using the texture coordinates
    vec4 textureColor = texture(u_texture, v_texcoord);
    
    // Apply gamma correction to the RGB components
    vec3 gammaCorrectedColor = pow(textureColor.rgb, vec3(GAMMA));
    
    // Set the output color with gamma-corrected RGB values and the original alpha value
    o_colour = vec4(gammaCorrectedColor, textureColor.a);
    }