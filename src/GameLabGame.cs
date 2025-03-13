using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
        private const int width = 1200, height = 1024;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Model model;
        private Model model2;
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix world2 = Matrix.CreateTranslation(new Vector3(2.0f, -2.0f, 1.0f));
        private Matrix world3 = Matrix.CreateRotationZ((float)(Math.PI / 2));
        private Matrix view = Matrix.CreateLookAt(new Vector3(5.0f, 5.0f, 5.0f), new Vector3(0, 0, 0), Vector3.UnitZ);
        private Matrix projection = Matrix.CreateOrthographic(15, 15, 0.1f, 100f);

        private RingOfDoom ring;
        private LinkedList<Projectile> projectiles = new LinkedList<Projectile>();


        public GameLabGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            //_graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            model = Content.Load<Model>("BIG");
            model2 = Content.Load<Model>("Monke");

            base.Initialize();

            //initialize the ring of doom, im curently passing not the dimensions of the plane but the dimensions of the window
            this.ring = new RingOfDoom(width, height);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            //try to create a random projectile and check if it is not null
            if (Projectile.CreateRandomProjectile(gameTime) is Projectile proj)
            {
                projectiles.AddLast(proj);
                //throw the projectile form a random point on the ring to a position of a random player
                proj.Throw(ring.RndCircPoint(), new Vector3(0, 0, 0));
            }

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
            //circle drawing
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));
            this.DrawModel(this.model, this.world, this.view, this.projection);
            this.DrawModel(this.model2, this.world3 * this.world2, this.view, this.projection);
            _spriteBatch.End();

            base.Draw(gameTime);
        }



    }
}
