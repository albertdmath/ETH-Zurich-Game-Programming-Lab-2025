using System;
using System.Collections.Generic;
using System.Linq;
using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace src.GameObjects
{
    /** This class manages menu state. It looks weird cause it uses Singleton pattern. If you refactor I will cut of your prenats.
      * This holds the state of the game, active stuff and so on AND the functionality to reset it.
      */
    public class GameStateManager
    {
        private const float ARENA_SCALE = 5f;

        private static readonly Dictionary<ProjectileType, (float scale, float height)> Properties = new()
        {
            { ProjectileType.Tomato, (1f, 0f) },
            { ProjectileType.Swordfish, (0.9f, 0f) },
            { ProjectileType.Frog, (0.7f, 0.15f) },
            { ProjectileType.Coconut, (0.3f, 0f) },
            { ProjectileType.Banana, (1f, 0f) },
            { ProjectileType.Turtle, (0.3f, 0f) },
            { ProjectileType.Mjoelnir, (1.5f, 0.2f) },
            { ProjectileType.Spear, (0.9f, 0f) },
            { ProjectileType.Chicken, (0.9f, 0f) },
            { ProjectileType.Barrel, (1f, 0f) }
        };


        // Model references for initializing the instances
        private DrawModel arenaModel;
        private List<DrawModel> marketModels;
        private List<DrawModel> playerHatModels;
        private DrawModel playerModel;
        private DrawModel playerModelShell;
        private DrawModel playerHandModel;
        private List<DrawModel> indicatorModel;

        private List<DrawModel> mobModels;
        private List<DrawModel> areaDamageModels;
        private Dictionary<ProjectileType, DrawModel> projectileModels;
        private GameModel arena;
        private DrawModel walkingTurtle;
        private DrawModel barrel2;

        // This is the mob that might or might not be angry
        private Mob mob;
        public readonly List<Player> players = new List<Player>();
        public readonly List<Player> livingPlayers = new List<Player>();
        public readonly List<Projectile> projectiles = new List<Projectile>();
        public readonly List<Market> markets = new List<Market>();

        private MenuStateManager menuStateManager;

        // Singleton instancing
        private GameStateManager() { }

        private static readonly GameStateManager instance = new();

        public static GameStateManager GetGameStateManager()
        {
            return instance;
        }

        public void Initialize(DrawModel arenaModel, List<DrawModel> marketModels, List<DrawModel> playerHatModels, DrawModel playerModel, DrawModel playerModelShell, DrawModel playerHandModel, List<DrawModel> indicatorModel,  List<DrawModel> mobModels, List<DrawModel> areaDamageModels, Dictionary<ProjectileType, DrawModel> projectileModels, DrawModel walkingTurtle, DrawModel barrel2)
        {
            this.menuStateManager = MenuStateManager.GetMenuStateManager();
            this.arenaModel = arenaModel;
            this.marketModels = marketModels;
            this.playerHatModels = playerHatModels;
            this.playerModel = playerModel;
            this.playerHandModel = playerHandModel;
            this.mobModels = mobModels;
            this.areaDamageModels = areaDamageModels;
            this.projectileModels = projectileModels;
            this.indicatorModel = indicatorModel;
            this.playerModelShell = playerModelShell;
            this.walkingTurtle = walkingTurtle;
            this.barrel2 = barrel2;

            arena = new GameModel(arenaModel, ARENA_SCALE);
            // jesterGame = new GameModel(jesterModel,2.0f);
            // jesterGame.SwitchAnimation(0,true);
            // jesterGame.Orientation = Vector3.Normalize(new Vector3(0f, 9, 7));
    
        }

        public void InitializeMob() { mob = new(mobModels); }

        public void InitializePlayers()
        {
            float[] playerStartPositions = { -1.5f, -0.5f, 0.5f, 1.5f };
            float scaling = 0.5f;
            /*
            Input[] inputs = { new InputDual(new Input(),new InputController(PlayerIndex.One)), 
                new InputDual(new InputKeyboard(),new InputController(PlayerIndex.Two)), 
                new InputController(PlayerIndex.Three), new InputController(PlayerIndex.Four)};
            for(int i = 0; i<MenuStateManager.GetMenuStateManager().NUM_PLAYERS; ++i)
                players.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), inputs[i], 0, mob.Ellipse, playerModels[i], scaling));
            SRY BOUT THAT*/
            players.Add(new Player(new Vector3(playerStartPositions[0], 0, 0), new InputControllerKeyboard(0), 0, mob.Ellipse, playerModel, playerModelShell, playerHandModel, playerHatModels[0], indicatorModel[0], indicatorModel[4], scaling));
            //players.Add(new Player(new Vector3(playerStartPositions[1], 0, 0), new InputKeyboard(), 1, mob.Ellipse, playerModels[1], scaling));
            for (int i = 1; i < menuStateManager.NUM_PLAYERS; ++i)
            {
                players.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), (GamePad.GetState(i).IsConnected) ? new InputController((PlayerIndex)i) : new InputKeyboard(), i, mob.Ellipse, playerModel, playerModelShell, playerHandModel, playerHatModels[i], indicatorModel[i], indicatorModel[i+4], scaling));
            }

            foreach (Player player in players)
                livingPlayers.Add(player);
        }

        private void InitializeMarkets()
        {
            // Market positions (corners)
            Vector3[] positions = 
            {
                new(-7.8f, 0, -3.5f),
                new(7.8f, 0, -3.5f),
                new(-5.7f, 0, 3.7f),
                new(4.7f, 0, 4.2f)
            };

            // Random projectile type selection logic
            //this should check for throwable
            List<ProjectileType> availableTypes = Projectile.ProjectileProbability.Keys
                                                .Where(type => Projectile.ProjectileProbability[type] > 0)
                                                .ToList();
                                                
            float totalWeight = availableTypes.Sum(type => Projectile.ProjectileProbability[type]);

            for (int i = 0; i < 4; i++)
            {
                // Refill if empty
                if (!availableTypes.Any()) 
                    availableTypes = Projectile.ProjectileProbability.Keys
                                    .Where(type => Projectile.ProjectileProbability[type] > 0)
                                    .ToList();
                
                float randomValue = Rng.NextFloat(totalWeight);
                ProjectileType selectedType = default;

                foreach (var type in availableTypes)
                {
                    randomValue -= Projectile.ProjectileProbability[type];
                    if (randomValue > 0) continue;
                    
                    selectedType = type;
                    availableTypes.Remove(type);
                    totalWeight -= Projectile.ProjectileProbability[type];
                    break;
                }
                // Create market with selected type
                markets.Add(new Market(positions[i], selectedType, marketModels[i], projectileModels[selectedType], Properties[selectedType].height, 4f));
            }
        }

        public Projectile CreateProjectile(ProjectileType type)
        {
            Projectile projectile;
            switch (type)
            {
                case ProjectileType.Frog:
                    projectile = new Frog(type, projectileModels[ProjectileType.Frog], Properties[ProjectileType.Frog].scale, Properties[ProjectileType.Frog].height);
                    break;
                case ProjectileType.Swordfish:
                    projectile = new Swordfish(type, projectileModels[ProjectileType.Swordfish], Properties[ProjectileType.Swordfish].scale, Properties[ProjectileType.Swordfish].height);
                    break;
                case ProjectileType.Tomato:
                    projectile = new Tomato(type, projectileModels[ProjectileType.Tomato], areaDamageModels[1], Properties[ProjectileType.Tomato].scale, Properties[ProjectileType.Tomato].height);
                    break;
                case ProjectileType.Coconut:
                    projectile = new Coconut(type, projectileModels[ProjectileType.Coconut], Properties[ProjectileType.Coconut].scale, Properties[ProjectileType.Coconut].height);
                    break;
                case ProjectileType.Banana:
                    projectile = new Banana(type, projectileModels[ProjectileType.Banana], Properties[ProjectileType.Banana].scale, Properties[ProjectileType.Banana].height);
                    break;
                case ProjectileType.Turtle:
                    projectile = new Turtle(type, projectileModels[ProjectileType.Turtle], walkingTurtle, Properties[ProjectileType.Turtle].scale, Properties[ProjectileType.Turtle].height);
                    break;
                case ProjectileType.Spear:
                    projectile = new Spear(type, projectileModels[ProjectileType.Spear], Properties[ProjectileType.Spear].scale, Properties[ProjectileType.Spear].height);
                    break;
                case ProjectileType.Mjoelnir:
                    projectile = new Mjoelnir(type, projectileModels[ProjectileType.Mjoelnir], areaDamageModels[0], Properties[ProjectileType.Mjoelnir].scale, Properties[ProjectileType.Mjoelnir].height);
                    break;
                case ProjectileType.Chicken:
                    projectile = new Chicken(type, projectileModels[ProjectileType.Chicken], Properties[ProjectileType.Chicken].scale, Properties[ProjectileType.Chicken].height);
                    break;
                case ProjectileType.Barrel:
                    projectile = new Barrel(type, projectileModels[ProjectileType.Barrel], barrel2, Properties[ProjectileType.Barrel].scale, Properties[ProjectileType.Barrel].height);
                    break;
                default:
                    throw new ArgumentException("Invalid projectile type: " + type);
            }
            projectiles.Add(projectile);
            return projectile;
        }

        public void UpdateGame(float dt)
        {        
            //PROJECTILE COLLISION CHECKS WITH...
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = projectiles[i];

                //OUT OF BOUNDS
                if (MathF.Abs(projectile.Position.X) > GameLabGame.ARENA_HEIGHT || MathF.Abs(projectile.Position.Z) > GameLabGame.ARENA_WIDTH)
                {
                    projectiles.RemoveAt(i);
                    continue;
                }

                //GROUND
                if (projectile.Position.Y < 0f)
                    projectile.OnGroundHit();
                
                // ELLIPSES
                if (mob.Ellipse.Outside(projectile.Position.X, projectile.Position.Z))
                    projectile.OnMobHit();
                
                // PROJECTILE
                for (int j = i-1; j >= 0; j--)
                {
                    Projectile projectile1 = projectiles[j];
                    if (projectile.Hitbox.Intersects(projectile1.Hitbox))
                    {
                        projectile.OnProjectileHit(projectile1);
                        projectile1.OnProjectileHit(projectile);
                    }
                }

                // PLAYER
                if(projectile.Holder == null)
                {
                    foreach (Player player in players)
                    {
                        if(player.Life > 0 && projectile.Hitbox.Intersects(player.Hitbox))
                        {
                            projectile.OnPlayerHit(player);
                        }
                    }
                }
            }

            projectiles.RemoveAll(x => x.ToBeDeleted);

            // PLAYER COLLISION CHECKS WITH...
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];

                //OUT OF BOUNDS
                //if (MathF.Abs(player.Position.X) > GameLabGame.ARENA_HEIGHT || MathF.Abs(player.Position.Z) > GameLabGame.ARENA_WIDTH)
                    //player.onBoundsHit();

                //ELLIPSES
                //if (mob.Ellipse.Outside(projectile.Position.X, projectile.Position.Z))
                    //player.OnMobHit();

                //MARKET
                foreach (Market market in markets)
                {
                    if (player.Hitbox.Intersects(market.Hitbox))
                        player.ObjectCollision(market);
                }

                // PLAYER
                for (int j = i + 1; j < players.Count; j++)
                    player.PlayerCollision(players[j]);
                
                // CATCHING
                Hand hand = player.Hand;
                if(hand.IsCatching)
                {
                    // PROJECTILE
                    foreach (Projectile projectile in projectiles)
                    {
                        if (hand.Hitbox.Intersects(projectile.Hitbox))
                            player.Catch(projectile);
                    }

                    // MARKET
                    foreach (Market market in markets)
                    {
                        if (hand.Hitbox.Intersects(market.Hitbox) && market.GrabProjectile())
                        {
                            Projectile projectile = CreateProjectile(market.Type);
                            player.Catch(projectile);
                        }
                    }
                }
            }

            projectiles.RemoveAll(x => x.ToBeDeleted);
            
            // UPDATES
            mob.updateWrap(dt);

            foreach (Player player in players)
                player.updateWrap(dt);
            
            foreach (Market market in markets)
                market.updateWrap(dt);
            
            foreach (Projectile projectile in projectiles)
                projectile.updateWrap(dt);
        }


        public void ShaderTest(Shader testShader, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            testShader.setWorldMatrix(Matrix.Identity);
            testShader.setViewMatrix(view);
            testShader.setProjectionMatrix(projection);
            arena.Draw(view, projection, testShader, graphicsDevice, false);
        }
        public void DrawGame(RenderTarget2D output, PBR lightingShader, GraphicsDevice graphicsDevice, VertexBuffer fullScreenQuad, RenderTarget2D FragPosMap, RenderTarget2D NormalMap, RenderTarget2D AlbedoMap, RenderTarget2D RoughnessMetallicMap, RenderTarget2D ShadowMap, RenderTarget2D OcclusionMap, SpriteBatch spriteBatch, bool test)
        {
            graphicsDevice.SetRenderTarget(output);
            graphicsDevice.SetVertexBuffer(fullScreenQuad);
            graphicsDevice.Clear(Color.White);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            lightingShader.setFragPosTexture(FragPosMap);
            lightingShader.setNormalTexture(NormalMap);
            lightingShader.setTexture(AlbedoMap);
            lightingShader.setShadowTexture(ShadowMap);
            lightingShader.setRoughnessTexture(RoughnessMetallicMap);
            if (OcclusionMap != null){
            lightingShader.setOcclusionTexture(OcclusionMap);
            }

            // You don’t need an index buffer for this simple triangle list
            foreach (var pass in lightingShader.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2); // 2 triangles = 6 vertices
            }
            graphicsDevice.SetRenderTarget(null);
          if (test)
            {
                if(output != null){
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(output, new Rectangle(400, 400, 400, 400), Color.White);
                spriteBatch.End(); 
                }
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(FragPosMap, new Rectangle(0, 0, 400, 400), Color.White);
                spriteBatch.End(); 
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(NormalMap, new Rectangle(0, 400, 400, 400), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(AlbedoMap, new Rectangle(0, 800, 400, 400), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(ShadowMap, new Rectangle(400, 0, 400, 400), Color.White);
                spriteBatch.End();

            }

        }

        public void DepthMapPass(Shader depthShader, Matrix view, Matrix projection, GraphicsDevice graphicsDevice, RenderTarget2D depthMap, SpriteBatch spriteBatch, bool test)
        {
            graphicsDevice.SetRenderTarget(depthMap);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            // graphicsDevice.RasterizerState = this.shadowRasterizer;

            arena.Draw(view, projection, depthShader, graphicsDevice, true);
            // arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            foreach (Market market in markets)
            {
                market.Draw(view, projection, depthShader, graphicsDevice, true);
                market.DrawFish(view, graphicsDevice, depthShader, true);
            }

            // Draw all active projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(view, projection, depthShader, graphicsDevice, true);
                // projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all players
            foreach (Player player in players)
            {
                player.Draw(view, projection, depthShader, graphicsDevice, true);
                //player.Hitbox.DebugDraw(graphicsDevice, view, projection);
            }
            mob.Draw(view, projection, depthShader, graphicsDevice, true);
            graphicsDevice.SetRenderTarget(null);
          if (test)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(depthMap, new Rectangle(0, 0, 800, 800), Color.White);
                spriteBatch.End(); 

            }
        }

        public void HBAOPass(HBAOShader hBAOShader, RenderTarget2D PosMap, RenderTarget2D NormalMap, RenderTarget2D HBAOmap, VertexBuffer fullscreenquad, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, bool test){
            graphicsDevice.SetRenderTarget(HBAOmap);
            graphicsDevice.SetVertexBuffer(fullscreenquad);
            graphicsDevice.Clear(Color.White);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            hBAOShader.setFragPosTexture(PosMap);
            hBAOShader.setNormalTexture(NormalMap);
            foreach (var pass in hBAOShader.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2); // 2 triangles = 6 vertices
            }
            graphicsDevice.SetRenderTarget(null);
          if (test)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(HBAOmap, new Rectangle(0, 0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                spriteBatch.End(); 

            }
        }
        public void GeometryPass(Shader geometryShader, Shader shadowShader, Matrix view, Matrix projection, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap, RenderTargetBinding[] targets, SpriteBatch spriteBatch, bool test)
        {
            graphicsDevice.SetRenderTarget(shadowMap);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            // graphicsDevice.RasterizerState = this.shadowRasterizer;

            arena.Draw(view, projection, shadowShader, graphicsDevice, true);
            // arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            foreach (Market market in markets)
            {
                market.Draw(view, projection, shadowShader, graphicsDevice, true);
                market.DrawFish(view, graphicsDevice, shadowShader, true);
            }
            // jesterGame.Draw(view, projection, shadowShader, graphicsDevice, true);

            // Draw all active projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(view, projection, shadowShader, graphicsDevice, true);
                // projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all players
            foreach (Player player in players)
            {
                player.Draw(view, projection, shadowShader, graphicsDevice, true);
                //player.Hitbox.DebugDraw(graphicsDevice, view, projection);
            }
            mob.Draw(view, projection, shadowShader, graphicsDevice, true);
            
            graphicsDevice.SetRenderTargets(targets);
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
             graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // graphicsDevice.RasterizerState = this.renderingRasterizer;

            // Set background color
            graphicsDevice.Clear(Color.DeepSkyBlue);

            geometryShader.setMetallic(arena.DrawModel.metallic);
            geometryShader.setRoughness(arena.DrawModel.roughness);

            arena.Draw(view, projection, geometryShader, graphicsDevice, false);
            // Matrix[] check = jesterGame.GetFinalBoneMatrices();
            //     geometryShader.setFinalBoneMatrices(jesterGame.GetFinalBoneMatrices());
            //     geometryShader.setRoughness(jesterGame.DrawModel.roughness);
            //    jesterGame.Draw(view, projection, geometryShader, graphicsDevice, false);
            // arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            
            // //draw all markets
            foreach (Market market in markets)
            {
                geometryShader.setMetallic(market.DrawModel.metallic);
                geometryShader.setRoughness(market.DrawModel.roughness);
                market.Draw(view, projection, geometryShader, graphicsDevice, false);
                market.DrawFish(view, graphicsDevice, geometryShader, false);
                //market.Hitbox.DebugDraw(graphicsDevice,view,projection);
            }

            // Draw all active projectiles:
            foreach (Projectile projectile in projectiles)
            {
                geometryShader.setMetallic(projectile.DrawModel.metallic);
                geometryShader.setRoughness(projectile.DrawModel.roughness);

                projectile.Draw(view, projection, geometryShader, graphicsDevice, false);
                // projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all Players
            foreach (Player player in players)
            {
                if (player.IsCrawling())
                {
                    graphicsDevice.BlendState = BlendState.NonPremultiplied;
                    geometryShader.setOpacityValue(0.4f);

                }
                else
                {
                    graphicsDevice.BlendState = BlendState.Opaque;
                    geometryShader.setOpacityValue(1.0f);
                }

                geometryShader.setMetallic(player.DrawModel.metallic);
                geometryShader.setRoughness(player.DrawModel.roughness);

                player.Draw(view, projection, geometryShader, graphicsDevice, false);
                //player.Hitbox.DebugDraw(graphicsDevice, view, projection);
            }

            // Draw mob
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            geometryShader.setOpacityValue(1.0f);
            mob.Draw(view, projection, geometryShader, graphicsDevice, false);
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            geometryShader.setOpacityValue(0.2f);

            graphicsDevice.BlendState = BlendState.Opaque;
            geometryShader.setOpacityValue(1.0f);
            // graphicsDevice.BlendState = BlendState.NonPremultiplied;
            // lightingShader.setOpacityValue(0.7f);
            geometryShader.setRoughness(0.3f);
            geometryShader.setMetallic(0.0f);
            // graphicsDevice.BlendState = BlendState.Opaque;
            // lightingShader.setOpacityValue(1.0f);



            graphicsDevice.SetRenderTarget(null);
            if (test)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw((RenderTarget2D)targets[0].RenderTarget, new Rectangle(0, 0, 400, 400), Color.White);
                spriteBatch.End(); 
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw((RenderTarget2D)targets[1].RenderTarget, new Rectangle(0, 400, 400, 400), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw((RenderTarget2D)targets[2].RenderTarget, new Rectangle(400, 0, 400, 400), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(shadowMap, new Rectangle(400, 400, 400, 400), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw((RenderTarget2D)targets[3].RenderTarget, new Rectangle(800, 0, 400, 400), Color.White);
                spriteBatch.End();

            }


        }

public void FilterPass(Filter filterShader, RenderTarget2D inputTexture, RenderTarget2D normalTexture, RenderTarget2D fragPosTexture, RenderTarget2D outputTexture, GraphicsDevice graphicsDevice, VertexBuffer fullscreenquad, SpriteBatch spriteBatch, bool test)
        {
            graphicsDevice.SetRenderTarget(outputTexture);
            graphicsDevice.SetVertexBuffer(fullscreenquad);
            graphicsDevice.Clear(Color.White);
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            if(fragPosTexture != null){
            filterShader.setFragPosTexture(fragPosTexture);
            }   
            if(normalTexture != null){
                 filterShader.setNormalTexture(normalTexture);
            }   
            if(inputTexture != null){             
            filterShader.setAlbedoTexture(inputTexture);
            }


        
            foreach (var pass in filterShader.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2); // 2 triangles = 6 vertices
            }
            graphicsDevice.SetRenderTarget(null);
          if (test)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(outputTexture, new Rectangle(0, 0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                spriteBatch.End(); 

            }
        }

        public void StartNewGame()
        {
            players.Clear();
            projectiles.Clear();
            markets.Clear();
            livingPlayers.Clear();

            InitializeMob();
            InitializeMarkets();
            InitializePlayers();
        }

        public bool GameIsOver(){
            return livingPlayers.Count() == 1 && !(menuStateManager.NUM_PLAYERS==1);
        }
    }
}