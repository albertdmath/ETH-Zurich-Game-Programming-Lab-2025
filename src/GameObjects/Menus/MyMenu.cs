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
        private int CENTER_BUTTON_WIDTH = 400;
        private Desktop desktop;
        private Grid PauseGrid;
        private Grid MainMenuGrid;
        private MyMenuElement[] activeElements;
        private MyMenuElement[] pauseElements;
        private MyMenuElement[] mainElements;
        private bool menuopen=true;
        private bool controllerlocked = false;
        private bool insubMenu = false;
        SubMenu subMenu;
        EndMenu endMenu;
        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;
        public MyMenu(GameLabGame game, int DisplayWidth, int DisplayHeight){
            desktop = new Desktop();
            FontSystem MedievalFont = new FontSystem();
            byte[] ttfData = File.ReadAllBytes("../../../Content/OldLondon.ttf");
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
                Background = new SolidBrush(Color.Transparent),
                Width = CENTER_BUTTON_WIDTH/2,
                Height = CENTER_BUTTON_HEIGHT,
                OverBackground = new SolidBrush(Color.Red),
                DisabledBackground = new SolidBrush(Color.DarkGray),
                Border = new SolidBrush(Color.White),
                BorderThickness = new Myra.Graphics2D.Thickness{
                    Top=2,Left=2,Bottom=2,Right=2
                },
                TextBoxStyle = new TextBoxStyle{
                    Font = MedievalFont.GetFont(TEXTSIZE-5),
                    TextColor = Color.Black,
                    MessageFont = MedievalFont.GetFont(TEXTSIZE),
                    MaxHeight=100,
                    Height=CENTER_BUTTON_HEIGHT,
                },
                UpButtonStyle = new ImageButtonStyle{
                    LabelStyle = new LabelStyle{
                        
                        
                    }
                }
            };
            SpinButtonStyle ControllerPressedSpinbuttonStyle = new SpinButtonStyle{
                Background = new SolidBrush(Color.LightGray),
                Width = CENTER_BUTTON_WIDTH/2,
                //Height = CENTER_BUTTON_HEIGHT
            };





            ButtonStyle ControllerButtonStyle = new ButtonStyle{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.Transparent),
                
                Border = new SolidBrush(Color.Black),
                BorderThickness = new Thickness{
                    Left=5,Right=5,Top=5,Bottom=5
                },
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH
            };
            ButtonStyle DefaultButtonStyle = new ButtonStyle{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.Transparent),
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
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
            PauseGrid = new Grid
            {
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines=false,
                //Height=DisplayHeight/2,
                //Width=DisplayWidth/3
                //MAYBE LATER
            };

            //SETTINGS-SUBMENU
            //shadows, ambient occlusion, fxaa
            SettingsMenu settingsMenu = new SettingsMenu(desktop,PauseGrid,this,MedievalFont,TEXTSIZE);
            
            
            //MAIN-MENU-GRID
            MainMenuGrid = new Grid{
                RowSpacing = 1,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };

            PauseGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            PauseGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            
            MainMenuGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            MainMenuGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            
            
            //MainMenu-GRID-CONTENT============================
            MyButton MainMenuExit = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Exit",0,3,(s,a)=>{
                game.Exit();
            },MainMenuGrid,MedievalFont,TEXTSIZE);

            MyButton MainMenuStart = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Start Game",0,0,(s,a)=>{
                gameStateManager.StartNewGame();
                CloseMenu();
            },MainMenuGrid,MedievalFont,TEXTSIZE);

            MyButton MainSettings = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Settings",0,1,(s,a)=>{
                activeElements[controllerselectedbutton].UnHighlight();
                insubMenu = true;
                subMenu = settingsMenu;
                activeElements = settingsMenu.Activate(activeElements);
                oldcontrollerselectedbutton=controllerselectedbutton;
                controllerselectedbutton=0;
            },MainMenuGrid,MedievalFont,TEXTSIZE);

            MyButton MainTutorial = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Tutorial",0,2,(s,a)=>{
                //TUTORIAL-CALL
                menuStateManager.TUTORIAL_IS_OPEN=true;
            },MainMenuGrid,MedievalFont,TEXTSIZE);

            //BASEGRID-CONTENT===================================
            //CLOSEBUTTON
            MyButton closebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Close Game",0,4,(s,a)=>{
                game.Exit();//HARDCORE CLOSING
            },PauseGrid,MedievalFont,TEXTSIZE);
            //BACK TO MAIN MENU
            MyButton BacktoMainMenu = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Main Menu",0,3,(s,a)=>{
                menuStateManager.MAIN_MENU_IS_OPEN=true;
                activeElements[controllerselectedbutton].LeaveButton();
                activeElements[controllerselectedbutton].UnHighlight();

                controllerselectedbutton=0;
                controllerlocked=false;

                activeElements = mainElements;

                desktop.Root = MainMenuGrid;
                menuStateManager.PAUSE_MENU_IS_OPEN = false;
                menuStateManager.TRANSITION = false;
                menuStateManager.COUNTDOWN = false;
                MusicAndSoundEffects.playMainMenuMusic();
                gameStateManager.StartNewGame();
            },PauseGrid,MedievalFont,TEXTSIZE);
            
            
            //RELOADBUTTON
            MyButton reloadbutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Restart",0,1,(s,a)=>{
                menuStateManager.TRANSITION = true;
                MusicAndSoundEffects.LAST_VOLUME = MediaPlayer.Volume;
                gameStateManager.StartNewGame();//RELOADING
                CloseMenu();
            },PauseGrid,MedievalFont,TEXTSIZE);
            
            //RESUMEBUTTON
            MyButton resumebutton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Resume",0,0,(s,a) => {
                if(gameStateManager.GameIsOver()){
                    menuStateManager.COUNTDOWN = false;
                    gameStateManager.StartNewGame();
                }
                CloseMenu();
            },PauseGrid,MedievalFont,TEXTSIZE);

            
            //NUM_PLAYERS
            /*MySpinbutton NumPlayerSpinButton = new MySpinbutton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,menuStateManager.MIN_NUM_PLAYER,menuStateManager.MAX_NUM_PLAYER,false,menuStateManager.NUM_PLAYERS,true,"NumPlayerSpinButton",0,3,PauseGrid,(c,a) => {
                float? nullableFloat = a.NewValue;
                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            });*/


            //SETTINGS
            MyButton settingsButton = new MyButton(CENTER_BUTTON_WIDTH,CENTER_BUTTON_HEIGHT,"Settings",0,2,(s,a)=>{
                activeElements[controllerselectedbutton].UnHighlight();
                insubMenu = true;
                subMenu = settingsMenu;
                activeElements = settingsMenu.Activate(activeElements);
                oldcontrollerselectedbutton=controllerselectedbutton;
                controllerselectedbutton=0;
            },PauseGrid,MedievalFont,TEXTSIZE);

            //MUSIC/SFX-SLIDER
            /*
            MyHorizontalSlider Volume = new MyHorizontalSlider(0,100,45,2,2,(s,a)=>{
                MediaPlayer.Volume = a.NewValue*0.01f;
            },PauseGrid);
            MyHorizontalSlider SFXVolume = new MyHorizontalSlider(0,100,100,2,4,(s,a)=>{
                float? nullableFloat = a.NewValue;
                MusicAndSoundEffects.VOLUME = (float)(nullableFloat*0.01f ?? 0.5);
                MusicAndSoundEffects.angrymobInstance.Volume = (float)(nullableFloat*0.001f ?? 0.1f);
            },PauseGrid);*/
            

            //LABELS
            /*
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
            PauseGrid.Widgets.Add(VolumeLabel);

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
            PauseGrid.Widgets.Add(SFXVolumeLabel);*/

            
