using Myra.Graphics2D.UI;
using src.GameObjects;

public abstract class SubMenu{
    protected bool insubElement=false;
    protected bool menuopen=false;
    protected int CENTER_BUTTON_HEIGHT = 100;
    protected int CENTER_BUTTON_WIDTH = 400;
    protected Desktop desktop;
    protected Grid _grid;
    protected Grid returnGrid;
    protected MyMenuElement[] menuElements;
    protected MyMenuElement[] oldMenuElements;
    protected GameStateManager gameStateManager;
    protected MenuStateManager menuStateManager;
    protected MyMenu ParentMenu;
    public SubMenu(Desktop desktop, Grid r, MyMenu P){
        this.desktop = desktop;
        this.ParentMenu = P;
        gameStateManager = GameStateManager.GetGameStateManager();
        menuStateManager = MenuStateManager.GetMenuStateManager();
        menuopen = false;
        insubElement = false;
        returnGrid=r;
    }
    public abstract MyMenuElement[] Activate(MyMenuElement[] R);
    public abstract MyMenuElement[] DeActivate();
}