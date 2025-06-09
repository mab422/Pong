using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonoGameLibrary.Graphics;
using MonoGameLibrary;
using System.Drawing;
using System.Reflection.Metadata;

namespace Pong.Mechanics
{
    internal class Ball
    {
        Circle _bounds;

        Vector2 _position;
        Vector2 _velocity;

        int _centerRow;
        int _centerColumn;

        Sprite _ball;

        private Tilemap _tilemap;

        Rectangle _roomBounds;



        private const float speed = 4.0f;

        private Vector2 scale = new Vector2(4.0f, 4.0f);

        public Ball(Tilemap _tilemap, Rectangle _roombounds, Sprite _ball) {

            this._tilemap = _tilemap;
            this._roomBounds = _roombounds;
            this._centerRow = _tilemap.Rows /2;
            this._centerColumn = _tilemap.Columns / 2;
            this._position = new Vector2(_centerColumn * _tilemap.TileWidth, _centerRow * _tilemap.TileHeight);
            this._ball = _ball;
            

            

        }

        



    }
}