/*
            Label NumPlayerLabel = new Label{
                Text = "#Players:",
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness{Top=8,Bottom=5},
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Font = MedievalFont.GetFont(TEXTSIZE)
            };
            Grid.SetColumn(NumPlayerLabel,0);
            Grid.SetRow(NumPlayerLabel,2);
            PauseGrid.Widgets.Add(NumPlayerLabel);*/
            

            /*VerticalStackPanel panel = new VerticalStackPanel();

            CheckButton c1 = new CheckButton{Content = new Label{
                Text = "option"
            }};
            CheckButton c2 = new CheckButton{Content = new Label{
                Text = "option1"
            }};
            CheckButton c3 = new CheckButton{Content = new Label{
                Text = "option2",
                TextColor = Color.Coral,
                DisabledTextColor = Color.White,
            }};
            List<CheckButton> radioGroup = new List<CheckButton>{c1,c2,c3};

            foreach(CheckButton cb in radioGroup){
                cb.PressedChanged += (s,a)=>{
                    if(cb.IsChecked){
                        foreach(CheckButton other in radioGroup){
                            if(other!=cb){
                                other.IsChecked = false;
                            }
                        }
                    }else{
                        bool test = false;
                        foreach(CheckButton cb in radioGroup){
                            test = cb.IsChecked || test;
                        }
                        if(!test){
                            cb.IsChecked=true;
                        }
                    }
                };
                panel.Widgets.Add(cb);
            }
            Grid.SetColumn(panel,4);
            PauseGrid.Widgets.Add(panel);*/
