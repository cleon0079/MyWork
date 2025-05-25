package comp3170.ass1.sceneobjects;

import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glDrawElements;
import static org.lwjgl.opengl.GL15.GL_ELEMENT_ARRAY_BUFFER;
import static org.lwjgl.opengl.GL15.glBindBuffer;

import java.awt.Color;

import org.joml.Matrix4f;
import org.joml.Vector3f;
import org.joml.Vector4f;

import comp3170.GLBuffers;
import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;

public class Beam extends SceneObject {
	
	private static final String VERTEX_SHADER = "simple_vertex.glsl";
	private static final String FRAGMENT_SHADER = "simple_fragment.glsl";
	
	private Shader shader;
	private Vector4f[] vertices;
	private int vertexBuffer;
	private int[] indices;
	private int indexBuffer;
	private Vector3f[] colours;
	private int colourBuffer;
	
	// Check if is using vertex coloring
	private boolean useVertexColoring = false;
	
	// Color black if didn't set any color
	private float[] colour = { 0.0f, 0.0f, 0.0f };
	
	/**
	 * Draw a tractor beam
	 *
	 * @param useVertexColoring Pass in true then use vertex coloring, false then not
	 */
	public Beam(boolean useVertexColoring) {
		this.useVertexColoring = useVertexColoring;
		shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
		
		vertices = new Vector4f[] {
			new Vector4f(  -0.3f, -1f, 0, 1),
			new Vector4f(-0.1f,  0f, 0, 1),
			new Vector4f( 0.1f,  0f, 0, 1),
			new Vector4f(   0.3f, -1f, 0, 1),
		};		
		vertexBuffer = GLBuffers.createBuffer(vertices);
		
		indices = new int[] {  
			0, 1, 2,
			0, 2, 3,
		};	
		indexBuffer = GLBuffers.createIndexBuffer(indices);
		
		colours = new Vector3f[] {
			new Vector3f(1.0f, 1.0f, 0.0f),
			new Vector3f(1.0f, 0.0f, 0.0f),
			new Vector3f(1.0f, 0.0f, 0.0f),
			new Vector3f(1.0f, 1.0f, 0.0f),
		};
		
		colourBuffer = GLBuffers.createBuffer(colours);
	}
	
	public void setColour(Color colour) {
		colour.getRGBColorComponents(this.colour);
	}
	
	@Override
	protected void drawSelf(Matrix4f mvpMatrix) {
		shader.enable();
		shader.setUniform("u_mvpMatrix", mvpMatrix);
		shader.setAttribute("a_position", vertexBuffer);
		shader.setAttribute("a_colour", colourBuffer);
		shader.setUniform("u_renderingUFO", useVertexColoring);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
		glDrawElements(GL_TRIANGLES, indices.length, GL_UNSIGNED_INT, 0);
	}

	
	
}
