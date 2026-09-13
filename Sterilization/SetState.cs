using static Sterilization.SetState;

namespace Sterilization
{
	internal class SetState(SetStates state, string user, string? info, Comment? comment)
	{
		public enum SetStates
		{
			New,
			AcceptedAtSSD,
			PreWashed,
			AssociatedToWasher,
			Washed,
			Packed,
			AssociatedToSterilizer,
			Sterilized,
			AssociatedToTrolley,
			Dispatched,
			AcceptedAtCustomer,
			Dirty
		}

		public SetStates State { get; } = state;
		public string User { get; } = user;
		public DateTime Date { get; } = DateTime.Now;
		public string? Info { get; } = info;
		public Comment? Comment { get; } = comment;

		public override string ToString()
		{
			List<string> data = [User, State.ToString(), Date.ToString()];

			if (Info != null) data.Add(Info);
			if (Comment != null) data.Add(Comment.ToString());

			return string.Join("|", data);
		}
	}
}
