using System.Collections.Generic;
using System.Diagnostics;
using Accord.Math.Distances;
using Microsoft.Xna.Framework;

public class Animator
{

    private double totalTime;
    private double currentTime;

    private double currentTimeNextAnim;
    private double totalTimeNextAnim;

    bool _loop;

    bool _nextLoop;
    bool _ended;

    bool _nextEnded;

    bool _stopped;

    bool _breakPoint = false;
    float breakTime;
    float deltaTime;


    bool _nextStopped;

    float nextBreakTime;



    float speed; 
    float nextSpeed;

    private GameAnimation animation;

    private GameAnimation nextAnimation;

    private float blendFactor;
    private float blendSpeed;


    public Matrix[] finalBoneMatrices;

    public Animator(GameAnimation animation, bool loop)
    {
        currentTime = 0.0f;
        _loop = loop;
        this.animation = animation;
        this.finalBoneMatrices = new Matrix[100];
        this.totalTime = animation.GetDuration();
        this.nextAnimation = null;
        this.totalTimeNextAnim = 0.0f;
        this.currentTimeNextAnim = 0.0f;
        this._nextLoop = false;
        this._breakPoint = false;
        this.breakTime = 0.0f;
        this.blendFactor = 1.0f;
        this._stopped = false;
        this.blendSpeed = 0.0f;

        this.speed = 1.0f;
        this.nextSpeed = 1.0f;

        this._nextStopped = false;
        this.nextBreakTime = 0.0f;
        for (int i = 0; i < 100; i++)
        {
            finalBoneMatrices[i] = Matrix.Identity;
        }

    }

    public bool checkEnded()
    {
        return _ended;
    }

    private void finalizeAnimationSwitch()
    {
        this.animation = nextAnimation;
        this.nextAnimation = null;
        this.currentTime = this.currentTimeNextAnim;
        this.totalTime = this.totalTimeNextAnim;
        this.totalTimeNextAnim = 0.0f;
        this.currentTimeNextAnim = 0.0f;
        this.blendFactor = 1.0f; 
        this.blendSpeed = 0.0f;
        _stopped = _nextStopped;
        this.breakTime = nextBreakTime;
        nextBreakTime = 0.0f;
        _nextStopped = false;
        _ended = _nextEnded;
        this._loop = _nextLoop;
        this.speed = nextSpeed;
        this.nextSpeed = 1.0f;
        _nextLoop = false;
        _nextEnded = false;


    }

    public void UpdateAnimation(float dt)
    {
        deltaTime = dt * speed;
        if (animation != null)
        {
            if(!_stopped || !_ended){
                currentTime += animation.GetTicksPerSecond() * deltaTime;

                if(breakTime > 0.0f){
                    var s = 0;
                }
                if(breakTime > 0.0f && currentTime > animation.GetDuration() * breakTime){
                    _stopped = true;    
                }

            if (this.nextAnimation != null)
            {
                this.currentTimeNextAnim += nextAnimation.GetTicksPerSecond() * deltaTime;

                if (_nextLoop)
                {
                    this.currentTimeNextAnim = this.currentTimeNextAnim % nextAnimation.GetDuration();
                }
            }
            if (_loop)
            {
                this.currentTime = currentTime % animation.GetDuration();

            }
            else
            {
                currentTime = MathHelper.Min((float)currentTime, ((float)animation.GetDuration()) - 0.01f);
                if(this.nextAnimation != null && !_nextLoop){
                this.currentTimeNextAnim = MathHelper.Min((float)this.currentTimeNextAnim, ((float)nextAnimation.GetDuration()) - 0.01f);
                    if(this.currentTimeNextAnim == nextAnimation.GetDuration() - 0.01f){
                        _nextEnded = true;
                    }
                }
                if (currentTime == animation.GetDuration() - 0.01f )
                {
                    if (this.nextAnimation != null)
                    {
                        finalizeAnimationSwitch();
                    }
                    else
                    {
                        _ended = true;
                    }


                }
            }
            CalculateBoneTransform(animation.GetRootNode(), Matrix.Identity);
            if(this.nextAnimation != null){
                this.blendFactor -= blendSpeed;
            }

            if (this.blendFactor <= 0)
            {
                finalizeAnimationSwitch();
            }
            }
         
        }
    }


    public void cancelBreak(){
        if(_stopped){
            _stopped = false; 
            breakTime = 0.0f;
        }
    }
    private void CalculateBoneTransform(AssimpNodeData node, Matrix parentTransform)
    {
        string nodeName = node.name;
        Matrix nodeTransform = node.transformation;
        Bone bone = animation.FindBone(nodeName);
        Matrix globalTransform = Matrix.Identity;

        if (bone != null)
        {
            AnimationTransform transformOld = bone.Update((float)currentTime);
            nodeTransform = bone.GetLocalTransform();

            if (nextAnimation != null)
            {
                Bone nextBone = nextAnimation.FindBone(nodeName);
                if (nextBone != null)
                {
                    AnimationTransform transformNew = nextBone.Update((float)this.currentTimeNextAnim);
                    Vector3 BlendedScaling = blendFactor * transformOld.scaling + (1.0f - blendFactor) * transformNew.scaling;
                    Quaternion BlendedRoation = Quaternion.Slerp(transformOld.rotation, transformNew.rotation, 1.0f - blendFactor);
                    Vector3 BlendedTranslation = blendFactor * transformOld.translation + (1.0f - blendFactor) * transformNew.translation;
                    Matrix transMatrix = Matrix.CreateTranslation(BlendedTranslation);
                    Matrix rotationMatrix = Matrix.CreateFromQuaternion(BlendedRoation);
                    Matrix scaleMatrix = Matrix.CreateScale(BlendedScaling);
                    nodeTransform = scaleMatrix * rotationMatrix * transMatrix;
                }
            }
        }

        globalTransform = nodeTransform * parentTransform;

        var boneInfoMap = animation.GetBoneIDMap();

        if (boneInfoMap.ContainsKey(nodeName))
        {
            int index = boneInfoMap[nodeName].id;
            Matrix offset = boneInfoMap[nodeName].offset;
            finalBoneMatrices[index] = offset * globalTransform;

        }

        for (int i = 0; i < node.childrenCount; i++)
        {
            CalculateBoneTransform(node.children[i], globalTransform);
        }
    }

    public Matrix[] GetFinalBoneMatrices()
    {
        return finalBoneMatrices;
    }

    public void SwitchAnimation(GameAnimation animation, bool loop,  float blendSpeed = 0.0001f, float breakPoint = 0.0f, float speed = 1.0f)
    {
        if(this.speed != speed){
            this.speed = speed;
        }
        if(this.nextAnimation == null && animation.name != this.animation.name){
        this.nextAnimation = animation;
        this.totalTimeNextAnim = this.nextAnimation.GetDuration();
        this.currentTimeNextAnim = 0.0f;
        this.nextSpeed = speed;
        blendFactor = 1.0f;
        if(breakPoint > 0.0f){
            nextBreakTime = breakPoint;
        }
        _ended = false;
        _nextLoop = loop;
        this.blendSpeed = blendSpeed;
        }
    }

    public void SetAnimation(GameAnimation animation, bool loop)
    {
        this.animation = animation;
        currentTime = 0.0f;
        _ended = false;

        for (int i = 0; i < 100; i++)
        {
            finalBoneMatrices[i] = Matrix.Identity;
        }
        _loop = loop;
    }


}