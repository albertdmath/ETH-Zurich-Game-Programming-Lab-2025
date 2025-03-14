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
        private Dictionary<int, Model> projectileModels;
        private RingOfDoom ring;
        private LinkedList<Projectile> projectiles = new LinkedList<Projectile>();
        private Player[] players = new Player[4];
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 5.0f, 5.0f), new Vector3(0, 0, 0), Vector3.UnitZ);
        private Matrix projection = Matrix.CreateOrthographic(15, 15, 0.1f, 1000f);
        private Matrix planeTran = Matrix.CreateTranslation(new Vector3(0, 0, 0));
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

            //initialize the ring of doom, im curently passing random plane dimensions
            int planeWidth = 10, planeHeight = 10;
            this.ring = new RingOfDoom(planeWidth, planeHeight);

            for(int i=0;i<4;++i)
                players[i] = new Player();
        
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            timeUntilNextProjectile -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeUntilNextProjectile <= 0)
            {
                timeUntilNextProjectile = (float)rng.NextDouble() * 5000f;
                int type = rng.Next(0, projectileModels.Count);
                projectiles.AddLast(Projectile.CreatePrj(type, ring.RndCircPoint(), players[rng.Next(0, 4)].GetPosition()));
            }

            //move all the projectiles
            foreach (Projectile projectile in projectiles) projectile.Move(gameTime);

            //move players
            foreach (Player player in players) player.Move(gameTime);

            //check hit detection

            //close the ring of doom
            ring.CloseRing(gameTime);

            //this.player.Position = this.ring.RingClamp(this.player.Position);

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
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _spriteBatch.Begin();

            // TODO: Add your drawing code here
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));

            DrawModel(plane, planeTran);

            foreach (Projectile projectile in projectiles)
                DrawModel(projectileModels[projectile.GetTipe()], Matrix.CreateTranslation(projectile.GetPosition()));

            foreach (Player player in players)
                DrawModel(jesterModel, Matrix.CreateTranslation(player.GetPosition()));

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
