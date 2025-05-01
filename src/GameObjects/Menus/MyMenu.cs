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
        private bool changegrid = false;
        private int controllerselectedbutton;
        private int oldcontrollerselectedbutton;
        private int CENTER_BUTTON_HEIGHT = 40;
        private int CENTER_BUTTON_WIDTH = 250;
        private Desktop desktop;
        private Grid _grid;
        private MyMenuElement[] menuElements;
        private MyMenuElement[] basemenuElements;
        private bool menuopen=true;
        private bool controllerlocked = false;
        private bool insubMenu = false;
        SubMenu subMenu;
        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;
        public MyMenu(GameLabGame game, int DisplayWidth, int DisplayHeight){
            desktop = new Desktop();
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
            SliderStyle DefaultSliderStyle = new SliderStyle{
                Background = new SolidBrush(Color.White),
                FocusedBorder = new SolidBrush(Color.Honeydew),
                DisabledBackground = new SolidBrush(Color.Aquamarine),
                DisabledBorder = new SolidBrush(Color.Beige),
                Border = new SolidBrush(Color.White),
                Height = 20,
                //NO IDEA SMTH LIKE PADDING Margin= new Thickness{Right=10,Left=10,Top=10,Bottom=10},
                //PADDING Padding = new Thickness{Right=0,Left=0,Top=0,Bottom=0},
                OverBackground = new SolidBrush(Color.Purple),
                OverBorder = new SolidBrush(Color.Red),
                FocusedBackground = new SolidBrush(Color.GreenYellow),
                BorderThickness = new Thickness{Right=2,Left=2,Top=2,Bottom=2},
                Width=CENTER_BUTTON_WIDTH,
            };
            SliderStyle ControllerSliderStyle = new SliderStyle{
                Background = new SolidBrush(Color.Red),
                FocusedBorder = new SolidBrush(Color.Honeydew),
                DisabledBackground = new SolidBrush(Color.Aquamarine),
                DisabledBorder = new SolidBrush(Color.Beige),
                Border = new SolidBrush(Color.White),
                Height = 20,
                //NO IDEA SMTH LIKE PADDING Margin= new Thickness{Right=10,Left=10,Top=10,Bottom=10},
                //PADDING Padding = new Thickness{Right=0,Left=0,Top=0,Bottom=0},
                OverBackground = new SolidBrush(Color.Purple),
                OverBorder = new SolidBrush(Color.Red),
                FocusedBackground = new SolidBrush(Color.GreenYellow),
                BorderThickness = new Thickness{Right=2,Left=2,Top=2,Bottom=2},
                Width=CENTER_BUTTON_WIDTH,
            };
            SliderStyle ControllerPressedSliderStyle = new SliderStyle{
                Background = new SolidBrush(Color.White),
                FocusedBorder = new SolidBrush(Color.Honeydew),
                DisabledBackground = new SolidBrush(Color.Aquamarine),
                DisabledBorder = new SolidBrush(Color.Beige),
                Border = new SolidBrush(Color.White),
                Height = 20,
                //NO IDEA SMTH LIKE PADDING Margin= new Thickness{Right=10,Left=10,Top=10,Bottom=10},
                //PADDING Padding = new Thickness{Right=0,Left=0,Top=0,Bottom=0},
                OverBackground = new SolidBrush(Color.Purple),
                OverBorder = new SolidBrush(Color.Red),
                FocusedBackground = new SolidBrush(Color.GreenYellow),
                BorderThickness = new Thickness{Right=2,Left=2,Top=2,Bottom=2},
                Width=CENTER_BUTTON_WIDTH,
            };
            Stylesheet.Current.HorizontalSliderStyles["default"] = DefaultSliderStyle;
            Stylesheet.Current.HorizontalSliderStyles["controller"] = ControllerSliderStyle;
            Stylesheet.Current.HorizontalSliderStyles["controllerpressed"] = ControllerPressedSliderStyle;
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
            
            _grid = new Grid
            {
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines=false,
                //Height=DisplayHeight/2,
                //Width=DisplayWidth/3
            };
            
            Grid startgrid = new Grid{
                RowSpacing = 1,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            
            startgrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            startgrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            startgrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            startgrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
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
            //STARTMENU/GRID
            MyButton startexit = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Exit","StartExitButton",0,4,(s,a)=>{
                game.Exit();
            },startgrid);

            MyButton startstartbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Start Game","StartButton",0,0,(s,a)=>{
                gameStateManager.StartNewGame();
                CloseMenu();
            },startgrid);

            MySpinbutton startNumPlayerSpinButton = new MySpinbutton(menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,menuStateManager.NUM_PLAYERS,true,"StartNumPlayerSpinButton",0,3,startgrid,(c,a)=>{
                float? nullableFloat = a.NewValue;
                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            });

            //BASEGRID
            //CLOSEBUTTON
            MyButton closebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Exit","ExitButton",0,5,(s,a)=>{
                game.Exit();//HARDCORE CLOSING
            },_grid);
            
            
            //RELOADBUTTON
            MyButton reloadbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"New Game","ReloadButton",0,3,(s,a)=>{
                gameStateManager.StartNewGame();//RELOADING
                CloseMenu();
            },_grid);
            
            //RESUMEBUTTON
            MyButton resumebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Resume","ResumeButton",0,0,(s,a) => {
                if(gameStateManager.GameIsOver()){
                    gameStateManager.StartNewGame();
                }
                CloseMenu();
            },_grid);

            
            //NUM_PLAYERS
            MySpinbutton NumPlayerSpinButton = new MySpinbutton(menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,menuStateManager.NUM_PLAYERS,true,"NumPlayerSpinButton",0,2,_grid,(c,a) => {
                float? nullableFloat = a.NewValue;
                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            });





            //SETTINGS-MENU
            SettingsMenu settingsMenu = new SettingsMenu(desktop,_grid);
            //SETTINGS
            MyButton settingsButton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Settings?","SettingsButton",0,4,(s,a)=>{
                menuElements[controllerselectedbutton].UnHighlight();
                insubMenu = true;
                subMenu = settingsMenu;
                menuElements = settingsMenu.Activate(menuElements);
                oldcontrollerselectedbutton=controllerselectedbutton;
                controllerselectedbutton=0;
            },_grid);

            //MUSICSLIDER
            MyHorizontalSlider Volume = new MyHorizontalSlider(0,100,45,2,2,(s,a)=>{
                MediaPlayer.Volume = a.NewValue*0.01f;
            },_grid);
            MyHorizontalSlider SFXVolume = new MyHorizontalSlider(0,100,100,2,4,(s,a)=>{
                float? nullableFloat = a.NewValue;
                MusicAndSoundEffects.VOLUME = (float)(nullableFloat*0.01f ?? 0.5);
                MusicAndSoundEffects.angrymobInstance.Volume = (float)(nullableFloat*0.001f ?? 0.1f);
            },_grid);
            /*MyButton VolumePlaceholder = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Volume","VolumePlaceholder",2,1,(s,a)=>{
                return;
            },_grid);*/
            Label VolumeLabel = new Label{
                Text = "Music",
                TextColor = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(Color.Beige),
                Padding = new Thickness{Top=5,Bottom=5}
            };
            Grid.SetColumn(VolumeLabel,2);
            Grid.SetRow(VolumeLabel,1);
            _grid.Widgets.Add(VolumeLabel);

            Label SFXVolumeLabel = new Label{
                Text = "SFX",
                TextColor = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(Color.Beige),
                Padding = new Thickness{Top=5,Bottom=5}
            };
            Grid.SetColumn(SFXVolumeLabel,2);
            Grid.SetRow(SFXVolumeLabel,3);
            _grid.Widgets.Add(SFXVolumeLabel);

            Label StartNumPlayerLabel = new Label{
                Text = "Number of Players:",
                TextColor = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(Color.Beige),
                Padding = new Thickness{Top=5,Bottom=5}
            };
            Grid.SetColumn(StartNumPlayerLabel,0);
            Grid.SetRow(StartNumPlayerLabel,2);
            startgrid.Widgets.Add(StartNumPlayerLabel);

            Label NumPlayerLabel = new Label{
                Text = "Number of Players:",
                TextColor = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(Color.Beige),
                Padding = new Thickness{Top=5,Bottom=5}
            };
            Grid.SetColumn(NumPlayerLabel,0);
            Grid.SetRow(NumPlayerLabel,1);
            _grid.Widgets.Add(NumPlayerLabel);
            //TEST IN PROGRESS
            /*
            CheckButton checkBox = new CheckButton
                {
                    IsChecked = true,
                    Content = new Label{
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "Sound"
                    }
                };

            // Optional: handle toggle events
            checkBox.PressedChanged += (s,e)=>{
                menuStateManager.SOUND_ENABLED = !menuStateManager.SOUND_ENABLED;
            };
            Grid.SetColumn(checkBox,3);
            Grid.SetRow(checkBox,0);
            _grid.Widgets.Add(checkBox);


            //SLIDER TESTS
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
            _grid.Widgets.Add(sl);
            sl.SetStyle("default");
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
            _grid.Widgets.Add(slsfx);*/

            //SET DESKTOP FOR STARTMENU
            
            desktop.Root = startgrid;
            //desktop.Root = grid;
            //ELEMENTÄRÄIs
            menuElements = new MyMenuElement[]{startstartbutton,startNumPlayerSpinButton,startexit};
            basemenuElements = new MyMenuElement[]{resumebutton,NumPlayerSpinButton,reloadbutton,settingsButton,closebutton,Volume,SFXVolume};
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, GamePadState gamePadState, GamePadState previousGamePadState){
            if(changegrid){
                desktop.Root = _grid;
                changegrid=false;
                menuElements = basemenuElements;
            }
            


            if((keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)) || (gamePadState.Buttons.Start == ButtonState.Pressed && previousGamePadState.Buttons.Start == ButtonState.Released)){
                if(menuopen){
                    if(keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape) && insubMenu){
                        CloseSubMenu();
                    }else{
                        CloseMenu();
                    }
                }else{
                    OpenMenu();
                }
            }

            if(menuopen){
                if(!controllerlocked){
                    //EXTRA EXIT WITH B PER REQUEST
                    if(gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released){
                        if(insubMenu){
                            CloseSubMenu();
                            menuElements[controllerselectedbutton].Highlight();
                        }else{
                            CloseMenu();
                        }
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
                        controllerlocked = menuElements[controllerselectedbutton].Click();
                    }
                }else{//IN SUBELEMENT LOGIC currently spinbutton only, NEED subelements with own navigation
                    if(gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released){
                        controllerlocked = !menuElements[controllerselectedbutton].LeaveButton();
                    }else if (gamePadState.DPad.Down == ButtonState.Pressed || gamePadState.DPad.Up == ButtonState.Pressed || gamePadState.DPad.Left == ButtonState.Pressed || gamePadState.DPad.Right == ButtonState.Pressed){
                        menuElements[controllerselectedbutton].ControllerValueChange(gamePadState, previousGamePadState);
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
            if(insubMenu){
                CloseSubMenu();
            }
            controllerselectedbutton=0;
            controllerlocked=false;
            menuopen=false;
            

            if(menuStateManager.START_MENU_IS_OPEN){
                menuStateManager.START_MENU_IS_OPEN=false;
                changegrid = true;
            }
        }
        private void CloseSubMenu(){
            menuElements[controllerselectedbutton].LeaveButton();
            menuElements[controllerselectedbutton].UnHighlight();
            menuElements = subMenu.DeActivate();
            controllerselectedbutton=oldcontrollerselectedbutton;
            controllerlocked=false;
            insubMenu=false;
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
