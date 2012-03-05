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
	public class Block
	{
		private GraphicsDevice graphicsDevice;

		private Constants.Shape BlockShape;
		private Point Position;
		private int BlockSize;

		private List<Nibbit> Nibs;

		public Block(Block b)
		{
			this.graphicsDevice = b.graphicsDevice;
			this.BlockShape = b.BlockShape;
			this.Position = b.Position;
			this.BlockSize = b.BlockSize;

			this.Nibs = new List<Nibbit>();
			foreach (Nibbit n in b.Nibs)
				this.Nibs.Add(new Nibbit(n));
		}

		public Block(GraphicsDevice device)
		{
			graphicsDevice = device;
			this.Nibs = new List<Nibbit>();

			Random r = new Random();
			this.BlockShape = (Constants.Shape)r.Next(Constants.NUM_SHAPES);

			switch (BlockShape)
			{
				case Constants.Shape.I:
					BlockSize = 4;
					Nibs.Add(new Nibbit(Color.Cyan, new Point(2, 0), device));
					Nibs.Add(new Nibbit(Color.Cyan, new Point(2, 1), device));
					Nibs.Add(new Nibbit(Color.Cyan, new Point(2, 2), device));
					Nibs.Add(new Nibbit(Color.Cyan, new Point(2, 3), device));
					break;
				case Constants.Shape.O:
					BlockSize = 2;
					Nibs.Add(new Nibbit(Color.Yellow, new Point(0, 0), device));
					Nibs.Add(new Nibbit(Color.Yellow, new Point(1, 0), device));
					Nibs.Add(new Nibbit(Color.Yellow, new Point(0, 1), device));
					Nibs.Add(new Nibbit(Color.Yellow, new Point(1, 1), device));
					break;
				case Constants.Shape.J:
					BlockSize = 3;
					Nibs.Add(new Nibbit(Color.Blue, new Point(1, 0), device));
					Nibs.Add(new Nibbit(Color.Blue, new Point(1, 1), device));
					Nibs.Add(new Nibbit(Color.Blue, new Point(1, 2), device));
					Nibs.Add(new Nibbit(Color.Blue, new Point(0, 2), device));
					break;
				case Constants.Shape.L:
					BlockSize = 3;
					Nibs.Add(new Nibbit(Color.Orange, new Point(1, 0), device));
					Nibs.Add(new Nibbit(Color.Orange, new Point(1, 1), device));
					Nibs.Add(new Nibbit(Color.Orange, new Point(1, 2), device));
					Nibs.Add(new Nibbit(Color.Orange, new Point(2, 2), device));
					break;
				case Constants.Shape.S:
					BlockSize = 3;
					Nibs.Add(new Nibbit(Color.GreenYellow, new Point(0, 2), device));
					Nibs.Add(new Nibbit(Color.GreenYellow, new Point(1, 2), device));
					Nibs.Add(new Nibbit(Color.GreenYellow, new Point(1, 1), device));
					Nibs.Add(new Nibbit(Color.GreenYellow, new Point(2, 1), device));
					break;
				case Constants.Shape.Z:
					BlockSize = 3;
					Nibs.Add(new Nibbit(Color.Red, new Point(0, 1), device));
					Nibs.Add(new Nibbit(Color.Red, new Point(1, 1), device));
					Nibs.Add(new Nibbit(Color.Red, new Point(1, 2), device));
					Nibs.Add(new Nibbit(Color.Red, new Point(2, 2), device));
					break;
				case Constants.Shape.T:
					BlockSize = 3;
					Nibs.Add(new Nibbit(Color.MediumPurple, new Point(0, 1), device));
					Nibs.Add(new Nibbit(Color.MediumPurple, new Point(1, 1), device));
					Nibs.Add(new Nibbit(Color.MediumPurple, new Point(2, 1), device));
					Nibs.Add(new Nibbit(Color.MediumPurple, new Point(1, 2), device));
					break;
				default: throw new Exception("Block.Block(): Invalid shape!");
			}

			this.Position = new Point((Constants.PLAYFIELD_WIDTH - BlockSize) / 2, -BlockSize + 1);
			//this.Position = new Point(2, 2);
		}

		public void Translate(Point p)
		{
			Position.X += p.X;
			Position.Y += p.Y;
		}

		public void Rotate(bool ClockWise)
		{
			Transpose();
			Flip(ClockWise);
		}

		private void Transpose()
		{
			foreach (Nibbit n in Nibs)
				n.Transpose();
		}

		private void Flip(bool Horizontal)
		{
			foreach (Nibbit n in Nibs)
				n.Flip(Horizontal, BlockSize);
		}

		// Draw this Nibbit, pass in the offset of the parent block in nibbits
		public void Draw()
		{
			foreach (Nibbit n in Nibs)
				n.Draw(Position);
		}

		public bool OnScreen()
		{
			foreach (Nibbit n in Nibs)
			{
				if (!n.OnScreen(Position))
					return false;
			}

			return true;
		}

		public List<Nibbit> EmancipateNibbits()
		{
			List<Nibbit> eNibs = new List<Nibbit>();
			foreach (Nibbit n in Nibs)
			{
				n.Offset(Position);
				eNibs.Add(n);
			}

			return eNibs;
		}

		public bool Contains(Nibbit n)
		{
			foreach (Nibbit myNib in Nibs)
			{
				Nibbit offsetNib = new Nibbit(myNib);
				offsetNib.Offset(Position);

				if (offsetNib.Equals(n))
					return true;
			}

			return false;
		}
	}
}
