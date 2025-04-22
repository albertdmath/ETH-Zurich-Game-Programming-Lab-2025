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
//CLASS NOT USED
namespace src.GameObjects{
    public class MyMenu{
        private int controllerselectedbutton;
        private int CENTER_BUTTON_HEIGHT = 40;
        private int CENTER_BUTTON_WIDTH = 250;
        private Desktop desktop;
        private MyMenuElement[] menuElements;
        private bool menuopen=false;
        private bool insubElement=false;
        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;
        public MyMenu(GameLabGame game, int DisplayWidth, int DisplayHeight){
            //DEFINE CUSTOM STYLES
            SpinButtonStyle ControllerSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.Black),
                OverBackground = new SolidBrush(Color.Gray),
                DisabledBackground = new SolidBrush(Color.DarkGray),
                Width = CENTER_BUTTON_WIDTH
            };
            SpinButtonStyle DefaultSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.DarkSlateGray),
                Width = CENTER_BUTTON_WIDTH,
                OverBackground = new SolidBrush(Color.Gray),
                DisabledBackground = new SolidBrush(Color.DarkGray)
            };
            SpinButtonStyle ControllerPressedSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.LightGray),
                Width = CENTER_BUTTON_WIDTH
            };
            ButtonStyle ControllerButtonStyle = new ButtonStyle{
                Background = new SolidBrush(Color.Black),
                OverBackground = new SolidBrush(Color.Gray),
                PressedBackground = new SolidBrush(Color.SeaGreen),
                DisabledBackground = new SolidBrush(Color.Gray),
                LabelStyle = new LabelStyle{
                    TextColor = Color.White
                },
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH
            };
            ButtonStyle DefaultButtonStyle = new ButtonStyle{
                Background = new SolidBrush(Color.DarkSlateGray),
                OverBackground = new SolidBrush(Color.DarkSlateBlue),
                PressedBackground = new SolidBrush(Color.BlueViolet),
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH
            };
            //ADD CUSTOM STYLES
            Stylesheet.Current.SpinButtonStyles["controller"] = ControllerSpinbuttonStyle;
            Stylesheet.Current.SpinButtonStyles["default"] = DefaultSpinbuttonStyle;
            Stylesheet.Current.SpinButtonStyles["controllerpressed"] = ControllerPressedSpinbuttonStyle;

            Stylesheet.Current.ButtonStyles["controller"] = ControllerButtonStyle;
            Stylesheet.Current.ButtonStyles["default"] = DefaultButtonStyle;
            
            gameStateManager = GameStateManager.GetGameStateManager();
            menuStateManager = MenuStateManager.GetMenuStateManager();
            
            controllerselectedbutton=0;

            //WINDOW
            Window gridWindow = new Window
            {
                Width = 500,
                Height = 250,
            };
            Grid grid = new Grid
            {
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines=true,
                //Height=DisplayHeight/2,
                //Width=DisplayWidth/3
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            /*MYRA REFERENCE CODE
            var helloworld = new Label{
                Id="label",
                Text = "hello world!"
            };
            grid.Widgets.Add(helloworld);
            
            var combo = new ComboView();
            Grid.SetColumn(combo,1);
            Grid.SetRow(combo,0);

            combo.Widgets.Add(new Label{Text = "Red", TextColor = Color.Red});
            combo.Widgets.Add(new Label{Text = "Green", TextColor = Color.Green});
            combo.Widgets.Add(new Label{Text = "Blue", TextColor = Color.Blue});

            grid.Widgets.Add(combo);
            */
            //CLOSEBUTTON
            MyButton closebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Exit","ExitButton",0,3,(s,a)=>{
                game.Exit();//HARDCORE CLOSING
            },grid);
            
            //RELOADBUTTON
            MyButton reloadbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"New Game","ReloadButton",0,2,(s,a)=>{
                gameStateManager.StartNewGame();//RELOADING
                CloseMenu();
            },grid);
            
            //RESUMEBUTTON
            MyButton resumebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Resume","ResumeButton",0,0,(s,a) => {
                if(gameStateManager.GameIsOver()){
                    gameStateManager.StartNewGame();
                }
                CloseMenu();
            },grid);

            MySpinbutton NumPlayerSpinButton = new MySpinbutton(menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,menuStateManager.NUM_PLAYERS,true,"NumPlayerSpinButton",0,1,grid,(c,a) => {
                float? nullableFloat = a.NewValue;
                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            });

            
            
            //TEST IN PROGRESS
            
            CheckButton checkBox = new CheckButton
                {
                    IsChecked = true,
                    Content = new Label{
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = " THIS IS A CHECKBOX"
                    }
                };

                // Optional: handle toggle events
            checkBox.EnabledChanged += (s, e) =>
            {
                menuStateManager.SOUND_ENABLED=!menuStateManager.SOUND_ENABLED;
            };
            Grid.SetColumn(checkBox,3);
            Grid.SetRow(checkBox,0);
            grid.Widgets.Add(checkBox);
            
            HorizontalSlider sl = new HorizontalSlider{
                Width = CENTER_BUTTON_WIDTH,
                Minimum=0,
                Maximum=1,
                Value=1,
                
            };
            sl.ValueChanged += (s,e) => {
                MediaPlayer.Volume = e.NewValue;
            };
            Grid.SetColumn(sl,3);
            Grid.SetRow(sl,2);
            grid.Widgets.Add(sl);

            HorizontalSlider slsfx = new HorizontalSlider{
                Width = CENTER_BUTTON_WIDTH,
                Minimum = 0,
                Maximum = 1,
                Value = 1
            };
            slsfx.ValueChanged += (s,e) => {
                MusicAndSoundEffects.VOLUME=slsfx.Value;
            };
            Grid.SetColumn(slsfx,3);
            Grid.SetRow(slsfx,3);
            grid.Widgets.Add(slsfx);

            desktop = new Desktop();
            desktop.Root = grid;
            //ELEMENTÄRÄI
            menuElements = new MyMenuElement[]{resumebutton,NumPlayerSpinButton,reloadbutton,closebutton};
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, GamePadState gamePadState, GamePadState previousGamePadState){
            if(keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape) || (gamePadState.Buttons.Start == ButtonState.Pressed && previousGamePadState.Buttons.Start == ButtonState.Released)){
                if(menuopen){
                    CloseMenu();
                }else{
                    OpenMenu();
                }
            }
            if(menuopen){
                if(!insubElement){
                    //EXTRA EXIT WITH B PER REQUEST
                    if(gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released){
                        CloseMenu();
                    }

                    //CONTROLLER NAVIGATION
                    if (gamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released)
                    {
                        menuElements[controllerselectedbutton].UnHighlight();
                        controllerselectedbutton = mod(controllerselectedbutton + 1,menuElements.Length);
                        menuElements[controllerselectedbutton].Highlight();
                    }
                    if(gamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released){
                        menuElements[controllerselectedbutton].UnHighlight();
                        controllerselectedbutton = mod(controllerselectedbutton - 1,menuElements.Length);
                        menuElements[controllerselectedbutton].Highlight();
                    }
                    if(gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released){
                        insubElement = menuElements[controllerselectedbutton].Click();
                    }
                }else{//IN SUBELEMENT LOGIC currently spinbutton only, NEED subelements with own navigation
                    if(gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released){
                        insubElement = !menuElements[controllerselectedbutton].LeaveButton();
                    }
                    if (gamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released){
                        menuElements[controllerselectedbutton].ControllerValueChange(-1);
                    }
                    if (gamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released){
                        menuElements[controllerselectedbutton].ControllerValueChange(1);
                    }
                }
            }

        }
        public bool menuisopen(){
            return menuopen;
        }
        private void CloseMenu(){
            menuElements[controllerselectedbutton].LeaveButton();
            menuElements[controllerselectedbutton].UnHighlight();
            controllerselectedbutton=0;
            menuopen=false;
        }
        public void OpenMenu(){
            //menuElements[controllerselectedbutton].Highlight(); //SHOULD FIRST BUTTON BE HIGHLIGHTED IF WE OPEN THE MENU??? I SAY NO
            menuopen=true;
        }
        public void Draw(){
            if(menuopen){
                desktop.Render();
            }
        }
        
        
        
        
        
        
        private int mod(int k, int n) {  return ((k %= n) < 0) ? k+n : k;  }
        private void SpinChangesValue(float? nullableFloat){
            menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
            gameStateManager.StartNewGame();
        }
    }
}
