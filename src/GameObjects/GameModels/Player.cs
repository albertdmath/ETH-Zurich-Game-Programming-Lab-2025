using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace src.GameObjects;

/** Anything regarding player characters, from movement to the actual Model files goes here. **/
public class Player : GameModel
{
    // Public fields:
    public int Id { get; private set; }
    public int Life { get; private set; } = 5;
    public Hand Hand { get; private set; }

    // Private fields:
    // Consts
    private const float NORMAL_SPEED = 3f;
    private const float STAMINA_REGEN = 3f;

    // Variables
    private enum PlayerState
    {
        Idle,
        NormalMovement,
        Catching,
        HoldingProjectile,
        Dashing,
        Aiming,
        Stunned,
        Crawling,
        JumpingWithTheMightyHammerOfTheThousandThunders,
        DroppingThenNormalMovement,
        FloatingWithChicken
    }
    private PlayerState playerState;
    private float speed = NORMAL_SPEED;
    private float stamina = STAMINA_REGEN;
    private Projectile projectileHeld = null;
    private bool armor = false;
    private float dashTime = 0f,dashSpeed = 12f, flySpeed = 0f;
    private float actionPushedDuration;
    private float stunDuration = 0f;
    private float immunity = 0f;
    private float lastProjectileImmunity = 0f;
    private float timeSinceStartOfCatch = 0f;
    private float friction = 9f;
    private Projectile lastThrownProjectile = null; // Store last thrown projectile
    private  List<DrawModel> playerModels;
    private readonly DrawModel playerModelShell;
    private const float CATCH_COOLDOWN = 1.0f;
    private readonly Input input;
    private readonly Ellipse ellipse;
    private Vector3 inertia,inertiaUp;
    private Vector3 gravity = new(0,-30f,0);
    private bool outside = false;

    private readonly JesterHat jesterHat;
    private readonly AimIndicator aimIndicator;
    private const float speedOfCharging = 2f;
    private PlayerState playerStateBeforeDashing;

    private readonly GameStateManager gameStateManager;

    public Player(Vector3 position, Input input, int id, Ellipse ellipse, List<DrawModel> models, DrawModel playerModelShell, DrawModel playerHandModel, DrawModel hatModel, DrawModel indicatorModel, DrawModel indicatorArrowModel, float scale) : base(models[id], scale)
    {
        Position = position;
        Orientation = new Vector3(0, 0, 1f);
        this.input = input;
        this.ellipse = ellipse;
        this.jesterHat = new JesterHat(this, hatModel, scale);
        this.Id = id;
        inertia = new Vector3(0, 0, 0);
        gameStateManager = GameStateManager.GetGameStateManager();
        Hand = new Hand(this, playerHandModel, 0.6f);
        inertiaUp = new Vector3(0, 0, 0);
        aimIndicator = new AimIndicator(this, indicatorModel,indicatorArrowModel, 1f);
        playerState = PlayerState.Idle;
        playerModels = models;
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
        Position += speed * inertia * dt + inertiaUp * dt;
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
        Position += speed * inertia * dt;
    }
    private void InAir(float dt)
    {
        // Inertia to keep some movement from last update;
        inertiaUp += gravity*dt;
        // Updating the position of the player
        Position += flySpeed * Orientation * dt + inertiaUp * dt;
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

    public bool IsCrawling()
    {
        return playerState == PlayerState.Crawling;
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
        if (input.Dash() && stamina >= STAMINA_REGEN)
        {
            playerStateBeforeDashing = playerState;
            playerState = PlayerState.Dashing;
            dashTime = 0.1f;
            stamina = 0f;
            dashSpeed = 12f;
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
        
        float speedUp = 1 + 2 * actionPushedDuration * actionPushedDuration;
        //Console.WriteLine("Throwing projectile with orientation: " + Orientation + "and origin:" + this.Position + "and target: " + aimIndicator.Target + " and speedup: " + speedUp);
        //9 is the current maximum of the speedUp
        if (projectileHeld.Action(speedUp/9, aimIndicator.Target, outside))
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
                this.DrawModel = playerModels[Id];
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
                speed = 1f;
                gameStateManager.livingPlayers.Remove(this);
                playerState = PlayerState.Crawling;
            }
        }
    }
    public bool GainLife()
    {
        if (Life >= 5) return false;
        
        Life++;
        return true;
    }

    public bool GetHit(Projectile projectile)
    {
        if (!GetAffected(projectile))
            return false;

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
    public bool SetArmor()
    {
        if(armor) return false;

        this.armor = true;
        this.DrawModel = playerModelShell;
        return true;
    }
    public void StartDashingWithProjectileInHand(float speed)
    {
        dashSpeed = speed;
        dashTime = actionPushedDuration * speedOfCharging / speed;
        Console.WriteLine("Dashing with projectile for: " + dashTime + " and speed: " + speed);
        playerState = PlayerState.Dashing;
        playerStateBeforeDashing = PlayerState.DroppingThenNormalMovement;

    }
    public void JumpAndStrike()
    {
        flySpeed = actionPushedDuration * speedOfCharging;
        inertiaUp += new Vector3(0f,15f,0f);
        playerState = PlayerState.JumpingWithTheMightyHammerOfTheThousandThunders;
        
    }
    public void FlyWithChicken()
    {
        playerState = PlayerState.FloatingWithChicken;
        speed = 1.5f;
    }

    public void CatchDrunkMan(DrunkMan drunkMan)
    {
        Orientation = Vector3.Normalize(new Vector3(drunkMan.Position.X, 0f, drunkMan.Position.Z) - new Vector3(Position.X, 0f, Position.Z));
        Drop();
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

    public void ObjectCollision(GameModel obj)
    {
        Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X - obj.Position.X, 0f, Position.Z - obj.Position.Z));
        while (Hitbox.Intersects(obj.Hitbox))
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
                (projectile.Holder as Player).playerState = PlayerState.Idle;
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
        //this cannot overflow
        stamina += dt;
        input.EndVibrate(dt);
        if(input.Direction().Length() > 0){
            this.playerState = PlayerState.NormalMovement;
        } else {
            this.playerState = PlayerState.Idle;
        }

        switch (playerState)
        {
            case PlayerState.Idle: 
                this.SwitchAnimation(1, true, 0.2f);
                CanCatch();
                CanDash();
                break;
            case PlayerState.NormalMovement:
                Move(dt);
                this.SwitchAnimation(2, true, 0.05f);
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
                    actionPushedDuration = actionPushedDuration >= 2f ? 2f : actionPushedDuration;
                    aimIndicator.PlaceIndicator(actionPushedDuration,speedOfCharging,projectileHeld.IndicatorModel);
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
                    speed = NORMAL_SPEED;
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
            case PlayerState.FloatingWithChicken:
                Move(dt);
                Chicken chicken = projectileHeld as Chicken;
                Position = new(Position.X, chicken.YCoordinate, Position.Z);
                if(ellipse.Outside(Position.X, Position.Z))
                {
                    speed = NORMAL_SPEED;
                    Drop();
                }
                else if(chicken.YCoordinate <= 0)
                {
                    Position = new Vector3(Position.X, 0, Position.Z);
                    speed = NORMAL_SPEED;
                    Drop();
                } 
                break;
        }

        actionPushedDuration = input.Action() ? actionPushedDuration + dt : 0f;
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
        {
            base.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            Hand.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            jesterHat.Draw(view, projection, shader, graphicsDevice, shadowDraw);
        }
        if(playerState == PlayerState.Aiming)
            aimIndicator.Draw(view, projection, shader, graphicsDevice, shadowDraw);
    }
}
