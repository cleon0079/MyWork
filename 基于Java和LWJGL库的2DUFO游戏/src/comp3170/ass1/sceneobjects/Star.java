package comp3170.ass1.sceneobjects;

import static comp3170.Math.TAU;
import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glDrawElements;
import static org.lwjgl.opengl.GL15.GL_ELEMENT_ARRAY_BUFFER;
import static org.lwjgl.opengl.GL15.glBindBuffer;

import java.awt.Color;

import org.joml.Matrix4f;
import org.joml.Vector4f;

import comp3170.GLBuffers;
import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;

public class Star extends SceneObject {
	
	private static final String VERTEX_SHADER = "simple_vertex.glsl";
	private static final String FRAGMENT_SHADER = "simple_fragment.glsl";
	
	private Shader shader;
	private Vector4f[] vertices;
	private int vertexBuffer;
	private int[] indices;
	private int indexBuffer;
	
	// Check if is using vertex coloring
	private boolean useVertexColoring = false;
	
	// Color black if didn't set any color
	private float[] colour = { 0.0f, 0.0f, 0.0f };
	
	/**
	 * Draw a star
	 *
	 * @param sides how many pointer is the star
	 */
	public Star(int sides) {
		shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
		
		vertices = new Vector4f[2 * sides + 1];	
		// Set the first vertex as the center of the shape
		vertices[0] = new Vector4f(0, 0, 0, 1);
		
		for (int i = 0; i < vertices.length - 1; i++) {
			// If i is odd, keep the inside vertex, if even keep the outside vertex
			if ((i + 1) % 2 != 0) {
				vertices[i + 1] = new Vector4f(0.23f, 0f , 0, 1).rotateZ(TAU/(sides * 2) * i);
			}
			else {
				vertices[i + 1] = new Vector4f(0.5f, 0f, 0, 1).rotateZ(TAU/(sides * 2) * i);
			}
		}
		vertexBuffer = GLBuffers.createBuffer(vertices);
		
		
		indices = new int[sides * 6];	
		
		// An index counter
		int k = 0;
		for (int i = 0; i < sides; i++) {
			// Index of the center vertex
			indices[k++] = 0; 
			// Index of the current vertex
			indices[k++] = i * 2 + 1;
			// Index of the next vertex
			indices[k++] = i * 2 + 2;
					
			// Index of the center vertex
			indices[k++] = 0;
			// Index of the current vertex
			indices[k++] = i * 2 + 2;
			// Index of the next vertex, wrapping around to the first vertex when i = sides
			indices[k++] = (i * 2 + 3) % (sides * 2);	

		}
		indexBuffer = GLBuffers.createIndexBuffer(indices);
		
	}
	
	public void setColour(Color colour) {
		colour.getRGBColorComponents(this.colour);
	}
	
	@Override
	protected void drawSelf(Matrix4f mvpMatrix) {
		shader.enable();
		shader.setUniform("u_mvpMatrix", mvpMatrix);
		shader.setAttribute("a_position", vertexBuffer);
		shader.setUniform("u_colour", colour);
		shader.setUniform("u_renderingUFO", useVertexColoring);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
		glDrawElements(GL_TRIANGLES, indices.length, GL_UNSIGNED_INT, 0);
	}

	
	
}