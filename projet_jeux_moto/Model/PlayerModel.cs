using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace projet_jeux_moto.Model
{
    public class PlayerModel
    {
        public Vector2 MotoPosition { get; set; }
        public float MotoSpeed { get; set; }
        public float MotoDistance { get; set;}
        public float MotoTime { get; set;}
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Texture2D Texture { get; set; }
    }

}
    


