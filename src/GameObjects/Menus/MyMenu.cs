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
using System.IO;
using FontStashSharp;
//CLASS NOT USED
namespace src.GameObjects{
    public class MyMenu{
        private int TEXTSIZE = 42;
        private bool changegrid = false;
        private int controllerselectedbutton;
        private int oldcontrollerselectedbutton;
        private int CENTER_BUTTON_HEIGHT = 100;
        private int CENTER_BUTTON_WIDTH = 320;
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
            FontSystem MedievalFont = new FontSystem();
            byte[] ttfData = File.ReadAllBytes("./Content/OldLondon.ttf");
            MedievalFont.AddFont(ttfData);

            //DEFINE CUSTOM STYLES
            SpinButtonStyle ControllerSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.Black),
                OverBackground = new SolidBrush(Color.Gray),
                DisabledBackground = new SolidBrush(Color.DarkGray),
                Width = CENTER_BUTTON_WIDTH/2,
                //Height = CENTER_BUTTON_HEIGHT
            };
            SpinButtonStyle DefaultSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.DarkRed),
                Width = CENTER_BUTTON_WIDTH/2,
                //Height = CENTER_BUTTON_HEIGHT,
                OverBackground = new SolidBrush(Color.Red),
                DisabledBackground = new SolidBrush(Color.DarkGray)
            };
            SpinButtonStyle ControllerPressedSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.LightGray),
                Width = CENTER_BUTTON_WIDTH/2,
                //Height = CENTER_BUTTON_HEIGHT
            };
            ButtonStyle ControllerButtonStyle = new ButtonStyle{
                Background = new SolidBrush(Color.BlueViolet),
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
                Background = new SolidBrush(Color.Gold),
                OverBackground = new SolidBrush(Color.RoyalBlue),
                PressedBackground = new SolidBrush(Color.DarkBlue),
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
                FocusedBorder = new SolidBrush(Color.DarkViolet),
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
                Background = new SolidBrush(Color.DarkViolet),
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

            //MENU-GRID            
            _grid = new Grid
            {
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines=false,
                //Height=DisplayHeight/2,
                //Width=DisplayWidth/3
                //MAYBE LATER
            };
            
            //START-MENU-GRID
            Grid startgrid = new Grid{
                RowSpacing = 1,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            
            startgrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
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
            //STARTMENU-GRID============================
            MyButton startexit = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Exit","StartExitButton",0,4,(s,a)=>{
                game.Exit();
            },startgrid,MedievalFont,TEXTSIZE);

            MyButton startstartbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Start Game","StartButton",0,0,(s,a)=>{
                gameStateManager.StartNewGame();
                CloseMenu();
            },startgrid,MedievalFont,TEXTSIZE);

            MySpinbutton startNumPlayerSpinButton = new MySpinbutton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,menuStateManager.NUM_PLAYERS,true,"StartNumPlayerSpinButton",0,3,startgrid,(c,a)=>{
                float? nullableFloat = a.NewValue;
                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            });

            //BASEGRID===================================
            //CLOSEBUTTON
            MyButton closebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Exit","ExitButton",0,6,(s,a)=>{
                game.Exit();//HARDCORE CLOSING
            },_grid,MedievalFont,TEXTSIZE);
            
            
            //RELOADBUTTON
            MyButton reloadbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"New Game","ReloadButton",0,4,(s,a)=>{
                gameStateManager.StartNewGame();//RELOADING
                CloseMenu();
            },_grid,MedievalFont,TEXTSIZE);
            
            //RESUMEBUTTON
            MyButton resumebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Resume","ResumeButton",0,0,(s,a) => {
                if(gameStateManager.GameIsOver()){
                    gameStateManager.StartNewGame();
                }
                CloseMenu();
            },_grid,MedievalFont,TEXTSIZE);

            
            //NUM_PLAYERS
            MySpinbutton NumPlayerSpinButton = new MySpinbutton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,menuStateManager.NUM_PLAYERS,true,"NumPlayerSpinButton",0,3,_grid,(c,a) => {
                float? nullableFloat = a.NewValue;
                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            });





            //SETTINGS-SUBMENU
            //shadows, ambient occlusion, fxaa
            SettingsMenu settingsMenu = new SettingsMenu(desktop,_grid,this,MedievalFont,TEXTSIZE);
            //SETTINGS
            MyButton settingsButton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Settings?","SettingsButton",0,5,(s,a)=>{
                menuElements[controllerselectedbutton].UnHighlight();
                insubMenu = true;
                subMenu = settingsMenu;
                menuElements = settingsMenu.Activate(menuElements);
                oldcontrollerselectedbutton=controllerselectedbutton;
                controllerselectedbutton=0;
            },_grid,MedievalFont,TEXTSIZE);

            //MUSIC/SFX-SLIDER
            MyHorizontalSlider Volume = new MyHorizontalSlider(0,100,45,2,2,(s,a)=>{
                MediaPlayer.Volume = a.NewValue*0.01f;
            },_grid);
            MyHorizontalSlider SFXVolume = new MyHorizontalSlider(0,100,100,2,4,(s,a)=>{
                float? nullableFloat = a.NewValue;
                MusicAndSoundEffects.VOLUME = (float)(nullableFloat*0.01f ?? 0.5);
                MusicAndSoundEffects.angrymobInstance.Volume = (float)(nullableFloat*0.001f ?? 0.1f);
            },_grid);
            

            //LABELS
            Label VolumeLabel = new Label{
                Text = "Music:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                //Background = new SolidBrush(Color.Blue),
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Font = MedievalFont.GetFont(TEXTSIZE)
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
                Font = MedievalFont.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(SFXVolumeLabel,2);
            Grid.SetRow(SFXVolumeLabel,3);
            _grid.Widgets.Add(SFXVolumeLabel);

            Label StartNumPlayerLabel = new Label{
                Text = "#Players:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT/2,
                Width = CENTER_BUTTON_WIDTH/2,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Font = MedievalFont.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(StartNumPlayerLabel,0);
            Grid.SetRow(StartNumPlayerLabel,2);
            startgrid.Widgets.Add(StartNumPlayerLabel);//STARTGRID

            Label NumPlayerLabel = new Label{
                Text = "#Players:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT/2,
                Width = CENTER_BUTTON_WIDTH/2,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Font = MedievalFont.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(NumPlayerLabel,0);
            Grid.SetRow(NumPlayerLabel,2);
            _grid.Widgets.Add(NumPlayerLabel);
            
            //TESTING IN PROGRESS
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
            _grid.Widgets.Add(slsfx);*///TESTS END

            //SET DESKTOP FOR STARTMENU
            desktop.Root = startgrid;
            //desktop.Root = grid;

            //ELEMENTÄRÄIs
            menuElements = new MyMenuElement[]{startstartbutton,startNumPlayerSpinButton,startexit};
            basemenuElements = new MyMenuElement[]{resumebutton,NumPlayerSpinButton,reloadbutton,settingsButton,closebutton,Volume,SFXVolume};
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, GamePadState gamePadState, GamePadState previousGamePadState){
            //STARTMENU CHANGE
            if(changegrid){
                desktop.Root = _grid;
                changegrid=false;
                menuElements = basemenuElements;
            }
            

            //OPEN AND CLOSE (SUB)MENU
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

            //CONTROLLER NAVIGATION
            if(menuopen){
                if(!controllerlocked){
                    //EXTRA EXIT WITH B
                    if(gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released){
                        if(insubMenu){
                            CloseSubMenu();
                            menuElements[controllerselectedbutton].Highlight();
                        }else{
                            CloseMenu();
                        }
                    }

                    //ACTUAL NAVIGATION
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
                }else{//IN SUBELEMENT NAVIGATION
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
            //CHANGE CONTROLLERSELECTED BUTTONS TO DEFAULT STYLE
            menuElements[controllerselectedbutton].LeaveButton();
            menuElements[controllerselectedbutton].UnHighlight();
            //CLOSE ALL SUBMENUS
            if(insubMenu){
                CloseSubMenu();
            }
            //RESET CONTROLLER-SELECTION
            controllerselectedbutton=0;
            controllerlocked=false;
            menuopen=false;
            
            //STARTMENU SHENANIGANS
            if(menuStateManager.START_MENU_IS_OPEN){
                menuStateManager.START_MENU_IS_OPEN=false;
                changegrid = true;
            }
        }
        public void CloseSubMenu(){
            //CHANGE CONTROLLERSELECTED BUTTONS TO DEFAULT STYLE
            menuElements[controllerselectedbutton].LeaveButton();
            menuElements[controllerselectedbutton].UnHighlight();

            //CHANGE DESKTOP-GRID
            menuElements = subMenu.DeActivate();

            //REMEMBER OLD CONTROLLER-POSITION
            controllerselectedbutton=oldcontrollerselectedbutton;
            controllerlocked=false;
            insubMenu=false;
        }
        public void OpenMenu(){
            //menuElements[controllerselectedbutton].Highlight(); //SHOULD THE FIRST BUTTON BE HIGHLIGHTED IF WE OPEN THE MENU??? I SAY NO
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
