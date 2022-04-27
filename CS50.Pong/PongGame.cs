using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace CS50.Pong
{
    public class PongGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        private SpriteFont? _joystixSmallFont;
        private SoundEffect _paddleHitSound;
        private SoundEffect _wallHitSound;
        private SoundEffect _pointSound;
        private ScreenParameters? _screenParameters;

        private const int WINDOW_WIDTH = 1280;
        private const int WINDOW_HEIGHT = 720;
        private const int WINDOW_VIRTUAL_WIDTH = 432;
        private const int WINDOW_VIRTUAL_HEIGHT = 243;
        private const int PADDLE_SPEED = 200;
        private const int PADDLE_WIDTH = 5;
        private const int PADDLE_HEIGHT = 20;
        private const int BALL_WIDTH = 4;
        private const int BALL_HEIGHT = 4;
        private const float BALL_SPEED = 100f;
        private const int WIN_SCORE = 10;
        private const float BALL_SPEED_MULTIPLIER = 1.05f;

        private RenderTarget2D? _virtualRenderTarget;

        private Color _backgroundColor;

        private int _player1Score = 0;
        private int _player2Score = 0;

        private Paddle _player1Paddle;
        private Paddle _player2Paddle;
        private Ball _ball;

        private Random? _random;
        private Vector2 _ballStartingPosition;

        private GameState _gameState;

        private FpsGameComponent _fpsGameComponent;

        public PongGame() : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content/bin";
        }

        protected override void Initialize()
        {
            _screenParameters = new ScreenParameters(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_VIRTUAL_WIDTH, WINDOW_VIRTUAL_HEIGHT);

            _graphics.PreferredBackBufferWidth = _screenParameters.RealWidth;
            _graphics.PreferredBackBufferHeight = _screenParameters.RealHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _virtualRenderTarget = new RenderTarget2D(GraphicsDevice, _screenParameters.VirtualWidth, _screenParameters.VirtualHeight);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundColor = new Color(40, 45, 52, 255);

            Components.Add(new InputHandler(this));

            _player1Paddle = new Paddle(
                PADDLE_WIDTH,
                PADDLE_HEIGHT,
                new Vector2(PADDLE_WIDTH, _screenParameters.VirtualHeight / 2),
                PADDLE_SPEED);

            _player2Paddle = new Paddle(
                PADDLE_WIDTH,
                PADDLE_HEIGHT,
                new Vector2(_screenParameters.VirtualWidth - PADDLE_WIDTH, _screenParameters.VirtualHeight / 2),
                PADDLE_SPEED);

            _random = new Random();

            _ballStartingPosition = new Vector2(_screenParameters.VirtualWidth / 2 - BALL_WIDTH / 2, _screenParameters.VirtualHeight / 2 - BALL_HEIGHT);

            _ball = new Ball(
                BALL_WIDTH,
                BALL_HEIGHT,
                _ballStartingPosition,
                GetRandomBallVelocity());

            _gameState = GameState.Start;

            base.Initialize();
        }

        private Vector2 GetRandomBallVelocity()
        {
            return new Vector2(_random.Next(2) == 0 ? BALL_SPEED : -BALL_SPEED, (float)_random.Next(-50, 51));
        }

        protected override void LoadContent()
        {
            _joystixSmallFont = Content.Load<SpriteFont>("Fonts/joystix");
            _paddleHitSound = Content.Load<SoundEffect>("Sounds/Hit_Hurt");
            _wallHitSound = Content.Load<SoundEffect>("Sounds/Hit_Hurt5");
            _pointSound = Content.Load<SoundEffect>("Sounds/Powerup");

            _fpsGameComponent = new FpsGameComponent(this, _joystixSmallFont, _screenParameters);

            Components.Add(_fpsGameComponent);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }
            else if (InputHandler.IsKeyPressed(Keys.Enter))
            {
                switch (_gameState)
                {
                    case GameState.Start:
                        {
                            _gameState = GameState.Play;

                            break;
                        }
                    case GameState.Win:
                        {
                            _gameState = GameState.Start;

                            break;
                        }
                    case GameState.Play:
                        {
                            ResetGame();

                            break;
                        }
                }
            }

            float deltaTime = CalculateDeltaTime(gameTime);

            ReadInputAndUpdatePaddles(deltaTime);

            _player1Paddle.Update(deltaTime);
            _player2Paddle.Update(deltaTime);

            switch (_gameState)
            {
                case GameState.Play:
                    {
                        _ball.Update(deltaTime);

                        break;
                    }
            }

            if (_ball.IsCollide(_player1Paddle))
            {
                _ball.Velocity = new Vector2(-_ball.Velocity.X * BALL_SPEED_MULTIPLIER, _ball.Velocity.Y < 0 ? -(float)_random.Next(10,100) : _random.Next(10,100));
                _ball.Position = new Vector2(_ball.Position.X + (_ball.Position.X - _ball.Width / 2 - _player1Paddle.Position.X + _player1Paddle.Width / 2), _ball.Position.Y);

                _paddleHitSound.Play();
            }
            else if (_ball.IsCollide(_player2Paddle))
            {
                _ball.Velocity = new Vector2(-_ball.Velocity.X * BALL_SPEED_MULTIPLIER, _ball.Velocity.Y < 0 ? -(float)_random.Next(10, 150) : _random.Next(10, 150));
                _ball.Position = new Vector2(_ball.Position.X - ((_ball.Position.X + _ball.Width / 2) - (_player2Paddle.Position.X - _player2Paddle.Width / 2)), _ball.Position.Y);

                _paddleHitSound.Play();
            }

            if (_ball.AABB.Top <= 0)
            {
                _ball.Position = new Vector2(_ball.Position.X, _ball.Width / 2);
                _ball.Velocity *= new Vector2(1, -1);

                _wallHitSound.Play();
            }
            else if (_ball.AABB.Bottom >= _screenParameters.VirtualHeight)
            {
                _ball.Position = new Vector2(_ball.Position.X, _screenParameters.VirtualHeight - _ball.Width / 2);
                _ball.Velocity *= new Vector2(1, -1);

                _wallHitSound.Play();
            }

            if (_ball.Position.X <= 0)
            {
                ResetBall();

                _player2Score++;

                _pointSound.Play();
            }
            else if (_ball.Position.X >= _screenParameters.VirtualWidth)
            {
                ResetBall();

                _player1Score++;

                _pointSound.Play();
            }

            if (_player1Score == WIN_SCORE
                || _player2Score == WIN_SCORE)
            {
                _gameState = GameState.Win;
            }

            base.Update(gameTime);
        }

        private void ResetBall()
        {
            ResetBallPosition();
            _ball.Velocity = GetRandomBallVelocity();
        }

        private void ResetGame()
        {
            ResetBallPosition();

            _player1Score = 0;
            _player2Score = 0;

            _gameState = GameState.Start;
        }

        private static float CalculateDeltaTime(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;
        }

        private void ResetBallPosition()
        {
            _ball.Position = _ballStartingPosition;
            _ball.Velocity = GetRandomBallVelocity();
        }

        private void ReadInputAndUpdatePaddles(float deltaTime)
        {
            if (InputHandler.IsKeyDown(Keys.W))
            {
                _player1Paddle.Position = new Vector2(
                    _player1Paddle.Position.X,
                    MathF.Max(PADDLE_HEIGHT / 2, _player1Paddle.Position.Y + -PADDLE_SPEED * deltaTime));
            }
            else if (InputHandler.IsKeyDown(Keys.S))
            {
                _player1Paddle.Position = new Vector2(
                    _player1Paddle.Position.X,
                    MathF.Min(_screenParameters.VirtualHeight - PADDLE_HEIGHT / 2, _player1Paddle.Position.Y + PADDLE_SPEED * deltaTime));
            }

            if (InputHandler.IsKeyDown(Keys.Up))
            {
                _player2Paddle.Position = new Vector2(
                    _player2Paddle.Position.X,
                    MathF.Max(PADDLE_HEIGHT / 2, _player2Paddle.Position.Y + -PADDLE_SPEED * deltaTime));
            }
            else if (InputHandler.IsKeyDown(Keys.Down))
            {
                _player2Paddle.Position = new Vector2(
                    _player2Paddle.Position.X,
                    MathF.Min(_screenParameters.VirtualHeight - PADDLE_HEIGHT / 2, _player2Paddle.Position.Y + PADDLE_SPEED * deltaTime));
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_spriteBatch is null)
            {
                throw new InvalidOperationException($"{nameof(_spriteBatch)} shouldn't be null");
            }

            if (_screenParameters is null)
            {
                throw new InvalidOperationException($"{nameof(_screenParameters)} shouldn't be null");
            }

            GraphicsDevice.SetRenderTarget(_virtualRenderTarget);
            GraphicsDevice.Clear(_backgroundColor);

            _spriteBatch.Begin();

            DrawPlayer1Paddle();
            DrawPlayer2Paddle();
            DrawBall();
            DrawScoreText(_joystixSmallFont);

            if (_gameState == GameState.Win)
            {
                DrawWinnerText(_joystixSmallFont);
            }

            _spriteBatch.End();

            DrawToScreen();

            base.Draw(gameTime);
        }

        private void DrawScoreText(SpriteFont font)
        {
            var player1ScoreScreenSize = font.MeasureString(_player1Score.ToString());
            var player2ScoreScreenSize = font.MeasureString(_player2Score.ToString());
            _spriteBatch.DrawString(font, _player1Score.ToString(), new Vector2(WINDOW_VIRTUAL_WIDTH / 2 - player1ScoreScreenSize.X * 2, WINDOW_VIRTUAL_HEIGHT / 3), Color.White);
            _spriteBatch.DrawString(font, _player2Score.ToString(), new Vector2(WINDOW_VIRTUAL_WIDTH / 2 + player2ScoreScreenSize.X, WINDOW_VIRTUAL_HEIGHT / 3), Color.White);
        }

        private void DrawWinnerText(SpriteFont font)
        {
            var winnerText = _player1Score == WIN_SCORE
                ? "Player 1 is WINNER!"
                : "Player 2 is WINNER!";

            var textScreenSize = font.MeasureString(winnerText);

            _spriteBatch.DrawString(font, winnerText, new Vector2(_screenParameters.VirtualWidth / 2 - textScreenSize.X / 2, _screenParameters.VirtualWidth / 4), Color.White);
        }

        private void DrawToScreen()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_virtualRenderTarget, _screenParameters.ScreenRectangle, Color.White);
            _spriteBatch.End();
        }

        private void DrawBall()
        {
            _spriteBatch.DrawRectangle(
                _ball.Position.X,
                _ball.Position.Y,
                _ball.Width,
                _ball.Height,
                Color.White,
                1);
        }

        private void DrawPlayer1Paddle()
        {
            _spriteBatch.DrawRectangle(
                _player1Paddle.Position.X - PADDLE_WIDTH / 2,
                _player1Paddle.Position.Y - PADDLE_HEIGHT / 2,
                _player1Paddle.Width,
                _player1Paddle.Height,
                Color.White,
                1);
        }

        private void DrawPlayer2Paddle()
        {
            _spriteBatch.DrawRectangle(
                _player2Paddle.Position.X - PADDLE_WIDTH / 2,
                _player2Paddle.Position.Y - PADDLE_HEIGHT / 2,
                _player2Paddle.Width,
                _player2Paddle.Height,
                Color.White,
                1);
        }
    }
}