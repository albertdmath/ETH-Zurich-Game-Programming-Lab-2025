using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using FontStashSharp;
using System.Linq;

namespace src.GameObjects;
    public class ProjectileMenu : SubMenu{
        private readonly FontSystem fontSystem;
        private readonly int TEXTSIZE;
        private readonly Container scrollContainer;
                
        public ProjectileMenu(Desktop desktop, Grid r, MyMenu p, FontSystem fontSystem, int textsize):base(desktop,r,p)
        {

            this.fontSystem=fontSystem;
            TEXTSIZE=textsize/2;
            _grid = new Grid
            {
                RowSpacing = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = false
            };
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var list = Projectile.ProjectileProbability.ToList();
            int n = list.Count;
            menuElements = new MyHorizontalSlider[n];

            for (int i = 0; i < n; i++)
            {
                var (key, value) = list[i];
                
                Label label = new Label{
                    Text =  $"{key}:",
                    TextColor = Color.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness{Top=8,Bottom=5},
                    Height = CENTER_BUTTON_HEIGHT,
                    Width = CENTER_BUTTON_WIDTH,
                    TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                    
                    Font = fontSystem.GetFont(TEXTSIZE)
                };

                Grid.SetColumn(label,0);
                Grid.SetRow(label, i);

                _grid.Widgets.Add(label);

                menuElements[i] = new MyHorizontalSlider(0,1000,(int)(value*1000),1,i,(s,a)=>{
                    Projectile.ProjectileProbability[key] = a.NewValue*0.001f;
                },_grid);
            }

            ScrollViewer scrollViewer = new ScrollViewer
            {
                Content = _grid,
                ShowVerticalScrollBar = false,  // This is the Myra-specific property
                ShowHorizontalScrollBar = false,
                Height = 5 * (CENTER_BUTTON_HEIGHT + 5) // Show 5 items at once
            };

            // Create a container with top margin
            this.scrollContainer = new Panel
            {
                Widgets = { scrollViewer },
                Padding = new Thickness(0, 400, 0, 0) // Adjust Y_OFFSET as needed
            };

        }


        public override MyMenuElement[] Activate(MyMenuElement[] R)
        {
            returnGrid = (Grid)desktop.Root;
            oldMenuElements = R;
            desktop.Root = _grid; // Use the scrollViewer instead of grid directly
            menuopen = true;
            return menuElements;
        }

        public override MyMenuElement[] DeActivate()
        {
            desktop.Root = returnGrid;
            menuopen=false;
            return oldMenuElements;
        }
        
    }
