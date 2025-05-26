#version 410

in vec4 a_position;	// vertex position as a homogeneous 3D point (x,y,z,1) in model space 
in vec2 a_texcoord;    // UV coordinates

uniform mat4 u_mvpMatrix;			// MODEL -> NDC

out vec2 v_texcoord;   // UV coordinates

void main() {
    // Pass the texture coordinates directly to the fragment shader
    v_texcoord = a_texcoord;
    gl_Position = u_mvpMatrix * a_position;
}

