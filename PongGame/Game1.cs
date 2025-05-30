﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PongGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _rectangleTexture;
    private Rectangle _player1, _player2, _ball;
    private Vector2 _ballVelocity;
    private float _player1Velocity = 0f;
    private float _player2Velocity = 0f;
    private const float Acceleration = 0.5f;
    private const float MaxSpeed = 5f;
    private const float Friction = 0.2f;
    private int _scorePlayer1 = 0;
    private int _scorePlayer2 = 0;
    private SpriteFont _font;
    private double _restartTimer = 0;
    private bool _waitingToRestart = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 768;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        int screenWidth = _graphics.PreferredBackBufferWidth;
        int screenHeight = _graphics.PreferredBackBufferHeight;

        int paddleWidth = 20;
        int paddleHeight = 100;
        int ballSize = 20;

        _player1 = new Rectangle(50, (screenHeight - paddleHeight) / 2, paddleWidth, paddleHeight);
        _player2 = new Rectangle(screenWidth - 50 - paddleWidth, (screenHeight - paddleHeight) / 2, paddleWidth, paddleHeight);
        _ball = new Rectangle((screenWidth - ballSize) / 2, (screenHeight - ballSize) / 2, ballSize, ballSize);
        _ballVelocity = new Vector2(4, 4);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Criando uma textura branca 1x1 para desenhar retângulos
        _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
        _rectangleTexture.SetData(new[] { Color.White });
        _font = Content.Load<SpriteFont>("DefaultFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_waitingToRestart)
        {
            _restartTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_restartTimer <= 0)
            {
                _waitingToRestart = false;
                _ballVelocity = new Vector2(
                    4 * (Random.Shared.Next(0, 2) == 0 ? 1 : -1),
                    4 * (Random.Shared.Next(0, 2) == 0 ? 1 : -1)
                );
            }
            return;
        }


        KeyboardState kState = Keyboard.GetState();

        //Movimentação do player 1 (W e S)
        if (kState.IsKeyDown(Keys.W))
            _player1Velocity -= Acceleration;
        else if (kState.IsKeyDown(Keys.S))
            _player1Velocity += Acceleration;
        else
        {
            if (_player1Velocity > 0)
                _player1Velocity -= Friction;
            else if (_player1Velocity < 0)
                _player1Velocity += Friction;
        }

        //Movimentação do player 2 (Setas)
        if (kState.IsKeyDown(Keys.Up))
            _player2Velocity -= Acceleration;
        else if (kState.IsKeyDown(Keys.Down))
            _player2Velocity += Acceleration;
        else
        {
            if (_player2Velocity > 0)
                _player2Velocity -= Friction;
            else if (_player2Velocity < 0)
                _player2Velocity += Friction;
        }

        _player1Velocity = MathHelper.Clamp(_player1Velocity, -MaxSpeed, MaxSpeed);
        _player2Velocity = MathHelper.Clamp(_player2Velocity, -MaxSpeed, MaxSpeed);

        _player1.Y += (int)_player1Velocity;
        _player2.Y += (int)_player2Velocity;

        _player1.Y = MathHelper.Clamp(_player1.Y, 0, _graphics.PreferredBackBufferHeight - _player1.Height);
        _player2.Y = MathHelper.Clamp(_player2.Y, 0, _graphics.PreferredBackBufferHeight - _player2.Height);

        //Atualiza a posição da bola
        _ball.X += (int)_ballVelocity.X;
        _ball.Y += (int)_ballVelocity.Y;

        //Colisão com topo e base da tela

        if (_ball.Y <= 0 || _ball.Y + _ball.Height >= _graphics.PreferredBackBufferHeight)
            _ballVelocity.Y *= -1;

        //Colisão com Raquetes
        if (_ball.Intersects(_player1))
        {
            if (_ballVelocity.X < 0)
            {
                _ball.X = _player1.Right;
                _ballVelocity.X *= -1;

                float relativeY = (_ball.Center.Y - _player1.Center.Y);
                _ballVelocity.Y = relativeY * 0.1f;
            }
        }

        if (_ball.Intersects(_player2))
        {
            if (_ballVelocity.X > 0)
            {
                _ball.X = _player2.Left - _ball.Width;
                _ballVelocity.X *= -1;

                float relativeY = (_ball.Center.Y - _player2.Center.Y);
                _ballVelocity.Y = relativeY * 0.1f;
            }
        }

        if (_ball.X < 0)
        {
            _scorePlayer2++;
            ResetBall();
        }
        else if (_ball.X > _graphics.PreferredBackBufferWidth)
        {
            _scorePlayer1++;
            ResetBall();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        int screenWidth = _graphics.PreferredBackBufferWidth;

        string score1 = _scorePlayer1.ToString();
        string score2 = _scorePlayer2.ToString();

        Vector2 size1 = _font.MeasureString(score1);
        Vector2 size2 = _font.MeasureString(score2);

        GraphicsDevice.Clear(Color.BlueViolet);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, score1, new Vector2(100, 20), Color.White);
        _spriteBatch.DrawString(_font, score2, new Vector2(screenWidth - 100 - size2.X, 20), Color.White);
        _spriteBatch.Draw(_rectangleTexture, _player1, Color.White);
        _spriteBatch.Draw(_rectangleTexture, _player2, Color.White);
        _spriteBatch.Draw(_rectangleTexture, _ball, Color.White);
        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
    private void ResetBall()
    {
        _ball.X = (_graphics.PreferredBackBufferWidth - _ball.Width) / 2;
        _ball.Y = (_graphics.PreferredBackBufferHeight - _ball.Height) / 2;
        _ballVelocity = Vector2.Zero;
        _waitingToRestart = true;
        _restartTimer = 1.0;
    }
}
