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
                ShowGridLines = true
            };
            MyButton testbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Settingsi eis","sb1",0,0,(s,a)=>{
                gameStateManager.CreateProjectile(ProjectileType.Mjoelnir, new Vector3(1,2,2), new Vector3(1,0,0));
            },_grid);
            menuElements = new MyMenuElement[]{testbutton};
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