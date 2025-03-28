using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Accord.Collections;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : GameModel
    {
        public static List<Player> active = new List<Player>();

        public int Id { get; set; }
        // Private fields:
        private float playerSpeed = 2f;
        public int Life { get; set; } = 5;
        public float Stamina { get; set; } = 0f;
        public Projectile projectileHeld;
        private float dashTime = 0f;
        private const float TIME_CATCH_THROW = 0.5f;
        private float timeSinceThrow = 0f;
        private float actionPushedDuration;
        private float stunDuration = 0f;
        public bool notImportant = false;
        private bool mob = false;

        private Input input;
        private Ellipse ellipse;

        private Vector3 Inertia;

        public Player(Vector3 position, Input input, int id, Ellipse ellipse, Model model,float scale) : base(model,scale)
        {
            Position = position;
            Orientation = new Vector3(0,0,1f);
            this.input = input;
            this.ellipse = ellipse;
            projectileHeld = null;
            this.Id = id;
            Inertia = new Vector3(0,0,0);
            // Remove hat from hitbox; this is trashcode and needs to be removed / done better at some point
            this.Hitbox.BoundingBoxes.RemoveAt(this.Hitbox.BoundingBoxes.Count - 1);
        }

        public static void Initialize(Ellipse ellipse, List<Model> models)
        {
            float[] playerStartPositions = { -1.5f, -0.5f, 0.5f, 1.5f };

            active.Add(new Player(new Vector3(playerStartPositions[0], 0, 0), new Input(), 0, ellipse, models[0], 0.5f));
            active.Add(new Player(new Vector3(playerStartPositions[1], 0, 0), new InputKeyboard(), 1, ellipse, models[1], 0.5f));

            /*
            // TODO: fix player creation
            for (int i = 0; i < GameLabGame.NUM_PLAYERS; i++)
            {
                PlayerIndex idx = (PlayerIndex)i;
                if (GamePad.GetState(idx).IsConnected)
                    active.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), new InputController(idx), i, ellipse, models[i], 0.5f));
            }
            */
        }

        // The player move method:
        public void Move(float dt)
        {
            Vector3 dir = input.Direction();
            //inertia to keep some movement from last update;
            Inertia -=(9f*dt)* Inertia;
            if (stunDuration<=0f && dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Inertia += (9f*dt)*dir;
                // Limit Inertia to a vector of length 1
                if (Inertia.Length() > 1f)
                    Inertia = Vector3.Normalize(Inertia);
            }
            // Only update orientation if inertia is not 0
            if (Inertia.Length() > 0)
            {
                Orientation = Vector3.Normalize(Inertia);
            }
            // Updating the position of the player
            Position += playerSpeed * Inertia * dt;
        }

        // Simple method to move back after collision. Not used and outdated
        public void MoveBack(float dt)
        {
            Position -= playerSpeed * Orientation * dt * 1f;
        }


        // Method to test for a collision with a projectile and potentially grab it:
        public bool GrabOrHit(Projectile projectile)
        {
            if (projectile != projectileHeld && Hitbox.Intersects(projectile.Hitbox))
            {
                // case 1 no projectile held and action button pressed not to long ago -> catch it
                if (stunDuration<=0f && input.Action()
                    && projectileHeld == null
                    && actionPushedDuration<0.7f
                    )
                {
                    projectileHeld = projectile;
                    timeSinceThrow = 0f;
                    projectile.Catch(this);
                    Console.WriteLine("Grabbing " + projectile.Type);
                    // Here the player speed is set for the movement with projectile in hand
                    playerSpeed = 0.9f;

                    // Handle the equip sound effect.
                    // For anyone reading, it takes 3 parameters: volume, pitch, pan.
                    MusicAndSoundEffects.equipSFX.Play(0.7f, 0.0f, 0.0f);
                    return false;
                } else // the player is hit by the projectile
                {
                    Life -= notImportant ? 0 : 1;
                    if (Life == 0f)
                    {
                        // For now the player is moved down to indacet crawling. Later done with an animation
                        Position = Position - new Vector3(0, 0.2f, 0);
                        playerSpeed = 1f;
                    }

                    // SFX handling:
                    if(GameLabGame.SOUND_ENABLED) {
                        switch(projectile.Type)
                        {
                            case ProjectileType.Frog:
                                MusicAndSoundEffects.frogSFX.Play(0.5f, 0.0f, 0.0f);
                                break;
                            case ProjectileType.Swordfish:
                                MusicAndSoundEffects.swordfishSFX.Play(0.8f, 0.0f, 0.0f);
                                break;
                            case ProjectileType.Tomato:
                                MusicAndSoundEffects.tomatoSFX.Play(0.9f, 0.0f, 0.0f);
                                break;
                            default:
                                break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        // Method to throw an object:
        private void Throw(float dt)
        {
            if(timeSinceThrow == 0f)
            {
                if (input.Action() && actionPushedDuration == 0f) // Aiming
                {
                    playerSpeed = 0f;
                }
                else if (!input.Action() && actionPushedDuration > 0f && playerSpeed == 0f) // Releasing projectile
                {
                    if(projectileHeld.Type!= ProjectileType.Frog){
                        actionPushedDuration = (actionPushedDuration < 4f) ? actionPushedDuration : 4f;
                        float speedUp = 1f+ actionPushedDuration * actionPushedDuration * 2f;
                        projectileHeld.Throw(speedUp);
                        playerSpeed = 2f;
                        timeSinceThrow += dt;
                        Console.WriteLine("Throwing projectile with orientation: " + Orientation+ " and speedup: " +speedUp);
                    }else{
                        Projectile.active.Remove(projectileHeld);
                        playerSpeed = 2f;
                        Life++;
                        timeSinceThrow += dt;
                        Console.WriteLine("Eating frog. ");
                    }
                }
            }else { // Immune to hit by thrwon projectile for 1s. Also blocks catchin a new projectile
                if(timeSinceThrow > 1f)
                    projectileHeld = null;
                timeSinceThrow += dt;
            }
        }

        // Method to dash. Current dash cost 30
        private bool Dash(float dt)
        {
            if (stunDuration<=0f && input.Dash() && dashTime <= 0f && Stamina > 30f)
            {
                dashTime = 0.1f;
                Stamina -= 30f;
            }
            if (dashTime > 0f)
            {
                Position += 6 * playerSpeed * Orientation * dt;
                dashTime -= dt;
                return true;
            }
            else
            {
                return false;
            }
        }
        // Spawning a projectile in hand when part of the mob. Currently only swordfish
        private bool Spawn()
        {
            if(input.Action() && Stamina>40f && projectileHeld == null)
            {
                projectileHeld = Projectile.CreateProjectile(ProjectileType.Swordfish,Position,Orientation);
                projectileHeld.Catch(this);
                playerSpeed = 0.9f;
                timeSinceThrow = 0f;
                playerSpeed = 0.3f;
                Stamina -= 40f;
                return false;
            }
            return true;
        }

        public void playerCollision(Player player){
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X-player.Position.X,0f,Position.Z-player.Position.Z));
            while(Hitbox.Intersects(player.Hitbox)){
                Position += dir;
                player.Position -= dir;
                updateHitbox();
                player.updateHitbox();
            }
        }
        public void mobCollision(Zombie zombie){
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X-zombie.Position.X,0f,Position.Z-zombie.Position.Z));
            if(Hitbox.Intersects(zombie.Hitbox))
            {
                while(Hitbox.Intersects(zombie.Hitbox)){
                    Position += dir;
                    updateHitbox();
                }
                Orientation = ellipse.Normal(Position.X,Position.Z);
                Inertia = 1.6f * Orientation;
                stunDuration = 1f;
            }
        }
        // Update function called each update
        public override void Update(float dt)
        {
            Stamina += dt * 5f;
            Stamina = (Stamina > 100f) ? 100f : Stamina;
            if (Life > 0f) // Behaviour when alive
            {   
                stunDuration -= dt;
                if (!Dash(dt))
                {
                    Move(dt);
                    if (stunDuration<=0f && projectileHeld != null)
                    {
                        Throw(dt);
                    }
                }
            } else if(mob) // Behaviour when part of mob
            {
                if(Spawn())
                {
                    Move(dt);
                    if (projectileHeld != null)
                    {
                        Throw(dt);
                    }
                    while(ellipse.Inside(Position.X,Position.Z))
                        Position += ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
                }
            } else // Crawling
            {
                Move(dt);
                if (ellipse.Outside(Position.X,Position.Z))
                {
                    Position = Position + new Vector3(0, 0.2f, 0);
                    mob = true;
                    playerSpeed = 2f;
                }
            }
            //notImportant = notImportant || (input.Action() && input.Dash());
            actionPushedDuration = (input.Action()) ? actionPushedDuration + dt : 0f;
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            if(stunDuration<=0f || ((int)(stunDuration*10f))%2==0)
                base.Draw(view, projection);
        }
    }
}