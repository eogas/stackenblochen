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
		List<int> RowsToDrop;
		KeyboardState lastKBState;

		double lastSeconds = 0;
		double dropDelay = 0.5;
		int currentScore = 0;
		int[] scores = new int[4] { 100, 300, 500, 800 };

		SpriteFont scoreFont;
		Texture2D midpointLine;

		public StackenBlochen()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 2 * Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE;
			graphics.PreferredBackBufferHeight = Constants.PLAYFIELD_HEIGHT * Constants.NIBBIT_SIZE;
			this.IsFixedTimeStep = false;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			LockedNibbits = new List<Nibbit>();
			lastKBState = Keyboard.GetState();
			RowsToDrop = new List<int>();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			midpointLine = new Texture2D(GraphicsDevice, 1, 1);
			midpointLine.SetData(new Color[] { Color.Black });
			scoreFont = Content.Load<SpriteFont>("Consolas");
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

		// TODO: Make this faster
		private void RemoveRows()
		{
			bool fullRow;
			for (int y = Constants.PLAYFIELD_HEIGHT - 1; y >= 0; y--)
			{
				fullRow = true;
				for (int x = 0; x < Constants.PLAYFIELD_WIDTH; x++)
				{
					Nibbit temp = new Nibbit(Color.White, new Point(x, y), GraphicsDevice);
					if (!LockedNibbits.Contains(temp))
					{
						fullRow = false;
						break;
					}
				}

				if (fullRow)
				{
					// Lambda expression bitches!  Remove all Nibbets in row y!
					LockedNibbits.RemoveAll(n => n.CompareRow(y) == 0);
					RowsToDrop.Insert(0, y);
				}
			}
		}

		private void DropRows()
		{
			foreach (int row in RowsToDrop)
			{
				foreach (Nibbit nib in LockedNibbits.FindAll(n => n.CompareRow(row) < 0))
					nib.Offset(new Point(0, 1));
			}

			currentScore += scores[RowsToDrop.Count - 1];
			RowsToDrop.Clear();
		}

		protected override void Update(GameTime gameTime)
		{
			if (lastSeconds >= dropDelay)
			{
				// Do we need to drop some rows?
				if (RowsToDrop.Count != 0)
				{
					DropRows();
				}
				else // move the current piece down
				{
					Block newBlock = new Block(CurrentBlock);
					newBlock.Translate(new Point(0, 1));

					// lock block
					if (!newBlock.AboveGround() || !ValidPosition(newBlock))
					{
						LockedNibbits.AddRange(CurrentBlock.EmancipateNibbits());
						RemoveRows();
						CurrentBlock = new Block(GraphicsDevice);
					}
					else
						CurrentBlock = newBlock;
				}

				lastSeconds = 0;
			}

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
				dropDelay = 0.0625;
			else
				dropDelay = 0.5;

			lastKBState = Keyboard.GetState();
			lastSeconds += gameTime.ElapsedGameTime.TotalSeconds;

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			CurrentBlock.Draw();

			foreach (Nibbit n in LockedNibbits)
				n.Draw(Point.Zero);

			spriteBatch.Begin();

			spriteBatch.Draw(midpointLine,
				new Rectangle(Constants.NIBBIT_SIZE * Constants.PLAYFIELD_WIDTH, 0, 1,
				Constants.NIBBIT_SIZE * Constants.PLAYFIELD_HEIGHT), Color.Black);

			spriteBatch.DrawString(scoreFont, "Score: " + currentScore.ToString(),
				new Vector2(Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE + 10, 5),
				Color.Black);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
