package comp3170.ass1;

import java.awt.Color;
import java.util.Random;

import org.joml.Matrix4f;
import org.joml.Vector3f;
import org.joml.Vector4f;

import comp3170.InputManager;
import comp3170.SceneObject;
import static comp3170.Math.TAU;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_DOWN;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_LEFT;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_RIGHT;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_UP;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_W;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_A;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_S;
import static org.lwjgl.glfw.GLFW.GLFW_KEY_D;

import comp3170.ass1.sceneobjects.Window;
import comp3170.ass1.sceneobjects.Beam;
import comp3170.ass1.sceneobjects.Building;
import comp3170.ass1.sceneobjects.Star;
import comp3170.ass1.sceneobjects.UFO;


public class Scene extends SceneObject {
	
	// Root of all object
	public static Scene root;
	// The world unit height and width
	private float worldWidth;
	private float worldHeight;
	// The aspect ratio of the screen
	private float aspect = 1f;
	Random random;

	
	// The building's pivot that connect to the root
	private SceneObject bubildingPivot;
	// All the building object array
	private Building[] building;
	// All the windows object array
	private Window[][] windows;
	
	// Size of the building
	private int buildingSize = 5;
	// Number of buildings
	private int buildingCount = 500;
	// Level for each building
	private int buildingLevel;
	// The minimum height for building
	private int buildingHeightMin = 10;
	// The max height for building
	private int buildingHeightMax = 50;
	// Color for each building
	private Color buildingColor;
	// A random position.x for each building
	private float randomBuildingPosition;
	// Given two color for window
	Color[] windowColors = {Color.yellow, Color.black};
	// The window Off set(left and right)
	private float windowOffSet = 0.2f;
	// Size of the window
	private float windowSize = 0.2f;
	
	
	// The star's pivot that connect to the root
	private SceneObject starPivot;
	// All the star object array
	private Star[] stars;
	
	// The amount of the star
	private int starCount = 1000;
	// How many point the star has
	private int starPoint = 5;
	// A random position.x for each star
	private float randomStarPositionX;
	// A random position.y for each star
	private float randomStarPositionY;
	// A random rotation for each star
	private float randomStarRotate;
	// A random size for each star
	private float randomStarSize;
	// The minimum size for the star
	private float starSizeMin = 1f;
	// The max size for the star
	private float starSizeMax = 2f;
	
	
	// The UFO's pivot that connect to the root
	private SceneObject ufoPivot;
	// The UFO body object
	private UFO ufo;
	// The UFO head Object
	private UFO ufoHead;
	// The UFO tractor beam object
	private Beam beam;
	
	// A boolean to check if is using vertex coloring
	private boolean isVertexColor = true;
	// Size of the UFO
	private float ufoSize = 3f;
	// The current position of the UFO
	private Vector3f ufoStartPosition = new Vector3f();
	// The movement speed of the UFO
	private float moveSpeed = 10f;
	// The scaling speed of the beam
	private float beamScaleSpeed = 2f;
	// The boundary for the beam to shrink back to the UFO
	private float upScaleBoundary = 0.35f;
	// The rotation speed of the beam to rotate
	private float rotationSpeed = TAU/3;
	// The rotation boundary for the beam on left and right
	private float rotationBoundary = TAU/6;
	// A vec3 to store the current scale of the beam
	private Vector3f beamCurrentScale = new Vector3f();
	// The beam's current rotating angle on Z
	private float beamCurrentRotationZ = 0f;
	
	
	// Camera object
	public Camera camera;
	// A matrix for calculation
	private Matrix4f ufoCalMatrix = new Matrix4f().identity();
	// A vec3 to save the position of UFO from UFO model to world matrix
	private Vector3f ufoWorldPosition = new Vector3f();
	// A vec4 to save the position of UFO from UFO in view space
	private Vector4f ufoWorldPositionVec4 = new Vector4f();
	// The boundary for the UFO(When hit then camera follow)
	private float cameraBoundaryForUFO;
	// The size of the boundary for UFO in view port
	private float cameraBoundarySize = 1f / 4f;
	
