using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

//using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

//local imports
using src.GameObjects;

namespace GameLab
{
    public class GameLabGame : Game
    {
        protected static Random rng = new Random();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        // Private fields:
        private Model arena, playerModel;
        private Dictionary<ProjectileType, Model> projectileModels = new Dictionary<ProjectileType, Model>();
        private LinkedList<Projectile> hitProjectiles = new LinkedList<Projectile>();

        // Camera settings
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 10, 7), new Vector3(0, 0, 0), Vector3.Up);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        // Arena transformations
        private Matrix arenaScaling = Matrix.CreateScale(new Vector3(0.5f));
        private Matrix arenaTranslation = Matrix.CreateTranslation(new Vector3(0, -1f, 0));

        // Player transformations
        private Matrix playerScaling = Matrix.CreateScale(new Vector3(1.5f));
        private Matrix playerTranslation = Matrix.CreateTranslation(new Vector3(0, 0.2f, 0));

        // Projectile transformations:
        private Matrix projectileRotation = Matrix.Identity;
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
            _graphics.IsFullScreen = false; // Enable full screen
            _graphics.ApplyChanges();

            // Initialize the ring of doom:
            int planeWidth = 10, planeHeight = 10;
            Ring.active = new Ring(planeWidth, planeHeight);

            Player.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the player/projectile models
            // Textured arena model currently named test TODO change that and remove old arena model too
            arena = Content.Load<Model>("arena");
            playerModel = Content.Load<Model>("player");
            font = Content.Load<SpriteFont>("font");

            // Load the projectile models
            projectileModels.Add(ProjectileType.Frog, Content.Load<Model>("frog"));
            projectileModels.Add(ProjectileType.Swordfish, Content.Load<Model>("swordfish"));
            //it should be a tomato
            projectileModels.Add(ProjectileType.Tomato, Content.Load<Model>("tomato"));

            // Load Sounds:
            //Sounds.bgMusic = Content.Load<Song>("Audio/yoga-dogs-all-good-folks");
            //MediaPlayer.Play(Sounds.bgMusic);
            //MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Spawn a new projectile:
            Projectile.MobShoot(dt, rng);

            // Move all the projectiles
            foreach (Projectile projectile in Projectile.active) 
                projectile.Update(dt);

            // Move players
            foreach (Player player in Player.active)
            {
                player.update(dt);
                if (player.projectileHeld != null && !Projectile.active.Contains(player.projectileHeld))
                    Projectile.active.AddLast(player.projectileHeld);
            }


            // Super basic hit detection until we can figure out bounding spheres, just using 0.5 which is quite arbitrary for now:
            foreach (Player player in Player.active)
            {
                foreach (Projectile projectile in Projectile.active)
                {
                    //we should decide how much distance
                    if (player.GrabOrHit(projectile))
                        hitProjectiles.AddLast(projectile);
                }
            }

            foreach (Projectile projectile in hitProjectiles)
                Projectile.active.Remove(projectile);

            //if two die at the same time only one will win...
            foreach (Player player in Player.active)
            {
                if (player.Life <= 0)
                {
                    Player.active.Remove(player);
                    break;
                }
            }

            hitProjectiles = new LinkedList<Projectile>();

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
        private void DrawHealthAndStamina()
        {
            switch (Player.active.Count)
            {
                case 4:
                    _spriteBatch.DrawString(font, "Lives: " + Player.active[3].Life + "  Stamina: " + (int)Player.active[3].Stamina, new Vector2(1500, 950), Color.Yellow);
                    goto case 3;
                case 3:
                    _spriteBatch.DrawString(font, "Lives: " + Player.active[2].Life + "  Stamina: " + (int)Player.active[2].Stamina, new Vector2(10, 950), Color.Orange);
                    goto case 2;
                case 2:
                    _spriteBatch.DrawString(font, "Lives: " + Player.active[1].Life + "  Stamina: " + (int)Player.active[1].Stamina, new Vector2(1500, 10), Color.White);
                    goto case 1;
                case 1:
                    _spriteBatch.DrawString(font, "Lives: " + Player.active[0].Life + "  Stamina: " + (int)Player.active[0].Stamina, new Vector2(10, 10), Color.Red);
                    goto default;
                default:
                    break;
            }
        }

        private void DrawWin()
        {
            if (Player.active.Count == 1)
            {
                int n = 0;
                for (int i = 0; i < Player.active.Count; i++)
                    n = Player.active.Contains(Player.active[i]) ? i : n;
                Player.active[n].notImportant = true;
                _spriteBatch.DrawString(font, "Player " + Player.active[0].Id + "  wins!", new Vector2(750, 475), Color.Gold);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue); // Background color
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // This resolves upscaling issues when going fullscreen
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw the ring of doom:
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));

            // Draw the arena:
            DrawModel(arena, arenaScaling * arenaTranslation);

            // Draw all active projectiles:
            foreach (Projectile projectile in Projectile.active)
            {
                Matrix RotationMatrix = Matrix.CreateRotationY((float)(Math.PI / 2 - Math.Atan2(projectile.Orientation.Z, projectile.Orientation.X)));
                DrawModel(projectileModels[projectile.Type], projectileRotation * RotationMatrix * Matrix.CreateTranslation(projectile.Position));
            }

            // Draw all players:
            foreach (Player player in Player.active)
                DrawModel(playerModel, Matrix.CreateRotationY((float)Math.Atan2(-1f * player.Orientation.X, -1f * player.Orientation.Z)) * Matrix.CreateTranslation(player.Position) * playerTranslation);

            DrawHealthAndStamina();
            DrawWin();

            _spriteBatch.End();

            /*
            OrientedBoundingBox obb1 = OrientedBoundingBox.ComputeOBB(arena.Meshes[15], arenaScaling);
            BoundingBoxRenderer.DrawOBB(GraphicsDevice, obb1, view, projection);

            OrientedBoundingBox obb2 = OrientedBoundingBox.ComputeOBB(playerModel.Meshes[1], Matrix.CreateRotationY((float)Math.Atan2(-1f * Player.active[0].Orientation.X, -1f * Player.active[0].Orientation.Z)) * Matrix.CreateTranslation(Player.active[0].Position) * playerTranslation);
            BoundingBoxRenderer.DrawOBB(GraphicsDevice, obb2, view, projection);

            if (obb1.Intersects(obb2))
            {
                Console.WriteLine("yay");
            }
            */
            base.Draw(gameTime);
        }
    }
}
