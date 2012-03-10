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

		private List<List<Nibbit>> NibSets;
		private int NibSetIndex = 0;

		public Block(Block b)
		{
			this.graphicsDevice = b.graphicsDevice;
			this.BlockShape = b.BlockShape;
			this.Position = b.Position;
			this.BlockSize = b.BlockSize;
			this.NibSetIndex = b.NibSetIndex;

			this.NibSets = new List<List<Nibbit>>();
			foreach (List<Nibbit> l in b.NibSets)
			{
				List<Nibbit> tmp = new List<Nibbit>();

				foreach (Nibbit n in l)
					tmp.Add(new Nibbit(n));

				NibSets.Add(tmp);
			}
		}

		public Block(GraphicsDevice device)
		{
			graphicsDevice = device;
			this.NibSets = new List<List<Nibbit>>();

			Random r = new Random();
			this.BlockShape = (Constants.Shape)r.Next(Constants.NUM_SHAPES);

			switch (BlockShape)
			{
				/*
				 *  - - 0 -    - - - -
				 *  - - 0 -    - - - -
				 *  - - 0 -    0 0 0 0
				 *  - - 0 -    - - - -
				 */
				case Constants.Shape.I:
					BlockSize = 4;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.Red, new Point(2, 0), device));
					NibSets[0].Add(new Nibbit(Color.Red, new Point(2, 1), device));
					NibSets[0].Add(new Nibbit(Color.Red, new Point(2, 2), device));
					NibSets[0].Add(new Nibbit(Color.Red, new Point(2, 3), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[1].Add(new Nibbit(Color.Red, new Point(0, 2), device));
					NibSets[1].Add(new Nibbit(Color.Red, new Point(1, 2), device));
					NibSets[1].Add(new Nibbit(Color.Red, new Point(2, 2), device));
					NibSets[1].Add(new Nibbit(Color.Red, new Point(3, 2), device));

					break;

				/*
				 *  0 0
				 *  0 0
				 */
				case Constants.Shape.O:
					BlockSize = 2;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.Gold, new Point(0, 0), device));
					NibSets[0].Add(new Nibbit(Color.Gold, new Point(0, 1), device));
					NibSets[0].Add(new Nibbit(Color.Gold, new Point(1, 0), device));
					NibSets[0].Add(new Nibbit(Color.Gold, new Point(1, 1), device));

					break;

				/*
				 *  - - -    - 0 -    - - -    - 0 0
				 *  0 0 0    - 0 -    0 - -    - 0 -
				 *  - - 0    0 0 -    0 0 0    - 0 -
				 */
				case Constants.Shape.J:
					BlockSize = 3;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.MediumBlue, new Point(0, 1), device));
					NibSets[0].Add(new Nibbit(Color.MediumBlue, new Point(1, 1), device));
					NibSets[0].Add(new Nibbit(Color.MediumBlue, new Point(2, 1), device));
					NibSets[0].Add(new Nibbit(Color.MediumBlue, new Point(2, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[1].Add(new Nibbit(Color.MediumBlue, new Point(0, 2), device));
					NibSets[1].Add(new Nibbit(Color.MediumBlue, new Point(1, 0), device));
					NibSets[1].Add(new Nibbit(Color.MediumBlue, new Point(1, 1), device));
					NibSets[1].Add(new Nibbit(Color.MediumBlue, new Point(1, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[2].Add(new Nibbit(Color.MediumBlue, new Point(0, 1), device));
					NibSets[2].Add(new Nibbit(Color.MediumBlue, new Point(0, 2), device));
					NibSets[2].Add(new Nibbit(Color.MediumBlue, new Point(1, 2), device));
					NibSets[2].Add(new Nibbit(Color.MediumBlue, new Point(2, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[3].Add(new Nibbit(Color.MediumBlue, new Point(1, 0), device));
					NibSets[3].Add(new Nibbit(Color.MediumBlue, new Point(1, 1), device));
					NibSets[3].Add(new Nibbit(Color.MediumBlue, new Point(1, 2), device));
					NibSets[3].Add(new Nibbit(Color.MediumBlue, new Point(2, 0), device));

					break;

				/*
				 *  - - -    0 0 -    - - -    - 0 -
				 *  0 0 0    - 0 -    - - 0    - 0 -
				 *  0 - -    - 0 -    0 0 0    - 0 0
				 */
				case Constants.Shape.L:
					BlockSize = 3;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.DarkOrange, new Point(0, 1), device));
					NibSets[0].Add(new Nibbit(Color.DarkOrange, new Point(1, 1), device));
					NibSets[0].Add(new Nibbit(Color.DarkOrange, new Point(2, 1), device));
					NibSets[0].Add(new Nibbit(Color.DarkOrange, new Point(0, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[1].Add(new Nibbit(Color.DarkOrange, new Point(0, 0), device));
					NibSets[1].Add(new Nibbit(Color.DarkOrange, new Point(1, 0), device));
					NibSets[1].Add(new Nibbit(Color.DarkOrange, new Point(1, 1), device));
					NibSets[1].Add(new Nibbit(Color.DarkOrange, new Point(1, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[2].Add(new Nibbit(Color.DarkOrange, new Point(0, 2), device));
					NibSets[2].Add(new Nibbit(Color.DarkOrange, new Point(1, 2), device));
					NibSets[2].Add(new Nibbit(Color.DarkOrange, new Point(2, 2), device));
					NibSets[2].Add(new Nibbit(Color.DarkOrange, new Point(2, 1), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[3].Add(new Nibbit(Color.DarkOrange, new Point(1, 0), device));
					NibSets[3].Add(new Nibbit(Color.DarkOrange, new Point(1, 1), device));
					NibSets[3].Add(new Nibbit(Color.DarkOrange, new Point(1, 2), device));
					NibSets[3].Add(new Nibbit(Color.DarkOrange, new Point(2, 2), device));

					break;

				/*
				 *  - - -    0 - -
				 *  - 0 0    0 0 -
				 *  0 0 -    - 0 -
				 */
				case Constants.Shape.S:
					BlockSize = 3;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.Magenta, new Point(0, 2), device));
					NibSets[0].Add(new Nibbit(Color.Magenta, new Point(1, 2), device));
					NibSets[0].Add(new Nibbit(Color.Magenta, new Point(1, 1), device));
					NibSets[0].Add(new Nibbit(Color.Magenta, new Point(2, 1), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[1].Add(new Nibbit(Color.Magenta, new Point(0, 0), device));
					NibSets[1].Add(new Nibbit(Color.Magenta, new Point(0, 1), device));
					NibSets[1].Add(new Nibbit(Color.Magenta, new Point(1, 1), device));
					NibSets[1].Add(new Nibbit(Color.Magenta, new Point(1, 2), device));

					break;

				/*
				 *  - - -    - - 0
				 *  0 0 -    - 0 0
				 *  - 0 0    - 0 -
				 */
				case Constants.Shape.Z:
					BlockSize = 3;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.GreenYellow, new Point(0, 1), device));
					NibSets[0].Add(new Nibbit(Color.GreenYellow, new Point(1, 1), device));
					NibSets[0].Add(new Nibbit(Color.GreenYellow, new Point(1, 2), device));
					NibSets[0].Add(new Nibbit(Color.GreenYellow, new Point(2, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[1].Add(new Nibbit(Color.GreenYellow, new Point(2, 0), device));
					NibSets[1].Add(new Nibbit(Color.GreenYellow, new Point(2, 1), device));
					NibSets[1].Add(new Nibbit(Color.GreenYellow, new Point(1, 1), device));
					NibSets[1].Add(new Nibbit(Color.GreenYellow, new Point(1, 2), device));

					break;

				/*
				 *  - - -    - 0 -    - 0 -    - 0 -
				 *  0 0 0    0 0 -    0 0 0    - 0 0
				 *  - 0 -    - 0 -    - - -    - 0 -
				 */
				case Constants.Shape.T:
					BlockSize = 3;

					NibSets.Add(new List<Nibbit>());
					NibSets[0].Add(new Nibbit(Color.Cyan, new Point(0, 1), device));
					NibSets[0].Add(new Nibbit(Color.Cyan, new Point(1, 1), device));
					NibSets[0].Add(new Nibbit(Color.Cyan, new Point(1, 2), device));
					NibSets[0].Add(new Nibbit(Color.Cyan, new Point(2, 1), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[1].Add(new Nibbit(Color.Cyan, new Point(0, 1), device));
					NibSets[1].Add(new Nibbit(Color.Cyan, new Point(1, 0), device));
					NibSets[1].Add(new Nibbit(Color.Cyan, new Point(1, 1), device));
					NibSets[1].Add(new Nibbit(Color.Cyan, new Point(1, 2), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[2].Add(new Nibbit(Color.Cyan, new Point(0, 1), device));
					NibSets[2].Add(new Nibbit(Color.Cyan, new Point(1, 0), device));
					NibSets[2].Add(new Nibbit(Color.Cyan, new Point(1, 1), device));
					NibSets[2].Add(new Nibbit(Color.Cyan, new Point(2, 1), device));

					NibSets.Add(new List<Nibbit>());
					NibSets[3].Add(new Nibbit(Color.Cyan, new Point(1, 0), device));
					NibSets[3].Add(new Nibbit(Color.Cyan, new Point(1, 1), device));
					NibSets[3].Add(new Nibbit(Color.Cyan, new Point(1, 2), device));
					NibSets[3].Add(new Nibbit(Color.Cyan, new Point(2, 1), device));

					break;

				default: throw new Exception("Block.Block(): Invalid shape!");
			}

			this.Position = new Point((Constants.PLAYFIELD_WIDTH - BlockSize) / 2, -BlockSize);
			//this.Position = new Point(2, 2);
		}

		public void Translate(Point p)
		{
			Position.X += p.X;
			Position.Y += p.Y;
		}

		public void Rotate(bool ClockWise)
		{
			if (ClockWise)
			{
				if (NibSetIndex < NibSets.Count - 1)
					NibSetIndex++;
				else
					NibSetIndex = 0;
			}
			else
			{
				if (NibSetIndex > 0)
					NibSetIndex--;
				else
					NibSetIndex = NibSets.Count - 1;
			}
		}

		// Draw this Nibbit, pass in the offset of the parent block in nibbits
		public void Draw()
		{
			foreach (Nibbit n in NibSets[NibSetIndex])
				n.Draw(Position);
		}

		public bool InBounds()
		{
			foreach (Nibbit n in NibSets[NibSetIndex])
			{
				if (!n.InBounds(Position))
					return false;
			}

			return true;
		}

		public bool AboveGround()
		{
			foreach (Nibbit n in NibSets[NibSetIndex])
			{
				if (!n.AboveGround(Position))
					return false;
			}

			return true;
		}

		public List<Nibbit> EmancipateNibbits()
		{
			List<Nibbit> eNibs = new List<Nibbit>();
			foreach (Nibbit n in NibSets[NibSetIndex])
			{
				n.Offset(Position);
				eNibs.Add(n);
			}

			return eNibs;
		}

		public bool Contains(Nibbit n)
		{
			foreach (Nibbit myNib in NibSets[NibSetIndex])
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
