using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stackenblochen
{
	public static class Constants
	{
		// playfield width and height in nibbits
		public const int PLAYFIELD_WIDTH = 10;
		public const int PLAYFIELD_HEIGHT = 20;

		// nibbit width/height in pixels
		public const int NIBBIT_SIZE = 20;

		public const int NUM_SHAPES = 7;
		public enum Shape { I = 0, O, L, J, S, Z, T };
	}
}
