package comp3170.ass3.sceneobjects;

import static org.lwjgl.glfw.GLFW.GLFW_KEY_DOWN;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_LEFT;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_PAGE_DOWN;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_PAGE_UP;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_RIGHT;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_UP;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_1;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_2;
import static org.lwjgl.opengl.GL11.GL_COLOR_BUFFER_BIT;
import static org.lwjgl.opengl.GL11.GL_DEPTH_BUFFER_BIT;
import static org.lwjgl.opengl.GL11.GL_SCISSOR_TEST;
import static org.lwjgl.opengl.GL11.glClear;
import static org.lwjgl.opengl.GL11.glClearColor;
import static org.lwjgl.opengl.GL11.glDisable;
import static org.lwjgl.opengl.GL11.glEnable;
import static org.lwjgl.opengl.GL11.glScissor;
import static org.lwjgl.opengl.GL11.glViewport;

import org.joml.Matrix4f;
import org.joml.Vector3f;

import comp3170.InputManager;
import comp3170.SceneObject;

import static comp3170.Math.TAU;

public class CleonCamera implements Camera {

    // Model matrix for the camera
	private Matrix4f modelMatrix = new Matrix4f().identity();

    // Distance from the origin
	private float distance;
    // Angles representing the orientation of the camera
	private Vector3f angle;
	// If the camera is in orthographic
	private boolean isOrtho = false;
	
    private float near; // Near clipping plane distance
    private float far; // Far clipping plane distance
    private float fovy; // Field of view in the y direction
    private float aspect; // Aspect ratio

    private float zoomLevel; // Current zoom level
    private float zoomSpeedOrtho = 2f; // Speed of zooming
    private float minZoom = 0.1f; // Minimum zoom level
    private float maxZoom = 2f; // Maximum zoom level
    
    private float minFov = TAU / 24; // Minimum field of view
    private float maxFov = TAU / 4; // Maximum field of view
    private float zoomSpeedPers = TAU / 30; // Speed of zooming

    private int windowWidth = 1000; // Default Window width
    private int windowHeight = 1000; // Default Window height
    
    private float mapWidth; // Width of the map
    private float mapHeight; // Height of the map
    
    private SceneObject target; // Target scene object
    private float topDownAngle = -TAU/4;
    

    // Constructor
	public CleonCamera(float distance, float near, float far, float mapWidth, float mapHeight, float fovy, float aspect, SceneObject target) {
		this.distance = distance;
		angle = new Vector3f(topDownAngle,0,0); // Initial angles (rotation around x-axis)
		modelMatrix.translate(0,0,distance); // Translate the camera along the z-axis
		
		// OrthographicCamera
        this.near = near;
        this.far = far;
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.zoomLevel = 1f;
        
        // PerspectiveCamera
        this.fovy = fovy;
        this.aspect = aspect;
        this.target = target;
	}

	/**
	 * Get the view matrix
	 */
	public Matrix4f getViewMatrix(Matrix4f dest) {
		// Invert the model matrix to get the view matrix
		return modelMatrix.invert(dest);
	}

	/**
	 * Get the model matrix for the camera
	 */
	public Matrix4f getCameraMatrix(Matrix4f dest) {
		return modelMatrix.get(dest);
	}
	
    // Set window size
    public void setWindowSize(int width, int height) {
        this.windowWidth = width;
        this.windowHeight = height;
        this.aspect = (float) width / height; // Update aspect ratio
    	if(isOrtho) {
            if (aspect >= 1) {
                // Window is wider than the map
                mapWidth = mapHeight * aspect;
            } else {
                // Window is taller than the map
                mapHeight = mapWidth / aspect;
            }
            
            adjustViewport(); // Adjust viewport based on window and map dimensions
    	}
    	else {
	        glDisable(GL_SCISSOR_TEST); // Disable scissor test
	        glClearColor(0.53f, 0.81f, 0.92f, 1.0f); // Set clear color to sky blue
	        glClear(GL_COLOR_BUFFER_BIT); // Clear the color buffer
            glViewport(0, 0, width, height); // Set the OpenGL viewport
    	}
    }

