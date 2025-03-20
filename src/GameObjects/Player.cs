using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : GameModel
    {
        public static List<Player> active = new List<Player>();

        public int Id { get; set; }
        // Private fields:
        private float playerSpeed = 2f;
        public int Life { get; set; } = 3;
        public float Stamina { get; set; } = 0f;
        public Projectile projectileHeld;
        //private ProjectileType typeOfProjectileHeld = ProjectileType.None;
        private float dashTime = 0f;
        private const float TIME_CATCH_THROW = 0.5f;
        private float timeSinceThrow = 0f;
        private float actionPushedDuration;
        public bool notImportant = false;
        private bool mob = false;

        private Input input;
        private Ellipse ellipse;

        private Vector3 Inertia;

        public Player(Vector3 position, Input input, int id, Ellipse ellipse, Model model) : base(model)
        {
            Position = position;
            this.input = input;
            this.ellipse = ellipse;
            projectileHeld = null;
            this.Id = id;
            Inertia = new Vector3(0,0,0);
        }

        public static void Initialize(Ellipse ellipse, Model model)
        {
            float[] playerStartPositions = { -0.75f, -0.25f, 0.25f, 0.75f };

            // This should be removed, for Debug!
            active.Add(new Player(new Vector3(playerStartPositions[0], 0, 0), new Input(), 0, ellipse, model));

            // TODO: fix player creation
            for (int i = 0; i < GameLabGame.NUM_PLAYERS; i++)
            {
                PlayerIndex idx = (PlayerIndex)i;
                if (GamePad.GetState(idx).IsConnected)
                    active.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), new InputController(idx), i + 1, ellipse, model));
            }
        }

        // The player move method:
        public void Move(float dt)
        {
            Vector3 dir = input.Direction();
            //inertia to keep some movement from last update;
            Inertia -=(5f*dt)* Inertia;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Inertia += (5f*dt)*dir;
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
                if (input.Action()
                    && projectileHeld == null
                    && actionPushedDuration<0.7f
                    )
                {
                    projectileHeld = projectile;
                    projectile.Caught(this);
                    timeSinceThrow = 0f;
                    Console.WriteLine("Grabbing " + projectile.Type);
                    // Here the player speed is set for the movement with projectile in hand
                    playerSpeed = 0.3f;
                    return false;
                } else // the player is hit by the projectile
                {
                    Life -= notImportant ? 0 : 1;
                    if (Life == 0f)
                    {
                        // For now the player is moved down to indacet crawling. Later done with an animation
                        Position = Position - new Vector3(0, -0.2f, 0);
                        playerSpeed = 1f;
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
                    actionPushedDuration = (actionPushedDuration < 4f) ? actionPushedDuration : 4f;
                    float speedUp = 1f+ actionPushedDuration * actionPushedDuration * 0.5f;
                    projectileHeld.Throw(speedUp);
                    playerSpeed = 2f;
                    timeSinceThrow += dt;
                    Console.WriteLine("Throwing projectile with orientation: " + Orientation+ " and speedup: " +speedUp);
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
            if (input.Dash() && dashTime <= 0f && Stamina > 30f)
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
                projectileHeld = Projectile.createProjectile(ProjectileType.Swordfish,Position,Orientation,GameLabGame.projectileModels[ProjectileType.Swordfish]);
                projectileHeld.Caught(this);
                playerSpeed = 0.3f;
                Stamina -= 40f;
                return false;
            }
            return true;
        }

        // Update function called each update
        public override void Update(float dt)
        {
            Stamina += dt * 5f;
            Stamina = (Stamina > 100f) ? 100f : Stamina;
            if (Life > 0f) // Behaviour when alive
            {
                if (!Dash(dt))
                {
                    Move(dt);
                    if (projectileHeld != null)
                    {
                        Throw(dt);
                    }
                    else
                    {
                        
                    }
                }
                while(ellipse.Outside(Position.X,Position.Z))
                    Position += playerSpeed * ellipse.Normal(Position.X,Position.Z) * dt * 0.1f;
            } else if(mob) // Behaviour when part of mob
            {
                if(Spawn())
                {
                    Move(dt);
                    if (projectileHeld != null)
                    {
                        Throw(dt);
                    }
                    else
                    {
                        timeSinceThrow += dt;
                        projectileHeld = (timeSinceThrow < 1f) ? projectileHeld : null;
                    }
                    while(ellipse.Inside(Position.X,Position.Z))
                        Position += playerSpeed * ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
                }
            } else // Crawling
            {
                Move(dt);
                if (Math.Abs(Position.X) > 7.5f || Math.Abs(Position.Z) > 4f)
                {
                    Position = Position + new Vector3(0, 0f, 0);
                    mob = true;
                    playerSpeed = 2f;
                }
            }
            notImportant = notImportant || (input.Action() && input.Dash());
            actionPushedDuration = (input.Action()) ? actionPushedDuration + dt : 0f;
        }
    }
}