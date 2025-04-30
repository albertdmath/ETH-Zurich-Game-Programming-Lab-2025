
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Assimp;
using System;
using Microsoft.Xna.Framework;
using Assimp.Configs;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using src.GameObjects;
using Assimp.Unmanaged;



/** 
Custom Vertex Struct for GameModel
**/

[StructLayout(LayoutKind.Sequential)]
public struct GameModelVertex : IVertexType
{

    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoord;   

    public Vector4 BoneIds;

    public Vector4 BoneWeights;

    public GameModelVertex(Vector3 pos, Vector3 norm, Vector2 texCoord)
    {
        Position = pos;
        Normal = norm;
        this.TexCoord = texCoord;
        BoneIds = new Vector4(0,0,0,0); // -1 means no bone assigned
        BoneWeights = new Vector4(1.0f,1.0f,1.0f,1.0f); // 0 means no weight assigned
    }


    
//     public Vector3 Position
// {
//     get { return position; }
//     set { position = value; }
// }


//     public Vector3 Normal
// {
//     get { return normal; }
//     set { normal = value; }
// }

// public Vector2 TexCoord
// {
//     get { return texCoord; }
//     set { texCoord = value; }
// }


// public Vector4 BoneIds {
//     get { return boneIds; }
//     set { boneIds = value; }
// }

// public Vector4 BoneWeights {
//     get { return boneWeights; } 
//     set { boneWeights = value; }
// }

     // Tell MonoGame what this struct looks like
     //This is important so we can pass it to the shader correctly
     
    public static readonly VertexDeclaration vertexDecl = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2)
    );

   

     VertexDeclaration IVertexType.VertexDeclaration
{
    get { return vertexDecl; }
}

};

public struct BoneInfo
{
	/*id is index in finalBoneMatrices*/
	public int id;

	/*offset matrix transforms vertex from model space to bone space*/
	public Matrix offset;

};

public class GameMesh
{
    public List<GameModelVertex> vertices { get; set; }
    public List<int> indices { get; set; }

    public VertexBuffer vertexBuffer; 
    public IndexBuffer indexBuffer;

    public Texture2D diffuse { get; set; }
    public Texture2D normal { get; set; }
    public Texture2D specular { get; set; }

    public bool hasDiffuse { get; set; }
    public bool hasSpec { get; set; }
    public bool hasNormal { get; set; }

};

public class DrawModel
{


    public List<Texture2D> textures { get; }

    public List<GameMesh> meshes { get; set; }

    public float metallic { get; set; }
    public float roughness { get; set; }

    private AssimpContext context;

    private string modelFilePath;

    public Scene scene { get; set;}

    public bool hasAnimations { get; set; } = false;

    private Dictionary<string, BoneInfo> boneInfoMap = new Dictionary<string, BoneInfo>();
    private int boneCount = 0;

    public DrawModel(string path, float metallic, float roughness, GraphicsDevice graphicsDevice)
    {
        this.context = new AssimpContext();
        this.modelFilePath = path;
        AssimpSetup(path,graphicsDevice);
        this.textures = new List<Texture2D>();
        this.metallic = metallic;
        this.roughness = roughness;
  
    }

    public Dictionary<string,BoneInfo> getBoneInfoMap(){
        return this.boneInfoMap; 
    }

