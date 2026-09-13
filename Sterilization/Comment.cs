using System;
using System.Collections.Generic;
using System.Text;

namespace Sterilization
{
	internal class Comment(string author, string text)
	{
		public string Author { get; } = author;
		public string Text { get; } = text;
		public DateTime Date { get; } = DateTime.Now;

		public override string ToString()
		{
			return string.Join("|", [Author, Text, Date]);
		}
	}
}
