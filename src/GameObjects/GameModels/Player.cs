using Microsoft.Xna.Framework;
using System;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : GameModel
    {
        public enum PlayerState
        {
            NormalMovement,
            Catching,
            HoldingProjectile,
            Dashing,
            Aiming,
            Stunned,
            PartOfMob,
            PartOfMobHoldingProjectile,
            Crawling,
            TestDashing
        }
        public int Id { get; set; }
        // Private fields:
        private float playerSpeed = 2f;
        public int Life { get; set; } = 5;
        public float Stamina { get; set; } = 3f;
        public Projectile projectileHeld;
        private float dashTime = 0f;
        private float actionPushedDuration;
        private float stunDuration = 0f;
        public bool notImportant = false;
        private float immunity = 0f;
        private float lastProjectileImmunity = 0f;
        private float timeSinceStartOfCatch = 0f;
        private float friction = 9f;
        private Projectile lastThrownProjectile = null; // Store last thrown projectile

        private Input input;
        private Ellipse ellipse;
        private Vector3 inertia;
        public Hand Hand { get; private set; }
        public PlayerState playerState, playerStateBeforeDashing;

        private GameStateManager gameStateManager;

        public Player(Vector3 position, Input input, int id, Ellipse ellipse, DrawModel model, float scale) : base(model, scale)
        {
            Position = position;
            Orientation = new Vector3(0, 0, 1f);
            this.input = input;
            this.ellipse = ellipse;
            projectileHeld = null;
            this.Id = id;
            inertia = new Vector3(0, 0, 0);
            // Remove hat from hitbox; this is trashcode and needs to be removed / done better at some point
            this.Hitbox.BoundingBoxes.RemoveAt(this.Hitbox.BoundingBoxes.Count - 1);
            gameStateManager = GameStateManager.GetGameStateManager();
            Hand = new Hand(this, model, 0.25f);
            playerState = PlayerState.NormalMovement;
        }

        // ---------------------
        //  Beginning functions for player movement
        // ---------------------
        private void Move(float dt)
        {
            Vector3 dir = input.Direction();
            //inertia to keep some movement from last update;
            inertia -= (9f * dt) * inertia;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                inertia += (9f * dt) * dir;
                // Limit inertia to a vector of length 1
                if (inertia.Length() > 1f)
                    inertia = Vector3.Normalize(inertia);
            }
            // Only update orientation if inertia is not 0
            if (inertia.Length() > 0)
            {
                Orientation = Vector3.Normalize(inertia);
            }
            // Updating the position of the player
            Position += playerSpeed * inertia * dt;
        }

        private void Slide(float dt)
        {
            // Inertia to keep some movement from last update;
            inertia -= (friction * dt) * inertia;

            // Updating the position of the player
            Position += playerSpeed * inertia * dt;
        }

        private void Aim(float dt)
        {
            Vector3 dir = input.Direction();
            //inertia to keep some movement from last update;
            inertia -= (9f * dt) * inertia;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                inertia += (9f * dt) * dir;
                // Limit inertia to a vector of length 1
                if (inertia.Length() > 1f)
                    inertia = Vector3.Normalize(inertia);
            }
            // Only update orientation if inertia is not 0
            if (inertia.Length() > 0)
            {
                Orientation = Vector3.Normalize(inertia);
            }
            // Hand moves behind to indicate charge up...
        }
        // Method to dash
        private void Dash(float dt)
        {
            if (dashTime > 0f)
            {
                Position += 6 * playerSpeed * Orientation * dt;
                dashTime -= dt;
            }
            else
            {
                playerState = playerStateBeforeDashing;
            }
        }
        // ---------------------
        // End of functions for player movement
        // Start of private functions to change state of player
        // ---------------------
        private void CanCatch()
        {
            if (input.Action())
            {
                Hand.IsCatching = true;
                playerState = PlayerState.Catching;
                timeSinceStartOfCatch = 0f;
            }
        }
        private void CanDash()
        {
            if (input.Dash() && Stamina >= 3f)
            {
                playerStateBeforeDashing = playerState;
                playerState = PlayerState.Dashing;
                dashTime = 0.1f;
                Stamina = 0f;
            }
        }
        // Method to throw an object:
        private void Throw()
        {
            float speedUp = 1 + 2 * (float)Math.Pow(Math.Min(actionPushedDuration, 4f), 2f);
            lastThrownProjectile = projectileHeld;
            lastProjectileImmunity = 1f;
            projectileHeld.Throw(speedUp);
            projectileHeld = null;
            Console.WriteLine("Throwing projectile with orientation: " + Orientation + " and speedup: " + speedUp);
        }
        // Spawning a projectile in Hand when part of the mob. Currently only swordfish
        private void Spawn()
        {
            if (input.Action() && projectileHeld == null)
            {
                projectileHeld = gameStateManager.CreateProjectile(ProjectileType.Swordfish, Position, Orientation);
                projectileHeld.Catch(this);
                Stamina -= 3f;
                playerState = PlayerState.PartOfMobHoldingProjectile;
            }

        }
        // ---------------------
        // End of private functions to change state of player
        // Start of public functions to change state of player. Meant to be called by projectile, after a collision
        // ---------------------
        public bool GetHit(Projectile projectile)
        {
            // Check for the thrown immunity
            if (lastProjectileImmunity > 0 && projectile == lastThrownProjectile)
                return false;

            // check for general immunity
            if (gameStateManager.livingPlayers.Count != 1 && immunity <= 0)
            {
                input.Vibrate();
                Life--;
                immunity = 1f;
                if (Life == 0f)
                {
                    if (projectileHeld != null)
                    {
                        projectileHeld.Throw(1f);
                        projectileHeld = null;
                    }

                    // For now the player is moved down to indacet crawling. Later done with an animation
                    Position = Position - new Vector3(0, 0.2f, 0);
                    playerSpeed = 1f;
                    gameStateManager.livingPlayers.Remove(this);
                    playerState = PlayerState.Crawling;
                }
            }

            return true;
        }
        public void StunAndSlip(float stunDuration, float friction) // advised value for normal behaviour is friction = 9f
        {
            this.friction = friction;
            this.stunDuration = stunDuration;
            playerState = PlayerState.Stunned;
        }


        // ---------------------
        // End of public functions to change state of player. Meant to be called by projectile, after a collision
        // Start of public functions to change state of player. Meant to be called by GameStateManager. Handles mainly collisions
        // ---------------------
        public void PlayerCollision(Player player)
        {
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X - player.Position.X, 0f, Position.Z - player.Position.Z));
            while (Hitbox.Intersects(player.Hitbox))
            {
                Position += dir;
                player.Position -= dir;
                updateHitbox();
                player.updateHitbox();
            }
        }
        public void MobCollision(Zombie zombie)
        {
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X - zombie.Position.X, 0f, Position.Z - zombie.Position.Z));
            if (Hitbox.Intersects(zombie.Hitbox))
            {
                while (Hitbox.Intersects(zombie.Hitbox))
                {
                    Position += dir;
                    updateHitbox();
                }
                Orientation = ellipse.Normal(Position.X, Position.Z);
                inertia = 1.6f * Orientation;
                StunAndSlip(1f, 9f);
            }
        }
        // Method to test for a collision with a projectile and potentially grab it:
        public void Catch(Projectile projectile)
        {
            Hand.StopCatching();
            projectileHeld = projectile;
            projectile.Catch(this);
            MusicAndSoundEffects.playProjectileSFX(projectile.Type);
            playerState = PlayerState.HoldingProjectile;
            Console.WriteLine("Grabbing " + projectile.Type);
        }


        // Update function called each update
        public override void Update(float dt)
        {
            Stamina += dt;
            Stamina = (Stamina > 3) ? 3f : Stamina;
            input.EndVibrate(dt);

            switch (playerState)
            {
                case PlayerState.NormalMovement:
                    Move(dt);
                    CanCatch();
                    CanDash();
                    break;
                case PlayerState.Catching:
                    timeSinceStartOfCatch += dt;
                    Move(dt);
                    if (timeSinceStartOfCatch > 2f)
                        playerState = PlayerState.NormalMovement;
                    break;
                case PlayerState.HoldingProjectile:
                    Move(dt);
                    if (input.Action() && actionPushedDuration == 0f)
                        playerState = PlayerState.Aiming;
                    else
                        CanDash();
                    break;
                case PlayerState.Dashing:
                    Dash(dt);
                    break;
                case PlayerState.Aiming:
                    Aim(dt);
                    if (input.Action())
                        actionPushedDuration += dt;
                    else
                    {
                        Throw();
                        playerSpeed = 2f;
                        playerState = Life > 0 ? PlayerState.NormalMovement : PlayerState.PartOfMob;
                    }
                    break;
                case PlayerState.Stunned:
                    Slide(dt);
                    stunDuration -= dt;
                    if (stunDuration < 0f)
                        playerState = (projectileHeld == null) ? PlayerState.NormalMovement : PlayerState.HoldingProjectile;
                    break;
                case PlayerState.PartOfMob:
                    Move(dt);
                    while (ellipse.Inside(Position.X, Position.Z))
                        Position += ellipse.Normal(Position.X, Position.Z) * dt * -0.1f;
                    CanDash();
                    Spawn();
                    break;
                case PlayerState.PartOfMobHoldingProjectile:
                    Move(dt);
                    while (ellipse.Inside(Position.X, Position.Z))
                        Position += ellipse.Normal(Position.X, Position.Z) * dt * -0.1f;
                    if (input.Action() && actionPushedDuration == 0f)
                        playerState = PlayerState.Aiming;
                    else
                        CanDash();
                    break;
                case PlayerState.Crawling:
                    Move(dt);
                    if (ellipse.Outside(Position.X, Position.Z))
                    {
                        Position = Position + new Vector3(0, 0.2f, 0);
                        playerSpeed = 2f;
                        playerState = PlayerState.PartOfMob;
                    }
                    break;
            }
            actionPushedDuration = input.Action() ? actionPushedDuration + dt : 0f;
            immunity -= dt;
            lastProjectileImmunity -= dt;
            Hand.updateWrap(dt);
        }

        public override void Draw(Matrix view, Matrix projection, Shader shader, bool shadowDraw)
        {
            // Blink every 0.1 seconds when either stunDuration or immunity are active
            bool shouldDraw = true;
            if (stunDuration > 0)
                shouldDraw = (int)(stunDuration * 10) % 2 == 0;

            if (immunity > 0)
                shouldDraw = (int)(immunity * 10) % 2 == 0;

            if (shouldDraw)
                base.Draw(view, projection, shader, shadowDraw);
            Hand.Draw(view, projection, shader, shadowDraw);
        }
        public void Slip(float t){
            return;
        }
    }
}