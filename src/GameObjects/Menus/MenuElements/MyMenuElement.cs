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
    public abstract void Press();
    public abstract void LeaveButton();
    public abstract void ChangeValue();
}