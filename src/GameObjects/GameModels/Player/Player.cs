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
        MjoelnirJump,
        UsingSpear,
        FloatingWithChicken
    }
    private PlayerState playerState;
    private float speed = NORMAL_SPEED;
    public Projectile projectileHeld {get; private set;} = null;
    private bool armor = false;

    private bool crawling = false;
    
    private bool currentlyCatching = false;
    private float dashTime = 0f,dashSpeed = 12f, flySpeed = 0f;
    private float actionPushedDuration;
    private float stunDuration = 0f;
    private float immunity = 0f;
    private float lastProjectileImmunity = 0f;
    private float timeSinceStartOfCatch = 0f;
    private float friction = 9f;

    private PlayerState playerWalkingOrIdleBefore;
    private Projectile lastThrownProjectile = null; // Store last thrown projectile
    private  DrawModel playerModel;
    private readonly DrawModel playerModelShell;
    private const float CATCH_COOLDOWN = 1.5f;
    private readonly Input input;
    private Vector3 inertia,inertiaUp;
    private Vector3 gravity = new(0,-30f,0);
    private bool outside = false;
    private float timerSpear = 0;
    private Vector3 positionSpear;

    private readonly JesterHat jesterHat;
    private readonly AimIndicator aimIndicator;
    private const float speedOfCharging = 2f;
    private PlayerState playerStateBeforeDashing;
    private readonly Stamina Stamina;

    private readonly GameStateManager gameStateManager;

    public Player(Vector3 position, Input input, int id, DrawModel model, DrawModel playerModelShell, DrawModel playerHandModel, DrawModel indicatorModel, DrawModel indicatorArrowModel, DrawModel staminaModel, float scale, DrawModel jesterHeadModel) : base(model, scale)
    {
        Position = position;
        Orientation = new Vector3(0, 0, 1f);
        this.input = input;
        this.Id = id;
        inertia = new Vector3(0, 0, 0);
        gameStateManager = GameStateManager.GetGameStateManager();
        Hand = new Hand(this, playerHandModel, 0.6f);
        inertiaUp = new Vector3(0, 0, 0);
        aimIndicator = new AimIndicator(this, indicatorModel,indicatorArrowModel, 1f);
        playerState = PlayerState.NormalMovement;
        playerModel = model;
        this.jesterHat = new JesterHat(this,jesterHeadModel,0.5f);
        this.playerModelShell = playerModelShell;
        this.Stamina = new Stamina(this, staminaModel);
    }

    // ---------------------
    //  Beginning functions for player movement
    // ---------------------
    private void Move(float dt)
    {
        // 1. Get and validate input
        Vector3 displacement = input.Direction();

        if (displacement.LengthSquared() > 0)
        {
            displacement = Vector3.Normalize(displacement);
            inertia += (9f * dt) * displacement;
        }

        if(inertia.LengthSquared() > float.Epsilon)
        {
            Orientation = Vector3.Normalize(inertia);
            inertia -= (9f * dt) * inertia;
            if (inertia.LengthSquared() > 1f)
                inertia = Orientation;
        }

        
        // 4. Handle gravity and grounding
        if (Position.Y <= float.Epsilon) // Small epsilon for floating point precision
        {
            Position = new Vector3(Position.X, 0, Position.Z);
            inertiaUp = Vector3.Zero;
        }
        else
        {
            inertiaUp += gravity * dt;
        }
        
        Position += (speed * inertia + inertiaUp) * dt;

    if(!crawling && !(playerState == PlayerState.FloatingWithChicken)){
        if (inertia.LengthSquared() < 0.1)
        {
            this.playerWalkingOrIdleBefore = PlayerState.Idle;
            if(!this.Hand.IsCatching){
                        
                playIdleAnimations(0.1f,1.0f);
            } else {
                playIdleAnimations(0.07f,1.0f);
            }


        }
        else
        {
            this.playerWalkingOrIdleBefore = PlayerState.NormalMovement;

                if(!this.Hand.IsCatching){
                        
                           playWalkingAnimations(0.1f,1.0f);
            } else {
                playWalkingAnimations(0.07f,1.0f);
            }


        }
    }
    }

        private void Slide(float dt)
    {
        // Apply friction to horizontal movement
        inertia -= (friction * dt) * inertia;

        // If airborne, apply gravity
        if (Position.Y > 0)
        {
            inertiaUp += gravity * dt;
            // Update position with both horizontal slide and vertical gravity
            Position += speed * inertia * dt + inertiaUp * dt;
            
            // Ground collision check
            if (Position.Y <= 0)
            {
                Position = new Vector3(Position.X, 0, Position.Z);
                inertiaUp = Vector3.Zero;
            }
        }
        else
        {
            // Standard ground sliding
            Position += speed * inertia * dt;
        }
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
            this.SwitchAnimation(5, false, 0.05f);
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
    // Start of private functions to change state of playeraaaw
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
        if (input.Dash() && Stamina.Dash())
        {

            playerStateBeforeDashing = playerState;
            playerState = PlayerState.Dashing;
            dashTime = 0.1f;
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
        if (gameStateManager.livingPlayers.Count != 1 && immunity <= 0 && Life > 0)
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
                speed = 1f;
                        if(!crawling){
            crawling = true;
            this.flipModel();
        }
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

        playDamageAnimations(0.7f,0.5f);
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
    public void UseSpear(float time)
    {
        timerSpear = time;
        Vector3 orthogonalHolderOrientation = new(-Orientation.Z, Orientation.Y, Orientation.X);
        positionSpear = Orientation * 0.2f + orthogonalHolderOrientation * 0.2f + new Vector3(0, 0.2f, 0);
        playerState = PlayerState.UsingSpear;
    }
    public void JumpAndStrike()
    {
        flySpeed = actionPushedDuration * speedOfCharging;
        inertiaUp += new Vector3(0f,15f,0f);
        playerState = PlayerState.MjoelnirJump;
        
    }
    public void FlyWithChicken()
    {
        playerState = PlayerState.FloatingWithChicken;
        speed = 1.5f;
    }

    // ---------------------
    // End of public functions to change state of player. Meant to be called by projectile, after a collision
    // Start of public functions to change state of player. Meant to be called by GameStateManager. Handles mainly collisions
    // ---------------------
    public void OnMobHit(Ellipse ellipse)
    {
        bool isOutside = ellipse.Outside(Position.X, Position.Z);

        // from outside to inside
        if(outside && !isOutside)
        {
            while (ellipse.Inside(Position.X, Position.Z))
                    Position += ellipse.Normal(Position.X, Position.Z) * -0.1f;
        }
        // from inside to outside
        else if(isOutside && !outside)
        {
            if(playerState == PlayerState.Crawling)
            {
                crawling = false;
                this.flipModel();
                speed = NORMAL_SPEED;
                outside = true;
                playerState = PlayerState.NormalMovement;
            }
            else
            { 
                if(Position.Y > 0)
                    speed = NORMAL_SPEED;
                
                inertia = 3f * ellipse.Normal(Position.X, Position.Z);
                StunAndSlip(1f, 9f);
            }
        } 
    }

    private void playWalkingAnimations(float blendfactor,float speed){
        SwitchAnimation(9, true, blendfactor,0.0f,speed);
        this.jesterHat.SwitchAnimation(2, true, blendfactor,0.0f,speed);
    }

    private void playIdleAnimations(float blendfactor,float speed){
        SwitchAnimation(7, true, blendfactor,0.0f,speed);
        this.jesterHat.SwitchAnimation(1, true, blendfactor,0.0f,speed);
    }


    private void playDamageAnimations(float blendfactor,float speed){
        SwitchAnimation(4, true, blendfactor,0.0f,speed);
        this.jesterHat.SwitchAnimation(0, true, blendfactor,0.0f,speed);
    }



    public void UpdateJesterHatAnimation(float dt){
        if(jesterHat != null){
            jesterHat.UpdateAnimation(dt);
        }
    }

    public void OnObjectHit(GameModel obj)
    {
        Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X - obj.Position.X, 0f, Position.Z - obj.Position.Z));
        Position += dir;
        updateHitbox();
    }

    // Method to test for a collision with a projectile and potentially grab it:
    public void Catch(Projectile projectile)
    {
        if (lastProjectileImmunity > 0 && projectile == lastThrownProjectile)
            return;
        
        GameModel prevHolder = projectile.Holder;
        if(!projectile.Catch(this))
            return;
            
        Hand.StopCatching();

        projectileHeld = projectile;
        if(prevHolder != null) 
        {
            if(prevHolder is Player)
            {
                (prevHolder as Player).projectileHeld = null;
                (prevHolder as Player).playerState = PlayerState.NormalMovement;
            }
            else if(prevHolder is Zombie)
            {
                (prevHolder as Zombie).projectileHeld = null;
            }
        }

        MusicAndSoundEffects.playProjectileSFX(projectile.Type);
        playerState = PlayerState.HoldingProjectile;
    }
    // Method to tdrop the current projectile
    public void Drop()
    {
        if(projectileHeld != null)
        {
            projectileHeld.ToBeDeleted = true;
            projectileHeld = null;
        }
        
        playerState = PlayerState.NormalMovement;
    }

    //ANIMATIONS
    //0: Grabbing
    //1: Grabbing while walking
    //2: Overhead Grab
    //3: Crawling
    //4: Twitching
    //5: ChaCha real smooth
    //6: Dash
    //7: Idle
    //8: Throw
    //9: Walking
    // Update function called each update
    public override void Update(float dt)
    {
        //this cannot overflow
        input.EndVibrate(dt);


        switch (playerState)
        {
            case PlayerState.NormalMovement:
                Move(dt);
                CanCatch();
                CanDash();
                break;
            case PlayerState.Catching:
            if(Hand.IsCatching){
                if(this.playerWalkingOrIdleBefore == PlayerState.NormalMovement){
                    this.SwitchAnimation(1, false, 0.4f);
            } else{
                    this.SwitchAnimation(0, false, 0.4f);
            }
               
            }

                timeSinceStartOfCatch += dt;
                Move(dt);
                if (timeSinceStartOfCatch > CATCH_COOLDOWN){

                    playerState = PlayerState.NormalMovement;
                }

                break;
            case PlayerState.HoldingProjectile:
                Move(dt);
                if (input.Action() && actionPushedDuration == 0f)
                    playerState = PlayerState.Aiming;
                else
                    CanDash();
                break;
            case PlayerState.Dashing:
                playWalkingAnimations(0.2f,4.0f);
                Dash(dt);
                break;
            case PlayerState.Aiming:
                Aim(dt);
                playIdleAnimations(0.2f,1.0f);
                if (input.Action())
                {
                    actionPushedDuration = actionPushedDuration >= 2f ? 2f : actionPushedDuration;
                    aimIndicator.PlaceIndicator(actionPushedDuration,speedOfCharging,projectileHeld.IndicatorModel);
                }
                else
                {
                    if(this.projectileHeld.Type == ProjectileType.Spear){
                        playWalkingAnimations(0.2f,4.0f);
                    } else {
                        SwitchAnimation(8,false,0.2f);
                    }

                    DoActionWithProjectile();
                }
                break;
            case PlayerState.Stunned:
                playDamageAnimations(0.2f,1.0f);
                Slide(dt);
                stunDuration -= dt;
                if (stunDuration < 0f)
                    playerState = (projectileHeld == null) ? PlayerState.NormalMovement : PlayerState.HoldingProjectile;
                break;
            case PlayerState.Crawling:
                this.SwitchAnimation(3, true, 0.5f,0.0f, 2.0f);
                Move(dt);
                break;
            case PlayerState.MjoelnirJump:
                InAir(dt);
                this.SwitchAnimation(6,false, 0.2f,0.5f,2.0f);
                if(Position.Y<=0)
                {
                    inertiaUp = new(0, 0, 0);
                    Position = new(Position.X, 0, Position.Z);
                    (projectileHeld as Mjoelnir).Explode();
                    lastProjectileImmunity = 1f;
                    lastThrownProjectile = projectileHeld;
                    projectileHeld = null;
                    this.animator.cancelBreak();
                    playerState = PlayerState.NormalMovement;
                }
                break;
            case PlayerState.UsingSpear:
                timerSpear -= dt;
                if(timerSpear < 0)
                    Drop();
                else
                    Position = projectileHeld.Position - positionSpear;
                break;
            case PlayerState.FloatingWithChicken:
                Move(dt);
                this.SwitchAnimation(2, false, 0.3f, 0.7f, 2.0f);
                Chicken chicken = projectileHeld as Chicken;
                Position = new(Position.X, chicken.YCoordinate, Position.Z);
                if(chicken.YCoordinate <= 0)
                {
                    this.animator.cancelBreak();
                    Position = new(Position.X, 0, Position.Z);
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
        Stamina.Update(dt);
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
            shader.setFinalBoneMatrices(this.GetFinalBoneMatrices());
            base.Draw(view, projection, shader, graphicsDevice, shadowDraw);

           //Hand.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            shader.setFinalBoneMatrices(jesterHat.GetFinalBoneMatrices());
            if(Life >0){
                jesterHat.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            }

        }

        Stamina.Draw(view, shader, graphicsDevice, shadowDraw);

        if(playerState == PlayerState.Aiming)
            aimIndicator.Draw(view, projection, shader, graphicsDevice, shadowDraw);

    }
}
