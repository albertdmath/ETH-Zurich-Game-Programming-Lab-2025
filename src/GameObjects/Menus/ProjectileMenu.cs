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

namespace src.GameObjects;
    public class ProjectileMenu : SubMenu{
        private FontSystem fontSystem;
        private int TEXTSIZE;
        
        public ProjectileMenu(Desktop desktop, Grid r, MyMenu p, FontSystem fontSystem, int textsize):base(desktop,r,p)
        {
            this.fontSystem=fontSystem;
            TEXTSIZE=textsize/2;
            _grid = new Grid{
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            
            //SOUNDLABELS
            Label Banana = new Label{
                Text = "Banana:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Banana,0);
            Grid.SetRow(Banana,0);

            Label Coconut = new Label{
                Text = "Coconut:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Coconut,0);
            Grid.SetRow(Coconut,1);

            Label Frog = new Label{
                Text = "Frog:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Frog,0);
            Grid.SetRow(Frog,2);

            Label Mjoelnir = new Label{
                Text = "Mjoelnir:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Mjoelnir,0);
            Grid.SetRow(Mjoelnir,3);

            Label Spear = new Label{
                Text = "Spear:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Spear,0);
            Grid.SetRow(Spear,4);

            Label Swordfish = new Label{
                Text = "Swordfish:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Swordfish,2);
            Grid.SetRow(Swordfish,0);

            Label Tomato = new Label{
                Text = "Tomato:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Tomato,2);
            Grid.SetRow(Tomato,1);

            Label Turtle = new Label{
                Text = "Turtle:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Turtle,2);
            Grid.SetRow(Turtle,2);

            Label Chicken = new Label{
                Text = "Chicken:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Chicken,2);
            Grid.SetRow(Chicken,3);

            Label Barrel = new Label{
                Text = "Barrel:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(Barrel,2);
            Grid.SetRow(Barrel,4);
            

            //SHADOWLABELS
            /*Label shadowLabelVolume = new Label{
                Text = "Music:",
                Font = fontSystem.GetFont(TEXTSIZE),
                TextColor = Color.Black,
                Left = VolumeLabel.Left+2,
                Top = VolumeLabel.Top-16, //THESE ARE HIGHLY EXPERIMENTAL
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(shadowLabelVolume,0);
            Grid.SetRow(shadowLabelVolume,1);
            _grid.Widgets.Add(shadowLabelVolume);
            */

            _grid.Widgets.Add(Banana);
            _grid.Widgets.Add(Coconut);
            _grid.Widgets.Add(Frog);
            _grid.Widgets.Add(Mjoelnir);
            _grid.Widgets.Add(Spear);
            _grid.Widgets.Add(Swordfish);
            _grid.Widgets.Add(Tomato);
            _grid.Widgets.Add(Turtle);
            _grid.Widgets.Add(Chicken);
            _grid.Widgets.Add(Barrel);

            //MUSIC/SFX-SLIDER
            MyHorizontalSlider BananaSlider = new MyHorizontalSlider(0,1000,80,1,0,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Banana] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider CoconutSlider = new MyHorizontalSlider(0,1000,150,1,1,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Coconut] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider FrogSlider = new MyHorizontalSlider(0,1000,100,1,2,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Frog] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider MjoelnirSlider = new MyHorizontalSlider(0,1000,100,1,3,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Mjoelnir] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider SpearSlider = new MyHorizontalSlider(0,1000,100,1,4,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Spear] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider SwordfishSlider = new MyHorizontalSlider(0,1000,300,3,0,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Swordfish] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider TomatoSlider = new MyHorizontalSlider(0,1000,200,3,1,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Tomato] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider TurtleSlider = new MyHorizontalSlider(0,1000,80,3,2,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Turtle] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider ChickenSlider = new MyHorizontalSlider(0,1000,5,3,3,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Chicken] = a.NewValue*0.001f;
            },_grid);
            MyHorizontalSlider BarrelSlider = new MyHorizontalSlider(0,1000,3,3,4,(s,a)=>{
                Projectile.ProjectileProbability[ProjectileType.Barrel] = a.NewValue*0.001f;
            },_grid);

            //menuElements = new MyMenuElement[]{testbutton,testbutton1,testbutton2,testbutton3,testbutton4,testbutton5,testbutton6,testbutton7,testbutton8,testbutton9,FXAA,SHADOWS,AMBIENT_OCCLUSION,FULLSCREEN,backbutton};
            menuElements = new MyMenuElement[]{BananaSlider,CoconutSlider,FrogSlider,MjoelnirSlider,SpearSlider,SwordfishSlider,TomatoSlider,TurtleSlider,ChickenSlider,BarrelSlider};
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
