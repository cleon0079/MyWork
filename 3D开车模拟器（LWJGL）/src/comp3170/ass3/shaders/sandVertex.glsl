#version 410

in vec4 a_position;    // Vertex position in model space
in vec2 a_texcoord;    // Texture coordinates (UV)
in vec3 a_normal;      // Vertex normal

uniform mat4 u_mvpMatrix;           // Model-View-Projection Matrix

out vec2 v_texcoord;    // Interpolated texture coordinates (UV) for fragment shader
out vec3 v_normal;      // Interpolated normal vector for fragment shader

void main() {
    v_texcoord = a_texcoord;    // Pass texture coordinates to fragment shader
    v_normal = a_normal;        // Pass normal vector to fragment shader
    gl_Position = u_mvpMatrix * a_position;   // Transform vertex position to normalized device coordinates
}