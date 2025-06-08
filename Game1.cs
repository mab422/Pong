using System.Diagnostics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace Pong


{
    public class Game1 : Core
    {
        Sprite _ball;
        Sprite _computer;
        Sprite _player;

        private int start = 1;
        private Vector2 _ballPosition;
        private Vector2 _ballVelocity;

        private Vector2 newBallPosition;

        private Vector2 _computerPosition;
        private Vector2 _playerPosition;

        private const float PADDLE_SPEED = 4.0f;
        private const float BALL_SPEED = 14.0f;




        private Tilemap _tilemap;
        private Rectangle _roomBounds;

        

        //Rectangle _cRect = new Rectangle(9, 588, 8, 64);
       //Rectangle _pRect = new Rectangle(1262, 588, 8, 64);
       // Rectangle _ball = new Rectangle(636, 356, 8, 8);


        public Game1() : base("Pong", 1280, 720, false)
        {
            
        }

        protected override void Initialize()
        {           // TODO: Add your initialization logic here
            base.Initialize();

            Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;

            _roomBounds = new Rectangle(
                 (int)_tilemap.TileWidth,
                 (int)_tilemap.TileHeight,
                 screenBounds.Width - (int)_tilemap.TileWidth * 2,
                 screenBounds.Height - (int)_tilemap.TileHeight * 2
             );

            int centerRow = _tilemap.Rows / 2;
            int centerColumn = _tilemap.Columns / 2;

            

            _ballPosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);
            
            

            _computerPosition = new Vector2(_tilemap.TileWidth, (_tilemap.TileHeight * centerRow) - 1 );
            

            _playerPosition = new Vector2((_tilemap.Columns * _tilemap.TileWidth) - _tilemap.TileWidth - 40, (_tilemap.TileHeight * centerRow) - 1);

            AssignBallVelocity();
            
        }


        protected override void LoadContent()
        {
            base.LoadContent();
            TextureAtlas atlas = TextureAtlas.FromFile(Content, "Images/atlas-definition.xml");

            _ball = atlas.CreateSprite("ball");
            _ball.Scale = new Vector2(4.0f, 4.0f);

            _player = atlas.CreateSprite("player");
            _player.Scale = new Vector2(4.0f, 4.0f);

            _computer = atlas.CreateSprite("computer");
            _computer.Scale = new Vector2(4.0f, 4.0f);

            // Create the tilemap from the XML configuration file.
            _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
            _tilemap.Scale = new Vector2(4.0f, 4.0f);

            

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            newBallPosition = _ballPosition + _ballVelocity;

           

            // TODO: Add your update logic here
            CheckKeyboardInput();

            CheckBallPosition();

            CheckPaddleBounds();

            CheckBallBounds();

            _ballPosition = newBallPosition;

            
           

            base.Update(gameTime);
        }

        private void CheckPaddleBounds() {
            Rectangle _playerBounds = new Rectangle((int)_playerPosition.X,
                (int)_playerPosition.Y, 
                (int)_player.Width, 
                (int)_player.Height
            );

            Rectangle _computerBounds = new Rectangle(
                (int)_computerPosition.X, 
                (int)_computerPosition.Y, 
                (int)_computer.Width,
                (int)_computer.Height
            );

            if (_playerBounds.Top < _roomBounds.Top)
            {
                _playerPosition.Y = _roomBounds.Top;
            }
            else if (_playerBounds.Bottom > _roomBounds.Bottom)
            {
                _playerPosition.Y = _roomBounds.Bottom - _player.Height;
            }

            if (_computerBounds.Top < _roomBounds.Top)
            {
                _computerPosition.Y = _roomBounds.Top;
            }
            else if (_computerBounds.Bottom > _roomBounds.Bottom) {
                _computerPosition.Y = _roomBounds.Bottom - _computer.Height;
            }

        
        }
        private void CheckBallBounds() {
            Rectangle _ballBounds = new Rectangle(
                (int)_ballPosition.X,
                (int)_ballPosition.Y,
                (int)_ball.Width,
                (int)_ball.Height                
            );

            Rectangle _playerBounds = new Rectangle(
                (int)_playerPosition.X,
                (int)_playerPosition.Y,
                (int)_player.Width,
                (int)_player.Height
            );

            Rectangle _computerBounds = new Rectangle(
                (int)_computerPosition.X,
                (int)_computerPosition.Y,
                (int)_computer.Width,
                (int)_computer.Height
            );

            //Added this to fix the != Zero statement.. otherwise ball would get stuck in a loop... I was too stupid/it was late 
            int paddleCheck = 0;

            Vector2 normal = Vector2.Zero;

            if (_ballBounds.Intersects(_computerBounds))
            {
                paddleCheck++;
                normal.X = Vector2.UnitX.X;                
                _ballVelocity = Vector2.Reflect(_ballVelocity, normal);
                newBallPosition = _ballPosition + _ballVelocity;
            }
            if (_ballBounds.Intersects(_playerBounds))
            {
                if (start == 1)
                {
                    start = 0;
                    paddleCheck++;

                    float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

                    float x = (float)Math.Cos(angle);
                    float y = (float)Math.Sin(angle);
                    Vector2 direction = new Vector2(x, y);

                    _ballVelocity = direction * BALL_SPEED;
                    
                    newBallPosition = _ballPosition + _ballVelocity;
                }
                else
                {
                    paddleCheck++;
                    normal.X = -Vector2.UnitX.X;
                    _ballVelocity = Vector2.Reflect(_ballVelocity, normal);
                    newBallPosition = _ballPosition + _ballVelocity;
                }
            }

            if (_ballBounds.Top < _roomBounds.Top) {
                normal.Y = Vector2.UnitY.Y;
                newBallPosition.Y = _roomBounds.Top;
            }
            else if (_ballBounds.Bottom > _roomBounds.Bottom)
            {
                normal.Y = -Vector2.UnitY.Y;
                newBallPosition.Y = _roomBounds.Bottom - _ball.Height;
            }

            if (normal != Vector2.Zero && paddleCheck == 0)
            {

                _ballVelocity = Vector2.Reflect(_ballVelocity, normal);

            }
            else if (normal != Vector2.Zero && paddleCheck > 0){
                paddleCheck = 0;
            }

            if (_ballBounds.Left < _roomBounds.Left)
            {
                int centerRow = _tilemap.Rows / 2;
                int centerColumn = _tilemap.Columns / 2;
                newBallPosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

            }
            else if (_ballBounds.Right > _roomBounds.Right)
            {
                int centerRow = _tilemap.Rows / 2;
                int centerColumn = _tilemap.Columns / 2;
                newBallPosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

            }


            

        }

        private void CheckBallPosition() {

            if (_ballPosition.Y + (_ball.Height/2) > _computerPosition.Y ) {
                _computerPosition.Y += PADDLE_SPEED;
            }
            if (_ballPosition.Y + (_ball.Height / 2) < (_computerPosition.Y + _computer.Height) ) { 
                _computerPosition.Y -= PADDLE_SPEED;
            }
        
        }

        private void CheckKeyboardInput() {

            float speed = PADDLE_SPEED;

            // If the W or Up keys are down, move the slime up on the screen.
            if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
            {
                _playerPosition.Y -= speed;
            }

            // if the S or Down keys are down, move the slime down on the screen.
            if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
            {
                _playerPosition.Y += speed;
            }

            // If the A or Left keys are down, move the slime left on the screen.
            if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
            {
                
            }

            // If the D or Right keys are down, move the slime right on the screen.
            if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
            {

                
            }
        }

        private void AssignBallVelocity()
        {
            // Generate a random angle
            float angle = (float)(Math.PI * 2);

            // Convert angle to a direction vector
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);

            // Multiply the direction vector by the movement speed
            _ballVelocity = direction * BALL_SPEED;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw the tilemap.
            _tilemap.Draw(SpriteBatch);

            _ball.Draw(SpriteBatch, _ballPosition);

            _computer.Draw(SpriteBatch, _computerPosition);

            _player.Draw(SpriteBatch, _playerPosition);

            SpriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
