using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using GameLab;
using System;
using Myra.Attributes;

public abstract class MyMenuElement{
    public abstract void Highlight();
    public abstract void UnHighlight();
    public abstract bool Click();//return true if goes down into subelement
    public abstract bool LeaveButton();//return true if leaves subelement
    public abstract void ControllerValueChange(int sign);
}