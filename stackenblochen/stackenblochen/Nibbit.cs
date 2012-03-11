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
	/*
	 * The Nibbet is a single square on the playfield
	 * It can either be contained in a Block, or exist
	 * on its own in the playfield. A nibbet is also a
	 * unit of measure, and is defined as the size of
	 * a Nibbet.  This value is set, in pixels, in the
	 * Constants class as NIBBET_SIZE
	 */
	public class Nibbit : IEquatable<Nibbit>
	{
		private Color BlockColor;
		private Point Position;
		private Rectangle Rect;

		private Texture2D Tex;
		private SpriteBatch spriteBatch;

		public Nibbit(Nibbit n)
		{
			this.BlockColor = n.BlockColor;
			this.Position = n.Position;
			this.Rect = n.Rect;
			this.Tex = n.Tex;
			this.spriteBatch = n.spriteBatch;
		}

		// color: the color of this Nibbit
		// offset: the location in the parent block of this Nibbit in nibbits
		public Nibbit(Color color, Point offset, GraphicsDevice device)
		{
			this.BlockColor = color;
			this.Position = offset;
			this.Rect = new Rectangle(0, 0, Constants.NIBBIT_SIZE, Constants.NIBBIT_SIZE);

			spriteBatch = new SpriteBatch(device);
			this.Tex = new Texture2D(device, 1, 1);
			this.Tex.SetData(new Color[] { this.BlockColor });
		}

		public bool InBounds(Point offset)
		{
			return offset.X + Position.X >= 0 &&
				offset.X + Position.X < Constants.PLAYFIELD_WIDTH;
		}

		public bool AboveGround(Point offset)
		{
			return offset.Y + Position.Y < Constants.PLAYFIELD_HEIGHT;
		}

		public void Offset(Point p)
		{
			Position.X += p.X;
			Position.Y += p.Y;
		}

		public int CompareRow(int row)
		{
			return Position.Y.CompareTo(row);
		}

		// Draw this Nibbit, pass in the offset of the parent block in nibbits
		public void Draw(Point offset)
		{
			Rect.X = offset.X * Constants.NIBBIT_SIZE + Position.X * Constants.NIBBIT_SIZE;
			Rect.Y = offset.Y * Constants.NIBBIT_SIZE + Position.Y * Constants.NIBBIT_SIZE;

			spriteBatch.Begin();
			spriteBatch.Draw(Tex, Rect, BlockColor);
			spriteBatch.End();
		}

		public bool Equals(Nibbit n)
		{
			return this.Position == n.Position;
		}
	}
}
