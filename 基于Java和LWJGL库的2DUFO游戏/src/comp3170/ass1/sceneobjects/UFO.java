package comp3170.ass1.sceneobjects;

import static org.lwjgl.opengl.GL41.*;

import java.awt.Color;

import org.joml.Matrix4f;
import org.joml.Vector3f;
import org.joml.Vector4f;

import comp3170.GLBuffers;
import comp3170.Shader;
import comp3170.ShaderLibrary;
import comp3170.SceneObject;
import static comp3170.Math.TAU;

public class UFO extends SceneObject {

	private static final String VERTEX_SHADER = "simple_vertex.glsl";
	private static final String FRAGMENT_SHADER = "simple_fragment.glsl";
	public static int NSIDES = 30;

	private Shader shader;
	private Vector4f[] vertices;
	private int vertexBuffer;
	private int[] indices;
	private int indexBuffer;
	
	// Vertex coloring buffer
	private Vector3f[] colours;
	private int colourBuffer;
	
	// Radius to draw a ellipse
	private float xRadius = 0.8f;
	private float yRadius = 0.4f;

	// Check if is using vertex coloring
		private boolean useVertexColoring = false;
		
		// Color black if didn't set any color
		private float[] colour = { 0.0f, 0.0f, 0.0f };

	/**
	 * Draw a ellipse
	 *
	 * @param useVertexColoring Pass in true then use vertex coloring, false then not
	 */
	public UFO(boolean useVertexColoring) {
		this.useVertexColoring = useVertexColoring;
		shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);

		vertices = new Vector4f[NSIDES + 1];
		vertices[0] = new Vector4f(0, 0, 0, 1); // center
		
		// Create a rotation matrix to rotate subsequent vertices around the z-axis
		Matrix4f rotate = new Matrix4f();
		
		for (int i = 0; i < NSIDES; i++) {
			// Calculate the angle for the current side
			float angle = i * TAU / NSIDES;
			// Rotate the matrix around the z-axis by the calculated angle
			rotate.rotationZ(angle);
			// Set the position of the current vertex initially along the x-axis with the given radius
			vertices[i + 1] = new Vector4f(xRadius, 0, 0, 1);
			// Apply rotation to the vertex position to create the shape
			vertices[i + 1].mul(rotate);
			// Scale the y-coordinate of the vertex to adjust for the elliptical shape
			vertices[i + 1].y *= yRadius / xRadius;
		}
		vertexBuffer = GLBuffers.createBuffer(vertices);

		
		indices = new int[NSIDES * 3];
		
		// An index counter
		int k = 0;
		for (int i = 1; i <= NSIDES; i++) {
			// Index of the center vertex
			indices[k++] = 0;
			// Index of the current vertex
			indices[k++] = i;
			// Index of the next vertex, wrapping around to the first vertex when i = NSIDES
			indices[k++] = (i % NSIDES) + 1; 
		}
		indexBuffer = GLBuffers.createIndexBuffer(indices);
		
		// If using vertex coloring then get the coloring buffer
		if(useVertexColoring) {
			// Coloring change between sides
			colours = new Vector3f[NSIDES + 1];
			// Center color will be light green
			colours[0] = new Vector3f(0.0f, 1.0f, 0.0f);
			
			for(int i = 1; i < colours.length; i++) {
				// If i is odd then use light green, otherwise use dark green
				colours[i] = new Vector3f(0.0f, (i)%2 != 0 ? 1.0f : 0.5f, 0.0f);
			}
			colourBuffer = GLBuffers.createBuffer(colours);
		}
	}

	public void setColour(Color colour) {
		colour.getRGBColorComponents(this.colour);
	}

	@Override
	protected void drawSelf(Matrix4f mvpMatrix) {
		shader.enable();
		shader.setUniform("u_mvpMatrix", mvpMatrix);
		shader.setAttribute("a_position", vertexBuffer);

		// If we are using vertex coloring then pass in colorBuffer, otherwise use normal coloring.
		if(useVertexColoring) {
			shader.setAttribute("a_colour", colourBuffer);
		}
		else {
			shader.setUniform("u_colour", colour);
		}
		shader.setUniform("u_renderingUFO", useVertexColoring);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
		glDrawElements(GL_TRIANGLES, indices.length, GL_UNSIGNED_INT, 0);
	}

}
