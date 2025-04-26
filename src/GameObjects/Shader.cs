using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Shader {
   public Effect  effect {get; set;}

   public Shader(Effect effect){
    this.effect = effect; 
   }

   public void setViewMatrix(Matrix view){
     effect.Parameters["View"].SetValue(view);
   }

      public void setProjectionMatrix(Matrix projection){
        effect.Parameters["Projection"].SetValue(projection);
   }

   
      public void setWorldMatrix(Matrix world){
        effect.Parameters["World"].SetValue(world);
   }

         public void setLightSpaceMatrix(Matrix lightSpace){
        effect.Parameters["LightViewProjection"].SetValue(lightSpace);
   }

   public void setLight(Light light){
    effect.Parameters["LightDirection"].SetValue(light.direction);
    effect.Parameters["LightColor"].SetValue(light.color);
   }

   
    public void setTexture(Texture2D tex){
        effect.Parameters["TextureSampler+ModelTexture"].SetValue(tex);
    }

    public void setShadowTexture(Texture2D shadowMap){
      effect.Parameters["ShadowSampler"].SetValue(shadowMap);
    }

    public void setNormalMatrix(Matrix view, Matrix world){
      Matrix normalMatrix = Matrix.Transpose(Matrix.Invert(world * view));
      effect.Parameters["NormalMatrix"].SetValue(normalMatrix);
    }
    
  public void setMetallic(float metallic){
    effect.Parameters["metallic"].SetValue(metallic);
  }

  
  public void setRoughness(float roughness){
    effect.Parameters["roughness"].SetValue(roughness);
  }
    public void setOpacityValue(float opacity){
      effect.Parameters["OpacityVal"].SetValue(opacity);
    }

  public void setOcclusionTexture(Texture2D occlusionMap){
    effect.Parameters["OcclusionSampler+OcclusionTexture"].SetValue(occlusionMap);
  }

  public void setFinalBoneMatrices(Matrix[] matrices){
    effect.Parameters["FinalBoneMatrices"].SetValue(matrices);
  }


}


public class Filter : Shader {
  public Filter(Effect effect) : base(effect){

  }

  public void setAlbedoTexture(Texture2D tex){
    effect.Parameters["AlbedoSampler+AlbedoTexture"].SetValue(tex);
  }

  public void setNormalTexture(Texture2D normalMap){
    effect.Parameters["NormalPosSampler+NormalTexture"].SetValue(normalMap);
  }


   public void setFragPosTexture(Texture2D fragPosTexture){
    effect.Parameters["FragPosSampler+FragPosTexture"].SetValue(fragPosTexture);
  }

  public void setFilterSize(int filterSize){
    effect.Parameters["filterSize"].SetValue(filterSize);
  }

  public void SetRenderTargetResolution(Vector2 resolution){
    effect.Parameters["renderTargetResolution"].SetValue(resolution);
  }

}

public class PhongShading : Shader {

    public PhongShading(Effect effect) : base(effect){

    }




    public void setCameraPosition(Vector3 CameraPos){
        effect.Parameters["CameraPosition"].SetValue(CameraPos);
    }
}


public class HBAOShader : Shader {
  public HBAOShader(Effect effect) : base(effect){

  }

  public void setSampleDirections(Vector3[] sampleDirections){
    effect.Parameters["sampleDirections"].SetValue(sampleDirections);
  }

  public void setStrengthPerRay(float strengthPerRay){
    effect.Parameters["strengthPerRay"].SetValue(strengthPerRay);
  }


  public void sethalfSampleRadius(float halfSampleRadius){
    effect.Parameters["halfSampleRadius"].SetValue(halfSampleRadius);
  }

    public void setFalloff(float falloff){
    effect.Parameters["fallOff"].SetValue(falloff);
  }

  public void setDitherScale(float ditherScale){
    effect.Parameters["ditherScale"].SetValue(ditherScale);
  }
  public void setBias(float bias){
    effect.Parameters["bias"].SetValue(bias);
  }

  public void SetRenderTargetResolution(Vector2 resolution){
    effect.Parameters["renderTargetResolution"].SetValue(resolution);
  }

  public void setupSampleDirections()
  {
    int size = 8;
    var sampleDirs = new Vector2[size];
    for (int i = 0; i < size; i++)
    {
      float angle = MathF.PI * 2 * i / size;
      sampleDirs[i] = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }
    effect.Parameters["sampleDirections"].SetValue(sampleDirs);
  }

    public void setNormalTexture(Texture2D normalMap){
    effect.Parameters["NormalPosSampler+NormalTexture"].SetValue(normalMap);
  }


   public void setFragPosTexture(Texture2D fragPosTexture){
    effect.Parameters["FragPosSampler+FragPosTexture"].SetValue(fragPosTexture);
  }

   public void setDitherTexture(Texture2D DitherTexture){
    effect.Parameters["DitherSampler+DitherTexture"].SetValue(DitherTexture);
  }


}


public class PBR:PhongShading {
  
  public PBR(Effect effect) : base(effect) {

  } 

  public void setViewInverse(Matrix viewInverse){
    effect.Parameters["ViewInverse"].SetValue(viewInverse);
  }
  public void setNormalTexture(Texture2D normalMap){
    effect.Parameters["NormalPosSampler+NormalTexture"].SetValue(normalMap);
  }


   public void setFragPosTexture(Texture2D fragPosTexture){
    effect.Parameters["FragPosSampler+FragPosTexture"].SetValue(fragPosTexture);
  }

  public void setRoughnessTexture(Texture2D roughnessTexture){
    effect.Parameters["RoughnessSampler+RoughnessTexture"].SetValue(roughnessTexture);
  }

  public void setMetallicTexture(Texture2D metallicTexture){
    effect.Parameters["MetallicSampler+MetallicTexture"].SetValue(metallicTexture);
  }
  
}