	/**
	 * The root of all object
	 *
	 * @param worldWidth The world unit of the view on width
	 * @param worldHeight The world unit of the view on height
	 */
	public Scene(float worldWidth, float worldHeight)
	{
		// Set the root to this object
		root = this;
		random = new Random();
		// Store the width and height
		this.worldWidth = worldWidth;
		this.worldHeight = worldHeight;
			
		// Draw the star in the back
		starPivot = new SceneObject();
		starPivot.setParent(root);
		starPivot.getMatrix();
		
		// Then draw the building in front of the stars
		bubildingPivot = new SceneObject();
		bubildingPivot.setParent(root);
		
		// Then draw the UFO on top of buildings and stars, and scale the UFO size
		ufoPivot = new SceneObject();
		ufoPivot.setParent(root);
		ufoPivot.getMatrix()
		.scale(ufoSize);
		
		camera = new Camera();
		camera.setParent(root);
		// Keep the camera's size same as the UFO
		camera.getMatrix()
		.scale(ufoSize);
		
		building = new Building[buildingCount];
		windows = new Window[buildingCount][];
		stars = new Star[starCount];
		
		drawStar();
		drawBuilding();
		drawUFO();
		
		// Get the starting position for the UFO
		ufoPivot.getMatrix().getTranslation(ufoStartPosition);
		// Set up the boundary for the UFO for camera to follow
		cameraBoundaryForUFO = worldWidth * cameraBoundarySize * aspect;
	}
	
	/**
	 * Updates the state of the UFO and camera.
	 *
	 * @param dt The delta time
	 * @param input The input manager instance
	 */
	public void update(float dt, InputManager input) {
		
		ufoMovement(dt, input);
		beamScaleAndRotate(dt, input);
		// Clear the input after use
		input.clear();
	}
	
	/**
	 * Updates the aspect when resize the window
	 *
	 * @param width The window's width
	 * @param heidht The window's height
	 */
	public void updateAspect(float width, float heidht) {
		this.aspect = width / heidht;
	}
	
	/**
	 * If UFO hit the boundary then camera move with UFO
	 *
	 * @param dt The delta time
	 * @param input The input manager instance
	 */
	private void cameraFollow(float dt, InputManager input) {
		// Get the UFO's world position by pass in a matrix for calculate the model to world matrix
		ufoPivot.getModelToWorldMatrix(ufoCalMatrix).getTranslation(ufoWorldPosition);
		// Pass the vec3 position from model to world matrix to a vec4 position
		ufoWorldPositionVec4 = new Vector4f(ufoWorldPosition, 1);
		// Use the vec4 position times the view matrix to get the UFO position in view coordinate
		ufoWorldPositionVec4.mul(camera.getViewMatrix(ufoCalMatrix));
		// Update the new boundary for the UFO
		cameraBoundaryForUFO = worldWidth * cameraBoundarySize * aspect;
		
		// If the UFO is crossing the boundary of the camera, then the camera move with the UFO
		if (ufoWorldPositionVec4.x <= -cameraBoundaryForUFO) {
		    camera.getMatrix()
			.translate(ufoStartPosition.x - moveSpeed * dt, 0f, 0f);
	    }
		if (ufoWorldPositionVec4.x >= cameraBoundaryForUFO) {
		    camera.getMatrix()
			.translate(ufoStartPosition.x + moveSpeed * dt, 0f, 0f);
		}
		
	}
	
