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
	public enum GameState { Start, Play, Pause, Lose }

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
		Texture2D pixel;
		GameState State = GameState.Start;

		// Various rectangles
		Rectangle midlineRect;
		Rectangle scoreSideRect;
		Rectangle pauseOverlayRect;
		Color pauseOverlayColor;

		// Text location vectors
		Vector2 gameoverVec;
		Vector2 startVec;
		Vector2 pauseVec1;
		Vector2 pauseVec2;
		Vector2 scoreVec;

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

			// dumb hack, use a 1x1 pixel to draw lines and stuff
			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });

			scoreFont = Content.Load<SpriteFont>("Segoe");
			CurrentBlock = new Block(GraphicsDevice);

			// Set up various rectangles/lines
			scoreSideRect = new Rectangle(Constants.NIBBIT_SIZE * Constants.PLAYFIELD_WIDTH, 0,
					Constants.NIBBIT_SIZE * Constants.PLAYFIELD_WIDTH,
					Constants.NIBBIT_SIZE * Constants.PLAYFIELD_HEIGHT);

			midlineRect = new Rectangle(Constants.NIBBIT_SIZE * Constants.PLAYFIELD_WIDTH, 0, 1,
				Constants.NIBBIT_SIZE * Constants.PLAYFIELD_HEIGHT);

			pauseOverlayRect = new Rectangle(0, 0,
				Constants.NIBBIT_SIZE * Constants.PLAYFIELD_WIDTH * 2,
				Constants.NIBBIT_SIZE * Constants.PLAYFIELD_HEIGHT);
			pauseOverlayColor = Color.FromNonPremultiplied(new Vector4(0, 0, 0, 0.5f));

			#region Vectors
			// Precalculate position vectors for text
			Vector2 tmp = scoreFont.MeasureString("Paused");
			pauseVec1 = new Vector2(
				Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE - tmp.X * 0.5f,
				Constants.PLAYFIELD_HEIGHT * Constants.NIBBIT_SIZE * 0.25f - tmp.Y);

			tmp = scoreFont.MeasureString("Press 'P' to resume");
			pauseVec2 = new Vector2(
				Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE - tmp.X * 0.5f,
				Constants.PLAYFIELD_HEIGHT * Constants.NIBBIT_SIZE * 0.25f);

			tmp = scoreFont.MeasureString("Game Over");
			gameoverVec = new Vector2(
				Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE * 1.5f - tmp.X * 0.5f,
				Constants.PLAYFIELD_HEIGHT * Constants.NIBBIT_SIZE * 0.5f - tmp.Y * 1.5f);

			tmp = scoreFont.MeasureString("Press Enter to play");
			startVec = new Vector2(
				Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE * 1.5f - tmp.X * 0.5f,
				Constants.PLAYFIELD_HEIGHT * Constants.NIBBIT_SIZE * 0.5f - tmp.Y * 0.5f);

			scoreVec = new Vector2(Constants.PLAYFIELD_WIDTH * Constants.NIBBIT_SIZE + 10, 5);

			// clamp values to ints otherwise text looks rubbish
			ClampVector(ref pauseVec1);
			ClampVector(ref pauseVec2);
			ClampVector(ref gameoverVec);
			ClampVector(ref startVec);
			ClampVector(ref scoreVec);
			#endregion
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

		private void UpdateBlocks()
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

					// Are any nibbits overflowing?
					foreach (Nibbit n in LockedNibbits)
					{
						if (n.CompareRow(0) < 1)
							State = GameState.Lose;
					}
				}

				lastSeconds = 0;
			}
		}

		private void HandleMovement()
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
				dropDelay = 0.0625;
			else
				dropDelay = 0.5;
		}

		private void ClampVector(ref Vector2 vector)
		{
			vector.X = (int)vector.X;
			vector.Y = (int)vector.Y;
		}

		protected override void Update(GameTime gameTime)
		{
			switch (State)
			{
				case GameState.Pause:
					if (KeyPressed(Keys.P))
						State = GameState.Play;
					break;

				case GameState.Lose:
				case GameState.Start:
					if (KeyPressed(Keys.Enter))
					{
						LockedNibbits.Clear();
						State = GameState.Play;
					}
					break;

				case GameState.Play:
					if (KeyPressed(Keys.P))
					{
						State = GameState.Pause;
						break;
					}

					UpdateBlocks();
					HandleMovement();
					lastSeconds += gameTime.ElapsedGameTime.TotalSeconds;
					break;
			}

			lastKBState = Keyboard.GetState();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			CurrentBlock.Draw();

			foreach (Nibbit n in LockedNibbits)
				n.Draw(Point.Zero);

			spriteBatch.Begin();

			spriteBatch.Draw(pixel, scoreSideRect, Color.WhiteSmoke);
			spriteBatch.Draw(pixel, midlineRect, Color.DarkGray);

			spriteBatch.DrawString(scoreFont, "Score: " + currentScore.ToString(), scoreVec, Color.Black);

			switch (State)
			{
				case GameState.Pause:
					spriteBatch.Draw(pixel, pauseOverlayRect, pauseOverlayColor);
					spriteBatch.DrawString(scoreFont, "Paused", pauseVec1, Color.White);
					spriteBatch.DrawString(scoreFont, "Press 'P' to resume", pauseVec2, Color.White);

					break;
				case GameState.Lose:
					spriteBatch.DrawString(scoreFont, "Game Over", gameoverVec, Color.Black);
					spriteBatch.DrawString(scoreFont, "Press Enter to play", startVec, Color.Black);
					break;
				case GameState.Start:
					spriteBatch.DrawString(scoreFont, "Press Enter to play", startVec, Color.Black);
					break;
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
