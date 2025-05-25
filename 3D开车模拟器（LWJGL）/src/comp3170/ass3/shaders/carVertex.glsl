#version 410

// Input attributes from the vertex buffer
in vec4 a_position;    // Vertex position as a homogeneous 3D point (x,y,z,1) in model space
in vec4 a_normal;      // Vertex normal as a homogeneous 3D vector (x,y,z,0) in model space
in vec2 a_texcoord;    // UV coordinates

// Uniforms provided by the application
uniform mat4 u_mvpMatrix;   // Model-View-Projection matrix (MODEL -> NDC)
uniform vec4 u_colour;      // Uniform color

// Outputs to the fragment shader
out vec4 v_normal;     // Normal vector in world coordinates (currently not transformed)
out vec2 v_texcoord;   // UV coordinates

void main() {
    // Pass the texture coordinates directly to the fragment shader
    v_texcoord = a_texcoord;

    // Compute the clip space position of the vertex
    gl_Position = u_mvpMatrix * a_position;
}