    public int getBoneCount(){
        return this.boneCount; 
    }
    private Matrix AssimpToXna(System.Numerics.Matrix4x4 m)            // row-major
{   
    // A1 A2 A3 A4 in Assimp is M11 M12 M13 M14 in XNA, …
    return new Matrix(
        m.M11, m.M21, m.M31, m.M41,
        m.M12, m.M22, m.M32, m.M42,
     m.M13, m.M23, m.M33, m.M43,
       m.M14, m.M24, m.M34, m.M44
    );
}
    public void incrementBoneCount(){
        this.boneCount++; 
    }
//This is the setup function for Assimp, it loads the model and sets up the vertex buffers and index buffers for each mesh in the model
//It also extracts the textures from the model and sets them up for use in the shader
//IMPORTANT: Your path is relative to the executable, not the project file. That means you either need to actually copy the model to the executable folder or use a relative path from the executable to the model.
//Be careful when releasing this: All models you are using MUST be in the same folder as the executable or you will get a file not found exception.
//This is a bit of a pain, but it is how Assimp works. You can also use absolute paths, but that is not recommended as it will break when you release the game.
    private void AssimpSetup(string path, GraphicsDevice graphicsDevice)
    {
                //This is what an example file path looks like for us
            string MODELFILEPATH = "../../../Content/arena.dae"; 
        Console.WriteLine("Assimp Importing File: " + path);
        //This generates a "scene" for your model
        this.scene = context.ImportFile(path, PostProcessSteps.Triangulate |
PostProcessSteps.GenerateNormals |
PostProcessSteps.JoinIdenticalVertices |
PostProcessSteps.CalculateTangentSpace |
PostProcessSteps.FlipWindingOrder);
        //Flip Winding order for the accursed Monogame Coordinate system

        if(scene.HasAnimations){
            hasAnimations = true; 
        } else {
            hasAnimations = false; 
        }

        //We now extract the positions, normals etc. from your scene and set them up for use in the shader

        int num_meshes = scene.MeshCount;
        this.meshes = new List<GameMesh>();
        for (int i = 0; i < num_meshes; i++)
        {
            Mesh currMesh = scene.Meshes[i];
            GameMesh gameMesh = new GameMesh();
            gameMesh.vertices = new List<GameModelVertex>();
            for (int j = 0; j < currMesh.VertexCount; j++)
            {
                GameModelVertex vertex = new GameModelVertex();
                Vector3 vec = new Vector3(currMesh.Vertices[j].X, currMesh.Vertices[j].Y, currMesh.Vertices[j].Z);
                vertex.Position = vec;

                if (currMesh.HasNormals)
                {
                    Vector3 normal = new Vector3(currMesh.Normals[j].X, currMesh.Normals[j].Y, currMesh.Normals[j].Z);
                    vertex.Normal = normal;
                }
                else
                {
                    vertex.Normal = Vector3.Zero;
                }

                if (currMesh.HasTextureCoords(0))
                {   
                    Vector2 texCoord = new Vector2(currMesh.TextureCoordinateChannels[0][j].X,
                    1.0f-currMesh.TextureCoordinateChannels[0][j].Y);
                    vertex.TexCoord = texCoord;
                }
                else
                {
                    vertex.TexCoord = Vector2.Zero;
                }
                vertex.BoneIds = new Vector4(-1,-1,-1,-1); 
                vertex.BoneWeights = new Vector4(0.0f,0.0f,0.0f,0.0f);
                gameMesh.vertices.Add(vertex);
                
            }
   
            //Fetch the indices
            gameMesh.indices = currMesh.GetIndices().ToList();
            Material material = scene.Materials[currMesh.MaterialIndex];
            extractTextures(material,gameMesh,scene,graphicsDevice);

            this.meshes.Add(gameMesh);
            if (currMesh.HasBones) {
                List<GameModelVertex> vertices = gameMesh.vertices;
					ExtractBoneWeightForVertices(ref vertices, currMesh, scene);
                gameMesh.vertices = vertices; //Update the vertex data in the list
			}
           
            setupBuffers(gameMesh, graphicsDevice);

        }
       
    }

