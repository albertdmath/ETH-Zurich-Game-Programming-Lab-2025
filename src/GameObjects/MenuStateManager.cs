using System;

namespace src.GameObjects
{
    /* This class manages menu state. It looks weird cause it uses Singleton pattern. If you refactor I will cut of your peanorz.
     * It holds everything that can be changed from the menu. E.g. options.
     */
    public class MenuStateManager
    {
        public bool ONWIN {get;set;}=false;
        public bool TUTORIAL_IS_OPEN {get;set;} = false;
        public bool MAIN_MENU_IS_OPEN {get;set;} = true;
        public bool SOUND_ENABLED { get ; set; } = true;
        public int NUM_PLAYERS { get ; set; } = 2;
        public int MAX_NUM_PLAYER {get;} = 4;
        public int MIN_NUM_PLAYER {get;} = 1;
        //GRAPHX SETTINGS
        public bool SHADOWS_ENABLED { get ; set; } = true;
        public bool AMBIENT_OCCLUSION_ENABLED{get;set;}=true;
        public bool FXAA_ENABLED {get;set;} = true;
        public bool FULLSCREEN {get;set;} = false;

        public float HUD_SCALE { get; set; } = 0.15f;

        public bool ENABLE_DEBUG_OUTPUT = true;

        private MenuStateManager(){}

        private static readonly MenuStateManager instance = new();

        public static MenuStateManager GetMenuStateManager()
        {
            return instance;
        }

        
    }
}