using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Unmanaged;
using Microsoft.Xna.Framework;

struct KeyPosition
{
   public Vector3 position;
    public double timeStamp;
};

struct KeyRotation
{
   public Quaternion orientation;
    public double timeStamp;
};

struct KeyScale
{
    public Vector3 scale;
    public double timeStamp;
};

public struct AssimpNodeData
{
    public Matrix transformation;
    public string name;
    public int childrenCount;
    public List<AssimpNodeData> children;
};

public class Bone {
     
       private List<KeyPosition> positions;
       private List<KeyRotation> rotations;
       private List<KeyScale> scales;

       private int NumPositions;
       private int NumRotations;
       private int NumScales;

       private Matrix localTransform;
       private string name; 
      private  int id;
      


      public Bone(string name, int ID, NodeAnimationChannel channel){
        this. name = name; 
        this.id = ID;   
        this.localTransform = Matrix.Identity;
        this.NumPositions = (int)channel.PositionKeyCount;
        this.NumRotations = (int)channel.RotationKeyCount;
        this.NumScales = (int)channel.ScalingKeyCount;
        this.positions = new List<KeyPosition>();
        this.rotations = new List<KeyRotation>();
        this.scales = new List<KeyScale>();

        for(int index = 0; index < NumPositions; index++){
            VectorKey key = channel.PositionKeys[index];
            double timeStamp = key.Time;
            KeyPosition data; 
            data. position = new Vector3(key.Value.X, key.Value.Y, key.Value.Z);
            data.timeStamp = timeStamp;
            positions.Add(data);
        }

        for(int index = 0; index < NumRotations; index++){
            QuaternionKey key = channel.RotationKeys[index];
            double timeStamp = key.Time;
            KeyRotation data; 
            data.orientation = new Quaternion(key.Value.X, key.Value.Y, key.Value.Z, key.Value.W);  
            data.timeStamp = timeStamp;
            rotations.Add(data);
        }
           
        for(int index = 0; index < NumScales; index++){
           VectorKey key = channel.ScalingKeys[index];
            double timeStamp = key.Time;
            KeyScale data; 
            data.scale = new Vector3(key.Value.X, key.Value.Y, key.Value.Z);
            data.timeStamp = timeStamp;
            scales.Add(data);
        }

      }

      public void Update(float animationTime){
        Matrix translation = InterpolatePosition(animationTime);
        Matrix rotation = InterpolateRotation(animationTime);
        Matrix scaling = InterpolateScaling(animationTime);
        this.localTransform = scaling  * rotation * translation;
      }


      public Matrix GetLocalTransform(){
        return this.localTransform;
      }
      public string GetBoneName(){
        return this.name;
      }

        public int GetBoneID(){
            return this.id;
        }


    public int GetPositionIndex(double animationTime){
        for(int index = 0; index < NumPositions - 1; index++){
            if(animationTime < positions[index + 1].timeStamp){
                return index;
            }
        }
        throw new Exception("Error: No position index found for the given animation time.");
    }

    public int GetRotationIndex(float animationTime)
    {
        for (int index = 0; index < NumRotations - 1; ++index)
        {
            if (animationTime < rotations[index + 1].timeStamp)
                return index;
        }
         throw new Exception("Error: No rotation index found for the given animation time.");
    }


    public int GetScaleIndex(float animationTime)
    {
        for (int index = 0; index < NumScales - 1; ++index)
        {
            if (animationTime < scales[index + 1].timeStamp)
                return index;
        }
         throw new Exception("Error: No scale index found for the given animation time.");
    }


    public float GetScaleFactor(double lastTimeStamp, double nextTimeStamp, double animationTime){
        double scaleFactor = 0.0f; 
        double midWayLength = animationTime - lastTimeStamp;
        double framesDiff = nextTimeStamp - lastTimeStamp;
        scaleFactor = midWayLength / framesDiff;
        return (float)scaleFactor; 
    }


  private  Matrix InterpolatePosition(float animationTime){
        if(NumPositions == 1){
            return Matrix.CreateTranslation(positions[0].position);
        }
        int index = GetPositionIndex(animationTime);
        int nextIndex = index + 1; 
        float scaleFactor = GetScaleFactor(positions[index].timeStamp, positions[nextIndex].timeStamp, animationTime);
        Vector3 finalPosition = Vector3.Lerp(positions[index].position, positions[nextIndex].position, scaleFactor);
        return Matrix.CreateTranslation(finalPosition); 
    }

