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

namespace src.GameObjects{
    public class MyButton : MyMenuElement{
        private int TEXTSIZE;
        private int WIDTH;
        private int HEIGHT;
        private string ID;
        private int COLUMN;
        private int ROW;
        private Action<object?,EventArgs> CLICK;
        private Grid GRID;
        private Button button;
        private FontSystem fontSystem;

        public MyButton(int width, int height, string text, string id, int column, int row, Action<object?,EventArgs> Click, Grid grid, FontSystem fontSystem, int textsize){
            TEXTSIZE = textsize;
            this.fontSystem=fontSystem;
            WIDTH=width;
            HEIGHT=height;
            ID=id;
            COLUMN=column;
            ROW=row;
            CLICK=Click;
            GRID=grid;
            
            button = new Button{
              Width=WIDTH,
              Height=HEIGHT,
              Content = new Label{
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = text,
                TextColor = Color.Black,
                Font = fontSystem.GetFont(TEXTSIZE)
              }  
            };
            Grid.SetColumn(button,COLUMN);
            Grid.SetRow(button,ROW);
            button.Click += new EventHandler(CLICK);
            button.SetStyle("default");
            GRID.Widgets.Add(button);
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
            button.Content = new Label{
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = t,
                TextColor = Color.Black,
                Font = fontSystem.GetFont(TEXTSIZE)
            };
        }
    }
}