	/**
	 * Movement of the UFO.
	 *
	 * @param dt The delta time
	 * @param input The input manager instance
	 */
	private void ufoMovement(float dt, InputManager input) {
		// When press W, move up on the Y by speed from the old position
		if (input.isKeyDown(GLFW_KEY_W)) {
			ufoPivot.getMatrix()
			.translate(ufoStartPosition.x, ufoStartPosition.y + moveSpeed * dt, 0f);
		}
		// When press A, move left on the X by speed from the old position, and move the camera
		if (input.isKeyDown(GLFW_KEY_A)) {
			ufoPivot.getMatrix()
			.translate(ufoStartPosition.x - moveSpeed * dt, ufoStartPosition.y, 0f);

			cameraFollow(dt,input);
		}
		// When press S, move down on the Y by speed from the old position
		if (input.isKeyDown(GLFW_KEY_S)) {
			ufoPivot.getMatrix()
			.translate(ufoStartPosition.x, ufoStartPosition.y - moveSpeed * dt, 0f);
		}
		// When press D, move right on the X by speed from the old position, and move the camera
		if (input.isKeyDown(GLFW_KEY_D)) {
			ufoPivot.getMatrix()
			.translate(ufoStartPosition.x + moveSpeed * dt, ufoStartPosition.y, 0f);

			cameraFollow(dt,input);
		}
	}
	
	/**
	 * Scaling and rotation of the beam.
	 *
	 * @param dt The delta time
	 * @param input The input manager instance
	 */
	private void beamScaleAndRotate(float dt, InputManager input) {
		// When press arrow UP then shrink the beam
		if(input.isKeyDown(GLFW_KEY_UP)) {
			beam.getMatrix().getScale(beamCurrentScale);
			
			// Only scale down when is in the boundary
			if(beamCurrentScale.y > upScaleBoundary) {
				beam.getMatrix()
				.scale(1f, 1f - beamScaleSpeed * dt, 1f);
			}
		}
		// When press arrow DOWN then grow the beam
		if(input.isKeyDown(GLFW_KEY_DOWN)) {
			beam.getMatrix()
			.scale(1f, 1f + beamScaleSpeed * dt, 1f);
		}
		// When press arrow LEFT then rotate the beam to the left
		if(input.isKeyDown(GLFW_KEY_LEFT)) {
			float newRotation = beamCurrentRotationZ - rotationSpeed * dt;
			beam.getMatrix().getScale(beamCurrentScale);
			
			// Only rotate when the beam is in the boundary
			if(Math.abs(newRotation) <= rotationBoundary) {
				beamCurrentRotationZ = newRotation;
				
				// Scale back to normal size then rotate then scale back
				beam.getMatrix()
				.scale(1f / beamCurrentScale.x, 1f / beamCurrentScale.y, 1f / beamCurrentScale.z)
				.rotateZ(-rotationSpeed * dt)
				.scale(beamCurrentScale);
			}
		}
		// When press arrow RIGHT then rotate the beam to the right
		if(input.isKeyDown(GLFW_KEY_RIGHT)) {
			float newRotation = beamCurrentRotationZ + rotationSpeed * dt;
			beam.getMatrix().getScale(beamCurrentScale);
			
			// Only rotate when the beam is in the boundary
			if(Math.abs(newRotation) <= rotationBoundary) {
				beamCurrentRotationZ = newRotation;
				
				// Scale back to normal size then rotate then scale back
				beam.getMatrix()
				.scale(1f / beamCurrentScale.x, 1f / beamCurrentScale.y, 1f / beamCurrentScale.z)
				.rotateZ(rotationSpeed * dt)
				.scale(beamCurrentScale);
			}
		}
	}
	
	/**
	 *  Draw the UFO body, head and the beam
	 */
	private void drawUFO() {
		// Create a beam object using vertex coloring
		beam = new Beam(isVertexColor);
		beam.setParent(ufoPivot);
		beam.getMatrix()
		.scale(1f, 1.5f, 1f); // Make the beam longer at the beginning(look nicer)
		
		// Create the UFO body but not using vertex coloring
		ufo = new UFO(!isVertexColor);
		ufo.setParent(ufoPivot);
		ufo.setColour(Color.gray);
		
		// Create the UFO head using vertex coloring
		ufoHead = new UFO(isVertexColor);
		ufoHead.setParent(ufo);
		ufoHead.getMatrix()
		.translate(0.0f, 0.2f, 0.0f) // Move a bit to the top to make it as a head
		.scale(0.7f, 0.8f, 1f); // Make it smaller to look like a head
	}
	
