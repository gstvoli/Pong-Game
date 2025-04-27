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

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _player1 = new Rectangle(50, 200, 20, 100);
        _player2 = new Rectangle(730, 200, 20, 100);
        _ball = new Rectangle(390, 240, 20, 20);
        _ballVelocity = new Vector2(4, 4);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        // Criando uma textura branca 1x1 para desenhar retângulos
        _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
        _rectangleTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        KeyboardState kState = Keyboard.GetState();

        //Movimentação do player 1 (W e S)
        if (kState.IsKeyDown(Keys.W) && _player1.Y > 0)
            _player1.Y -= 5;

        if (kState.IsKeyDown(Keys.S) && (_player1.Y + _player1.Height) < _graphics.PreferredBackBufferHeight)
            _player1.Y += 5;

        //Movimentação do player 2 (Setas)
        if (kState.IsKeyDown(Keys.Up) && _player2.Y > 0)
            _player2.Y -= 5;
        if (kState.IsKeyDown(Keys.Down) && (_player2.Y + _player2.Height) < _graphics.PreferredBackBufferHeight)
            _player2.Y += 5;

        //Atualiza a posição da bola
        _ball.X += (int)_ballVelocity.X;
        _ball.Y += (int)_ballVelocity.Y;

        //Colisão com topo e base da tela

        if (_ball.Y <= 0 || _ball.Y + _ball.Height >= _graphics.PreferredBackBufferHeight)
            _ballVelocity.Y *= -1;

        //Colisão com Raquetes
        if (_ball.Intersects(_player1) || _ball.Intersects(_player2))
            _ballVelocity.X *= -1;

        //Reset da bola (fora da tela)
        if (_ball.X <= 0 || _ball.X > _graphics.PreferredBackBufferWidth)
        {
            _ball.X = 390;
            _ball.Y = 240;
            _ballVelocity = new Vector2(4, 4);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.BlueViolet);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_rectangleTexture, _player1, Color.White);
        _spriteBatch.Draw(_rectangleTexture, _player2, Color.White);
        _spriteBatch.Draw(_rectangleTexture, _ball, Color.White);
        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
