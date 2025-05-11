using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Utility;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.TextureAtlases;
using GameLab;
using System.Collections.Generic;
using System;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using FontStashSharp;

namespace src.GameObjects{
    public class EndMenu : SubMenu{
        private FontSystem fontSystem;
        private int TEXTSIZE;
        public EndMenu(Desktop desktop, Grid r, MyMenu p, FontSystem FontSystem, int textsize, MyMenuElement[] main):base(desktop,r,p){
            fontSystem = FontSystem;
            TEXTSIZE = textsize;
            this.oldMenuElements = main;
            _grid = new Grid{
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            MyButton reload = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Restart",0,0,(s,a)=>{
                menuStateManager.COUNTDOWN = true;
                gameStateManager.StartNewGame();//RELOADING
                p.CloseEndMenu();
                p.CloseMenu();
            },_grid,fontSystem,TEXTSIZE);

            MyButton backtomain = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Main Menu",0,1,(s,a)=>{
                menuStateManager.MAIN_MENU_IS_OPEN=true;           
                menuStateManager.COUNTDOWN = false;
                menuStateManager.TRANSITION = false;
                menuStateManager.PAUSE_MENU_IS_OPEN = false;
                MusicAndSoundEffects.playMainMenuMusic();
                returnGrid = p.ToMainMenu();
                //p.setMenuElements(oldMenuElements);
                p.CloseEndMenu();
                gameStateManager.StartNewGame();

            },_grid,fontSystem,TEXTSIZE);
            

            menuElements = new MyMenuElement[]{reload,backtomain};

        }
        public override MyMenuElement[] Activate(MyMenuElement[] R)
        {
            //returnGrid = (Grid) desktop.Root;
            //oldMenuElements = R;
            desktop.Root = _grid;
            menuopen = true;
            return menuElements;
        }
        public override MyMenuElement[] DeActivate()
        {
            desktop.Root = returnGrid;
            menuopen = false;
            return oldMenuElements;
        }
        public bool Isopen(){
            return menuopen;
        }
        public void Close(){
            MusicAndSoundEffects.playMainMenuMusic(); 
            menuStateManager.MAIN_MENU_IS_OPEN=true;
                returnGrid = ParentMenu.ToMainMenu();
                ParentMenu.setMenuElements(oldMenuElements);
                ParentMenu.CloseEndMenu();
                gameStateManager.StartNewGame();
        }
    }
}