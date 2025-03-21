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
        public static GameModel arenaModel;
        public static Dictionary<ProjectileType, Model> projectileModels = new Dictionary<ProjectileType, Model>();
        private LinkedList<Projectile> hitProjectiles = new LinkedList<Projectile>();

        // Player settings
        public static int NUM_PLAYERS = 1;
        private Vector3 playerSpawnOrientation = new Vector3(0,0,-1);

        // Camera settings
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 9, 7), new Vector3(0, 0, 0.7f), Vector3.Up);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        // Arena transformations
        private Matrix arenaScaling = Matrix.CreateScale(new Vector3(0.5f));

        private Mob mob;

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
            projectileModels.Add(ProjectileType.Tomato, Content.Load<Model>("tomato"));


            // Initialize game models (they are only known at this point so they can't be in the initialize method)
            arenaModel = new GameModel(arena);
            arenaModel.Transform = arenaScaling;
            arenaModel.Hitbox.Transform(arenaScaling);

            // Initialize mob
            float height = 10f, width = 5f; //this should be the size of the arena
            mob = new Mob(height, width, projectileModels[ProjectileType.Frog]);

            // Initialize players
            Player.Initialize(mob.Ellipse, playerModel);

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
            
            // Spawn a new projectile
            Projectile.MobShoot(dt, rng);

            // Move all the projectiles
            foreach (Projectile projectile in Projectile.active) 
                projectile.updateWrap(dt);

            // Move players
            foreach (Player player in Player.active)
            {
                player.updateWrap(dt);
                if(player.projectileHeld != null && !Projectile.active.Contains(player.projectileHeld))
                    Projectile.active.AddLast(player.projectileHeld);
            }

            // Check if players got hit / grabbed something
            foreach (Player player in Player.active)
            {
                foreach (Projectile projectile in Projectile.active)
                {
                    //we should decide how much distance
                    if (player.GrabOrHit(projectile))
                        hitProjectiles.AddLast(projectile);
                }
            }

            // Remove projectiles that hit someone
            foreach (Projectile projectile in hitProjectiles)
                Projectile.active.Remove(projectile);

            // If two die at the same time only one will win...
            foreach (Player player in Player.active)
            {
                if (player.Life <= 0)
                {
                    Player.active.Remove(player);
                    break;
                }
            }

            hitProjectiles = new LinkedList<Projectile>();

            // Update mob
            mob.Update(dt);

            base.Update(gameTime);
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
                Texture2D pixel;
                pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });

                // Define text
                string winMessage = "Player " + Player.active[0].Id + " wins!";

                // Measure text size
                Vector2 textSize = font.MeasureString(winMessage);
                Vector2 textPosition = 
                    new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2 - 100, 
                                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2 - 50);

                // Define background rectangle position and size
                Vector2 padding = new Vector2(20, 10); // Add some padding around text
                Rectangle backgroundRect = new Rectangle(
                    (int)(0),
                    (int)(textPosition.Y - 50),
                    (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width),
                    (int)(textSize.Y + 100)
                );
                _spriteBatch.Draw(pixel, backgroundRect, Color.Black * 0.5f); // Semi-transparent black
                _spriteBatch.DrawString(font, winMessage, textPosition, Color.Gold);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue); // Background color
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // This resolves upscaling issues when going fullscreen
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //DrawModel(arena, arenaScaling);
            arenaModel.Draw(view,projection);
            arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            // Draw all active projectiles:
            foreach (Projectile projectile in Projectile.active)
            {
                projectile.Draw(view, projection);
                projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all players
            foreach (Player player in Player.active)
            {
                player.Draw(view, projection);
                player.Hitbox.DebugDraw(GraphicsDevice, view, projection);
            } 

            // Draw mob
            mob.Draw(view, projection);
           

            DrawHealthAndStamina();
            DrawWin();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
