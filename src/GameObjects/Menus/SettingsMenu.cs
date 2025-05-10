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
    public class SettingsMenu : SubMenu{
        private FontSystem fontSystem;
        private int TEXTSIZE;
        private MyButton FXAA;
        private MyButton SHADOWS;
        private MyButton AMBIENT_OCCLUSION;
        private MyButton FULLSCREEN;
        public SettingsMenu(Desktop desktop, Grid r, MyMenu p, FontSystem fontSystem, int textsize):base(desktop,r,p)
        {
            this.fontSystem=fontSystem;
            TEXTSIZE=textsize;
            _grid = new Grid{
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            /*
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
            },_grid);*/

            FXAA = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"FXAA: on",0,/*10*/0,(s,a)=>{
                if(menuStateManager.FXAA_ENABLED){
                    menuStateManager.FXAA_ENABLED = false;
                    FXAA.ChangeText("FXAA: off");
                }else{
                    menuStateManager.FXAA_ENABLED = true;
                    FXAA.ChangeText("FXAA: on");
                }
            },_grid,fontSystem,TEXTSIZE);
            
            SHADOWS = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Shadows: on",0,/*11*/1,(s,a)=>{
                if(menuStateManager.SHADOWS_ENABLED){
                    menuStateManager.SHADOWS_ENABLED = false;
                    SHADOWS.ChangeText("Shadows: off");
                }else{
                    menuStateManager.SHADOWS_ENABLED = true;
                    SHADOWS.ChangeText("Shadows: on");
                }
            },_grid,fontSystem,TEXTSIZE);

            AMBIENT_OCCLUSION = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"SSAO: on",0,/*12*/2,(s,a)=>{
                if(menuStateManager.AMBIENT_OCCLUSION_ENABLED){
                    menuStateManager.AMBIENT_OCCLUSION_ENABLED = false;
                    AMBIENT_OCCLUSION.ChangeText("SSAO: off");
                }else{
                    menuStateManager.AMBIENT_OCCLUSION_ENABLED = true;
                    AMBIENT_OCCLUSION.ChangeText("SSAO: on");
                }
            },_grid,fontSystem,TEXTSIZE);

            FULLSCREEN = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Fullscreen",0,3,(s,a)=>{
                if(menuStateManager.FULLSCREEN){
                    menuStateManager.FULLSCREEN = false;
                    FULLSCREEN.ChangeText("Window");
                }else{
                    menuStateManager.FULLSCREEN = true;
                    FULLSCREEN.ChangeText("Fullscreen");
                }
            },_grid,fontSystem,TEXTSIZE);

            MySpinbutton NumPlayerSpinButton = new MySpinbutton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,2,true,"whatever",0,4,_grid,fontSystem,textsize);

            MyButton backbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Back",0,/*13*/5,(s,a)=>{
                ParentMenu.CloseSubMenu();
            },_grid,fontSystem,TEXTSIZE);





            //SOUNDS
            Label VolumeLabel = new Label{
                Text = "Music:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(VolumeLabel,2);
            Grid.SetRow(VolumeLabel,1);
            _grid.Widgets.Add(VolumeLabel);

            Label SFXVolumeLabel = new Label{
                Text = "SFX:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(SFXVolumeLabel,2);
            Grid.SetRow(SFXVolumeLabel,3);
            _grid.Widgets.Add(SFXVolumeLabel);

            //MUSIC/SFX-SLIDER
            MyHorizontalSlider Volume = new MyHorizontalSlider(0,100,45,2,2,(s,a)=>{
                MediaPlayer.Volume = a.NewValue*0.01f;
            },_grid);
            MyHorizontalSlider SFXVolume = new MyHorizontalSlider(0,100,100,2,4,(s,a)=>{
                float? nullableFloat = a.NewValue;
                MusicAndSoundEffects.VOLUME = (float)(nullableFloat*0.01f ?? 0.5);
                MusicAndSoundEffects.angrymobInstance.Volume = (float)(nullableFloat*0.001f ?? 0.1f);
            },_grid);

            //menuElements = new MyMenuElement[]{testbutton,testbutton1,testbutton2,testbutton3,testbutton4,testbutton5,testbutton6,testbutton7,testbutton8,testbutton9,FXAA,SHADOWS,AMBIENT_OCCLUSION,FULLSCREEN,backbutton};
            menuElements = new MyMenuElement[]{FXAA,SHADOWS,AMBIENT_OCCLUSION,FULLSCREEN,NumPlayerSpinButton,backbutton,Volume,SFXVolume};
        }
        public override MyMenuElement[] Activate(MyMenuElement[] R)
        {
            returnGrid = (Grid) desktop.Root; //Dangerous
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