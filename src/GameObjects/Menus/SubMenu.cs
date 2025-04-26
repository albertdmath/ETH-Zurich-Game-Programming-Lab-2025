using Myra.Graphics2D.UI;
using src.GameObjects;

public abstract class SubMenu{
    protected bool insubElement;
    protected bool menuopen;
    protected int controllerselectedbutton;
    protected int CENTER_BUTTON_HEIGHT = 40;
    protected int CENTER_BUTTON_WIDTH = 250;
    protected Desktop desktop;
    protected Grid _grid;
    protected MyMenuElement[] menuElements;
    protected GameStateManager gameStateManager;
    protected MenuStateManager menuStateManager;
    public SubMenu(Desktop desktop){
        this.desktop = desktop;
        gameStateManager = GameStateManager.GetGameStateManager();
        menuStateManager = MenuStateManager.GetMenuStateManager();
        controllerselectedbutton = 0;
        menuopen = false;
        insubElement = false;
    }
    public abstract void ControllerNavigate();
    public abstract void Activate();
}