	/**
	 *  Draw out all the star in the stars array
	 */
	private void drawStar() {
		for(int i = 0; i < stars.length; i++){
			// Draw the star with given how many pointer 
			stars[i] = new Star(starPoint);
			stars[i].setParent(starPivot);
			stars[i].setColour(Color.yellow);
			
			// Get random position for each star, get random rotation for each star and random size for each star
			getRandomStar();
			
			stars[i].getMatrix()
			.translate(randomStarPositionX, randomStarPositionY, 0)
			.rotateZ(randomStarRotate)
			.scale(randomStarSize);
		}
	}
	
	/**
	 *  Get random position, random rotation, random size for each star
	 */
	private void getRandomStar() {
		// Get a random position.x for each star between (-250, 250) world unit in float(500x100 space)
		randomStarPositionX = random.nextFloat(worldWidth * 5f) - (worldWidth * 2.5f);
		// Get a random position.y for each building between (-50, 50) world unit in float(500x100 space)
		randomStarPositionY = random.nextFloat(worldHeight) - (worldHeight / 2f);
		// Get a random rotation for each star
		randomStarRotate = TAU / random.nextFloat();
		// Get a random size for each star between (1,2) world unit
		randomStarSize = random.nextFloat(starSizeMax - starSizeMin) + starSizeMin;
	}
	
	/**
	 *  Get random color, random level, random position for each building
	 */
	private void getRandomBuilding() {
		// For the body of the building, Purple color
		// With a randomly chosen saturation value from0% to 100%
		// And a brightness of 50% in HSB space
		buildingColor = Color.getHSBColor(0.7f, random.nextFloat(), 0.5f);
		// Get a random building level between [2, 9] in integer(10 to 50 world unit)
		buildingLevel = random.nextInt((buildingHeightMax / buildingSize) -  (buildingHeightMin / buildingSize)) + (buildingHeightMin / buildingSize);
		// Get a random position for each building between (-250, 250) world unit in float(500x100 space)
		randomBuildingPosition = random.nextFloat(worldWidth * 5f) - (worldWidth * 2.5f);
	}
	
	/**
	 *  Draw out all the building in the building array
	 */
	private void drawBuilding() {
		for(int i = 0; i < building.length; i++) {
			// Get random color, random level, random position for each building
			getRandomBuilding();
			building[i] = new Building(buildingLevel);
			building[i].setParent(bubildingPivot);
			building[i].setColour(buildingColor);
			
			// Place the building in a random position.x between (-250, 250) world unit
			// Place the building to the bottom
			// Scale the building to a proper size
			building[i].getMatrix()
			.translate(randomBuildingPosition, -worldHeight / 2f, 0f)
			.scale(buildingSize);
			
			// Draw the window for each building
			drawWindows(i);
		}
	}

	/**
	 * Draw the windows for the building
	 *
	 * @param id The index for this building
	 */
	private void drawWindows(int id) {
		// Each building have each amount of windows
		windows[id] = new Window[(buildingLevel - 1) * 2];
		
		// Index to check is it time to draw the next level of windows
		int windowEachLevel = 0;
		// Index to store if it is next level then add 1
		int level = 1;
		// Check if it is the left window or not
		boolean leftWindow = true;

		for(int i = 0; i < windows[id].length; i++) {
			windows[id][i] = new Window();
			windows[id][i].setParent(building[id]);
			
			// Get random color from yellow or black
			Color color = windowColors[random.nextInt(windowColors.length)];
			windows[id][i].setColour(color);
			
			// If we have draw 2 windows, then go to next level to draw
			if(windowEachLevel >= 2) {
				windowEachLevel = 0;
				level++;
			}
			
			// Place the left and right window on its level, and scale to the window's size
			windows[id][i].getMatrix()
			.translate((leftWindow ? windowOffSet : -windowOffSet), level, 0f)
			.scale(windowSize);
			
			// Add the index and turn the boolean
			windowEachLevel++;
			leftWindow = !leftWindow;
		}
	}
}
