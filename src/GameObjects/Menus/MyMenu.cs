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
using Myra.Graphics2D.UI;
//CLASS NOT USED
namespace src.GameObjects{
    public class MyMenu{
        private int controllerselectedbutton;
        private int CENTER_BUTTON_HEIGHT = 40;
        private int CENTER_BUTTON_WIDTH = 250;
        private Desktop desktop;
        private Object[] buttons;
        private bool menuopen=false;
        private bool inspinbutton=false;
        private GamePadState _prevGamePadState;
        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;
        public MyMenu(GameLabGame game){
            controllerselectedbutton=0;
            MyraEnvironment.Game = game;
            gameStateManager = GameStateManager.GetGameStateManager();
            menuStateManager = MenuStateManager.GetMenuStateManager();
            

            //WINDOW
            Window gridWindow = new Window
            {
                Title = "Pause Menu",
                Width = 300,
                Height = 250,
            };
            Grid grid = new Grid
            {
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines=true
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            /*
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
            Button closebutton = new Button{//CLOSES THE GAME
                Width = CENTER_BUTTON_WIDTH,
                Height = CENTER_BUTTON_HEIGHT,
                Content = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Close Game"
                }
            };
            Grid.SetColumn(closebutton,0);
            Grid.SetRow(closebutton,3);
            closebutton.Click += (s,a)=>{
                //var messageBox = Dialog.CreateMessageBox("Error", "Cant resume yet");
                //messageBox.ShowModal(desktop);
                game.Exit();//CLOSING
            };

            grid.Widgets.Add(closebutton);
            //RELOADBUTTON
            Button reloadbutton = new Button{//RELOADS THE GAME
                Width = CENTER_BUTTON_WIDTH,
                Height = CENTER_BUTTON_HEIGHT,
                Content = new Label{
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Reload",
                }
            };
            Grid.SetColumn(reloadbutton,0);
            Grid.SetRow(reloadbutton,2);
            reloadbutton.Click += (s,a)=>{
                gameStateManager.StartNewGame();//RELOADING
                CloseMenu();
            };

            grid.Widgets.Add(reloadbutton);
            //RESUMEBUTTON
            Button resumebutton = new Button{
                Width = CENTER_BUTTON_WIDTH,
                Height = CENTER_BUTTON_HEIGHT,
                Content = new Label{
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Resume"
                }
            };
            Grid.SetColumn(resumebutton,0);
            Grid.SetRow(resumebutton,0);
            resumebutton.Click += (s,a) => {
                CloseMenu();
            };
            grid.Widgets.Add(resumebutton);

            SpinButton spinButton = new SpinButton{
                Width=CENTER_BUTTON_WIDTH,
                Nullable=false,
                Minimum=1,
                Maximum=4,
                Value=menuStateManager.NUM_PLAYERS,
                Integer=true,
            };

            spinButton.ValueChanging += (c,a) => {
                float? nullableFloat = a.NewValue;
                SpinChangesValue(nullableFloat);
            };
            Grid.SetColumn(spinButton,0);
            Grid.SetRow(spinButton,1);

            grid.Widgets.Add(spinButton);

            desktop = new Desktop();
            desktop.Root = grid;

            //SUSSY OBJECT/BUTTON ARRAY
            buttons = new Object[]{resumebutton,spinButton,reloadbutton,closebutton};
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState){
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if(keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape) || (gamePadState.Buttons.Start == ButtonState.Pressed && _prevGamePadState.Buttons.Start == ButtonState.Released)){
                if(menuopen){
                    CloseMenu();
                }else{
                    OpenMenu();
                }
            }
            if(menuopen){
                if(!inspinbutton){
                    if (gamePadState.DPad.Down == ButtonState.Pressed && _prevGamePadState.DPad.Down == ButtonState.Released)
                    {
                        UnHighlight(controllerselectedbutton);
                        controllerselectedbutton = mod(controllerselectedbutton + 1,buttons.Length);
                        Highlight(controllerselectedbutton);
                    }
                    if(gamePadState.DPad.Up == ButtonState.Pressed && _prevGamePadState.DPad.Up == ButtonState.Released){
                        UnHighlight(controllerselectedbutton);
                        controllerselectedbutton = mod(controllerselectedbutton - 1,buttons.Length);
                        Highlight(controllerselectedbutton);
                    }
                    if(gamePadState.Buttons.A == ButtonState.Pressed && _prevGamePadState.Buttons.A == ButtonState.Released){
                        PressHighlighted(controllerselectedbutton);
                    }
                }else{//IN SPINBUTTON
                    if(gamePadState.Buttons.B == ButtonState.Pressed && _prevGamePadState.Buttons.B == ButtonState.Released){
                        LeaveButton(controllerselectedbutton);
                    }
                    if (gamePadState.DPad.Down == ButtonState.Pressed && _prevGamePadState.DPad.Down == ButtonState.Released){
                        Changespinbutton(-1,controllerselectedbutton);
                    }
                    if (gamePadState.DPad.Up == ButtonState.Pressed && _prevGamePadState.DPad.Up == ButtonState.Released){
                        Changespinbutton(1,controllerselectedbutton);
                    }
                }
            }
            
            

            


            
            
            _prevGamePadState = gamePadState;
        }
        public bool menuisopen(){
            return menuopen;
        }
        private void CloseMenu(){
            UnHighlight(controllerselectedbutton);
            LeaveButton(controllerselectedbutton);
            controllerselectedbutton=0;
            menuopen=false;
        }
        public void OpenMenu(){
            Highlight(controllerselectedbutton);
            menuopen=true;
        }
        public void Draw(){
            if(menuopen){
                desktop.Render();
            }
        }
        private void Highlight(int index)
        
        {
            for(int i=0;i<buttons.Length;++i)
            {
                
                if(i==index){
                    Type t = buttons[i].GetType();
                    if(t.Equals(typeof(Button))){
                        //((Button)buttons[i]).SetStyle(Stylesheet.LoadFromSource,"red");
                    ((Button)buttons[i]).Width=100;
                    //((Button)buttons[i]).SetStyle("blue");
                }else if(t.Equals(typeof(SpinButton))){
                    ((SpinButton)buttons[i]).Width=100;
                }
                }
                //i.Background = i == index ? Color.DarkGray : Color.Transparent;
            }
        }
        
        private void UnHighlight(int index){
            Type t = buttons[index].GetType();
            if(t.Equals(typeof(Button))){
                ((Button)buttons[index]).Width=CENTER_BUTTON_WIDTH;
                //((Button)buttons[index]).SetStyle("");
            }else if(t.Equals(typeof(SpinButton))){
                ((SpinButton)buttons[index]).Width=CENTER_BUTTON_WIDTH;
            }
        }
        private void PressHighlighted(int index){
            Type t = buttons[index].GetType();
            if(t.Equals(typeof(Button))){
                ((Button)buttons[index]).DoClick();
            }else if(t.Equals(typeof(SpinButton))){
                if(!inspinbutton){
                    inspinbutton=true;
                    ((SpinButton)buttons[index]).Opacity=0.5f;                   
                }
            }
        }
        private void LeaveButton(int index){
            Type t = buttons[index].GetType();
            if(t.Equals(typeof(SpinButton)) && inspinbutton){
                inspinbutton=false;
                ((SpinButton)buttons[index]).Opacity=1f;
            }
        }
        private void Changespinbutton(int sign, int index){
            Type t = buttons[index].GetType();
            if(t.Equals(typeof(SpinButton)) && inspinbutton && sign+menuStateManager.NUM_PLAYERS>0 && sign+menuStateManager.NUM_PLAYERS<5){
                /*float? v =((SpinButton)buttons[index]).Value;
                float v_int = v ?? 1;
                ((SpinButton)buttons[index]).Value = (float) v_int + sign;*/
                ((SpinButton)buttons[index]).Value += sign;
                SpinChangesValue(((SpinButton)buttons[index]).Value);
            }
        }
        private int mod(int k, int n) {  return ((k %= n) < 0) ? k+n : k;  }
        private void SpinChangesValue(float? nullableFloat){
            menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
            gameStateManager.StartNewGame();
        }
    }
}