/*
            ButtonStyle TestStyle = new ButtonStyle{
                Background = new SolidBrush(Color.Transparent),
                OverBackground = new SolidBrush(Color.Transparent),
                PressedBackground = new SolidBrush(Color.SeaGreen),
                DisabledBackground = new SolidBrush(Color.Gray),
                
                Height = CENTER_BUTTON_HEIGHT,
                Width = CENTER_BUTTON_WIDTH,
                Border = new SolidBrush(Color.Black),
                OverBorder = new SolidBrush(Color.White),
                BorderThickness = new Thickness{
                    Top = 5,Left=5,Right=5,Bottom=5
                },
            };

            Stylesheet.Current.ButtonStyles["samurai"] = TestStyle;


            Button b = new Button{
                Visible=true
            };
            b.Content = new Label{
                Text = "SAM",
                Font = MedievalFont.GetFont(TEXTSIZE),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            b.SetStyle("samurai");
            Grid.SetColumn(b,3);
            Grid.SetRow(b,0);
            PauseGrid.Widgets.Add(b);
            b.MouseEntered +=(s,a)=>{
                b.Content = new Label{
                Text = "SAM",
                Font = MedievalFont.GetFont(TEXTSIZE),
                TextColor = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            };

            b.MouseLeft +=(s,a)=>{
                b.Content = new Label{
                Text = "SAM",
                Font = MedievalFont.GetFont(TEXTSIZE),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            };*/

            //SET DESKTOP FOR STARTMENU
            desktop.Root = MainMenuGrid;
            //desktop.Root = grid;

            //ELEMENTÄRÄIs
            mainElements = new MyMenuElement[]{MainMenuStart,MainSettings,MainTutorial,MainMenuExit};
            pauseElements = new MyMenuElement[]{resumebutton,reloadbutton,settingsButton,BacktoMainMenu,closebutton};
            activeElements = mainElements;
            endMenu = new EndMenu(desktop,MainMenuGrid,this,MedievalFont,TEXTSIZE,mainElements);
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, GamePadState gamePadState, GamePadState previousGamePadState){
            //STARTMENU CHANGE

            if(changegrid){
                desktop.Root = PauseGrid;
                changegrid=false;
                activeElements = pauseElements;
            }

            if((Grid) desktop.Root == PauseGrid && menuopen){
                menuStateManager.MAIN_MENU_IS_OPEN = false;
                menuStateManager.PAUSE_MENU_IS_OPEN = true;
            }

            
            if(menuStateManager.TUTORIAL_IS_OPEN && ((keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)) || (gamePadState.Buttons.Start == ButtonState.Pressed && previousGamePadState.Buttons.Start == ButtonState.Released)))
            {
                menuStateManager.TUTORIAL_IS_OPEN = false;
                
            } else if((keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)) || (gamePadState.Buttons.Start == ButtonState.Pressed && previousGamePadState.Buttons.Start == ButtonState.Released)){
                if(menuopen){
                    if(keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape) && insubMenu){
                        CloseSubMenu();
                    }else{
                        if(menuStateManager.MAIN_MENU_IS_OPEN){
                            return;
                        }
                        if(endMenu.Isopen()){
                            endMenu.Close();
                        }else{

                            CloseMenu();
                        }
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
                        if(menuStateManager.TUTORIAL_IS_OPEN){
                            menuStateManager.TUTORIAL_IS_OPEN = false;
                        }else if(insubMenu){
                            CloseSubMenu();
                            activeElements[controllerselectedbutton].Highlight();
                        }else{
                            if(endMenu.Isopen()){
                            endMenu.Close();
                        }else{
                            CloseMenu();
                        }
                        }
                    }

                    //ACTUAL NAVIGATION
                    if (gamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released)
                    {
                        activeElements[controllerselectedbutton].UnHighlight();
                        controllerselectedbutton = mod(controllerselectedbutton + 1,activeElements.Length);
                        activeElements[controllerselectedbutton].Highlight();
                    }
                    if(gamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released){
                        activeElements[controllerselectedbutton].UnHighlight();
                        controllerselectedbutton = mod(controllerselectedbutton - 1,activeElements.Length);
                        activeElements[controllerselectedbutton].Highlight();
                    }
                    if(gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released){
                        controllerlocked = activeElements[controllerselectedbutton].Click();
                    }
                }else{//IN SUBELEMENT NAVIGATION
                    if(gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released){
                        controllerlocked = !activeElements[controllerselectedbutton].LeaveButton();
                    }else if (gamePadState.DPad.Down == ButtonState.Pressed || gamePadState.DPad.Up == ButtonState.Pressed || gamePadState.DPad.Left == ButtonState.Pressed || gamePadState.DPad.Right == ButtonState.Pressed){
                        activeElements[controllerselectedbutton].ControllerValueChange(gamePadState, previousGamePadState);
                    }
                }
            }

        }
        public bool menuisopen(){
            return menuopen;
        }
        public void CloseMenu(){
            //menuStateManager.TUTORIAL_IS_OPEN=false;
            //CHANGE CONTROLLERSELECTED BUTTONS TO DEFAULT STYLE
            activeElements[controllerselectedbutton].LeaveButton();
            activeElements[controllerselectedbutton].UnHighlight();

            if(menuStateManager.PAUSE_MENU_IS_OPEN){
                menuStateManager.PAUSE_MENU_IS_OPEN = false;
            }
            //CLOSE ALL SUBMENUS
            if(insubMenu){
                CloseSubMenu();
            }
            menuStateManager.ONWIN=false;
            //RESET CONTROLLER-SELECTION
            controllerselectedbutton=0;
            controllerlocked=false;
            menuopen=false;
            
            //STARTMENU SHENANIGANS
            if(menuStateManager.MAIN_MENU_IS_OPEN){
                menuStateManager.MAIN_MENU_IS_OPEN=false;
                menuStateManager.TRANSITION = true;
                MusicAndSoundEffects.LAST_VOLUME = MediaPlayer.Volume;
                changegrid = true;
            }
        }
        public void CloseEndMenu(){
            activeElements[controllerselectedbutton].LeaveButton();
            activeElements[controllerselectedbutton].UnHighlight();

            activeElements = endMenu.DeActivate();

            controllerselectedbutton=0;
            controllerlocked=false;
            insubMenu=false;
        }
        public void CloseSubMenu(){
            //CHANGE CONTROLLERSELECTED BUTTONS TO DEFAULT STYLE
            activeElements[controllerselectedbutton].LeaveButton();
            activeElements[controllerselectedbutton].UnHighlight();

            //CHANGE DESKTOP-GRID
            activeElements = subMenu.DeActivate();

            //REMEMBER OLD CONTROLLER-POSITION
            controllerselectedbutton=oldcontrollerselectedbutton;
            controllerlocked=false;
            insubMenu=false;
        }
        public void OpenMenu(){
            //activeElements[controllerselectedbutton].Highlight(); //SHOULD THE FIRST BUTTON BE HIGHLIGHTED IF WE OPEN THE MENU??? I SAY NO
            menuopen=true;
        }
        public void OpenWinMenu(){
            menuopen=true;
            //activeElements[controllerselectedbutton].UnHighlight();
                
                activeElements = endMenu.Activate(activeElements);
                controllerselectedbutton=0;
                oldcontrollerselectedbutton=controllerselectedbutton;
                activeElements[controllerselectedbutton].UnHighlight();
        }
        public void Draw(){
            if(menuopen){
                desktop.Render();
            }
        }
        
        
        
        
        
        
        private int mod(int k, int n) {  return ((k %= n) < 0) ? k+n : k;  }
        public Grid ToMainMenu(){
            return MainMenuGrid;
        }
        public void setMenuElements(MyMenuElement[] m){
            activeElements = m;
        }
    }
}
