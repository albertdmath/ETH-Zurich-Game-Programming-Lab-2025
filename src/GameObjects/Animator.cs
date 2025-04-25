using System.Collections.Generic;
using Accord.Math.Distances;
using Microsoft.Xna.Framework;

class Animator {

    private double totalTime; 
    private double currentTime;

    bool _loop;
    bool _ended; 

    float deltaTime;

    private Animation animation;
    public List<Matrix> finalBoneMatrices;
    
    public Animator(Animation animation, bool loop){
        currentTime = 0.0f;
        _loop = loop; 
        this.finalBoneMatrices = new List<Matrix>();
        this.animation = animation;
        this.totalTime = animation.GetDuration();

    }   

    bool checkEnded() {
        return _ended;
    }

    public void UpdateAnimation(float dt){
        deltaTime = dt;
        if(animation != null){
            currentTime += animation.GetTicksPerSecond() * dt;
            if(_loop){
                this.currentTime = currentTime % animation.GetDuration();
            } else {
                currentTime = MathHelper.Min((float)currentTime,((float)animation.GetDuration()) - 0.01f); 
                if(currentTime == animation.GetDuration() - 0.01f){
                    _ended = true; 
                }
            }
            CalculateBoneTransform(animation.GetRootNode(), Matrix.Identity);
        }
    }


    public void CalculateBoneTransform(AssimpNodeData node, Matrix parentTransform){
        string nodeName = node.name; 
        Matrix nodeTransform = node.transformation;
        Bone bone = animation.FindBone(nodeName);
        if(bone != null){
            bone.Update((float)currentTime);
            nodeTransform = bone.GetLocalTransform();

        }

        Matrix globalTransform = nodeTransform * parentTransform;

        var boneInfoMap = animation.GetBoneIDMap();

        if(boneInfoMap.ContainsKey(nodeName)){
            int index = boneInfoMap[nodeName].id;
            Matrix offset = boneInfoMap[nodeName].offset;
            finalBoneMatrices[index] = offset * globalTransform;
        }

        for(int i = 0; i < node.childrenCount; i++){
            CalculateBoneTransform(node.children[i], globalTransform);
        }
    }

    public List<Matrix> GetFinalBoneMatrices(){
        return finalBoneMatrices;
    }   

    public void PlayAnimation(Animation animation, bool loop){
        this.animation = animation;
        currentTime = 0.0f;
        _ended = false; 
        finalBoneMatrices.Clear();
        _loop = loop;
    }



}