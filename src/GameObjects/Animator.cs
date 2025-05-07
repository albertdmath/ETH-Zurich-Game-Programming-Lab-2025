using System.Collections.Generic;
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

    float deltaTime;

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
        this.blendFactor = 1.0f;
        this.blendSpeed = 0.0f;
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
        this._loop = _nextLoop;
    }

    public void UpdateAnimation(float dt)
    {
        deltaTime = dt;
        if (animation != null)
        {
            currentTime += animation.GetTicksPerSecond() * dt;
            if (this.nextAnimation != null)
            {
                this.currentTimeNextAnim += nextAnimation.GetTicksPerSecond() * dt;

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
                this.currentTimeNextAnim = MathHelper.Min((float)this.currentTimeNextAnim, ((float)nextAnimation.GetDuration()) - 0.01f);
                if (currentTime == animation.GetDuration() - 0.01f)
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

    public void SwitchAnimation(GameAnimation animation, bool loop, float blendSpeed = 0.0001f)
    {
        if(this.nextAnimation == null && animation.name != this.animation.name){
        this.nextAnimation = animation;
        this.totalTimeNextAnim = this.nextAnimation.GetDuration();
        this.currentTimeNextAnim = 0.0f;
        blendFactor = 1.0f;
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