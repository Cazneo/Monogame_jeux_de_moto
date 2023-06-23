using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace jeux_moto
{
    public enum GameState
    {
        MainMenu,
        Playing,
        HowToPlay,
        HighScore
    }

    public static class SpriteBatchExtensions
    {
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness = 1)
        {
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        }
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState currentGameState;
        Keys pauseKey;
        KeyboardState previousKeyboardState;
        Song backgroundMusic;
        Texture2D motoTexture;
        Vector2 motoPosition;
        float motoSpeed;
        Texture2D background;
        Texture2D carTexture;
        Texture2D howToPlayTexture;
        Texture2D highScoresTexture;
        Texture2D mainTexture;
        List<Vector2> carPositions;
        List<float> carSpeeds;
        List<float> carMoveDirections;
        float carSpeedMultiplier;
        float timeSinceLastCar;
        float carSpawnInterval;
        float carSpeed;
        Random rnd = new Random();
        int score;
        float distance;
        List<int> highScores = new List<int>();
        bool saveSuccessful;
        float saveSuccessfulTime;

        bool isPaused;

        SpriteFont font;

        public static Texture2D PixelTexture { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            currentGameState = GameState.MainMenu;
            isPaused = false;
            pauseKey = Keys.P;
        }
        //
        protected override void Initialize()
        {
            if (File.Exists("savegame.json"))
            {
                LoadGame("savegame.json");
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("road");
            motoTexture = Content.Load<Texture2D>("moto");
            carTexture = Content.Load<Texture2D>("car");
            font = Content.Load<SpriteFont>("ScoreFont2");
            howToPlayTexture = Content.Load<Texture2D>("background_Howplay");
            highScoresTexture = Content.Load<Texture2D>("background_HighScore");
            mainTexture = Content.Load<Texture2D>("background_Main");
            backgroundMusic = Content.Load<Song>("Musique");

            float motoScale = 128f / motoTexture.Height;
            motoTexture = ResizeTexture(motoTexture, motoScale);

            float carScale = 128f / carTexture.Height;
            carTexture = ResizeTexture(carTexture, carScale);

            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            PixelTexture.SetData(new[] { Color.White });

            InitializeGame();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        currentGameState = GameState.Playing;
                        InitializeGame();
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.L))
                    {
                        LoadGame("savegame.json");
                        currentGameState = GameState.Playing;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    {
                        Exit();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.H))
                    {
                        currentGameState = GameState.HowToPlay;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.I))
                    {
                        currentGameState = GameState.HighScore;
                    }
                    break;

                case GameState.Playing:
                    if (Keyboard.GetState().IsKeyDown(pauseKey) && previousKeyboardState.IsKeyUp(pauseKey))
                    {
                        isPaused = !isPaused;
                    }

                    if (isPaused && Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        SaveGame("savegame.json");
                        saveSuccessful = true;
                        saveSuccessfulTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Back))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    if (!isPaused)
                    {
                        UpdateGame(gameTime);
                    }
                    break;
                case GameState.HowToPlay:
                    if (Keyboard.GetState().IsKeyDown(Keys.Back))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    break;
                case GameState.HighScore:
                    if (Keyboard.GetState().IsKeyDown(Keys.Back))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    break;


            }

            previousKeyboardState = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();



            switch (currentGameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(mainTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); 
                    spriteBatch.DrawString(font, "Press SPACE to start a new game\nPress L to load the last saved game\nPress H to How to play\nPress I to HighScore\nPress Q to quit", new Vector2(200, 200), Color.Black);
                    break;
                case GameState.Playing:
                    spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(motoTexture, motoPosition, null, Color.White, 0f, new Vector2(0, motoTexture.Height / 2), 1f, SpriteEffects.None, 0f);
                    for (int i = 0; i < carPositions.Count; i++)
                    {
                        spriteBatch.Draw(carTexture, carPositions[i], null, Color.White, 0f, new Vector2(carTexture.Width / 2, carTexture.Height / 2), 1f, SpriteEffects.None, 0f);
                    }
                    spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(50, 50), Color.Black);
                    spriteBatch.DrawString(font, "Distance: " + Math.Round(distance, 2).ToString() + " m", new Vector2(50, 100), Color.Black);
                    if (isPaused)
                    {
                        spriteBatch.DrawString(font, "Game Paused - Press 'P' to continue\nPress 'S' to save game\nPress 'Return' to Main menu", new Vector2(200, 200), Color.Black);
                        if (saveSuccessful)
                        {
                            spriteBatch.DrawString(font, "Save Successful!", new Vector2(10, Window.ClientBounds.Height - 30), Color.Black);
                        }
                    }
                    break;
                case GameState.HowToPlay:
                    spriteBatch.Draw(howToPlayTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    break;
                case GameState.HighScore:
                    spriteBatch.Draw(highScoresTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    for (int i = 0; i < highScores.Count; i++)
                    {
                        
                        spriteBatch.DrawString(font, $"Score {i + 1}: {highScores[i]}", new Vector2(200, 100 + i * 30), Color.Black);
                    }
                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitializeGame()
        {
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            motoPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height * 0.8f);
            motoSpeed = 500f;

            carPositions = new List<Vector2>();
            carSpeeds = new List<float>();
            carMoveDirections = new List<float>();
            carSpeedMultiplier = 1f;
            timeSinceLastCar = 0;
            carSpawnInterval = 2f;
            carSpeed = 900f;

            score = 0;
            distance = 0;

            saveSuccessful = false;
        }

        private void UpdateGame(GameTime gameTime)
        {
            motoPosition.X += motoSpeed * ((Keyboard.GetState().IsKeyDown(Keys.Right) ? 1 : 0) - (Keyboard.GetState().IsKeyDown(Keys.Left) ? 1 : 0)) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            motoPosition.X = MathHelper.Clamp(motoPosition.X, 0, GraphicsDevice.Viewport.Width - motoTexture.Width);

            distance += (float)gameTime.ElapsedGameTime.TotalSeconds * carSpeedMultiplier * 1f;

            timeSinceLastCar += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastCar >= carSpawnInterval)
            {
                SpawnCar();
                timeSinceLastCar = 0;
            }

            for (int i = 0; i < carPositions.Count; i++)
            {
                carPositions[i] += new Vector2(0, carSpeeds[i]) * (float)gameTime.ElapsedGameTime.TotalSeconds * carSpeedMultiplier;
                if (carPositions[i].Y > GraphicsDevice.Viewport.Height)
                {
                    carPositions.RemoveAt(i);
                    carSpeeds.RemoveAt(i);
                    carMoveDirections.RemoveAt(i);
                    score += 3;
                }
            }

            Rectangle motoRect = new Rectangle((int)motoPosition.X, (int)motoPosition.Y, motoTexture.Width, motoTexture.Height);
            for (int i = 0; i < carPositions.Count; i++)
            {
                Rectangle carRect = new Rectangle((int)carPositions[i].X - carTexture.Width / 2, (int)carPositions[i].Y - carTexture.Height / 2, carTexture.Width, carTexture.Height);
                if (motoRect.Intersects(carRect))
                {
                    UpdateHighScores(score);
                    SaveGame("savegame.json");
                    currentGameState = GameState.MainMenu;
                }

            }

            if (saveSuccessful && gameTime.TotalGameTime.TotalSeconds - saveSuccessfulTime > 2)
            {
                saveSuccessful = false;
            }
        }

        private void SpawnCar()
        {
            float y = -carTexture.Height;
            float x = (float)(rnd.NextDouble() * (GraphicsDevice.Viewport.Width - carTexture.Width));
            carPositions.Add(new Vector2(x, y));
            carSpeeds.Add(carSpeed);
            carMoveDirections.Add(0);
        }

        private Texture2D ResizeTexture(Texture2D texture, float scale)
        {
            var scaledWidth = (int)(texture.Width * scale);
            var scaledHeight = (int)(texture.Height * scale);

            var renderTarget = new RenderTarget2D(graphics.GraphicsDevice, scaledWidth, scaledHeight);

            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(0, 0, scaledWidth, scaledHeight), Color.White);
            spriteBatch.End();

            graphics.GraphicsDevice.SetRenderTarget(null);

            return (Texture2D)renderTarget;
        }

        private void SaveGame(string filename)
        {
            var data = new
            {
                MotoPosition = motoPosition,
                MotoSpeed = motoSpeed,
                CarPositions = carPositions,
                CarSpeeds = carSpeeds,
                CarMoveDirections = carMoveDirections,
                CarSpeedMultiplier = carSpeedMultiplier,
                TimeSinceLastCar = timeSinceLastCar,
                CarSpawnInterval = carSpawnInterval,
                CarSpeed = carSpeed,
                Score = score,
                Distance = distance,
                HighScores = highScores // ajoutez cette ligne
            };

            string json = JsonConvert.SerializeObject(data);

            File.WriteAllText(filename, json);
        }
        private void UpdateHighScores(int newScore)
        {

            highScores.Add(newScore);


            highScores = highScores.OrderByDescending(score => score).Take(10).ToList();
        }


        private void LoadGame(string filename)
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);

                var data = JsonConvert.DeserializeObject<dynamic>(json);

                if (data != null)
                {
                    motoPosition = data.MotoPosition == null ? new Vector2() : data.MotoPosition.ToObject<Vector2>();
                    motoSpeed = data.MotoSpeed == null ? 0 : data.MotoSpeed.ToObject<float>();
                    carPositions = data.CarPositions == null ? new List<Vector2>() : data.CarPositions.ToObject<List<Vector2>>();
                    carSpeeds = data.CarSpeeds == null ? new List<float>() : data.CarSpeeds.ToObject<List<float>>();
                    carMoveDirections = data.CarMoveDirections == null ? new List<float>() : data.CarMoveDirections.ToObject<List<float>>();
                    carSpeedMultiplier = data.CarSpeedMultiplier == null ? 0 : data.CarSpeedMultiplier.ToObject<float>();
                    timeSinceLastCar = data.TimeSinceLastCar == null ? 0 : data.TimeSinceLastCar.ToObject<float>();
                    carSpawnInterval = data.CarSpawnInterval == null ? 0 : data.CarSpawnInterval.ToObject<float>();
                    carSpeed = data.CarSpeed == null ? 0 : data.CarSpeed.ToObject<float>();
                    score = data.Score == null ? 0 : data.Score.ToObject<int>();
                    distance = data.Distance == null ? 0 : data.Distance.ToObject<float>();
                    highScores = data.HighScores == null ? new List<int>() : data.HighScores.ToObject<List<int>>(); // ajoutez cette ligne
                }
                else
                {
                    InitializeGame();
                }
            }
            else
            {
                InitializeGame();
            }
        }


        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}