   private Matrix InterpolateRotation(float animationTime){
        if(NumRotations == 1){
            return Matrix.CreateFromQuaternion(rotations[0].orientation);
        }
        int index = GetRotationIndex(animationTime);
        int nextIndex = index + 1; 
        float scaleFactor = GetScaleFactor(rotations[index].timeStamp, rotations[nextIndex].timeStamp, animationTime);
        Quaternion finalOrientation = Quaternion.Slerp(rotations[index].orientation, rotations[nextIndex].orientation, scaleFactor);
        return Matrix.CreateFromQuaternion(finalOrientation); 
    }

  private  Matrix InterpolateScaling(float animationTime){
        if(NumScales == 1){
            return Matrix.CreateScale(scales[0].scale);
        }
        int index = GetScaleIndex(animationTime);
        int nextIndex = index + 1; 
        float scaleFactor = GetScaleFactor(scales[index].timeStamp, scales[nextIndex].timeStamp, animationTime);
        Vector3 finalScale = Vector3.Lerp(scales[index].scale, scales[nextIndex].scale, scaleFactor);
        return Matrix.CreateScale(finalScale); 
    }
}

public class GameAnimation {
    private List<Bone> bones = new List<Bone>();
    private int numBones;
    private double duration;
    private double ticksPerSecond;
    private string name; 
    private Matrix globalInverseTransform; 
    private AssimpNodeData rootNode; 

    private Dictionary<string, BoneInfo> boneInfoMap;

    public Matrix AssimpToXna(System.Numerics.Matrix4x4 m)            // row-major
{   
    // A1 A2 A3 A4 in Assimp is M11 M12 M13 M14 in XNA, …
    return new Matrix(
        m.M11, m.M21, m.M31, m.M41,
        m.M12, m.M22, m.M32, m.M42,
     m.M13, m.M23, m.M33, m.M43,
       m.M14, m.M24, m.M34, m.M44
    );
}

    public GameAnimation(string name, Animation anim, Scene scene, DrawModel model){
        this.name = name; 
        Animation currAnim = anim;
        this.duration = currAnim.DurationInTicks;
        this.ticksPerSecond = currAnim.TicksPerSecond;
        Node node = scene.RootNode;
        this.rootNode = new AssimpNodeData();
        this.bones = new List<Bone>();
        ReadAnimNodeHierarchy(ref rootNode,node);
        ReadMissingBones(currAnim, model);
        this.globalInverseTransform = Matrix.Invert(node.Transform);
        this.numBones = bones.Count;

    }

    public Bone FindBone(string name){
        for(int index = 0; index < bones.Count; index++){
            if(bones[index].GetBoneName() == name){
                return bones[index];
            }
        }
        return null; 
    }
    
    public double GetTicksPerSecond() { return ticksPerSecond; }

   public double GetDuration() { return duration; }

    private void ReadAnimNodeHierarchy(ref AssimpNodeData destination, Node currNode){
        destination.name = currNode.Name;
        destination.transformation = AssimpToXna(currNode.Transform);
        destination.childrenCount = currNode.ChildCount;
        destination.children = new List<AssimpNodeData>();
        for(int index = 0; index < currNode.ChildCount; index++){
            AssimpNodeData childData = new AssimpNodeData();
            ReadAnimNodeHierarchy(ref childData, currNode.Children[index]);
            destination.children.Add(childData);
        }
    }

    private void ReadMissingBones(Animation anim, DrawModel model){
        int size = (int)anim.NodeAnimationChannelCount;
        var boneInfoMap = model.getBoneInfoMap();

        int boneCount = model.getBoneCount();

        for(int i = 0; i < size; i++){

            string boneName = anim.NodeAnimationChannels[i].NodeName.ToString();
            if(!boneInfoMap.ContainsKey(boneName)){
                BoneInfo boneInfo = new BoneInfo();
                boneInfo.id = boneCount;
                boneInfo.offset =  Matrix.Identity;
                boneInfoMap[boneName] = boneInfo;
                model.incrementBoneCount();
                boneCount = model.getBoneCount();
            }  
            int boneIndex = boneInfoMap[boneName].id;
                Bone bone = new Bone(boneName, boneIndex,  anim.NodeAnimationChannels[i]);
                this.bones.Add(bone);
        }
        this.boneInfoMap = boneInfoMap;

    }

    public AssimpNodeData GetRootNode() { return rootNode; }


    public Dictionary<string,BoneInfo> GetBoneIDMap()
    {
        return boneInfoMap;
    }
}

