using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using GameLab;
using System;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using System.Reflection.Metadata;
using Accord;
using FontStashSharp;
using Myra.Graphics2D.Brushes;

namespace src.GameObjects{
    public class MyButton : MyMenuElement{
        private string TEXT;
        private int TEXTSIZE;
        private int WIDTH;
        private int HEIGHT;
        private int COLUMN;
        private int ROW;
        private Action<object?,EventArgs> CLICK;
        private Grid GRID;
        private Button button;
        private Button shadow;
        private FontSystem fontSystem;

        public MyButton(int width, int height, string text, int column, int row, Action<object?,EventArgs> Click, Grid grid, FontSystem fontSystem, int textsize){
            TEXTSIZE = textsize;
            this.fontSystem=fontSystem;
            WIDTH=width;
            HEIGHT=height;
            COLUMN=column;
            ROW=row;
            CLICK=Click;
            GRID=grid;
            TEXT = text;
            
            button = new Button{
              Width=WIDTH,
              Height=HEIGHT,
              Content = new Label{
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = text,
                TextColor = Color.White,
                Font = fontSystem.GetFont(TEXTSIZE)
              }  
            };
            shadow = new Button{
              Width=WIDTH,
              Height=HEIGHT,
              Content = new Label{
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text =  text,
                TextColor = Color.Black,
                Font = fontSystem.GetFont(TEXTSIZE),
                Left = button.Left+2,
                Top = button.Top+2,
                
              },
              Background = new SolidBrush(Color.Transparent),
              OverBackground = new SolidBrush(Color.Transparent),
              PressedBackground = new SolidBrush(Color.Transparent),
              HorizontalAlignment = HorizontalAlignment.Center,
              VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(shadow,COLUMN);
            Grid.SetRow(shadow,ROW);
            GRID.Widgets.Add(shadow);



            Grid.SetColumn(button,COLUMN);
            Grid.SetRow(button,ROW);
            button.Click += new EventHandler(CLICK);
            button.Click += (c,a)=>{
                MusicAndSoundEffects.playUIClickSFX();
            };
            button.SetStyle("default");
            GRID.Widgets.Add(button);


            button.MouseEntered +=(s,a)=>{
                MusicAndSoundEffects.playUIHoverSFX();
                button.Content = new Label{
                Text = TEXT,
                Font = fontSystem.GetFont(TEXTSIZE),
                TextColor = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            };

            button.MouseLeft +=(s,a)=>{
                button.Content = new Label{
                Text = TEXT,
                Font = fontSystem.GetFont(TEXTSIZE),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            };
        }
        public override void Highlight(){
            button.SetStyle("controller");
        }

        public override void UnHighlight()
        {
            button.SetStyle("default");
        }
        public override bool Click()
        {
            button.DoClick();
            return false;
        }
        public override bool LeaveButton()
        {//does nothing
            return false;
        }
        public override void ControllerValueChange(GamePadState p, GamePadState d)
        {//does nothing
        }
        public void ChangeText(string t){
            TEXT = t;
            button.Content = new Label{
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = t,
                TextColor = Color.Black,
                Font = fontSystem.GetFont(TEXTSIZE)
            };
            shadow.Content = new Label{
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text =  t,
                TextColor = Color.Black,
                Font = fontSystem.GetFont(TEXTSIZE),
                Left = button.Left+2,
                Top = button.Top+2,
            };
        }
    }
}