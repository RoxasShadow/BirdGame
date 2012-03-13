using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace WindowsGame3 {
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont statusFont, alertFont;
        private Song theme;
        private Color color;
        private Hashtable db, objects;
        private bool gameover, pause, mute;
        private List<Sprite> sprite, shot, enemies;
        private KeyboardState oldKeyState;
        private MouseState oldMouseState;
        private int time, lifes, score, scoreJump, num_enemies;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            color = Color.Black;
            gameover = true;
            pause = false;
            mute = true;
            time = score = 10;
            lifes = 3;
            scoreJump = 10;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true; // It has to be chosen by the player

            Hashtable text = new Hashtable();
            text.Add("time", "Time: ");
            text.Add("lifes", "Lifes: ");
            text.Add("score", "Score: ");
            text.Add("gameover", "GAME OVAH");
            text.Add("welcome", "SAVE => S" + Environment.NewLine + "LOAD => L" + Environment.NewLine + "DELETE => D" + Environment.NewLine + "MOVE => MOUSE" + Environment.NewLine + "PAUSE => ENTER" + Environment.NewLine + "RESTART => RIGHT CTRL || MOUSE SCROLL WHEEL" + Environment.NewLine + "(UN)MUTE => LEFT CTRL" + Environment.NewLine + "QUIT => ESC" + Environment.NewLine + Environment.NewLine + "PRESS ENTER TO START." + Environment.NewLine + "(C) 2012 - Giovanni Capuano");

            Hashtable resources = new Hashtable();
            resources.Add("player", "Sprites\\angry");
            resources.Add("enemy", "Sprites\\angry");
            resources.Add("shot", "Sprites\\shot");
            resources.Add("volumeOn", "Sprites\\volumeOn");
            resources.Add("volumeOff", "Sprites\\volumeOff");
            resources.Add("sky", "Backgrounds\\bg1");
            resources.Add("theme1", "Music\\theme1");

            db = new Hashtable();
            db.Add("text", text);
            db.Add("resources", resources);
        }

        // TODO: use this.Content to load your game content here
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            theme = Content.Load<Song>(GetResource("resources", "theme1"));
            statusFont = Content.Load<SpriteFont>("spritefont1");
            alertFont = Content.Load<SpriteFont>("spritefont2");
            num_enemies = GetHeight() / Content.Load<Texture2D>(GetResource("resources", "enemy")).Height;

            sprite = new List<Sprite>();
            enemies = new List<Sprite>();
            shot = new List<Sprite>();
            objects = new Hashtable();
        }

        // TODO: Add your initialization logic here
        protected override void Initialize() {
            base.Initialize();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(theme);
            sprite.Add(new Sprite(10, GetHeight() / 2, true, Content.Load<Texture2D>(GetResource("resources", "player"))));
            objects.Add("volume", new Sprite(100, 8, true, Content.Load<Texture2D>(GetResource("resources", "volumeOn"))));
        }

        // TODO: Unload any non ContentManager content here
        protected override void UnloadContent() {
        }

        // TODO: Add your update logic here
        protected override void Update(GameTime gameTime) {
            if(!mute)
                if(pause)
                    MediaPlayer.Pause();
                else if(gameover)
                    MediaPlayer.Stop();
                else
                    MediaPlayer.Resume();
            else
                MediaPlayer.Pause();

            KeyboardState newKeyState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if(newKeyState.IsKeyDown(Keys.Escape) && oldKeyState.IsKeyUp(Keys.Escape))
                this.Exit();

            if(newKeyState.IsKeyDown(Keys.S) && oldKeyState.IsKeyUp(Keys.S))
                DataHandling.Save("game.sav", GetSaveGameData());
            if(newKeyState.IsKeyDown(Keys.L) && oldKeyState.IsKeyUp(Keys.L))
                if(DataHandling.Exists("game.sav"))
                    SetSaveGameData((SaveGameData)DataHandling.Load("game.sav"));
            if(newKeyState.IsKeyDown(Keys.D) && oldKeyState.IsKeyUp(Keys.D))
                DataHandling.Delete("game.sav");

            if((newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released) || (newKeyState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter)))
                if(gameover)
                    Restart();
                else
                    pause = !pause;

            if(newKeyState.IsKeyDown(Keys.LeftControl) && oldKeyState.IsKeyUp(Keys.LeftControl)) {
                mute = !mute;
                ((Sprite)objects["volume"]).sprite = Content.Load<Texture2D>(GetResource("resources", mute ? "volumeOff" : "volumeOn"));
                MediaPlayer.Pause();
            }

            if((newMouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton == ButtonState.Released) || (newKeyState.IsKeyDown(Keys.RightControl) && oldKeyState.IsKeyUp(Keys.RightControl)))
                Restart();

            if(lifes == 0)
                gameover = true;
            if(!pause && !gameover) {
                if(enemies.Count(item => item.visible) <= num_enemies) {
                    Sprite enemy = new Sprite(GetWidth(), new Random().Next(20, GetHeight() - 50), true, Content.Load<Texture2D>(GetResource("resources", "enemy")));
                    int counter = 0;
                    do {
                        foreach(Sprite e in enemies) {
                            enemy = new Sprite(GetWidth(), new Random().Next(20, GetHeight() - 50), true, Content.Load<Texture2D>(GetResource("resources", "enemy")));
                            if(Utils.IsIntersect(e, enemy))
                                break;
                        }
                        ++counter;
                    } while(enemies.Count > counter);
                    enemies.Add(enemy);
                }
                foreach(Sprite e in enemies) {
                    e.x -= e.speed;
                    if(e.x == 0)
                        e.visible = false;
                }
                foreach(Sprite s in shot)
                    foreach(Sprite e in enemies)
                        if(Utils.IsIntersect(s, e)) {
                            e.visible = false;
                            s.visible = false;
                            score += scoreJump;
                        }
                foreach(Sprite s in sprite)
                    foreach(Sprite e in enemies)
                        if(Utils.IsIntersect(s, e)) {
                            --lifes;
                            e.visible = false;
                            break;
                        }

                if(newMouseState.Y >= 20 && newMouseState.Y <= GetHeight() - 50)
                    sprite[0].y = newMouseState.Y;
                if(newMouseState.X >= 0 && newMouseState.X <= GetWidth() - 50)
                    sprite[0].x = newMouseState.X;
                
                if((newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released) || (newKeyState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space)))
                    shot.Add(new Sprite(sprite[0].x, sprite[0].y+10, true, Content.Load<Texture2D>(GetResource("resources", "shot")), 3));
                foreach(Sprite s in shot) {
                    if(s.visible && s.x > GetWidth()) {
                        s.visible = false;
                        s.x = 0;
                        s.y = 0;
                    }
                    if(s.visible)
                        s.x += s.speed;
                }
                shot.RemoveAll(item => !item.visible); // I think It's slow. Make something like a counter.
                ++time;
            }
            oldKeyState = newKeyState;
            oldMouseState = newMouseState;
            base.Update(gameTime);
        }

        // TODO: Add your drawing code here
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if(gameover && lifes == 0) {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(alertFont, GetResource("text", "gameover"), new Vector2(0, GetHeight() / 2), Color.White);
            }
            else if(gameover && lifes == 3) {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(statusFont, GetResource("text", "welcome"), new Vector2(GetWidth()/2, GetHeight() / 2), Color.White);
            }
            else {
                spriteBatch.Draw(Content.Load<Texture2D>(GetResource("resources", "sky")), Vector2.Zero, Color.White);
                using(Sprite volume = (Sprite)objects["volume"])
                    spriteBatch.Draw(volume.sprite, new Vector2(volume.x, volume.y), Color.White);
                spriteBatch.DrawString(statusFont, GetResource("text", "time") + time, new Vector2(10, 10), color);
                spriteBatch.DrawString(statusFont, GetResource("text", "lifes") + lifes, new Vector2(GetWidth() - 100, 10), color);
                spriteBatch.DrawString(statusFont, GetResource("text", "score") + score, new Vector2(GetWidth() - 200, 10), color);
                spriteBatch.Draw(sprite[0].sprite, new Vector2(sprite[0].x, sprite[0].y), Color.White);
                foreach(Sprite s in shot)
                    if(s.visible)
                        spriteBatch.Draw(s.sprite, new Vector2(s.x, s.y), Color.White);
                foreach(Sprite e in enemies)
                    if(e.visible)
                        spriteBatch.Draw(e.sprite, new Vector2(e.x, e.y), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public string GetResource(string cat, string elm) {
            return (string)(((Hashtable)db[cat])[elm]);
        }

        public void Restart() {
            gameover = pause = false;
            time = score = 10;
            lifes = 3;
            scoreJump = 10;
            sprite[0].x = 10;
            sprite[0].y = GetHeight() / 2;
            enemies.Clear();
            shot.Clear();
            MediaPlayer.Stop();
        }

        public int GetWidth() {
            return graphics.PreferredBackBufferWidth;
        }
        
        public int GetHeight() {
            return graphics.PreferredBackBufferHeight;
        }

        /* Sprite cannot be serialized beacuse Texture2D is not serializable! */
        public SaveGameData GetSaveGameData() {
            SaveGameData data = new SaveGameData();
            /*data.sprite = sprite;
            data.shot = shot;
            data.enemies = enemies;*/
            data.time = time;
            data.lifes = lifes;
            data.score = score;
            return data;
        }

        public void SetSaveGameData(SaveGameData data) {
            /*sprite = data.sprite;
            shot = data.shot;
            enemies = data.enemies;*/
            time = data.time;
            lifes = data.lifes;
            score = data.score;
        }

        [Serializable]
        public struct SaveGameData {
            //public List<Sprite> sprite, shot, enemies;
            public int time, lifes, score;
        }
    }
}
