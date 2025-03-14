using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
//using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//local imports
using src.ObjectClasses;

namespace GameLab
{
    public class GameLabGame : Game
    {
        protected static Random rng = new Random();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Private fields:
        private Model arena, playerModel;
        private List<Model> projectileModels = new List<Model>();
        private RingOfDoom ring;
        private LinkedList<Projectile> activeProjectiles = new LinkedList<Projectile>();
        private Player[] players = new Player[4];

        // Camera settings
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, -5, 10), new Vector3(0, 0, 0), Vector3.UnitZ);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView( 
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        // Arena transformations
        private Matrix arenaScaling = Matrix.CreateScale(new Vector3(0.55f,0.55f,0.55f));
        private Matrix arenaTranslation = Matrix.CreateTranslation(new Vector3(8.2f, 3.5f, -0.5f));
        private static float timeUntilNextProjectile = 5000f; // Random interval before next projectile

        public GameLabGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true; // Enable full screen
            _graphics.ApplyChanges();

            // Initialize the ring of doom:
            int planeWidth = 10, planeHeight = 10;
            this.ring = new RingOfDoom(planeWidth, planeHeight);
            

            for(int i=0; i<4; i++)
                players[i] = new Player(new Vector3(0,0,0), 5.0f, 3, 3);
        
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the player/projectile models
            // Textured arena model currently named test TODO change that and remove old arena model too
            arena = Content.Load<Model>("test");
            playerModel = Content.Load<Model>("player");
            projectileModels.Add(Content.Load<Model>("frog"));
            projectileModels.Add(Content.Load<Model>("Monke"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
           
            timeUntilNextProjectile -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            // Spawn a new projectile:
            if (timeUntilNextProjectile <= 0)
            {
                timeUntilNextProjectile = (float) rng.NextDouble() * 5000f;
                int type = rng.Next(0, projectileModels.Count);
                activeProjectiles.AddLast(Projectile.createProjectile(type, ring.RndCircPoint(), players[rng.Next(0, 4)].Position));
            }
            
            // Move all the projectiles
            foreach (Projectile projectile in activeProjectiles) projectile.Move(gameTime);
            
            // Move players
            foreach (Player player in players) player.Move(gameTime);
            
            //check hit detection
            // Postpone the ring closing for the low target, right now functional minimum!!
            /*
            //close the ring of doom
            ring.CloseRing(gameTime);
            this.player.Position = this.ring.RingClamp(this.player.Position);
            */
            base.Update(gameTime);
        }

        private void DrawModel(Model model, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = this.view;
                    effect.Projection = this.projection;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // This resolves upscaling issues when going fullscreen
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // TODO: Add your drawing code here
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));

            DrawModel(arena, arenaScaling*arenaTranslation);

            foreach (Projectile projectile in activeProjectiles)
                DrawModel(projectileModels[projectile.Type], Matrix.CreateTranslation(projectile.Position));

            foreach (Player player in players)
                DrawModel(playerModel, Matrix.CreateTranslation(player.Position) * Matrix.CreateScale(new Vector3(1.5f, 1.5f, 1.5f)));

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
