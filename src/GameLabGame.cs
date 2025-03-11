using System;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameLab
{
    public class GameLabGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Model model; 
        private Model model2; 
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix world2 = Matrix.CreateTranslation(new Vector3(2.0f, -2.0f, 1.0f));
        private Matrix world3 = Matrix.CreateRotationZ((float)(Math.PI/2));
        private Matrix view = Matrix.CreateLookAt(new Vector3(5.0f, 5.0f, 5.0f), new Vector3(0, 0, 0), Vector3.UnitZ);
        private Matrix projection = Matrix.CreateOrthographic(15,15, 0.1f, 100f);

        public GameLabGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1024;
            //_graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            model = Content.Load<Model>("BIG");
            model2 = Content.Load<Model>("Monke");

            base.Initialize();
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

            // TODO: Add your update logic here

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
            this.DrawModel(this.model, this.world, this.view, this.projection);
            this.DrawModel(this.model2, this.world3*this.world2, this.view, this.projection);
            _spriteBatch.End();

            base.Draw(gameTime);
        }



    }
}