	/**
	 * Get the projection matrix
	 */
	public Matrix4f getProjectionMatrix(Matrix4f dest) {
		if(isOrtho) {
			float halfWidth = (mapWidth / 2) * zoomLevel;
	        float halfHeight = (mapHeight / 2) * zoomLevel;
	        return dest.setOrtho(-halfWidth, halfWidth, -halfHeight, halfHeight, near, far);
		}
		else {
			return dest.setPerspective(fovy, aspect, near, far); // Set perspective projection
		}
	};
	
    final static float ROTATION_SPEED = TAU / 4; // Rotation speed constant
	
    // Update the camera
	public void update(InputManager input, float deltaTime) {
		if (input.isKeyDown(GLFW_KEY_1)) {
			isOrtho = true;
			setWindowSize(windowWidth, windowHeight);
		}
		if (input.isKeyDown(GLFW_KEY_2)) {
			isOrtho = false;
			setWindowSize(windowWidth, windowHeight);
		}
		
		if (isOrtho) {
	        // Handle zooming in
	        if (input.isKeyDown(GLFW_KEY_PAGE_UP)) {
	            zoomLevel -= zoomSpeedOrtho * deltaTime;
	            if (zoomLevel < minZoom) {
	                zoomLevel = minZoom; // Clamp to minimum zoom level
	            }
	        }

	        // Handle zooming out
	        if (input.isKeyDown(GLFW_KEY_PAGE_DOWN)) {
	            zoomLevel += zoomSpeedOrtho * deltaTime;
	            if (zoomLevel > maxZoom) {
	                zoomLevel = maxZoom; // Clamp to maximum zoom level
	            }
	        }
	        // Update the view matrix with the new orientation
	        modelMatrix.identity().rotateX(topDownAngle).translate(0, 0, distance);
		}
		else {
	        modelMatrix.set(target.getMatrix()); // Set the model matrix to the target's matrix
	        if (input.isKeyDown(GLFW_KEY_PAGE_UP)) {
	            fovy -= zoomSpeedPers * deltaTime; // Zoom in
	            if (fovy < minFov) {
	                fovy = minFov; // Clamp to minimum field of view
	            }
	        }

	        if (input.isKeyDown(GLFW_KEY_PAGE_DOWN)) {
	            fovy += zoomSpeedPers * deltaTime; // Zoom out
	            if (fovy > maxFov) {
	                fovy = maxFov; // Clamp to maximum field of view
	            }
	        }
	        
	        // Handle camera rotation
	        if (input.isKeyDown(GLFW_KEY_UP)) {
	            angle.x -= ROTATION_SPEED * deltaTime; // Rotate up
	        }
	        if (input.isKeyDown(GLFW_KEY_DOWN)) {
	            angle.x += ROTATION_SPEED * deltaTime; // Rotate down
	        }
	        if (input.isKeyDown(GLFW_KEY_LEFT)) {
	            angle.y -= ROTATION_SPEED * deltaTime; // Rotate left
	        }
	        if (input.isKeyDown(GLFW_KEY_RIGHT)) {
	            angle.y += ROTATION_SPEED * deltaTime; // Rotate right
	        }
	        
	        modelMatrix.rotateY(angle.y); // Apply heading rotation
	        modelMatrix.rotateX(angle.x); // Apply pitch rotation
	        modelMatrix.translate(0, 0, distance); // Translate the camera
		}
	}
	
    private void adjustViewport() {
        aspect = (float) windowWidth / windowHeight;
        float mapAspectRatio = mapWidth / mapHeight;

        if (aspect >= mapAspectRatio) {
            // Window is wider than the map
            int viewportWidth = windowWidth;
            int viewportHeight = (int) (windowWidth / mapAspectRatio);
            int viewportY = (windowHeight - viewportHeight) / 2;
            glViewport(0, viewportY, viewportWidth, viewportHeight);
            glEnable(GL_SCISSOR_TEST);
            glScissor(0, viewportY, viewportWidth, viewportHeight);
            glClearColor(0, 0, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        } else {
            // Window is taller than the map
            int viewportHeight = windowHeight;
            int viewportWidth = (int) (windowHeight * mapAspectRatio);
            int viewportX = (windowWidth - viewportWidth) / 2;
            glViewport(viewportX, 0, viewportWidth, viewportHeight);
            glEnable(GL_SCISSOR_TEST);
            glScissor(viewportX, 0, viewportWidth, viewportHeight);
            glClearColor(0, 0, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }
    }
}