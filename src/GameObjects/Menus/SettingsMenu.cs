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

namespace src.GameObjects{
    public class SettingsMenu : SubMenu{
        public SettingsMenu(Desktop desktop, Grid r):base(desktop,r)
        {
            _grid = new Grid{
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            
            MyButton testbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Mjoelnir","sb0",0,0,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Mjoelnir);
            },_grid);
            MyButton testbutton1 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Banana","sb1",0,1,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Banana);
            },_grid);
            MyButton testbutton2 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Barrel","sb2",0,2,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Barrel);
            },_grid);
            MyButton testbutton3 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Chicken","sb3",0,3,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Chicken);
            },_grid);
            MyButton testbutton4 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Coconut","sb4",0,4,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Coconut);
            },_grid);
            MyButton testbutton5 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Frog","sb5",0,5,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Frog);
            },_grid);
            MyButton testbutton6 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Spear","sb6",0,6,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Spear);
            },_grid);
            MyButton testbutton7 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Swordfish","sb7",0,7,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Swordfish);
            },_grid);
            MyButton testbutton8 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Tomato","sb8",0,8,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Tomato);
            },_grid);
            MyButton testbutton9 = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Turtle","sb9",0,9,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Turtle);
            },_grid);

            menuElements = new MyMenuElement[]{testbutton,testbutton1,testbutton2,testbutton3,testbutton4,testbutton5,testbutton6,testbutton7,testbutton8,testbutton9};
        }
        public override MyMenuElement[] Activate(MyMenuElement[] R)
        {
            oldMenuElements = R;
            desktop.Root = _grid;
            menuopen=true;
            return menuElements;
        }
        public override MyMenuElement[] DeActivate()
        {
            desktop.Root = returnGrid;
            menuopen=false;
            return oldMenuElements;
        }
        
    }
}