using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS50.Pong
{
    class FpsGameComponent : DrawableGameComponent
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly ScreenParameters _screenParameters;
        private readonly Vector2 _position;

        public FpsGameComponent(Game game, SpriteFont font, ScreenParameters screenParameters) : base(game)
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _font = font ?? throw new ArgumentNullException(nameof(font));
            _screenParameters = screenParameters ?? throw new ArgumentNullException(nameof(screenParameters));

            var template = "FPS: ###";
            var size = _font.MeasureString(template);
            _position = new Vector2(_screenParameters.ScreenRectangle.Right - size.X, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"FPS: {MathF.Round(1000f / (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0)}", _position, Color.Green);
            _spriteBatch.End();
        }
    }
}