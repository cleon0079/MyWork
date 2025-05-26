package comp3170.ass3.sceneobjects;

import org.joml.Matrix4f;

import comp3170.InputManager;
import comp3170.SceneObject;


import static comp3170.Math.TAU;


public class MiniCamera implements Camera {

    // Model matrix for the camera
	private Matrix4f modelMatrix = new Matrix4f().identity();

    // Distance from the origin
	private float distance;
	
    private float near; // Near clipping plane distance
    private float far; // Far clipping plane distance
    
    private float mapWidth; // Width of the map
    private float mapHeight; // Height of the map
    
    private float topDownAngle = -TAU/4;
    
    private SceneObject target;
    

    // Constructor
	public MiniCamera(float distance, float near, float far, float mapWidth, float mapHeight, SceneObject car) {
		this.distance = distance;
		modelMatrix.translate(0,0,distance); // Translate the camera along the z-axis
		
        this.near = near;
        this.far = far;
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        
        this.target = car;
        
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
    }

	/**
	 * Get the projection matrix
	 */
	public Matrix4f getProjectionMatrix(Matrix4f dest) {
			float halfWidth = (mapWidth / 2);
	        float halfHeight = (mapHeight / 2);
	        return dest.setOrtho(-halfWidth, halfWidth, -halfHeight, halfHeight, near, far);
	}
	
    final static float ROTATION_SPEED = TAU / 4; // Rotation speed constant
	
    // Update the camera
	public void update(InputManager input, float deltaTime) {
	    // Update the view matrix with the new orientation
	    modelMatrix.identity().set(target.getMatrix()).setRotationXYZ(topDownAngle, 0, TAU/2).translate(0, 0, distance);
		
	}
}