package comp3170.ass3.sceneobjects;

import static org.lwjgl.glfw.GLFW.GLFW_KEY_1;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_2;
import static org.lwjgl.opengl.GL11.*;
import static org.lwjgl.opengl.GL14.*;
import static org.lwjgl.opengl.GL13.GL_TEXTURE0;
import static org.lwjgl.opengl.GL13.glActiveTexture;
import static org.lwjgl.opengl.GL15.GL_ELEMENT_ARRAY_BUFFER;
import static org.lwjgl.opengl.GL15.glBindBuffer;

import org.joml.Matrix4f;
import org.joml.Vector2f;
import org.joml.Vector4f;
import comp3170.GLBuffers;
import comp3170.InputManager;
import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;

public class MiniMap extends SceneObject {

	// File paths for vertex shader and fragment shader
    static final private String VERTEX_SHADER = "miniVertex.glsl";
    static final private String FRAGMENT_SHADER = "miniFragment.glsl";


    private Shader shader;
    
    private Vector4f[] vertices;
    private int vertexBuffer;
    
    private Vector2f[] texCoords;
    private int texCoordBuffer;
    
    private int[] indices;
    private int indexBuffer;

    private int textureID;
    
    private boolean isShow = true;

    public MiniMap() {
        shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
        
		// Define the vertices of the terrain
		vertices = new Vector4f[] {
				new Vector4f( 0, 0, 0, 1),
				new Vector4f(0, 0, 1, 1),
				new Vector4f( 1,0, 0, 1),
				new Vector4f(1,0, 1, 1),
		};
		
		// Define the indices of the two triangles forming the terrain
		indices = new int[] {
				0, 1, 2,
				3, 2, 1,
		};

		// Set texture coordinates to map the texture onto the terrain
		texCoords = new Vector2f[] {
				new Vector2f(0, 0),
				new Vector2f(0, 1),
				new Vector2f(1, 0),
				new Vector2f(1, 1),
		};

        vertexBuffer = GLBuffers.createBuffer(vertices);
        texCoordBuffer = GLBuffers.createBuffer(texCoords);
        indexBuffer = GLBuffers.createIndexBuffer(indices);
    }
    
    public void setTexture(int texture) {
    	this.textureID = texture;
    }

    @Override
    protected void drawSelf(Matrix4f mvpMatrix, int pass) {
    	if (isShow) {
    		 shader.enable();

    	        // Bind and set parameters for the desert texture
    	        glActiveTexture(GL_TEXTURE0);
    	        glBindTexture(GL_TEXTURE_2D, textureID);
    	        shader.setUniform("u_texture", 0);

    	        shader.setUniform("u_mvpMatrix", mvpMatrix);
    	        shader.setAttribute("a_position", vertexBuffer);
    	        shader.setAttribute("a_texcoord", texCoordBuffer);

    	        // Bind index buffer and draw elements
    	        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
    	        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
    	        glDrawElements(GL_TRIANGLES, indices.length, GL_UNSIGNED_INT, 0);
    	}
    }
    
    // Update the camera
	public void update(InputManager input, float deltaTime) {
		if (input.isKeyDown(GLFW_KEY_1)) {
			isShow = false;
		}
		if (input.isKeyDown(GLFW_KEY_2)) {
			isShow = true;
		}
	}
}