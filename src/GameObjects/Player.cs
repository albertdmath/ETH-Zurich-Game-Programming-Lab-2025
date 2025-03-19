using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;

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


        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Player(Vector3 position, Input input, int id)
        {
            Position = position;
            this.input = input;
            this.ellipse = ellipse;
            projectileHeld = null;
            this.Id = id;
        }

        public static void Initialize()
        {
            float[] playerStartPositions = { -0.75f, -0.25f, 0.25f, 0.75f };
            //this should be removed
            active.Add(new Player(new Vector3(playerStartPositions[0], 0, 0), new Input(), 0));
            for (int i = 0; i < 4; i++)
            {
                PlayerIndex idx = (PlayerIndex)i;
                if (GamePad.GetState(idx).IsConnected)
                    active.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), new InputController(idx), i + 1));
            }
        }

        // The player move method:
        public void Move(float dt)
        {
            Vector3 dir = input.Direction();
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Orientation = dir;
            }
            Position += playerSpeed * dir * dt;
        }
        public void MoveBack(float dt)
        {
            Position -= playerSpeed * Orientation * dt * 0.2f;
        }


        // Method to grab an object:
        public bool GrabOrHit(Projectile projectile)
        {
            //if(Life<=0f) return false;
            if (projectile != projectileHeld)
            {
                if (input.Action()
                    && projectileHeld == null
                    && Vector3.Distance(Position, projectile.Position) < 1.0f
                    && timeSinceThrow > 1f
                    && actionPushedDuration < 0.2f)
                {
                    projectileHeld = projectile;
                    projectile.Caught(this);
                    Console.WriteLine("Grabbing " + projectile.Type);
                    playerSpeed = 0.3f;
                    return false;
                }
                else if (Vector3.Distance(Position, projectile.Position) < 0.5)
                {
                    Life -= notImportant ? 0 : 1;
                    if (Life == 0f)
                    {
                        Position = Position - new Vector3(0, 0.2f, 0);
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
            if (input.Action() && actionPushedDuration == 0f)
            {
                playerSpeed = 0f;
            }
            else if (!input.Action() && actionPushedDuration > 0 && playerSpeed == 0f)
            {
                projectileHeld.Throw();
                projectileHeld = null;
                timeSinceThrow = 0f;
                playerSpeed = 2f;
                Console.WriteLine("Throwing projectile with orientation: " + Orientation);
            }
        }

        // Method to dash:
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
        private bool Spawn()
        {
            if (input.Dash() && dashTime <= 0f && Stamina > 40f && projectileHeld == null)
            {
                projectileHeld = Projectile.createProjectile(ProjectileType.Swordfish, Position, Orientation);
                projectileHeld.Caught(this);
                playerSpeed = 0.3f;
                Stamina -= 40f;
                return false;
            }
            return true;
        }


        public void update(float dt)
        {
            Stamina += dt * 5f;
            Stamina = (Stamina > 100f) ? 100f : Stamina;
            if (Life > 0f)
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
                        timeSinceThrow += dt;
                    }
                }
            }
            else if (mob)
            {
                if (Spawn())
                {
                    Move(dt);
                    if (projectileHeld != null)
                    {
                        Throw(dt);
                    }
                    else
                    {
                        timeSinceThrow += dt;
                    }
                    if (Math.Abs(Position.X) < 7.5f && Math.Abs(Position.Z) < 4f)
                    {
                        Move(-1f * dt);
                    }
                }
            }
            else
            {
                Move(dt);
                if (Math.Abs(Position.X) > 7.5f || Math.Abs(Position.Z) > 4f)
                {
                    Position = Position + new Vector3(0, 0.2f, 0);
                    mob = true;
                    playerSpeed = 2f;
                }
            }
            notImportant = notImportant || (input.Action() && input.Dash());
            actionPushedDuration = (input.Action()) ? actionPushedDuration + dt : 0f;
        }
    }
}