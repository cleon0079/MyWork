package comp3170.ass1;


import static org.lwjgl.opengl.GL11.GL_COLOR_BUFFER_BIT;
import static org.lwjgl.opengl.GL11.glClear;
import static org.lwjgl.opengl.GL11.glClearColor;
import static org.lwjgl.opengl.GL11.glViewport;

import java.io.File;

import org.joml.Matrix4f;

import comp3170.IWindowListener;
import comp3170.InputManager;
import comp3170.OpenGLException;
import comp3170.ShaderLibrary;
import comp3170.Window;

public class Assignment1 implements IWindowListener {
	
	public static Assignment1 instance;
	
	// Size of the screen
	private int screenWidth = 1000;
	private int screenHeight = 1000;
	
	// Size of the viewPort(world)
	private int viewWidth = 100;
	private int viewHeight = 100;
	
	final private File DIRECTORY = new File("src/comp3170/ass1/shaders");
	
	private Scene scene;
	private Window window;
	
	private InputManager input;
	private long oldTime;
	
	private Matrix4f mvpMatrix = new Matrix4f().identity();
	private Matrix4f viewMatrix= new Matrix4f().identity();
	private Matrix4f projectionMatrix = new Matrix4f().identity();
	
	public Assignment1() throws OpenGLException {
		instance = this;
		window = new Window("Assignment 1", screenWidth, screenHeight, this);
		window.setResizable(true);
		window.run();
	}

	@Override
	public void init() {
		
		input = new InputManager(window);
		
		new ShaderLibrary(DIRECTORY);
		
		// Pass the world size to scene
		scene = new Scene(viewWidth, viewHeight);
		
		oldTime = System.currentTimeMillis();
		
		// Set the background to dark blue
		glClearColor(0.0f,0.f,0.2f,1.0f);
	}
	
	private void update() {
		// Get the delta time 
		long time = System.currentTimeMillis();
		float deltaTime = (time - oldTime) / 1000f;
		oldTime = time;
		
		// Pass the delta time and the input to scene's update
		scene.update(deltaTime, input);
	}


	@Override
	public void draw() {	
		// Get the camera and use it as a mvpMatrix every draw is called
		scene.camera.getViewMatrix(viewMatrix);
		scene.camera.getProjectionMatrix(projectionMatrix, screenWidth, screenHeight, viewWidth, viewHeight);
		projectionMatrix.mul(viewMatrix, mvpMatrix);
		
		update();
		glClear(GL_COLOR_BUFFER_BIT);
		
		// Use mvpMatrix as the whole matrix to draw
		scene.draw(mvpMatrix);
	}

	@Override
	public void resize(int width, int height) {
		this.screenWidth = width;
		this.screenHeight = height;
		
		// Update the aspect when resize the window
		scene.updateAspect(width, height);
		
		// Keep drawing in full screen and the center is (0, 0)
		glViewport(0,0,width,height);
	}

	@Override
	public void close() {
	}
	
	public static void main(String[] args) throws OpenGLException {
		new Assignment1();
	}
}
