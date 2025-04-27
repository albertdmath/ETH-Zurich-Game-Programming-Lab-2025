using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Crawling,
            JumpingWithTheMightyHammerOfTheThousandThunders,
            DroppingThenNormalMovement
        }
        public int Id { get; set; }
        // Private fields:
        private float playerSpeed = 3f;
        public int Life { get; set; } = 5;
        public float Stamina { get; set; } = 3f;
        public Projectile projectileHeld;
        private float dashTime = 0f,dashSpeed = 12f, flySpeed = 0f;
        private float actionPushedDuration;
        private float jumpPushedDuration=0;
        private float stunDuration = 0f;
        public bool notImportant = false;
        private float immunity = 0f;
        private float lastProjectileImmunity = 0f;
        private float timeSinceStartOfCatch = 0f;
        private float friction = 9f;
        public Projectile lastThrownProjectile = null; // Store last thrown projectile
        public bool armor = false; // Store if player has armor
        private DrawModel playerModel;
        private DrawModel playerModelShell;
        private float CATCH_COOLDOWN = 1.0f;
        private Input input;
        private Ellipse ellipse;
        private Vector3 inertia,inertiaUp;
        private Vector3 gravity = new Vector3(0,-30f,0);
        private bool outside = false;
        public Hand Hand { get; private set; }

        public JesterHat jesterHat;
        public AimIndicator aimIndicator { get; private set; }
        public PlayerState playerState, playerStateBeforeDashing;

        private GameStateManager gameStateManager;

        public Player(Vector3 position, Input input, int id, Ellipse ellipse, DrawModel model, DrawModel playerModelShell, DrawModel playerHandModel, DrawModel hatModel, DrawModel indicatorModel, DrawModel indicatorArrowModel, float scale) : base(model, scale)
        {
            Position = position;
            Orientation = new Vector3(0, 0, 1f);
            this.input = input;
            this.ellipse = ellipse;
            projectileHeld = null;
            this.jesterHat = new JesterHat(this, hatModel, scale);
            this.Id = id;
            inertia = new Vector3(0, 0, 0);
            gameStateManager = GameStateManager.GetGameStateManager();
            Hand = new Hand(this, playerHandModel, 0.7f);
            inertiaUp = new Vector3(0, 0, 0);
            aimIndicator = new AimIndicator(this, indicatorModel,indicatorArrowModel, 1f);
            playerState = PlayerState.NormalMovement;
            playerModel = model;
            this.playerModelShell = playerModelShell;
        }

        // ---------------------
        //  Beginning functions for player movement
        // ---------------------
        private void Move(float dt)
        {
            Vector3 dir = input.Direction();
            //inertia to keep some movement from last update;
            inertia -= (9f * dt) * inertia;
            inertiaUp += gravity*dt;
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
            Position += playerSpeed * inertia * dt + inertiaUp * dt;
            if(Position.Y<=0)
            {
                inertiaUp = new Vector3(0, 0, 0);
                Position = new Vector3(Position.X, 0, Position.Z);
            }
            if (outside)
            {
                while (ellipse.Inside(Position.X, Position.Z))
                        Position += ellipse.Normal(Position.X, Position.Z) * dt * -0.1f;
            }
        }

        private void Slide(float dt)
        {
            // Inertia to keep some movement from last update;
            inertia -= (friction * dt) * inertia;

            // Updating the position of the player
            Position += playerSpeed * inertia * dt;
        }
        private void InAir(float dt)
        {
            // Inertia to keep some movement from last update;
            inertiaUp += gravity*dt;
            // Updating the position of the player
            Position += flySpeed *Orientation * dt + inertiaUp * dt;
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
                Position += dashSpeed * Orientation * dt;
                dashTime -= dt;
                //Console.WriteLine("Dashing in dash with projectile for: " + dashTime + " and speed: " + dashSpeed);
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
                dashSpeed = 12f;
            }
        }
        private void CanJump()
        {
            if (input.Jump() && jumpPushedDuration == 0f)
            {
                inertiaUp += new Vector3(0f,15f,0f);
            }
        }
        // Method to throw an object:
        private void Throw()
        {
            lastThrownProjectile = projectileHeld;
            lastProjectileImmunity = 1f;
            projectileHeld = null;
            playerState = PlayerState.NormalMovement;
        }
        private void DoActionWithProjectile()
        {
            float speedUp = 1 + 2 * (float)Math.Pow(actionPushedDuration, 2f);
            Console.WriteLine("Throwing projectile with orientation: " + Orientation + " and speedup: " + speedUp);
            if(outside)
            {
                projectileHeld.Throw(this.Position, aimIndicator.Position);
                Throw();
            }
            else if (projectileHeld.Action(speedUp, aimIndicator.Position))
                Throw();
        }
        
        // ---------------------
        // End of private functions to change state of player
        // Start of public functions to change state of player. Meant to be called by projectile, after a collision
        // ---------------------
        public void LoseLife(){
            if (gameStateManager.livingPlayers.Count != 1 && immunity <= 0)
            {
                input.Vibrate();
                MusicAndSoundEffects.playHitSFX();
                if(armor)
                {
                    armor = false;
                    this.DrawModel = playerModel;
                }
                else
                    Life--;
                immunity = 1f;
                if (Life == 0f)
                {
                    if (projectileHeld != null)
                    {
                        Drop();
                        projectileHeld = null;
                    }

                    // For now the player is moved down to indacet crawling. Later done with an animation
                    Position = Position - new Vector3(0, 0.2f, 0);
                    playerSpeed = 1f;
                    gameStateManager.livingPlayers.Remove(this);
                    playerState = PlayerState.Crawling;
                }
            }
        }
        public void GainLife()
        {
            Life += Life == 3 ? 0 : 1;
        }
        public bool GetHit(Projectile projectile)
        {
            // Check for the thrown immunity
            if (lastProjectileImmunity > 0 && projectile == lastThrownProjectile || projectile == projectileHeld)
                return false;

            // check for general immunity
            LoseLife();
            return true;
        }

        public bool GetAffected(Projectile projectile)
        {
            // Check for the thrown immunity
            if (lastProjectileImmunity > 0 && projectile == lastThrownProjectile || projectile == projectileHeld)
                return false;

            // check for general immunity
            return true;
        }
        public void StunAndSlip(float stunDuration, float friction) // advised value for normal behaviour is friction = 9f
        {
            Drop();
            this.friction = friction;
            this.stunDuration = stunDuration;
            playerState = PlayerState.Stunned;
        }
        public void SetArmor(bool armor)
        {
            this.armor = armor;
            this.DrawModel = playerModelShell;
        }
        public void StartDashingWithProjectileInHand(float speed)
        {
            dashSpeed = speed;
            dashTime = actionPushedDuration * 1f / speed;
            Console.WriteLine("Dashing with projectile for: " + dashTime + " and speed: " + speed);
            playerState = PlayerState.Dashing;
            playerStateBeforeDashing = PlayerState.DroppingThenNormalMovement;

        }
        public void JumpAndStrike()
        {
            flySpeed = actionPushedDuration * 1f / (15f/30f*2f);
            inertiaUp += new Vector3(0f,15f,0f);
            playerState = PlayerState.JumpingWithTheMightyHammerOfTheThousandThunders;
            
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

        public void MarketCollision(Market market)
        {
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X - market.Position.X, 0f, Position.Z - market.Position.Z));
            while (Hitbox.Intersects(market.Hitbox))
            {
                Position += dir;
                updateHitbox();
            }
        }
        // Method to test for a collision with a projectile and potentially grab it:
        public void Catch(Projectile projectile)
        {
            Hand.StopCatching();
            projectileHeld = projectile;
            if(projectile.Holder != null) 
            {
                if(projectile.Holder is Player)
                {
                    (projectile.Holder as Player).projectileHeld = null;
                    (projectile.Holder as Player).playerState = PlayerState.NormalMovement;
                }
                else if(projectile.Holder is Zombie)
                {
                    (projectile.Holder as Zombie).projectileHeld = null;
                }
            }
            projectile.Catch(this);
            MusicAndSoundEffects.playProjectileSFX(projectile.Type);
            playerState = PlayerState.HoldingProjectile;
            Console.WriteLine("Grabbing " + projectile.Type);
        }
        // Method to tdrop the current projectile
        public void Drop()
        {
            if(projectileHeld != null)
            {
                projectileHeld.ToBeDeleted = true;
                 Console.WriteLine("Dropping " + projectileHeld.Type);
            }
            projectileHeld = null;
            playerState = PlayerState.NormalMovement;
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
                    if (timeSinceStartOfCatch > CATCH_COOLDOWN)
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
                    {
                        actionPushedDuration += actionPushedDuration >= 4f ? 0f : dt;
                        aimIndicator.PlaceIndicator(actionPushedDuration,1f,true);
                    }
                    else
                    {
                        DoActionWithProjectile();
                    }
                    break;
                case PlayerState.Stunned:
                    Slide(dt);
                    stunDuration -= dt;
                    if (stunDuration < 0f)
                        playerState = (projectileHeld == null) ? PlayerState.NormalMovement : PlayerState.HoldingProjectile;
                    break;
                case PlayerState.Crawling:
                    Move(dt);
                    if (ellipse.Outside(Position.X, Position.Z))
                    {
                        Position = Position + new Vector3(0, 0.2f, 0);
                        playerSpeed = 2f;
                        outside = true;
                        playerState = PlayerState.NormalMovement;
                    }
                    break;
                case PlayerState.JumpingWithTheMightyHammerOfTheThousandThunders:
                    InAir(dt);
                    if(Position.Y<=0)
                    {
                        inertiaUp = new Vector3(0, 0, 0);
                        Position = new Vector3(Position.X, 0, Position.Z);
                        gameStateManager.CreateAreaDamage(projectileHeld.Position,3f,this,ProjectileType.Mjoelnir);
                        Drop();
                        playerState = PlayerState.NormalMovement;
                    }
                    break;
                case PlayerState.DroppingThenNormalMovement:
                    Drop();
                    playerState = PlayerState.NormalMovement;
                    goto case PlayerState.NormalMovement;
            }

            actionPushedDuration = input.Action() ? actionPushedDuration + dt : 0f;
            jumpPushedDuration = input.Jump() ? jumpPushedDuration + dt : 0f;
            immunity -= dt;
            lastProjectileImmunity -= dt;
            Hand.updateWrap(dt);
            jesterHat.updateWrap(dt);
        }

        public override void Draw(Matrix view, Matrix projection, Shader shader, GraphicsDevice graphicsDevice, bool shadowDraw)
        {
            // Blink every 0.1 seconds when either stunDuration or immunity are active
            bool shouldDraw = true;
            if (stunDuration > 0)
                shouldDraw = (int)(stunDuration * 10) % 2 == 0;

            if (immunity > 0)
                shouldDraw = (int)(immunity * 10) % 2 == 0;

            if (shouldDraw)
                base.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            Hand.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            jesterHat.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            if(playerState == PlayerState.Aiming)
                aimIndicator.Draw(view, projection, shader, graphicsDevice, shadowDraw);
        }
    }
}