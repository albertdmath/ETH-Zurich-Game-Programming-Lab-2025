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

[StructLayout(LayoutKind.Sequential)]
public struct GameModelVertex : IVertexType
{

    private Vector3 position;
    private Vector3 normal;
    private Vector2 texCoord;   

    public GameModelVertex(Vector3 pos, Vector3 norm, Vector2 texCoord)
    {
        position = pos;
        normal = norm;
        this.texCoord = texCoord;
    }

    
    public Vector3 Position
{
    get { return position; }
    set { position = value; }
}


    public Vector3 Normal
{
    get { return normal; }
    set { normal = value; }
}

public Vector2 TexCoord
{
    get { return texCoord; }
    set { texCoord = value; }
}

     // Tell MonoGame what this struct looks like
    public static readonly VertexDeclaration vertexDecl = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
    );

   

     VertexDeclaration IVertexType.VertexDeclaration
{
    get { return vertexDecl; }
}

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

    public Model model { get; set; }

    public List<Texture2D> textures { get; }

    public List<GameMesh> meshes { get; set; }

    public float metallic { get; set; }
    public float roughness { get; set; }

    private AssimpContext context;

    private string modelFilePath;
     public static readonly VertexDeclaration VertexDeclaration =
        new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

   


    public DrawModel(string path, Model model, float metallic, float roughness, GraphicsDevice graphicsDevice)
    {
        this.context = new AssimpContext();
        this.modelFilePath = path;
        AssimpSetup(path,graphicsDevice);
        this.model = model;
        this.textures = new List<Texture2D>();
        this.metallic = metallic;
        this.roughness = roughness;
        extractTextures();
    }

    private void AssimpSetup(string path, GraphicsDevice graphicsDevice)
    {
        Console.WriteLine("Assimp Importing File: " + path);
        var scene = context.ImportFile(path, PostProcessSteps.Triangulate |
PostProcessSteps.GenerateNormals |
PostProcessSteps.JoinIdenticalVertices |
PostProcessSteps.CalculateTangentSpace |
PostProcessSteps.FlipWindingOrder);
    
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
                gameMesh.vertices.Add(vertex);
            }
            gameMesh.indices = currMesh.GetIndices().ToList();
            Material material = scene.Materials[currMesh.MaterialIndex];
            extractTextures(material,gameMesh,scene,graphicsDevice);
            setupBuffers(gameMesh, graphicsDevice);
            this.meshes.Add(gameMesh);

        }
       
    }

    private void setupBuffers(GameMesh gameMesh, GraphicsDevice graphicsDevice)
    {
        gameMesh.vertexBuffer = new VertexBuffer(graphicsDevice, typeof(GameModelVertex), gameMesh.vertices.Count, BufferUsage.WriteOnly);
        gameMesh.indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, gameMesh.indices.Count, BufferUsage.WriteOnly);

        gameMesh.vertexBuffer.SetData(gameMesh.vertices.ToArray());
        gameMesh.indexBuffer.SetData(gameMesh.indices.ToArray());
    }

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
            string MODELFILEPATH = "../../../Content/arena.dae";
            //TODO: CHANGE THIS THIS IS CURSED AND JUST FOR TESTING 
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

    private void extractTextures()
    {
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                if (effect.Texture != null)
                {
                    this.textures.Add(effect.Texture);
                }
            }
        }
    }

}