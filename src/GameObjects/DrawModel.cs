using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Assimp;
using System;
using Microsoft.Xna.Framework;
using Assimp.Configs;
using System.Linq;
using System.IO;

public struct ModelVertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 UV;
};

public class GameMesh
{
    public List<ModelVertex> vertices { get; set; }
    public List<int> indices { get; set; }

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
        var scene = context.ImportFile("../../../Content/arena.dae", PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.CalculateTangentSpace);
        int num_meshes = scene.MeshCount;
        this.meshes = new List<GameMesh>();
        for (int i = 0; i < num_meshes; i++)
        {
            Mesh currMesh = scene.Meshes[i];
            GameMesh gameMesh = new GameMesh();
            gameMesh.vertices = new List<ModelVertex>();
            for (int j = 0; j < currMesh.VertexCount; j++)
            {
                ModelVertex vertex = new ModelVertex();
                vertex.position = new Vector3();
                vertex.normal = new Vector3();
                vertex.UV = new Vector2();
                vertex.position.X = currMesh.Vertices[i].X;
                vertex.position.Y = currMesh.Vertices[i].Y;
                vertex.position.Z = currMesh.Vertices[i].Z;

                if (currMesh.HasNormals)
                {
                    vertex.normal.X = currMesh.Normals[i].X;
                    vertex.normal.Y = currMesh.Normals[i].Y;
                    vertex.normal.Z = currMesh.Normals[i].Z;
                }
                else
                {
                    vertex.normal.X = 0.0f;
                    vertex.normal.Y = 0.0f;
                    vertex.normal.Z = 0.0f;
                }

                if (currMesh.HasTextureCoords(0))
                {
                    vertex.UV.X = currMesh.TextureCoordinateChannels[0][i].X;
                    vertex.UV.Y = currMesh.TextureCoordinateChannels[0][i].Y;
                }
                else
                {
                    vertex.UV.X = 0.0f;
                    vertex.UV.Y = 0.0f;
                }
                gameMesh.vertices.Add(vertex);
            }
            gameMesh.indices = currMesh.GetIndices().ToList();
            Material material = scene.Materials[currMesh.MaterialIndex];
            extractTextures(material,gameMesh,scene,graphicsDevice);
            this.meshes.Add(gameMesh);

        }

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
            string modelDir = Path.GetDirectoryName(MODELFILEPATH);
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