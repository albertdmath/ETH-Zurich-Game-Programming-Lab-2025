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
        private MenuStateManager menuStateManager;
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
            GRID.Widgets.Add(spinbutton);
            spinbutton.SetStyle("default");
        }
        public override void Highlight()
        {
            spinbutton.SetStyle("controller");
        }
        public override void UnHighlight()
        {
            spinbutton.SetStyle("default");
        }
        public override void ControllerValueChange(int sign)
        {
            
            //float? oldValue = spinbutton.Value;
            if(sign+spinbutton.Value<=MAXIMUM && sign+spinbutton.Value>=MINIMUM){
                Valuechanging.Invoke(spinbutton,new ValueChangingEventArgs<float?>(spinbutton.Value,spinbutton.Value+sign));
                //spinbutton.Value+=sign;
            }
        }
        public override bool Click()
        {
         controllerselected=true;
         spinbutton.SetStyle("controllerpressed");
         return true;   
        }
        public override void LeaveButton()
        {
            controllerselected=false;
            spinbutton.SetStyle("controller");
        }
    }
}