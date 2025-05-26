package comp3170.ass3.sceneobjects;

import static comp3170.Math.TAU;
import static org.lwjgl.opengl.GL11.GL_FILL;
import static org.lwjgl.opengl.GL11.GL_FRONT_AND_BACK;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_2D;
import static org.lwjgl.opengl.GL11.GL_TRIANGLES;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_INT;
import static org.lwjgl.opengl.GL11.glBindTexture;
import static org.lwjgl.opengl.GL11.glDrawElements;
import static org.lwjgl.opengl.GL11.glPolygonMode;
import static org.lwjgl.opengl.GL13.GL_TEXTURE0;
import static org.lwjgl.opengl.GL13.glActiveTexture;
import static org.lwjgl.opengl.GL15.GL_ELEMENT_ARRAY_BUFFER;
import static org.lwjgl.opengl.GL15.glBindBuffer;

import org.joml.Matrix4f;
import org.joml.Vector4f;

import comp3170.InputManager;
import comp3170.SceneObject;

public class Scene extends SceneObject {

    // Singleton instance of the scene
    public static Scene instance;
    
    private Axes3D axes;

    // Scene objects
    private Desert grid;       // The ground plane (desert)
    private Car car;           // The car model
    
    private MiniMap minimap;
    
    // Axes for front wheels to rotate
    private WheelAxis axisLeft;
    private WheelAxis axisRight;
    
    // Individual wheels
    private Wheel wheelFrontLeft;
    private Wheel wheelFrontRight;
    private Wheel wheelBackLeft;
    private Wheel wheelBackRight;
    
    // Camera object
    private CleonCamera camera;
    private MiniCamera miniCamera;

    // Camera parameters
    private static final float CAMERA_DISTANCE = 20f;
    private static final float CAMERA_NEAR = 0.1f;
    private static final float CAMERA_FAR = 1000f;
    private static final float CAMERA_WIDTH = 1000f;
    private static final float CAMERA_HEIGHT = 1000f;
    private static final float CAMERA_FOVY = TAU / 6;
    private static final float CAMERA_ASPECT = 1;
    
    // Scene constructor
    public Scene() {
        instance = this; // Assign the instance to this scene

        // Add a 3D axes object for reference
        axes = new Axes3D();
        axes.setParent(this);
        
        // Create and set up the ground plane (desert)
        grid = new Desert(1000);
        grid.setParent(this);
        grid.getMatrix().rotateZ(TAU / 2); // Rotate the grid to lay flat

        // Create and set up the car
        car = new Car();
        car.setParent(this);
        
        minimap = new MiniMap();
        minimap.setParent(this);
        
        // Create and position the left wheel axis
        axisLeft = new WheelAxis();
        axisLeft.setParent(car);
        axisLeft.getMatrix().translate(0.62f, 0.35f, 1.3f);
        
        // Create and attach the front left wheel to the left axis
        wheelFrontLeft = new Wheel();
        wheelFrontLeft.setParent(axisLeft);
        
        // Create and position the right wheel axis
        axisRight = new WheelAxis();
        axisRight.setParent(car);
        axisRight.getMatrix().translate(-0.62f, 0.35f, 1.3f);
        
        // Create and attach the front right wheel to the right axis
        wheelFrontRight = new Wheel();
        wheelFrontRight.setParent(axisRight);
        
        // Create and position the back left wheel directly to the car
        wheelBackLeft = new Wheel();
        wheelBackLeft.setParent(car);
        wheelBackLeft.getMatrix().translate(0.62f, 0.35f, -1.15f).rotateY(TAU / 2);
        
        // Create and position the back right wheel directly to the car
        wheelBackRight = new Wheel();
        wheelBackRight.setParent(car);
        wheelBackRight.getMatrix().translate(-0.62f, 0.35f, -1.15f);

        // Set up the camera to follow the car
        camera = new CleonCamera(CAMERA_DISTANCE, CAMERA_NEAR, CAMERA_FAR, CAMERA_WIDTH, CAMERA_HEIGHT, CAMERA_FOVY, CAMERA_ASPECT, car);
        
        miniCamera = new MiniCamera(10, CAMERA_NEAR, CAMERA_FAR, 50, 50, car);
    }

    // Getter for the camera
    public CleonCamera getCamera() {
        return camera;
    }

    public MiniCamera getMiniCamera() {
    	return miniCamera;
    }
    
    // Update method called every frame
    public void update(InputManager input, float deltaTime) {
        // Update the camera based on input and time
        camera.update(input, deltaTime);
        miniCamera.update(input, deltaTime);
        minimap.update(input, deltaTime);
        minimap.getMatrix().set(camera.getCameraMatrix(new Matrix4f())).translate(4, 5, -10).rotateX(TAU/4).scale(2);
        
        // Update the car based on input and time
        car.update(input, deltaTime);
        
        // Update the wheel axes based on input and time
        axisLeft.update(input, deltaTime, true);
        axisRight.update(input, deltaTime, false);
        
        // Update the wheels based on input, time, and car speed
        wheelFrontLeft.update(input, deltaTime, car.speed, true);
        wheelFrontRight.update(input, deltaTime, car.speed, false);
        wheelBackLeft.update(input, deltaTime, car.speed, true);
        wheelBackRight.update(input, deltaTime, car.speed, false);
        
        
    }

    // Handle window size changes
    public void onWindowSizeChanged(int width, int height) {
        camera.setWindowSize(width, height);
        miniCamera.setWindowSize(width, height);
    }
    
    public void setTexture(int texture) {
    	minimap.setTexture(texture);
    }
}