package comp3170.ass3.sceneobjects;

import static comp3170.Math.TAU;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_A;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_D;

import comp3170.InputManager;
import comp3170.SceneObject;

public class WheelAxis extends SceneObject {
    
    // Update the wheel axis rotation based on input
    public void update(InputManager input, float deltaTime, boolean left) {

        // If the wheel axis is on the left side
        if (left) {
            if (input.isKeyDown(GLFW_KEY_A)) {
                // Rotate the axis to the left
                getMatrix().setRotationXYZ(0, TAU / 2 + TAU / 12, 0);
            } else if (input.isKeyDown(GLFW_KEY_D)) {
                // Rotate the axis to the right
                getMatrix().setRotationXYZ(0, TAU / 2 - TAU / 12, 0);
            } else {
                // Set the axis to the default straight position
                getMatrix().setRotationXYZ(0, TAU / 2, 0);
            }
        } 
        // If the wheel axis is on the right side
        else {
            if (input.isKeyDown(GLFW_KEY_A)) {
                // Rotate the axis to the left
                getMatrix().setRotationXYZ(0, TAU / 12, 0);
            } else if (input.isKeyDown(GLFW_KEY_D)) {
                // Rotate the axis to the right
                getMatrix().setRotationXYZ(0, -TAU / 12, 0);
            } else {
                // Set the axis to the default straight position
                getMatrix().setRotationXYZ(0, 0, 0);
            }
        }
    }
}