    private void ExtractBoneWeightForVertices(ref List<GameModelVertex> vertices, Mesh mesh, Scene scene){
        for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++){
            int boneID = -1; 
            string boneName = mesh.Bones[boneIndex].Name;
            if(!boneInfoMap.ContainsKey(boneName)){
                BoneInfo newBoneInfo = new BoneInfo();
                newBoneInfo.id = boneCount; 
                newBoneInfo.offset = AssimpToXna(mesh.Bones[boneIndex].OffsetMatrix);
                boneInfoMap[boneName] = newBoneInfo;
                boneID = boneCount;
                incrementBoneCount();
            } else {
                boneID = boneInfoMap[boneName].id; 
            }
            if(boneID == -1){
                throw new Exception("Bone ID not found for bone: " + boneName);
            }

            var weights = mesh.Bones[boneIndex].VertexWeights;
            int numWeights = mesh.Bones[boneIndex].VertexWeightCount;;

            for(int weightIndex = 0; weightIndex < numWeights; weightIndex++){
                int vertexID = weights[weightIndex].VertexID;
                float weight = weights[weightIndex].Weight; 
                if(vertexID > vertices.Count){
                    throw new Exception("Vertex ID out of range: " + vertexID + " for bone: " + boneName);
                }
                GameModelVertex vertexData = vertices[vertexID];
                setVertexBoneData(ref vertexData, boneID, weight);
                vertices[vertexID] = vertexData; //Update the vertex data in the list
                
        }
        }
    }


    private void setVertexBoneData(ref GameModelVertex vertex, int boneId, float weight){
        //Check if the vertex already has a bone assigned
        if(vertex.BoneIds.X == -1){
            vertex.BoneIds.X = boneId; 
            vertex.BoneWeights.X = weight; 
        } else if(vertex.BoneIds.Y == -1){
            vertex.BoneIds.Y = boneId; 
            vertex.BoneWeights.Y = weight; 
        } else if(vertex.BoneIds.Z == -1){
            vertex.BoneIds.Z = boneId; 
            vertex.BoneWeights.Z = weight; 
        } else if(vertex.BoneIds.W == -1){
            vertex.BoneIds.W = boneId; 
            vertex.BoneWeights.W = weight; 
        } 


    }

    //This is OpenGL style: We set up Vertex and Index Buffer for every mesh in the model
    private void setupBuffers(GameMesh gameMesh, GraphicsDevice graphicsDevice)
    {
        gameMesh.vertexBuffer = new VertexBuffer(graphicsDevice, typeof(GameModelVertex), gameMesh.vertices.Count, BufferUsage.WriteOnly);
        gameMesh.indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, gameMesh.indices.Count, BufferUsage.WriteOnly);

        gameMesh.vertexBuffer.SetData(gameMesh.vertices.ToArray());
        gameMesh.indexBuffer.SetData(gameMesh.indices.ToArray());
    }
    
        //Here we extract the textures from the material and load them. 
        ////We check if the texture is embedded in the model or if it is a file path. If it is a file path, we load it from the file system
        private void extractTextures(Material material, GameMesh gameMesh, Scene scene, GraphicsDevice graphicsDevice)
    {
        if (material.HasTextureDiffuse)
        {
             var texSlot = material.TextureDiffuse;
            loadTexture(gameMesh,texSlot,scene,graphicsDevice,TextureType.Diffuse);


        }
        if (material.HasTextureNormal)
        {
            var texSlot = material.TextureNormal;
            loadTexture(gameMesh,texSlot,scene,graphicsDevice,TextureType.Normals);
        }
        if (material.HasTextureSpecular)
        {
            var texSlot = material.TextureSpecular;
            loadTexture(gameMesh,texSlot,scene,graphicsDevice,TextureType.Specular);
        }
    }

    private void loadTexture(GameMesh gameMesh, TextureSlot texSlot, Scene scene, GraphicsDevice graphicsDevice, TextureType type){
         string filePath = texSlot.FilePath;

         Console.WriteLine("Loading Texture: " + filePath + " of Type " + type.ToString());
            //This means the texture is baked in
            if (filePath.StartsWith("*"))
            {
                string embeddedKey = texSlot.FilePath.Substring(1); // remove leading "*"
                int embeddedIndex = int.Parse(embeddedKey);

                // scene is the Assimp.Scene you got from ImportFile
                var embeddedTex = scene.Textures[embeddedIndex];

                // embeddedTex.HasCompressedData indicates if it’s a compressed image (PNG/JPG bytes)
                // or uncompressed BGRA raw data
                if (embeddedTex.CompressedData != null && embeddedTex.CompressedData.Length > 0)
                {
                    // If compressed, we can load directly from memory
                    byte[] data = embeddedTex.CompressedData;
                    using (var ms = new MemoryStream(data))
                    {
                        if(type == TextureType.Diffuse){
                        gameMesh.diffuse = Texture2D.FromStream(graphicsDevice, ms);
                        gameMesh.hasDiffuse = true;
                        } 
                        if(type == TextureType.Normals){
                        gameMesh.normal = Texture2D.FromStream(graphicsDevice, ms);
                        gameMesh.hasNormal = true;
                        }
                         if(type == TextureType.Specular){
                        gameMesh.specular = Texture2D.FromStream(graphicsDevice, ms);
                        gameMesh.hasSpec = true;
                        }
                    }
                }
            }
            else
            {
          if (!Path.IsPathRooted(filePath))
        {
    
            string modelDir = Path.GetDirectoryName(this.modelFilePath);
            filePath = Path.Combine(modelDir, filePath);
        }

                if (File.Exists(filePath))
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        gameMesh.diffuse = Texture2D.FromStream(graphicsDevice, stream);
                        gameMesh.hasDiffuse = true;
                    }
                }
            }
    }

//Old function to extract textures from the Monogame model directly. 
    // private void extractTextures()
    // {
    //     foreach (ModelMesh mesh in model.Meshes)
    //     {
    //         foreach (BasicEffect effect in mesh.Effects)
    //         {
    //             if (effect.Texture != null)
    //             {
    //                 this.textures.Add(effect.Texture);
    //             }
    //         }
    //     }
    // }

}