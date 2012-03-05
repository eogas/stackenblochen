using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace stackenblochen
{
	public class StackenBlochen : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Block CurrentBlock;
		List<Nibbit> LockedNibbits;
		KeyboardState lastKBState;

		double lastSeconds = 0;
		double dropDelay = 1;

		public StackenBlochen()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE;
			graphics.PreferredBackBufferHeight = Constants.PLAYFIELD_HEIGHT * Constants.NIBBIT_SIZE;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			LockedNibbits = new List<Nibbit>();
			lastKBState = Keyboard.GetState();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			CurrentBlock = new Block(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
		}

		private bool KeyPressed(Keys key)
		{
			return lastKBState.IsKeyUp(key) && Keyboard.GetState().IsKeyDown(key);
		}

		private bool KeyReleased(Keys key)
		{
			return lastKBState.IsKeyDown(key) && Keyboard.GetState().IsKeyUp(key);
		}

		private bool ValidPosition(Block b)
		{
			// test against locked nibbits
			foreach (Nibbit n in LockedNibbits)
			{
				if (b.Contains(n))
					return false;
			}

			// Test against edges
			return b.InBounds();
		}

		protected override void Update(GameTime gameTime)
		{
			if (KeyPressed(Keys.Left))
			{
				Block newBlock = new Block(CurrentBlock);
				newBlock.Translate(new Point(-1, 0));

				if (ValidPosition(newBlock))
					CurrentBlock = newBlock;
			}
			
			if (KeyPressed(Keys.Right))
			{
				Block newBlock = new Block(CurrentBlock);
				newBlock.Translate(new Point(1, 0));

				if (ValidPosition(newBlock))
					CurrentBlock = newBlock;
			}
			
			if (KeyPressed(Keys.Up))
			{
				Block newBlock = new Block(CurrentBlock);
				newBlock.Rotate(true);

				if (ValidPosition(newBlock))
					CurrentBlock = newBlock;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Down))
				dropDelay = 0.125;
			else
				dropDelay = 1;

			lastKBState = Keyboard.GetState();

			if (lastSeconds >= dropDelay)
			{
				Block newBlock = new Block(CurrentBlock);
				newBlock.Translate(new Point(0, 1));

				// lock block
				if (!newBlock.AboveGround() || !ValidPosition(newBlock))
				{
					LockedNibbits.AddRange(CurrentBlock.EmancipateNibbits());
					CurrentBlock = new Block(GraphicsDevice);
				}
				else
					CurrentBlock = newBlock;

				lastSeconds = 0;
			}

			lastSeconds += gameTime.ElapsedGameTime.TotalSeconds;
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			CurrentBlock.Draw();

			foreach (Nibbit n in LockedNibbits)
				n.Draw(Point.Zero);

			base.Draw(gameTime);
		}
	}
}
