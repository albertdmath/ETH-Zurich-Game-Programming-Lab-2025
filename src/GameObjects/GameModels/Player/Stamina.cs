using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;


namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Stamina
    {
        public bool DashReady { get; private set; } = true;

        private const float TIME_CHARGE = 5f; // Time to charge the stamina
        private const int N_MODELS = 3; // Number of models to draw
        private const float SLICE_DURATION = TIME_CHARGE/N_MODELS; //How many slices to complete circle
        private readonly Player player;
        private readonly DrawModel[] models = new DrawModel[N_MODELS];
        private readonly float[] angles = new float[N_MODELS]; // Angles of the slices

        private float timeSpentCatching = TIME_CHARGE; // Time spent catching

        public Stamina(Player player, DrawModel model)
        {
            this.player = player;
            for (int i = 0; i < N_MODELS; i++)
            {
                models[i] = model;
                angles[i] = MathHelper.ToRadians(360f / N_MODELS * i);
            }
        }       

        public void Update(float dt)
        {
            timeSpentCatching += dt;
            DashReady = timeSpentCatching >= TIME_CHARGE;
        }

        public bool Dash()
        {
            if (DashReady)
            {
                timeSpentCatching = 0f;
                DashReady = false;
                return true;
            }
            return false;
        }
        
        // Draw function called each draw
        public void Draw(Matrix view, Shader shader, GraphicsDevice graphicsDevice, bool shadowDraw)
        {
            if (DashReady)
                return;
            
            for (int i = 0; i < N_MODELS; i++)
            {  
                DrawModel model = models[i];

                if(timeSpentCatching < SLICE_DURATION * i)
                    continue;
        
                Matrix Transform = Matrix.CreateRotationY(angles[i]) * Matrix.CreateTranslation(player.Position);
                
                foreach (GameMesh mesh in model.meshes)
                {   
                    VertexBuffer buff = mesh.vertexBuffer;
                    graphicsDevice.SetVertexBuffer(buff);
                    graphicsDevice.Indices = mesh.indexBuffer;
                    shader.setWorldMatrix(Transform);
                    if (!shadowDraw)
                    {   
                        if(mesh.hasDiffuse)
                            shader.setTexture(mesh.diffuse);
                        
                        shader.setNormalMatrix(view, Transform);
                        shader.setOpacityValue(timeSpentCatching / SLICE_DURATION - i);
                    }
                    foreach (var pass in shader.effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            baseVertex: 0,
                            startIndex: 0,
                            primitiveCount: mesh.indices.Count / 3
                        ); 
                    }
                }
            }
        }
    }
}