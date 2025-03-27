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
//CLASS NOT USED
namespace src.GameObjects{
    public class MyMenu{

        private Desktop desktop;
        Button closebutton;
        Button reloadbutton;
        private bool menuopen=false;
        private GraphicsDeviceManager graphics;
        public MyMenu(GraphicsDeviceManager _graphics, GameLabGame game){
            MyraEnvironment.Game = game;

            var grid = new Grid{
                RowSpacing = 8,
                ColumnSpacing = 8
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

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

            closebutton = new Button{//CLOSES THE GAME
                Width = 100,
                Height = 30,
                Content = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Close Game"
                }
            };
            Grid.SetColumn(closebutton,8);
            Grid.SetRow(closebutton,8);
            closebutton.Click += (s,a)=>{
                //var messageBox = Dialog.CreateMessageBox("Error", "Cant resume yet");
                //messageBox.ShowModal(desktop);
                game.Exit();//CLOSING
            };

            grid.Widgets.Add(closebutton);

            reloadbutton = new Button{//RELOADS THE GAME
                Width = 100,
                Height = 30,
                Content = new Label{
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Reload"
                }
            };
            Grid.SetColumn(reloadbutton,7);
            Grid.SetRow(reloadbutton,7);
            reloadbutton.Click += (s,a)=>{
                game.ReLoad();//RELOADING
                this.menuopen=false;
            };

            grid.Widgets.Add(reloadbutton);

            var spinButton = new SpinButton{
                Width=100,
                Nullable=true
            };
            Grid.SetColumn(spinButton,2);
            Grid.SetRow(spinButton,2);

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
