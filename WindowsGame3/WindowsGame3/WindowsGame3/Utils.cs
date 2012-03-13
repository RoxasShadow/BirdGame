using System;
using Microsoft.Xna.Framework;

namespace WindowsGame3 {
    public class Utils {
        public static Color GetRandomColor() {
            Random rand = new Random();
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }

        public static bool GoEnemies() {
            return new Random().Next(100) > 70;
        }

        public static bool IsIntersect(Sprite s1, Sprite s2) {
            Rectangle rect1 = new Rectangle(s1.x, s1.y, s1.sprite.Width, s1.sprite.Height);
            Rectangle rect2 = new Rectangle(s2.x, s2.y, s2.sprite.Width, s2.sprite.Height);
            if(!s1.visible || !s2.visible)
                return false;
            return rect1.Intersects(rect2);
        }
    }
}
