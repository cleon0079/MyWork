package comp3170.ass1;


import org.joml.AxisAngle4f;
import org.joml.Matrix4f;
import org.joml.Vector3f;

import comp3170.SceneObject;
import comp3170.Shader;
import comp3170.ShaderLibrary;

public class Camera extends SceneObject {
	private static final String VERTEX_SHADER = "simple_vertex.glsl";
	private static final String FRAGMENT_SHADER = "simple_fragment.glsl";
	
	private Shader shader;

	private Matrix4f camGraphMatrix;
	
    private Vector3f position = new Vector3f();
	private AxisAngle4f angle = new AxisAngle4f();

	private float aspect = 1f;
	
	public Camera() {
		
		shader = ShaderLibrary.instance.compileShader(VERTEX_SHADER, FRAGMENT_SHADER);
        camGraphMatrix = new Matrix4f().identity();
    }

	/**
	 * Get the camera's view matrix.
	 *
	 * @param dest A preallocated destination matrix
	 * @return	The view matrix
	 */
	public Matrix4f getViewMatrix(Matrix4f dest) {
		
		// Get the M > W matrix's position and angle
		this.getModelToWorldMatrix(camGraphMatrix).getTranslation(position);
		this.getModelToWorldMatrix(camGraphMatrix).getRotation(angle);
		
		// Save the TR and the invert to get view matrix
		dest.identity();
		dest.translate(position).rotate(angle);

		return dest.invert();	
	}

	
	/**
	 * Get the camera's projection matrix.
	 *
	 * @param dest A preallocated destination matrix
	 * @param screenWidth The width of the screen
	 * @param screenHeight The height of the screen
	 * @param worldWidth The width of the view(world unit)
	 * @param worldHeight The height of the view(world unit)
	 * @return	The projection matrix
	 */
	public Matrix4f getProjectionMatrix(Matrix4f dest, float screenWidth, float screenHeight, float worldWidth, float worldHeight) {
	    // Calculate the aspect ratio of the window
	    aspect = screenWidth / screenHeight;

	    // Set up the orthographic projection matrix
	    dest.identity();
	    dest.setOrtho(
	    		-(worldWidth * aspect) / 2.0f, 
	    		(worldWidth * aspect) / 2.0f, 
	    		-worldHeight / 2.0f,
	    		worldHeight / 2.0f, 
	    		-1.0f, 
	    		1.0f);

	    return dest;
	}
	
	@Override
	protected void drawSelf(Matrix4f mvpMatrix) {
		// Pass the mvpMatrix to the next object
		shader.setUniform("u_mvpMatrix", mvpMatrix);
	}

}
