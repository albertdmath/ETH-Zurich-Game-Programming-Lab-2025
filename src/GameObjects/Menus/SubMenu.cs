using Myra.Graphics2D.UI;
using src.GameObjects;

public abstract class SubMenu{
    protected bool insubElement=false;
    protected bool menuopen=false;
    public int controllerselectedbutton{get;set;} = 0;
    protected int CENTER_BUTTON_HEIGHT = 40;
    protected int CENTER_BUTTON_WIDTH = 250;
    protected Desktop desktop;
    protected Grid _grid;
    protected Grid returnGrid;
    protected MyMenuElement[] menuElements;
    protected MyMenuElement[] oldMenuElements;
    protected GameStateManager gameStateManager;
    protected MenuStateManager menuStateManager;
    public SubMenu(Desktop desktop, Grid r){
        this.desktop = desktop;
        gameStateManager = GameStateManager.GetGameStateManager();
        menuStateManager = MenuStateManager.GetMenuStateManager();
        controllerselectedbutton = 0;
        menuopen = false;
        insubElement = false;
        returnGrid=r;
    }
    public abstract MyMenuElement[] Activate(MyMenuElement[] R);
    public abstract MyMenuElement[] DeActivate();
}