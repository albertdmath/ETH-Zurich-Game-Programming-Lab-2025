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
using System.Linq.Expressions;

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
        private Button plus;
        private Button minus;
        private Button shadowplus;
        private Button shadowminus;

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
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Myra.Graphics2D.Thickness{
                    Left = 10
                }
            };
            shadowLabel = new Label{
                Text = $" Players: {VALUE}",
                Font = fontSystem.GetFont(TEXTSIZE),
                TextColor = Color.Black,
                Left = Label.Left+2,
                Top = Label.Top+2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Myra.Graphics2D.Thickness{
                    Left = 10
                }
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
            plus = new Button{
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
                Width = 80,
                VerticalAlignment=VerticalAlignment.Top,
                Border = new SolidBrush(Color.Transparent),
                BorderThickness = new Myra.Graphics2D.Thickness{
                    Left = 7
                },
                Padding = new Myra.Graphics2D.Thickness{
                    Left = 5
                }
            };
            shadowplus = new Button{
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
                Width = 80,
                VerticalAlignment=VerticalAlignment.Top,
                Border = new SolidBrush(Color.Transparent),
                BorderThickness = new Myra.Graphics2D.Thickness{
                    Left = 7
                },
                Padding = new Myra.Graphics2D.Thickness{
                    Left = 5
                }
            };
            plus.MouseEntered +=(s,a)=>{
                MusicAndSoundEffects.playUIHoverSFX();
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
                MusicAndSoundEffects.playUIClickSFX();
                if(VALUE+1<=menuStateManager.MAX_NUM_PLAYER){
                    ++VALUE;
                    
                    Label.Text = $" Players: {VALUE}";
                    shadowLabel.Text = $" Players: {VALUE}";
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
            minus = new Button{
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
                Width = 80,
                VerticalAlignment=VerticalAlignment.Top,
                Border = new SolidBrush(Color.Transparent),
                BorderThickness = new Myra.Graphics2D.Thickness{
                    Left = 7
                },
                Padding = new Myra.Graphics2D.Thickness{
                    Left = 5
                }
            };
            shadowminus = new Button{
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
                Width = 80,
                VerticalAlignment=VerticalAlignment.Top,
                Border = new SolidBrush(Color.Transparent),
                BorderThickness = new Myra.Graphics2D.Thickness{
                    Left = 7
                },
                Padding = new Myra.Graphics2D.Thickness{
                    Left = 5
                }
            };

            minus.MouseEntered +=(s,a)=>{
                MusicAndSoundEffects.playUIHoverSFX();
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
                MusicAndSoundEffects.playUIClickSFX();
                if(VALUE-1>=menuStateManager.MIN_NUM_PLAYER){
                    --VALUE;
                    
                    Label.Text = $" Players: {VALUE}";
                    shadowLabel.Text = $" Players: {VALUE}";
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
            minus.Border = new SolidBrush(Color.Blue);
            plus.Border = new SolidBrush(Color.Blue);
        }
        public override void UnHighlight()
        {
            minus.Border = new SolidBrush(Color.Transparent);
            plus.Border = new SolidBrush(Color.Transparent);
        }
        public override void ControllerValueChange(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            if(gamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released){
                MusicAndSoundEffects.playUIClickSFX();
                if(VALUE-1>=menuStateManager.MIN_NUM_PLAYER){
                    if(VALUE==menuStateManager.MAX_NUM_PLAYER){
                        plus.Content = new Label{
                    Text = "more",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                    }
                    --VALUE;
                    
                    Label.Text = $" Players: {VALUE}";
                    shadowLabel.Text = $" Players: {VALUE}";
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
            }else if(gamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released){
                MusicAndSoundEffects.playUIClickSFX();
                if(VALUE+1<=menuStateManager.MAX_NUM_PLAYER){
                    if(VALUE==menuStateManager.MIN_NUM_PLAYER){
                        minus.Content = new Label{
                    Text = "less",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
                    }
                    ++VALUE;
                    
                    Label.Text = $" Players: {VALUE}";
                    shadowLabel.Text = $" Players: {VALUE}";
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
            minus.Border = new SolidBrush(Color.Yellow);
            plus.Border = new SolidBrush(Color.Yellow);
            return true;   
        }
        public override bool LeaveButton()
        {
            controllerselected=false;
            minus.Border = new SolidBrush(Color.Blue);
            plus.Border = new SolidBrush(Color.Blue);
            
            plus.Content = new Label{
                    Text = "more",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };
            minus.Content = new Label{
                    Text = "less",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = fontSystem.GetFont(TEXTSIZE/2)
                };

            return true;
        }
    }
}