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
        private Model arena;
        private List<Model> players = new List<Model>();
        private List<Model> mobs = new List<Model>();

        public static GameModel arenaModel;
        public static Dictionary<ProjectileType, Model> projectileModels = new Dictionary<ProjectileType, Model>();
        private LinkedList<Projectile> hitProjectiles = new LinkedList<Projectile>();
        Texture2D playerHearts;
        private SoundEffectInstance angrymobInstance;
        public const bool SOUND_ENABLED = true;

        // Player settings
        public static int NUM_PLAYERS = 2;
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
        private int nAlivePlayers = NUM_PLAYERS;
        private Player lastPlayer;

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

            players.Add(Content.Load<Model>("player1"));
            players.Add(Content.Load<Model>("player2"));
            players.Add(Content.Load<Model>("player3"));
            players.Add(Content.Load<Model>("player4"));

            mobs.Add(Content.Load<Model>("mob1"));
            mobs.Add(Content.Load<Model>("mob2"));


            font = Content.Load<SpriteFont>("font");
            playerHearts = Content.Load<Texture2D>("player_heart");

            // Load the projectile models
            projectileModels.Add(ProjectileType.Frog, Content.Load<Model>("frog"));
            projectileModels.Add(ProjectileType.Swordfish, Content.Load<Model>("swordfish"));
            projectileModels.Add(ProjectileType.Tomato, Content.Load<Model>("tomato"));


            // Initialize game models (they are only known at this point so they can't be in the initialize method)
            arenaModel = new GameModel(arena,0.5f);

            // Initialize mob
            float height = 9f, width = 15f; //this should be the size of the arena
            mob = new Mob(height, width, mobs);

            // Initialize players
            Player.Initialize(mob.Ellipse, players);

            // Load Sounds:
            MusicAndSoundEffects.bgMusic = Content.Load<Song>("Audio/yoga-dogs-all-good-folks");
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.IsRepeating = true;
            MusicAndSoundEffects.equipSFX = Content.Load<SoundEffect>("Audio/equipSFX");
            MusicAndSoundEffects.frogSFX = Content.Load<SoundEffect>("Audio/frogSFX");
            MusicAndSoundEffects.swordfishSFX = Content.Load<SoundEffect>("Audio/swordfishSFX");
            MusicAndSoundEffects.tomatoSFX = Content.Load<SoundEffect>("Audio/tomatoSFX");
            MusicAndSoundEffects.angrymobSFX = Content.Load<SoundEffect>("Audio/angrymobSFX");
            angrymobInstance = MusicAndSoundEffects.angrymobSFX.CreateInstance();
            angrymobInstance.IsLooped = true;
            angrymobInstance.Volume = 0.3f;

            if(SOUND_ENABLED) {
                MediaPlayer.Play(MusicAndSoundEffects.bgMusic);
                angrymobInstance.Play();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

        
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move all the projectiles
            foreach (Projectile projectile in Projectile.active)
            {
                projectile.updateWrap(dt);
                if(projectile.Position.LengthSquared()>100f)
                    hitProjectiles.AddLast(projectile);
            }

            // Move players
            foreach (Player player in Player.active)
            {
                player.updateWrap(dt);
            }

            // Players bumping into each other
            for(int i = 0; i<Player.active.Count; ++i)
                for(int j = i+1; j<Player.active.Count; ++j)
                    Player.active[i].playerCollision(Player.active[j]);

            // Check if players got hit / grabbed something
            nAlivePlayers = 0;
            foreach (Player player in Player.active)
            {
                if (player.Life > 0)
                {
                    nAlivePlayers++;
                    lastPlayer = player;
                    foreach (Projectile projectile in Projectile.active)
                    {
                        if (projectile.Free() && player.GrabOrHit(projectile))
                            hitProjectiles.AddLast(projectile);
                    }
                }
            }

            // Remove projectiles that hit someone
            foreach (Projectile projectile in hitProjectiles)
                Projectile.active.Remove(projectile);

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
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[3].Stamina, new Vector2(1500, 950), Color.Green);
                    for(int i = 0; i < Player.active[3].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(1500 + 60*i, 910), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }          
                    goto case 3;
                case 3:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[2].Stamina, new Vector2(10, 950), Color.Yellow);
                    for(int i = 0; i < Player.active[2].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(10 + 60*i, 910), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 2;
                case 2:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[1].Stamina, new Vector2(1500, 50), Color.Blue);
                    for(int i = 0; i < Player.active[1].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(1500 + 60*i, 10), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 1;
                case 1:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[0].Stamina, new Vector2(10, 50), Color.Red);
                    for(int i = 0; i < Player.active[0].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(10 + 60*i, 10), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto default;
                default:
                    break;
            }
        }

        private void DrawWin()
        {
            if (nAlivePlayers == 1)
            {
                lastPlayer.notImportant = true;
                Texture2D pixel;
                pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });

                // Define text
                string winMessage = "Player " + (lastPlayer.Id+1) + " wins!";

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
            //arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            // Draw all active projectiles:
            foreach (Projectile projectile in Projectile.active)
            {
                projectile.Draw(view, projection);
                //projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all players
            foreach (Player player in Player.active)
            {
                player.Draw(view, projection);
                //player.Hitbox.DebugDraw(GraphicsDevice, view, projection);
            } 

            // Draw mob and player statistics:
            mob.Draw(view, projection);
            DrawHealthAndStamina();
            DrawWin();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
