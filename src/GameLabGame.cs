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

        private Model plane, jesterModel, arena;
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix world2 = Matrix.CreateTranslation(new Vector3(2.0f,-4.0f,2.0f));
        private Vector3 monkeposition = new Vector3(0, 0, 0);
        private Matrix world3 = Matrix.CreateRotationZ((float)(Math.PI / 3));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 5.0f, 5.0f), new Vector3(0, 0, 0), Vector3.UnitZ);
        private Matrix projection = Matrix.CreateOrthographic(15, 15, 0.1f, 1000f);
        private static float timeUntilNextProjectile = 5000f; // Random interval before next projectile

        private RingOfDoom ring;
        private LinkedList<Projectile> projectiles = new LinkedList<Projectile>();
        private Player[] players = new Player[4];
        private Dictionary<int, Model> projectileModels;


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
            
            //initialize the ring of doom, im curently passing random plane dimensions
            int planeWidth = 100, planeHeight = 100;
            this.ring = new RingOfDoom(planeWidth, planeHeight);

            //initialize the players
            for(int i=0;i<4;++i){
                players[i] = new Player();
            }

            base.Initialize(); //why?
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            plane = Content.Load<Model>("BIG");
            arena = Content.Load<Model>("arena");
            jesterModel = Content.Load<Model>("Jester");
        
            projectileModels = new Dictionary<int, Model>
            {
                { 0, Content.Load<Model>("Frog") }, // Projectile type 0: Frog
                { 1, Content.Load<Model>("Monke") }, // Projectile type 1: Swordfish
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
                Exit();
            }
            
            timeUntilNextProjectile -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //try to create a random projectile and check if it is not null
            if (timeUntilNextProjectile <= 0)
            {
                timeUntilNextProjectile = (float)rng.NextDouble() * 5000f;
                int type = rng.Next(0, projectileModels.Count);
                projectiles.AddLast(Projectile.CreateRndProjectile(type, ring.RndCircPoint(), players[rng.Next(0, 4)].GetPosition()));
            }

            //move all the projectiles
            foreach (Projectile projectile in projectiles) projectile.Move(gameTime);

            //move players
            foreach (Player player in players) player.Move();
                
            //check hit detection

            //close the ring of doom
            this.ring.CloseRing(gameTime);

            //this.player.Position = this.ring.RingClamp(this.player.Position);

            base.Update(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _spriteBatch.Begin();

            // TODO: Add your drawing code here
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));
            
            this.DrawModel(this.plane, this.world, this.view, this.projection);

            foreach (Projectile projectile in projectiles) this.DrawModel(projectileModels[projectile.GetTipe()], Matrix.CreateTranslation(projectile.GetPosition()), this.view, this.projection);

            foreach (Player player in players) this.DrawModel(this.jesterModel, Matrix.CreateTranslation(player.GetPosition()), this.view, this.projection);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
