using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;
using GameLab;
using System.Collections.Generic;
//CLASS NOT USED
namespace src.GameObjects{
    public class MyMenu{
        int CENTER_BUTTON_HEIGHT = 40;
        int CENTER_BUTTON_WIDTH = 250;
        private Desktop desktop;
        LinkedList<Button> buttons;
        private bool menuopen=false;

        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;
        public MyMenu(GameLabGame game){
            buttons = new LinkedList<Button>();
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
            buttons.AddLast(closebutton);

            Button reloadbutton = new Button{//RELOADS THE GAME
                Width = CENTER_BUTTON_WIDTH,
                Height = CENTER_BUTTON_HEIGHT,
                Content = new Label{
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Reload"
                }
            };
            Grid.SetColumn(reloadbutton,0);
            Grid.SetRow(reloadbutton,2);
            reloadbutton.Click += (s,a)=>{
                gameStateManager.StartNewGame();//RELOADING
                this.menuopen=false;
            };

            grid.Widgets.Add(reloadbutton);
            buttons.AddLast(reloadbutton);

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
                this.menuopen=!menuopen;
            };
            grid.Widgets.Add(resumebutton);
            buttons.AddLast(resumebutton);

            var spinButton = new SpinButton{
                Width=CENTER_BUTTON_WIDTH,
                Nullable=false,
                Minimum=1,
                Maximum=4,
                Value=menuStateManager.NUM_PLAYERS,
                Integer=true
            };

            spinButton.ValueChanging += (c,a) => {
                float? nullableFloat = a.NewValue;

                menuStateManager.NUM_PLAYERS = (int)(nullableFloat ?? 1);
                gameStateManager.StartNewGame();
            };
            Grid.SetColumn(spinButton,0);
            Grid.SetRow(spinButton,1);

            grid.Widgets.Add(spinButton);

            desktop = new Desktop();
            desktop.Root = grid;
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState){
            if(keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)){
                menuopen=!menuopen;
            }
        }
        public bool menuisopen(){
            return menuopen;
        }
        public void Draw(){
            if(menuopen){
                desktop.Render();
            }
        }
    }
}
