using System;
using System.Collections;
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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Model plane;
        private Model monkey;
        float movespeed = 0.2f;
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix world2 = Matrix.CreateTranslation(new Vector3(2.0f,-4.0f,2.0f));
        private Vector3 monkeposition = new Vector3(0, 0, 0);
        private Matrix world3 = Matrix.CreateRotationZ((float)(Math.PI / 3));
        private Matrix view = Matrix.CreateLookAt(new Vector3(5.0f, 5.0f, 5.0f), new Vector3(0, 0, 0), Vector3.UnitZ);
        private Matrix projection = Matrix.CreateOrthographic(15, 15, 0.1f, 1000f);

        private RingOfDoom ring;
        private LinkedList<Projectile> projectiles = new LinkedList<Projectile>();
        private Player[] players = new Player[4];


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

            base.Initialize(); //why?
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            plane = Content.Load<Model>("BIG");
            monkey = Content.Load<Model>("Monke");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
                Exit();
            }
            KeyboardState newState = Keyboard.GetState();
                if(newState.IsKeyDown(Keys.D)){
                    monkeposition.X += movespeed;
                }
                if(newState.IsKeyDown(Keys.W)){
                    monkeposition.Y -=movespeed;
                }
                if(newState.IsKeyDown(Keys.S)){
                    monkeposition.Y +=movespeed;
                }
                if(newState.IsKeyDown(Keys.A)){
                    monkeposition.X-=movespeed;
                }

            
            //try to create a random projectile and check if it is not null
            if (Projectile.CreateRandomProjectile(gameTime) is Projectile proj)
            {
                projectiles.AddLast(proj);
                //throw the projectile form a random point on the ring to a position of a random player
                proj.Throw(ring.RndCircPoint(), new Vector3(0, 0, 0));
            }

            //move all the projectiles
            foreach (Projectile projectile in projectiles) projectile.Move(gameTime);

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
            //foreach (Projectile projectile in projectiles) projectile.DrawProjectile();
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));
            
            this.DrawModel(this.plane, this.world, this.view, this.projection);
            this.DrawModel(this.monkey, this.world3 * Matrix.CreateTranslation(monkeposition), this.view, this.projection);
            _spriteBatch.End();
            Console.WriteLine(monkeposition.X);
            base.Draw(gameTime);
        }
    }
}
