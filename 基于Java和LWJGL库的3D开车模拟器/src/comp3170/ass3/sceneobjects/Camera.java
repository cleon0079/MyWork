package comp3170.ass3.sceneobjects;

import org.joml.Matrix4f;
import comp3170.InputManager;

public interface Camera {
    // Get the view matrix of the camera
    public Matrix4f getViewMatrix(Matrix4f dest);

    // Get the projection matrix of the camera
    public Matrix4f getProjectionMatrix(Matrix4f dest);

    // Update the camera based on user input and elapsed time
    public void update(InputManager input, float deltaTime);
}