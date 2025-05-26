package comp3170.ass3;

import static org.lwjgl.glfw.GLFW.*;
import static org.lwjgl.opengl.GL11.*;
import static org.lwjgl.opengl.GL13.*;
import static org.lwjgl.opengl.GL30.*;

import java.io.File;
import org.joml.Matrix4f;

import comp3170.GLBuffers;
import comp3170.IWindowListener;
import comp3170.InputManager;
import comp3170.OpenGLException;
import comp3170.ShaderLibrary;
import comp3170.TextureLibrary;
import comp3170.Window;
import comp3170.ass3.sceneobjects.CleonCamera;
import comp3170.ass3.sceneobjects.MiniCamera;
import comp3170.ass3.sceneobjects.MiniMap;
import comp3170.ass3.sceneobjects.Scene;

public class Assignment3 implements IWindowListener {

    private static final File SHADER_DIRS = new File("src/comp3170/ass3/shaders");
    private static final File TEXTURES_DIR = new File("src/comp3170/ass3/textures");
    
    private Window window;
    private int screenWidth = 1000;
    private int screenHeight = 1000;
    private Scene scene;
    private InputManager input;
    private long oldTime;
    
    private int frameBuffer;
    private int renderTexture;

    public Assignment3() throws OpenGLException {
        window = new Window("Assignment 3", screenWidth, screenHeight, this);
        window.setResizable(true);
        // Using 4x multisampling.
        glfwWindowHint(GLFW_SAMPLES, 4);
        window.run();
    }
    
    @Override
    public void init() {

    	// Enable multisampling for smoother rendering
        glEnable(GL_MULTISAMPLE);
        
        // Set background color to light blue
        glClearColor(0.53f, 0.81f, 0.92f, 1.0f);
        
        // Enable depth testing
        glClearDepth(1f);
        glEnable(GL_DEPTH_TEST);
        
        // Enable alpha blending
        glEnable(GL_BLEND);
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
        glDepthFunc(GL_LEQUAL);
        
        // Enable back-face culling
        glEnable(GL_CULL_FACE);
        glCullFace(GL_BACK);
        
        // Texture filtering settings
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glGenerateMipmap(GL_TEXTURE_2D);
 
        // Enable sRGB framebuffer for gamma correction
        glEnable(GL_FRAMEBUFFER_SRGB);
        
        // Load shader and texture libraries
        new ShaderLibrary(SHADER_DIRS);
        new TextureLibrary(TEXTURES_DIR);
        
        renderTexture = TextureLibrary.instance.createRenderTexture(screenWidth, screenHeight, GL_RGBA);
        
        try {
            frameBuffer = GLBuffers.createFrameBuffer(renderTexture);
        } catch (OpenGLException e) {
            e.printStackTrace();
            System.exit(1);
        }

        // Set up the scene
        scene = new Scene();
        scene.setTexture(renderTexture);

        // Initialize oldTime
        input = new InputManager(window);
        oldTime = System.currentTimeMillis();
    }

    private void update() {
        long time = System.currentTimeMillis();
        float deltaTime = (time - oldTime) / 1000f;
        oldTime = time;
        
        scene.update(input, deltaTime);

        input.clear();
    }

    private Matrix4f viewMatrix = new Matrix4f();
    private Matrix4f projectionMatrix = new Matrix4f();
    private Matrix4f mvpMatrix = new Matrix4f();

    @Override
    public void draw() {
        update();
        
        // Render to the framebuffer
        glBindFramebuffer(GL_FRAMEBUFFER, frameBuffer);
        // Clear color and depth buffers
        glClearDepth(1);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // Get view and projection matrices from the camera
        MiniCamera miniCamera = scene.getMiniCamera();
        miniCamera.getViewMatrix(viewMatrix);
        miniCamera.getProjectionMatrix(projectionMatrix);
        
        // Calculate model-view-projection matrix
        mvpMatrix.set(projectionMatrix).mul(viewMatrix);
        scene.draw(mvpMatrix);
        
        // Render to the screen
        glBindFramebuffer(GL_FRAMEBUFFER, 0);
        glViewport(0, 0, screenWidth, screenHeight);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        CleonCamera camera = scene.getCamera();
        camera.getViewMatrix(viewMatrix);
        camera.getProjectionMatrix(projectionMatrix);
        mvpMatrix.set(projectionMatrix).mul(viewMatrix);

        scene.draw(mvpMatrix);
        scene.setTexture(renderTexture);
    }
    
    @Override
    public void resize(int width, int height) {
        screenWidth = width;
        screenHeight = height;
        
        // Update scene with new window size and set viewport
        scene.onWindowSizeChanged(width, height);
        glViewport(0, 0, width, height);
    }

    @Override
    public void close() {}

    public static void main(String[] args) throws OpenGLException {
        new Assignment3();
    }
}