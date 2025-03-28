using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

//using System.Numerics;

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


        // Camera settings
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 9, 7), new Vector3(0, 0, 0.7f), Vector3.Up);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        // Arena transformations
        public const float ARENA_HEIGHT = 9f, ARENA_WIDTH = 15f;
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
            //this should be the coconut model
            projectileModels.Add(ProjectileType.Coconut, Content.Load<Model>("tomato"));


            // Initialize game models (they are only known at this point so they can't be in the initialize method)
            arenaModel = new GameModel(arena,0.5f);

            // Initialize mob
            mob = new Mob(mobs);

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

            // Update the projectiles
            // this is done to avoid modifying the list while iterating over it
            for (int i = Projectile.active.Count - 1; i >= 0; i--) 
                Projectile.active[i].updateWrap(dt);
            

            // Move players
            foreach (Player player in Player.active)
                player.updateWrap(dt);
            

            // CAN THIS NOT BE MOVED INSIDE THE UPDATE OF PLAYERS
            // Players bumping into each other
            for(int i = 0; i<Player.active.Count; ++i)
                for(int j = i+1; j<Player.active.Count; ++j)
                    Player.active[i].playerCollision(Player.active[j]);

            // Update mob
            mob.Update(dt);

            base.Update(gameTime);
        }

        private void DrawHealthAndStamina()
        {
            switch (Player.active.Count)
            {
                case 4:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[3].Stamina, new Vector2(1500, 950), Color.Yellow);
                    for(int i = 0; i < Player.active[3].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(1500 + 60*i, 910), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }          
                    goto case 3;
                case 3:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[2].Stamina, new Vector2(10, 950), Color.Green);
                    for(int i = 0; i < Player.active[2].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(10 + 60*i, 910), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 2;
                case 2:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[1].Stamina, new Vector2(1500, 50), Color.Pink);
                    for(int i = 0; i < Player.active[1].Life; i++) {
                        _spriteBatch.Draw(playerHearts, new Vector2(1500 + 60*i, 10), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 1;
                case 1:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)Player.active[0].Stamina, new Vector2(10, 50), Color.Blue);
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
            //this can be also used for the hit projectiles
            var alivePlayers = Player.active.Where(p => p.Life > 0).ToList();
            if (alivePlayers.Count() == 1)
            {
                Texture2D pixel;
                pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });

                // Define text
                string winMessage = "Player " + (alivePlayers[0].Id+1) + " wins!";

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
