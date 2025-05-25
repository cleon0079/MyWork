package comp3170.ass3.sceneobjects;

import static org.lwjgl.opengl.GL11.GL_LINEAR;
import static org.lwjgl.opengl.GL11.GL_LINEAR_MIPMAP_LINEAR;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_2D;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_MAG_FILTER;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_MIN_FILTER;
import static org.lwjgl.opengl.GL11.glTexParameteri;
import static org.lwjgl.opengl.GL15.*;
import static org.lwjgl.glfw.GLFW.*;
import static org.lwjgl.opengl.GL30.glGenerateMipmap;

import java.io.FileNotFoundException;
import org.joml.Matrix4f;
import org.joml.Vector4f;
import comp3170.GLBuffers;
import comp3170.InputManager;
import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;
import comp3170.TextureLibrary;
import comp3170.ass3.models.Model;
import comp3170.ass3.models.Model.Mesh;

public class Car extends SceneObject {

    // File paths for model and shaders
    private static final String OBJ_FILE = "src/comp3170/ass3/models/car.obj";
    static final private String VERTEX_SHADER = "carVertex.glsl";
    static final private String FRAGMENT_SHADER = "carFragment.glsl";
    static final private String TEXTURE = "car.png";

    // Default color for the car
    private Vector4f colour = new Vector4f(1, 1, 1, 1);

    // Buffers for the body, interior, and windows
    private int vertexBufferBody;
    private int indexBufferBody;
    private int nIndicesBody;
    
    private int vertexBufferInterior;
    private int indexBufferInterior;
    private int nIndicesInterior;
    
    private int vertexBufferWindows;
    private int indexBufferWindows;
    private int nIndicesWindows;
    
    private int uvBufferBody;
    private int uvBufferInterior;
    private int uvBufferWindows;
    
    private int Texture;

    // Shader program
    private Shader shader;

    // Constructor
    public Car() {
        shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
        shader.setStrict(false);  // Disable error checking for setAttribute and setUniform
        
        // Load the car model from the OBJ file
        Model model = null;
        try {
            model = new Model(OBJ_FILE);
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        }

        // Load the body submesh
        Mesh body = model.getMesh("Body");
        vertexBufferBody = GLBuffers.createBuffer(body.vertices);
        indexBufferBody = GLBuffers.createIndexBuffer(body.indices);
        nIndicesBody = body.indices.length;
        uvBufferBody = GLBuffers.createBuffer(body.uvs);

        // Load the interior submesh
        Mesh interior = model.getMesh("Interior");
        vertexBufferInterior = GLBuffers.createBuffer(interior.vertices);
        indexBufferInterior = GLBuffers.createIndexBuffer(interior.indices);
        nIndicesInterior = interior.indices.length;
        uvBufferInterior = GLBuffers.createBuffer(interior.uvs);

        // Load the windows submesh
        Mesh windows = model.getMesh("Windows");
        vertexBufferWindows = GLBuffers.createBuffer(windows.vertices);
        indexBufferWindows = GLBuffers.createIndexBuffer(windows.indices);
        nIndicesWindows = windows.indices.length;
        uvBufferWindows = GLBuffers.createBuffer(windows.uvs);

        // Load the textures for the car
        loadTextures();
    }

    // Load the car's texture
    private void loadTextures() {    
        try {
            Texture = TextureLibrary.instance.loadTexture(TEXTURE);
        } catch (Exception e) {
            e.printStackTrace();
        }     

        // Set the texture parameters
        glBindTexture(GL_TEXTURE_2D, Texture);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT); // Horizontal wrapping
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT); // Vertical wrapping
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glGenerateMipmap(GL_TEXTURE_2D);    
    }

    @Override
    protected void drawSelf(Matrix4f mvpMatrix, int pass) {
        shader.enable();

        // Set the uniform variables in the shader
        shader.setUniform("u_mvpMatrix", mvpMatrix);
        colour = new Vector4f(1, 1, 1, 1);
        shader.setUniform("u_colour", colour);

        // Bind the texture
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, Texture);
        shader.setUniform("u_texture", 0);

        // Draw the body
        shader.setAttribute("a_position", vertexBufferBody);
        shader.setAttribute("a_texcoord", uvBufferBody);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferBody);
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
        glDrawElements(GL_TRIANGLES, nIndicesBody, GL_UNSIGNED_INT, 0);

        // Draw the interior
        shader.setAttribute("a_position", vertexBufferInterior);
        shader.setAttribute("a_texcoord", uvBufferInterior);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferInterior);
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
        glDrawElements(GL_TRIANGLES, nIndicesInterior, GL_UNSIGNED_INT, 0);

        // Draw the windows with transparency
        colour = new Vector4f(1, 1, 1, 0.5f);
        shader.setUniform("u_colour", colour);
        shader.setAttribute("a_position", vertexBufferWindows);
        shader.setAttribute("a_texcoord", uvBufferWindows);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferWindows);
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
        glDrawElements(GL_TRIANGLES, nIndicesWindows, GL_UNSIGNED_INT, 0);
    }

    // Gear speeds
    private final float gear [] = {
        -2.5f, 0 , 2.5f, 5 , 7.5f, 10
    };

    // Current speed and gear index
    private float SPEED = 0;
    private int i = 1;
    public float speed = 0;

    // Update the car's position and rotation based on input
    public void update(InputManager input, float deltaTime) {
        SPEED = gear[i];

        // Shift up a gear
        if (input.wasKeyPressed(GLFW_KEY_W)) {
            if(i < 5) {
                i++;
            }
        }

        // Shift down a gear
        if (input.wasKeyPressed(GLFW_KEY_S)) {
            if(i > 0) {
                i--;
            }
        }

        // Calculate speed and move the car
        speed = SPEED * deltaTime;
        getMatrix().translate(0, 0, speed);

        // Calculate turn speed and rotate the car
        float turnSpeed = (SPEED / 5) * deltaTime;
        if (input.isKeyDown(GLFW_KEY_A)) {
            getMatrix().rotateY(turnSpeed);
        }

        if (input.isKeyDown(GLFW_KEY_D)) {
            getMatrix().rotateY(-turnSpeed);
        }    
    }
}