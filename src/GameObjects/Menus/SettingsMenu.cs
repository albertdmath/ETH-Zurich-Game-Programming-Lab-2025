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

namespace src.GameObjects{
    public class SettingsMenu : SubMenu{
        public SettingsMenu(Desktop desktop):base(desktop)
        {
            
        }
        public override void ControllerNavigate(){
            return;
        }
        public override void Activate()
        {
            throw new NotImplementedException();
        }
    }
}