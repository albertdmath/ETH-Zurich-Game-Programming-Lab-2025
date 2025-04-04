namespace src.GameObjects
{
    /* This class manages menu state. It looks weird cause it uses Singleton pattern. If you refactor I will cut of your peanorz.
     * It holds everything that can be changed from the menu. E.g. options.
     */
    public class MenuStateManager
    {
        public bool SOUND_ENABLED { get ; set; } = false ;
        public int NUM_PLAYERS { get ; set; } = 2;
        public const int MAX_NUM_PLAYER = 4;
        public const int MIN_NUM_PLAYER = 1;
        public bool SHADOWS_ENABLED { get ; set; } = true;

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