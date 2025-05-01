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

namespace src.GameObjects{
    public class MySpinbutton : MyMenuElement{
        private int MINIMUM;
        private int MAXIMUM;
        private bool ISNULLABLE;
        private int StartValue;
        private bool ISINTEGER;
        private string ID;
        private int COLUMN;
        private int ROW;
        private Grid GRID;
        private SpinButton spinbutton;
        public bool controllerselected {get;set;}=false;
        EventHandler<Myra.Events.ValueChangingEventArgs<float?>> Valuechanging;

        public MySpinbutton(int minimum, int maximum, bool isnullable, int startvalue, bool isinteger, string id, int column, int row, Grid grid, EventHandler<Myra.Events.ValueChangingEventArgs<float?>> Valuechanging){
            this.MINIMUM=minimum;
            this.MAXIMUM=maximum;
            this.ISNULLABLE=isnullable;
            this.Valuechanging=Valuechanging;
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
            spinbutton = new SpinButton{
                Nullable=ISNULLABLE,
                Minimum=MINIMUM,
                Maximum=MAXIMUM,
                Value=StartValue,
                Integer=ISINTEGER
            };
            
            spinbutton.ValueChanging += Valuechanging;

            Grid.SetColumn(spinbutton,COLUMN);
            Grid.SetRow(spinbutton,ROW);
            spinbutton.SetStyle("default");
            GRID.Widgets.Add(spinbutton);
        }
        public override void Highlight()
        {
            spinbutton.SetStyle("controller");
        }
        public override void UnHighlight()
        {
            spinbutton.SetStyle("default");
        }
        public override void ControllerValueChange(GamePadState gamePadState, GamePadState previousGamePadState)
        {
            int sign=0;
            if(gamePadState.DPad.Down == ButtonState.Pressed && previousGamePadState.DPad.Down == ButtonState.Released){
                sign=-1;
            }else if(gamePadState.DPad.Up == ButtonState.Pressed && previousGamePadState.DPad.Up == ButtonState.Released){
                sign=1;
            }else{
                return;
            }
            //float? oldValue = spinbutton.Value;
            if(sign+spinbutton.Value<=MAXIMUM && sign+spinbutton.Value>=MINIMUM){
                Valuechanging.Invoke(spinbutton,new ValueChangingEventArgs<float?>(spinbutton.Value,spinbutton.Value+sign));
                spinbutton.Value+=sign;//IDK WHY BUT WE NEED THIS
            }
        }
        public override bool Click()
        {
         controllerselected=true;
         spinbutton.SetStyle("controllerpressed");
         return true;   
        }
        public override bool LeaveButton()
        {
            controllerselected=false;
            spinbutton.SetStyle("controller");
            return true;
        }
    }
}