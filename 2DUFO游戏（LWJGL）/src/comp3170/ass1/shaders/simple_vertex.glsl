#version 410

in vec4 a_position; // The vertex position from our java class
in vec3 a_colour; // The colour attribute as a vec3
out vec3 v_colour; // Vectex color

uniform mat4 u_mvpMatrix;	// MODEL->WORLD

void main() {
	vec4 p = u_mvpMatrix * a_position;
	gl_Position = p;
	v_colour = a_colour;
}
