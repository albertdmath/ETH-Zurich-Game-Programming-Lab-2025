using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

//using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using src.GameObjects;

public class MusicAndSoundEffects
{
    // Sound effects:
    private static SoundEffect hitSFX;
    private static SoundEffect swordfishSFX;
    private static SoundEffect frogSFX;
    private static SoundEffect tomatoSFX;
    private static SoundEffect angrymobSFX;
    // Backing track:
    private static Song bgMusic;
    private static SoundEffectInstance angrymobInstance;

    public static void loadSFX(Microsoft.Xna.Framework.Content.ContentManager Content, bool SOUND_ENABLED) {
        bgMusic = Content.Load<Song>("Audio/myMedievalBeat");
        MediaPlayer.Volume = 0.1f;
        MediaPlayer.IsRepeating = true;
        hitSFX = Content.Load<SoundEffect>("Audio/hitSFX");
        frogSFX = Content.Load<SoundEffect>("Audio/frogSFX");
        swordfishSFX = Content.Load<SoundEffect>("Audio/swordfishSFX");
        tomatoSFX = Content.Load<SoundEffect>("Audio/tomatoSFX");
        angrymobSFX = Content.Load<SoundEffect>("Audio/angrymobSFX");
        angrymobInstance = angrymobSFX.CreateInstance();
        angrymobInstance.IsLooped = true;
        angrymobInstance.Volume = 0.1f;

        if(SOUND_ENABLED) {
            MediaPlayer.Volume = 1.2f;
            MediaPlayer.Play(bgMusic);
            angrymobInstance.Play();
        }
    }

    public static void playHitSFX() {
        hitSFX.Play(0.3f, 0.0f, 0.0f);
    }

    public static void playProjectileSFX(ProjectileType type) {
        // For anyone reading, "Play" function takes 3 parameters: volume, pitch, pan.
        switch(type)
        {
            case ProjectileType.Frog:
                frogSFX.Play(0.5f, 0.0f, 0.0f);
                break;
            case ProjectileType.Swordfish:
                MusicAndSoundEffects.swordfishSFX.Play(0.8f, 0.0f, 0.0f);
                break;
            case ProjectileType.Tomato:
                MusicAndSoundEffects.tomatoSFX.Play(0.9f, 0.0f, 0.0f);
                break;
            default:
                break;
        }
    }
}
