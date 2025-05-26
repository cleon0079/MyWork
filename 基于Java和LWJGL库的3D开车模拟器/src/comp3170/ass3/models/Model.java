package comp3170.ass3.models;

import static org.lwjgl.assimp.Assimp.aiImportFile;
import static org.lwjgl.assimp.Assimp.aiReleaseImport;

import java.io.File;
import java.io.FileNotFoundException;
import java.nio.IntBuffer;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;

import org.joml.Vector2f;
import org.joml.Vector4f;
import org.lwjgl.PointerBuffer;
import org.lwjgl.assimp.AIFace;
import org.lwjgl.assimp.AIMaterial;
import org.lwjgl.assimp.AIMaterialProperty;
import org.lwjgl.assimp.AIMesh;
import org.lwjgl.assimp.AIScene;
import org.lwjgl.assimp.AIVector3D;
import org.lwjgl.assimp.Assimp;

/**
 * A simple interface to the Assimp (Asset Importer) library, to import models
 * from Wavefront OBJ files. 
 * 
 * Note that a model may contain multiple meshes.
 * 
 * Example usage, to get the mesh 'MeshName' from the OBJ file path/filename.obj
 * 
 *    Model model = new Model("path/filename.obj");
 *    Mesh mesh = model.getMesh("MeshName");
 *    int vertexBuffer = GLBuffers.createBuffer(mesh.vertices);
 *    int indexBuffer = GLBuffers.createIndexBuffer(mesh.indices);
 *    
 * @author malcolmryan
 *
 */

public class Model {
	private Map<String,Mesh> meshes;
	private Material[] materials;


	public class Mesh {
		public Vector4f[] vertices;		// the vertex positions
		public Vector4f[] normals;		// the vertex normals
		public Vector2f[] uvs;			// the texture coords
		public int[] indices;			// the index buffer 
		public Material material;		// currently unused 	
	}
	
	// TODO: Implement materials
	public class Material {
		public Material(AIMaterial aiMaterial) {
		}		
	}
	
	/**
	 * Load a model from an OBJ file
	 * 
	 * @param filename
	 * @throws FileNotFoundException
	 */
	
	public Model(String filename) throws FileNotFoundException {
		File file = new File(filename);
		if (!file.exists()) {
			throw new FileNotFoundException(String.format("Model file '%s' not found.", filename));
		}
		AIScene scene = aiImportFile(filename, Assimp.aiProcess_Triangulate);

		meshes = new HashMap<>();
		
		PointerBuffer materialsBuffer = scene.mMaterials();
		materials = new Material[materialsBuffer.limit()]; 
		for (int i = 0; i < materials.length; i++) {
			materials[i] = new Material(AIMaterial.create(materialsBuffer.get(i)));
		}
		
		PointerBuffer meshes = scene.mMeshes();
		for (int i = 0; i < meshes.limit(); i++) {
			AIMesh mesh = AIMesh.create(meshes.get(i));
			processMesh(scene, mesh);
		}
		
		aiReleaseImport(scene);
	}

	private void processMesh(AIScene scene, AIMesh aiMesh) {

		String name = aiMesh.mName().dataString();
		System.out.println("Processing mesh: " + name);

		Mesh mesh = new Mesh();
		meshes.put(name, mesh);

		AIVector3D.Buffer mVertices = aiMesh.mVertices();
		mesh.vertices = new Vector4f[mVertices.limit()];		
		for (int i = 0; i < mVertices.limit(); i++) {
			AIVector3D v = mVertices.get(i);
			mesh.vertices[i] = new Vector4f(v.x(), v.y(), v.z(), 1);
		}

		AIVector3D.Buffer mNormals = aiMesh.mNormals();
		mesh.normals = new Vector4f[mNormals.limit()];
		for (int i = 0; i < mNormals.limit(); i++) {
			AIVector3D n = mNormals.get(i);
			mesh.normals[i] = new Vector4f(n.x(), n.y(), n.z(), 0);
		}

		AIVector3D.Buffer mTextureCoords = aiMesh.mTextureCoords(0);
		mesh.uvs = new Vector2f[mTextureCoords.limit()]; 
		for (int i = 0; i < mTextureCoords.limit(); i++) {
			AIVector3D uv = mTextureCoords.get(i);
			mesh.uvs[i] = new Vector2f(uv.x(), uv.y());
		}
		
		AIFace.Buffer mFaces = aiMesh.mFaces();
		mesh.indices = new int[mFaces.limit() * 3];	// note: assuming all faces are tris
		int k = 0;
		for (int i = 0; i < mFaces.limit(); i++) {
			AIFace aiFace = mFaces.get(i);
			IntBuffer mIndices = aiFace.mIndices();
			for (int j = 0; j < mIndices.limit(); j++) {
				mesh.indices[k++] = mIndices.get(j);
			}
		}

		mesh.material = materials[aiMesh.mMaterialIndex()];
	}
	
	/**
	 * Get the names of all the meshes in this model.
	 * @return
	 */
	public Set<String> getMeshes() {
		return meshes.keySet();
	}
	
	/**
	 * Get the mesh data for a particular mesh in the model.
	 * 
	 * @param meshName
	 * @return
	 */
	public Mesh getMesh(String meshName) {
		if (!meshes.containsKey(meshName)) {
			throw new IllegalArgumentException(String.format("Mesh '%s' does not exist", meshName));
		}
		
		return meshes.get(meshName);
	}


}
