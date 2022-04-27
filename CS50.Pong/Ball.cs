using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace CS50.Pong
{
    class Ball
    {
        public int Width { get; }
        public int Height { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        private RectangleF _aabb;
        public RectangleF AABB => _aabb;

        public Ball(int width, int height, Vector2 position, Vector2 velocity)
        {
            Width = width;
            Height = height;
            Position = position;
            Velocity = velocity;

            _aabb = new RectangleF(position.X - width / 2, position.Y - height / 2, width, height);
        }

        public void Update(float deltaTime)
        {
            UpdatePosition(deltaTime);
            UpdateAABB();
        }

        private void UpdateAABB()
        {
            _aabb.X = Position.X - Width / 2;
            _aabb.Y = Position.Y - Height / 2;
        }

        private void UpdatePosition(float deltaTime)
        {
            Position += Velocity * deltaTime;
        }

        public bool IsCollide(Paddle paddle)
        {
            return _aabb.Intersects(paddle.AABB);
        }
    }
}