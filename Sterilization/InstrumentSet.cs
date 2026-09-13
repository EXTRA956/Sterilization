using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;
using static Sterilization.SetState;

namespace Sterilization
{
	internal class InstrumentSet
	{
		public enum SetPackaging
		{
			Gem4,
			Gem1AndGem4,
			DoublePeelPouch,
			ContainerAndLid,
		}

		public int SerialNumber { get; }
		public SetState ActiveState { get; private set; }
		public List<SetState> StateLog { get; }
		public Comment? ActiveComment { get; private set; }
		public List<Comment> CommentLog { get; }
		public SetPackaging Packaging { get; private set; }
		public List<Instrument> Instruments { get; }

		public InstrumentSet(int serialNumber, string user, SetPackaging packaging, List<Instrument>? instruments)
		{
			SerialNumber = serialNumber;
			ActiveState = new SetState(SetStates.New, user, null, null);
			StateLog = [];
			StateLog.Add(ActiveState);

			CommentLog = [];
			Packaging = packaging;
			Instruments = instruments is null ? [] : [.. instruments];
		}

		public void RemoveInstrument(string name)
		{
			Instrument? instrument = Instruments.Find(instance => instance.Name == name);
			if (instrument == null)
			{ 
				throw new InvalidOperationException("Instrument Not Found");
			}
			else
			{
				Instruments.Remove(instrument);
			}
		}
		public void AddInstrument(Instrument instrument)
		{
			Instruments.Add(instrument);
		}
		public void TransitionState(SetState state)
		{
			if (state.Comment != null)
			{
				ActiveComment = state.Comment;
				CommentLog.Add(state.Comment);
			}

			ActiveState = state;
			StateLog.Add(state);
		}

		public void UpdatePackaging(SetPackaging packaging)
		{
			Packaging = packaging;
		}

		public override string ToString()
		{
			return SerialNumber.ToString();
		}
	}
}
