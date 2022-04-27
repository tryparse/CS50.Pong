using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace CS50.Pong
{
    class Paddle
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }

        private RectangleF _aabb;
        public RectangleF AABB => _aabb;

        public Paddle(int width, int height, Vector2 position, float speed)
        {
            Width = width;
            Height = height;
            Position = position;
            Speed = speed;

            _aabb = new RectangleF(position.X - width / 2, position.Y - height / 2, width, height);
        }

        public void Update(float deltaTime)
        {
            UpdateAABB();
        }

        private void UpdateAABB()
        {
            _aabb.X = Position.X - Width / 2;
            _aabb.Y = Position.Y - Height / 2;
        }
    }
}