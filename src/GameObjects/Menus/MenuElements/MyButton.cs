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

namespace src.GameObjects{
    public class MyButton : MyMenuElement{
        private int WIDTH;
        private int HEIGHT;
        private string TEXT;
        private string ID;
        private int COLUMN;
        private int ROW;
        private Action<object?,EventArgs> CLICK;
        private Grid GRID;
        private Button button;

        public MyButton(int width, int height, string text, string id, int column, int row, Action<object?,EventArgs> Click, Grid grid){
            WIDTH=width;
            HEIGHT=height;
            TEXT=text;
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
                Text = TEXT
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
    }
}