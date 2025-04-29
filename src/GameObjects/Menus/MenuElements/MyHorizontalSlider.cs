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

namespace src.GameObjects{
    public class MyHorizontalSlider : MyMenuElement{
        private bool controllerselected = false;
        private int MINIMUM;
        private int MAXIMUM;
        private int VALUE;
        private Grid grid;
        private EventHandler<Myra.Events.ValueChangedEventArgs<float>> CHANGE;
        private HorizontalSlider slider;
        public MyHorizontalSlider(int min, int max, int value, int column, int row, EventHandler<Myra.Events.ValueChangedEventArgs<float>> CHANGE, Grid grid){
            MINIMUM = min;
            MAXIMUM = max;
            if(value<=MAXIMUM && value>=MINIMUM){
                VALUE=value;
            }else{
                VALUE=MINIMUM;
            }
            this.grid=grid;

            slider = new HorizontalSlider{
                Minimum=MINIMUM,
                Maximum=MAXIMUM,
                Value=VALUE,
                
            };
            this.CHANGE=CHANGE;
            slider.ValueChanged += CHANGE;
            Grid.SetColumn(slider,column);
            Grid.SetRow(slider,row);
            slider.SetStyle("default");
            grid.Widgets.Add(slider);
        }
        public override void Highlight()
        {
            slider.SetStyle("controller");
        }
        public override bool Click()
        {
            controllerselected = true;
            slider.SetStyle("controllerpressed");
            return true;
        }
        public override void ControllerValueChange(int sign)
        {
            if(sign+slider.Value<=MAXIMUM && sign+slider.Value>=MINIMUM){

                CHANGE.Invoke(slider,new ValueChangedEventArgs<float>(slider.Value,slider.Value+sign));
                slider.Value+=sign;
            }
        }
        public override bool LeaveButton()
        {
            controllerselected = false;
            slider.SetStyle("controller");
            return true;
        }
        public override void UnHighlight()
        {
            slider.SetStyle("default");
        }
    }
}