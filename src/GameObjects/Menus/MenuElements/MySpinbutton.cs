using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using GameLab;
using System;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using info.lundin.math;
using Myra.Events;
using System.Reflection.Metadata;
using Myra.Graphics2D.Brushes;
using FontStashSharp;

namespace src.GameObjects{
    public class MySpinbutton : MyMenuElement{
        private int MINIMUM;
        private int MAXIMUM;
        private bool ISNULLABLE;
        private int StartValue;
        private int VALUE;
        private bool ISINTEGER;
        private string ID;
        private int COLUMN;
        private int ROW;
        private Grid GRID;
        public bool controllerselected {get;set;}=false;
        private Action<object?,EventArgs> PLUS;
        private Action<object?,EventArgs> MINUS;
        FontSystem fontSystem;
        int TEXTSIZE;
        private MenuStateManager menuStateManager;
        private GameStateManager gameStateManager;
        private Label Label;
        private Label shadowLabel;

        public MySpinbutton(int width, int height, int minimum, int maximum, bool isnullable, int startvalue, bool isinteger, string id, int column, int row, Grid grid,FontSystem fontSystem,int textsize){
            this.MINIMUM=minimum;
            this.MAXIMUM=maximum;
            this.ISNULLABLE=isnullable;
            menuStateManager = MenuStateManager.GetMenuStateManager();
            gameStateManager = GameStateManager.GetGameStateManager();
            this.VALUE = startvalue;
            this.fontSystem=fontSystem;
            TEXTSIZE=textsize;
            if(startvalue<=MAXIMUM && startvalue>=MINIMUM){
                this.StartValue=startvalue;
            }else{
                this.StartValue=MINIMUM;
            }
            this.ISINTEGER=isinteger;
            this.ID=id;
            this.COLUMN=column;
            this.ROW=row;
            this.GRID=grid;
            
            

            Label = new Label{
                Text = $" Players: {VALUE}",
                Font = fontSystem.GetFont(TEXTSIZE),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            shadowLabel = new Label{
                Text = $" Players: {VALUE}",
                Font = fontSystem.GetFont(TEXTSIZE),
                TextColor = Color.Black,
                Left = Label.Left+2,
                Top = Label.Top+2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(Label,0);
            Grid.SetRow(Label,0);
            Grid.SetColumn(shadowLabel,0);
            Grid.SetRow(shadowLabel,0);

            Grid layout = new Grid();
            layout.RowSpacing=2;
            layout.Widgets.Add(shadowLabel);
            layout.Widgets.Add(Label);

            //PLUS+++++++++++++++++++++++++++++++++++++++++++++
            Button plus = new Button{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.Transparent),
                Content = new Label{
                    Text = "more",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                },
                Height = 40,
                Width = 40
            };
            Button shadowplus = new Button{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.Transparent),
                Content = new Label{
                    Text = "more",
                    TextColor = Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2),
                    Left = plus.Left+2,
                    Top = plus.Top+2,
                },
                Height = 40,
                Width = 40
            };
            plus.MouseEntered +=(s,a)=>{
                plus.Content = new Label{
                    Text = "more",
                    TextColor = Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                shadowplus.Content = new Label{
                    Text = "more",
                    TextColor = Color.Transparent,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2),
                    Top = plus.Top+2,
                    Left = plus.Left+2
                };
            };
            plus.MouseLeft +=(s,a)=>{
                plus.Content = new Label{
                    Text = "more",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                shadowplus.Content = new Label{
                    Text = "more",
                    TextColor = Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2),
                    Top = plus.Top+2,
                    Left = plus.Left+2
                };
            };
            plus.Click += (s,a)=>{
                if(VALUE+1<=menuStateManager.MAX_NUM_PLAYER){
                    ++VALUE;
                    
                    Label.Text = $" Players : {VALUE}";
                    shadowLabel.Text = $" Players : {VALUE}";
                    menuStateManager.NUM_PLAYERS=VALUE;
                    gameStateManager.StartNewGame();
                }else{
                    plus.Content = new Label{
                    Text = "more",
                    TextColor = Color.Red,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                }
            };
            //PLUS+++++++++++++++++++++++++++++++++++++++++++++
            //MINUS--------------------------------------------
            Button minus = new Button{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.Transparent),
                Content = new Label{
                    Text = "less",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                },
                Height = 40,
                Width = 40,
                
            };
            Button shadowminus = new Button{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.Transparent),
                Content = new Label{
                    Text = "less",
                    TextColor = Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2),
                    Top = minus.Top+2,
                    Left = minus.Left+2
                },
                Height = 40,
                Width = 40
            };

            minus.MouseEntered +=(s,a)=>{
                minus.Content = new Label{
                    Text = "less",
                    TextColor = Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                shadowminus.Content = new Label{
                    Text = "less",
                    TextColor = Color.Transparent,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2),
                    Top = minus.Top+2,
                    Left = minus.Left+2
                };
            };
            minus.MouseLeft +=(s,a)=>{
                minus.Content = new Label{
                    Text = "less",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                shadowminus.Content = new Label{
                    Text = "less",
                    TextColor = Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2),
                    Top = minus.Top+2,
                    Left = minus.Left+2
                };
            };
            minus.Click += (s,a)=>{
                if(VALUE-1>=menuStateManager.MIN_NUM_PLAYER){
                    --VALUE;
                    
                    Label.Text = $" Players : {VALUE}";
                    shadowLabel.Text = $" Players : {VALUE}";
                    menuStateManager.NUM_PLAYERS=VALUE;
                    gameStateManager.StartNewGame();
                }else{
                    minus.Content = new Label{
                    Text = "less",
                    TextColor = Color.Red,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                }
            };
            //MINUS--------------------------------------------
            VerticalStackPanel spinbutton2 = new VerticalStackPanel();
            spinbutton2.Widgets.Add(plus);
            spinbutton2.Widgets.Add(minus);
            spinbutton2.VerticalAlignment= VerticalAlignment.Center;

            VerticalStackPanel shadowSpinbutton2 = new VerticalStackPanel();
            shadowSpinbutton2.Widgets.Add(shadowplus);
            shadowSpinbutton2.Widgets.Add(shadowminus);
            shadowSpinbutton2.VerticalAlignment = VerticalAlignment.Center;
            
            
            //layout.HorizontalAlignment = HorizontalAlignment.Left;
            
            
            
            Grid.SetColumn(spinbutton2,0);
            Grid.SetColumn(shadowSpinbutton2,0);
            layout.Widgets.Add(shadowSpinbutton2);
            layout.Widgets.Add(spinbutton2);
            Grid.SetColumn(layout,COLUMN);
            Grid.SetRow(layout,ROW);
            GRID.Widgets.Add(layout);
        }
        public override void Highlight()
        {
            //spinbutton.SetStyle("controller");
        }
        public override void UnHighlight()
        {
            //spinbutton.SetStyle("default");
        }
        public override void ControllerValueChange(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            int sign=0;
            if(gamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released){
                sign=-1;
            }else if(gamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released){
                sign=1;
            }else{
                return;
            }
            //float? oldValue = spinbutton.Value;
            //if(sign+spinbutton.Value<=MAXIMUM && sign+spinbutton.Value>=MINIMUM){
            //    Valuechanging.Invoke(spinbutton,new ValueChangingEventArgs<float?>(spinbutton.Value,spinbutton.Value+sign));
            //    spinbutton.Value+=sign;//IDK WHY BUT WE NEED THIS
            //}
        }
        public override bool Click()
        {
         controllerselected=true;
         //spinbutton.SetStyle("controllerpressed");
         return true;   
        }
        public override bool LeaveButton()
        {
            controllerselected=false;
            //spinbutton.SetStyle("controller");
            return true;
        }
    }
}