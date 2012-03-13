using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame3 {
    [Serializable]
    public class Sprite : IDisposable {
        public int x { get; set; }
        public int y { get; set; }
        public bool visible { get; set; }
        public Texture2D sprite { get; set; }
        public int speed { get; set; }

        public Sprite(int x, int y, bool visible, Texture2D sprite) {
            this.x = x;
            this.y = y;
            this.visible = visible;
            this.sprite = sprite;
            speed = 2;
        }

        public Sprite(int x, int y, bool visible, Texture2D sprite, int speed) {
            this.x = x;
            this.y = y;
            this.visible = visible;
            this.sprite = sprite;
            this.speed = speed;
        }

        void IDisposable.Dispose() { }
    }
}
