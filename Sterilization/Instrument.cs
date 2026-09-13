using System;
using System.Collections.Generic;
using System.Text;

namespace Sterilization
{
	internal class Instrument(string name, int? remainingUses)
	{
		public string Name { get; } = name;
		public int? RemainingUses { get; private set; } = remainingUses;
		public List<Comment> Comments { get; } =[];

		public void UseInstrument()
		{
			if (RemainingUses == null)
				return;

			RemainingUses -= 1;
		}

		public void AddComment(Comment comment)
		{
			Comments.Add(comment);
		}

		public override string ToString()
		{
			return $"{Name} (Remaining Uses: {RemainingUses?.ToString() ?? "inf"})";
		}
	}
}
