package comp3170.ass3.sceneobjects;

import static org.lwjgl.opengl.GL11.*;
import static org.lwjgl.opengl.GL14.*;
import static org.lwjgl.opengl.GL13.GL_TEXTURE0;
import static org.lwjgl.opengl.GL13.GL_TEXTURE1;
import static org.lwjgl.opengl.GL13.glActiveTexture;
import static org.lwjgl.opengl.GL15.GL_ELEMENT_ARRAY_BUFFER;
import static org.lwjgl.opengl.GL15.glBindBuffer;
import static org.lwjgl.opengl.GL30.glGenerateMipmap;

import java.io.IOException;
import org.joml.Matrix4f;
import org.joml.Vector2f;
import org.joml.Vector4f;
import org.joml.Vector3f;
import comp3170.GLBuffers;
import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;
import comp3170.TextureLibrary;
import comp3170.OpenGLException;

public class Desert extends SceneObject {

	// File paths for vertex shader and fragment shader
    static final private String VERTEX_SHADER = "sandVertex.glsl";
    static final private String FRAGMENT_SHADER = "sandFragment.glsl";
    

    //The file path of the terrain texture
    static final private String TEXTURE = "sand.jpg";
    static final private String ROAD_TEXTURE = "road.jpg";

    private Shader shader;
    private Vector4f[] vertices;
    private int vertexBuffer;
    
    private Vector2f[] texCoords;
    private int texCoordBuffer;
    
    private int[] indices;
    private int indexBuffer;

    private Vector3f[] normals;
    private int normalBuffer;
    
    private int textureID;
    private int roadTextureID;

    public Desert(int gridSize) {
        shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
        
		// Define the vertices of the terrain
		vertices = new Vector4f[] {
				new Vector4f( gridSize / 2, 0, gridSize / 2, 1),
				new Vector4f(-gridSize / 2, 0, gridSize / 2, 1),
				new Vector4f( gridSize / 2,0, -gridSize / 2, 1),
				new Vector4f(-gridSize / 2,0, -gridSize / 2, 1),
		};
		
		// Define the indices of the two triangles forming the terrain
		indices = new int[] {
				0, 1, 2,
				3, 2, 1,
		};
		
		// Set normals for the terrain (all pointing upwards)
		normals = new Vector3f[] {
				new Vector3f(0, 1, 0),
				new Vector3f(0, 1, 0),
				new Vector3f(0, 1, 0),
				new Vector3f(0, 1, 0),
		};
		
		// Set texture coordinates to map the texture onto the terrain
		texCoords = new Vector2f[] {
				new Vector2f(gridSize, gridSize),
				new Vector2f(0, gridSize),
				new Vector2f(gridSize, 0),
				new Vector2f(0, 0),
		};

        vertexBuffer = GLBuffers.createBuffer(vertices);
        normalBuffer = GLBuffers.createBuffer(normals);
        texCoordBuffer = GLBuffers.createBuffer(texCoords);
        indexBuffer = GLBuffers.createIndexBuffer(indices);
        
        loadTexture();
    }

    private void loadTexture() {
    	
        try {
        	 // Load desert texture
            textureID = TextureLibrary.instance.loadTexture(TEXTURE);
            
            // Load road texture
            roadTextureID = TextureLibrary.instance.loadTexture(ROAD_TEXTURE);
        } catch (IOException e) {
            e.printStackTrace();
            System.exit(1);
        } catch (OpenGLException e) {
            e.printStackTrace();
            System.exit(1);
        }
        
        // Set parameters for the desert texture
        glBindTexture(GL_TEXTURE_2D, textureID);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glGenerateMipmap(GL_TEXTURE_2D);

        // Set parameters for the road texture
        glBindTexture(GL_TEXTURE_2D, roadTextureID);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glGenerateMipmap(GL_TEXTURE_2D);
    }

    @Override
    protected void drawSelf(Matrix4f mvpMatrix) {
        shader.enable();

        // Bind and set parameters for the desert texture
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, textureID);
        shader.setUniform("u_texture", 0);

        // Bind and set parameters for the road texture
        glActiveTexture(GL_TEXTURE1);
        glBindTexture(GL_TEXTURE_2D, roadTextureID);
        shader.setUniform("u_roadTexture", 1);

        shader.setUniform("u_mvpMatrix", mvpMatrix);
        shader.setAttribute("a_position", vertexBuffer);
        shader.setAttribute("a_normal", normalBuffer);
        shader.setAttribute("a_texcoord", texCoordBuffer);

        // Bind index buffer and draw elements
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
        glDrawElements(GL_TRIANGLES, indices.length, GL_UNSIGNED_INT, 0);
    }
}