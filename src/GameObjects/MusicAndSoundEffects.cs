using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using src.GameObjects;

public class MusicAndSoundEffects
{
    // Sound effects:
    private static SoundEffect bananaSFX;
    private static SoundEffect coconutSFX;
    private static SoundEffect frogSFX;
    private static SoundEffect mjoelnirSFX;
    private static SoundEffect spearSFX;
    private static SoundEffect swordfishSFX;
    private static SoundEffect tomatoSFX;
    private static SoundEffect turtleSFX;
    
    // Other Sound effects:
    private static SoundEffect hitSFX;
    private static SoundEffect angrymobSFX;
    private static SoundEffectInstance angrymobInstance;
    // Backing track:
    private static Song bgMusic;
    private static MenuStateManager menuStateManager;

    public static void loadSFX(Microsoft.Xna.Framework.Content.ContentManager Content) {
        // Projectile sound effects:
        bananaSFX = Content.Load<SoundEffect>("Audio/bananaSFX");
        coconutSFX = Content.Load<SoundEffect>("Audio/coconutSFX");
        frogSFX = Content.Load<SoundEffect>("Audio/frogSFX");
        mjoelnirSFX = Content.Load<SoundEffect>("Audio/mjoelnirSFX");
        spearSFX = Content.Load<SoundEffect>("Audio/spearSFX");
        swordfishSFX = Content.Load<SoundEffect>("Audio/swordfishSFX");
        tomatoSFX = Content.Load<SoundEffect>("Audio/tomatoSFX");
        turtleSFX = Content.Load<SoundEffect>("Audio/turtleSFX");

        // Other sound effects:
        angrymobSFX = Content.Load<SoundEffect>("Audio/angrymobSFX");
        angrymobInstance = angrymobSFX.CreateInstance();
        angrymobInstance.IsLooped = true;
        angrymobInstance.Volume = 0.1f;
        hitSFX = Content.Load<SoundEffect>("Audio/hitSFX");
        
        // Loading the background music:
        bgMusic = Content.Load<Song>("Audio/EpicMedievalVibes");

        menuStateManager = MenuStateManager.GetMenuStateManager();

        if(menuStateManager.SOUND_ENABLED) {
            MediaPlayer.Volume = 0.45f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgMusic);
            angrymobInstance.Play();
        }
    }

    public static void playHitSFX() {
        if(menuStateManager.SOUND_ENABLED)
            hitSFX.Play(0.1f, 0.0f, 0.0f);
    }

    public static void playProjectileSFX(ProjectileType type) {
        // For anyone reading, "Play" function takes 3 parameters: volume, pitch, pan.
        if(menuStateManager.SOUND_ENABLED)
        {
            switch(type)
            {
                case ProjectileType.Banana:
                    bananaSFX.Play(0.1f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Coconut:
                    coconutSFX.Play(0.5f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Frog:
                    frogSFX.Play(0.2f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Mjoelnir:
                    mjoelnirSFX.Play(0.4f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Spear:
                    spearSFX.Play(0.2f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Swordfish:
                    swordfishSFX.Play(0.5f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Tomato:
                    tomatoSFX.Play(0.6f, 0.0f, 0.0f);
                    break;
                case ProjectileType.Turtle:
                    turtleSFX.Play(0.15f, 0.0f, 0.0f);
                    break;
                default:
                    break;
            }
        }
    }
}
