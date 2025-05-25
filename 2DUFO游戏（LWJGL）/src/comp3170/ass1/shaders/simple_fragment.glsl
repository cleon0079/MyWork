#version 410

uniform vec3 u_colour; // colour as a 3D vector (r,g,b)
in vec3 v_colour; // The vectex coloring
uniform bool u_renderingUFO; // Check if using vectex coloring

layout(location = 0) out vec4 o_colour; // (r,g,b,a)

void main() {
	// If using vectex coloring then output the vectex color, otherwise output the normal color
	if(u_renderingUFO){
		o_colour = vec4(v_colour,1);
	}
	else {
		o_colour = vec4(u_colour,1);
	}
}