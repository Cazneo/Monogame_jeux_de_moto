//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
//using projet_jeux_moto.Model;

//public class InputService
//{
//    private GameSettingsModel settings;

//    public InputService(GameSettingsModel settings)
//    {
//        this.settings = settings;
//    }

//    public void HandleInput(GameTime gameTime, PlayerModel player)
//    {
//        var keyboardState = Keyboard.GetState();

//        // Update player position based on keyboard input
//        player.Position.X += settings.MotoSpeed *
//            ((keyboardState.IsKeyDown(Keys.Right) ? 1 : 0) -
//            (keyboardState.IsKeyDown(Keys.Left) ? 1 : 0)) *
//            (float)gameTime.ElapsedGameTime.TotalSeconds;

//        // Ensure the player stays within the screen bounds
//        player.Position.X = MathHelper.Clamp(
//            player.Position.X,
//            0,
//            settings.GraphicsDevice.Viewport.Width - player.Width);
//    }
//}
