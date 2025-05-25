package comp3170.ass3.sceneobjects;

import static org.lwjgl.opengl.GL11.GL_FILL;
import static org.lwjgl.opengl.GL11.GL_FRONT_AND_BACK;
import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glDrawElements;
import static org.lwjgl.opengl.GL11.glPolygonMode;
import static org.lwjgl.opengl.GL15.*;

import java.io.FileNotFoundException;
import org.joml.Matrix4f;
import comp3170.GLBuffers;
import comp3170.InputManager;
import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;
import comp3170.ass3.models.Model;
import comp3170.ass3.models.Model.Mesh;

public class Wheel extends SceneObject {

    // File paths for model and shaders
    private static final String OBJ_FILE = "src/comp3170/ass3/models/wheel.obj";
    static final private String VERTEX_SHADER = "wheelVertex.glsl";
    static final private String FRAGMENT_SHADER = "wheelFragment.glsl";

    // Buffers for the wheel
    private int vertexBufferWheel;
    private int indexBufferWheel;
    private int nIndicesWheel;
    private int uvBufferWheel;

    // Shader program
    private Shader shader;

    // Constructor
    public Wheel() {
        shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
        shader.setStrict(false);  // Disable error checking for setAttribute and setUniform

        // Load the wheel model from the OBJ file
        Model model = null;
        try {
            model = new Model(OBJ_FILE);
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        }

        // Get the Wheel submesh
        Mesh wheel = model.getMesh("Wheel");
        vertexBufferWheel = GLBuffers.createBuffer(wheel.vertices);
        indexBufferWheel = GLBuffers.createIndexBuffer(wheel.indices);
        nIndicesWheel = wheel.indices.length;
        uvBufferWheel = GLBuffers.createBuffer(wheel.uvs);
    }

    @Override
    protected void drawSelf(Matrix4f mvpMatrix, int pass) {
        shader.enable();

        // Set the uniform variables in the shader
        shader.setUniform("u_mvpMatrix", mvpMatrix);

        // Draw the wheel
        shader.setAttribute("a_position", vertexBufferWheel);
        shader.setAttribute("a_texcoord", uvBufferWheel);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferWheel);
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
        glDrawElements(GL_TRIANGLES, nIndicesWheel, GL_UNSIGNED_INT, 0);
    }

    // Update the wheel's rotation based on speed and direction
    public void update(InputManager input, float deltaTime, float speed, boolean left) {
        if (left) {
            getMatrix().rotateX(-speed);
        } else {
            getMatrix().rotateX(speed);
        }
    }
}