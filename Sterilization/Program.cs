using System.Text.RegularExpressions;

namespace Sterilization
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Dictionary<int, InstrumentSet> sets = [];

			bool exit = false;
			string state = "login";
			string? warning = null;
			InstrumentSet? editingSet = null;

			string id = "SYSTEM";

			void ActOnState(string _state)
			{
				switch (_state)
				{
					case "login":
						{
							Console.WriteLine("Please enter your identifier [ID_XXX]: ");
							string? input = Console.ReadLine();
							if (input is null || !Regex.IsMatch(input.Trim(), @"^ID_\d{3}$"))
							{
								warning = $"{input} is not a valid ID.";
								return;
							}

							id = input;
							state = "actions";

							return;
						}
					case "actions":
						{
							Console.WriteLine("What would you like to do? [`Edit Set`, `Find Set`, `Create Set`,`Get History`, `Logout`]: ");
							string? input = Console.ReadLine();

							List<string> options = ["Edit Set", "Find Set", "Create Set", "Get History", "Logout"];

							if (input is null || !options.Contains(input))
							{
								warning = $"{input} is not a valid option.";
								return;
							}

							state = input;

							return;
						}
					case "Edit Set":
						{
							if (editingSet == null)
							{
								Console.WriteLine($"Available Sets: {String.Join(", ", sets.Values)}");
								Console.WriteLine("Enter the serial number: ");
								string? input = Console.ReadLine();
								bool success = Int32.TryParse(input, out int result);

								if (!success)
								{
									warning = $"{input} is not a valid number";
									state = "actions";
									return;
								}

								bool success2 = sets.TryGetValue(result, out InstrumentSet? set);

								if (!success2 || set is null)
								{
									warning = $"set {input} does not exist.";
									state = "actions";
									return;
								}

								editingSet = set;
							}
							
							Console.WriteLine();
							Console.WriteLine($"Editing Set: {editingSet.SerialNumber}");
							Console.WriteLine("Select an operation [`Add Instrument`, `Remove Instrument`, `Update Packaging`, `Force State`, `Delete Set`, `Cancel`]: ");
							string? input2 = Console.ReadLine();

							List<string> options = ["Add Instrument", "Remove Instrument", "Update Packaging", "Force State", "Delete Set", "Cancel"];

							if (input2 is null || !options.Contains(input2))
							{
								warning = $"{input2} is not a valid option.";
								return;
							}

							if (input2 == "Cancel")
							{
								state = "actions";
								editingSet = null;
								return;
							}

							state = input2;

							return;
						}
					case "Add Instrument":
						{
							if (editingSet == null)
							{
								warning = "No set selected.";
								state = "Edit Set";
								return;
							}
							
							Console.WriteLine($"Editing Set: {editingSet.SerialNumber} | Add Instrument");
							Console.WriteLine("Enter instrument name: ");
							string? input = Console.ReadLine();

							if (input is null || input == string.Empty)
							{
								warning = "Instrument name cannot be empty.";
								state = "Edit Set";
								return;
							}

							Console.WriteLine("Enter number of maximum uses [-1 = Infinite]: ");
							string? input2 = Console.ReadLine();
							bool success = Int32.TryParse(input2, out int value);

							if (!success)
							{
								warning = $"{input2} is not a valid number";
								state = "Edit Set";
								return;
							}

							Instrument newInstrument;
							if (value == -1) newInstrument = new(input, null);
							else newInstrument = new(input, value);

							editingSet.AddInstrument(newInstrument);
							warning = $"Succesfully added instrument {newInstrument.Name} from {editingSet.SerialNumber}";
							state = "Edit Set";

							return;
						}
					case "Remove Instrument":
						{
							if (editingSet == null)
							{
								warning = "No set selected.";
								state = "Edit Set";
								return;
							}

							Console.WriteLine($"Editing Set: {editingSet.SerialNumber} | Remove Instrument");
							Console.WriteLine($"Instruments in set: {string.Join(", ", editingSet.Instruments)}");
							Console.WriteLine("Enter instrument name: ");
							string? input = Console.ReadLine();

							if (input is null || input == string.Empty)
							{
								warning = "Instrument name cannot be empty.";
								state = "Edit Set";
								return;
							}

							try
							{
								editingSet.RemoveInstrument(input);
							}
							catch
							{
								warning = $"{input} is not an instrument in set {editingSet.SerialNumber}";
								state = "Edit Set";
								return;
							}

							warning = $"Succesfully removed instrument {input} from {editingSet.SerialNumber}";
							state = "Edit Set";

							return;
						}
					case "Update Packaging":
						{
							if (editingSet == null)
							{
								warning = "No set selected.";
								state = "Edit Set";
								return;
							}

							Console.WriteLine($"Editing Set: {editingSet.SerialNumber} | Update Packaging");
							Console.WriteLine($"Enter a packing type [{string.Join(", ", Enum.GetNames<InstrumentSet.SetPackaging>())}]: ");
							string? input = Console.ReadLine();

							bool success = InstrumentSet.SetPackaging.TryParse(input, false, out InstrumentSet.SetPackaging packaging);

							if (!success || !Enum.IsDefined(packaging))
							{
								warning = $"{input} is not a valid packing type.";
								state = "Edit Set";
								return;
							}

							editingSet.UpdatePackaging(packaging);
							warning = $"Succesfully updated packaging to {packaging} for {editingSet.SerialNumber}";
							state = "Edit Set";

							return;
						}
					case "Force State":
						{
							if (editingSet == null)
							{
								warning = "No set selected.";
								state = "Edit Set";
								return;
							}

							Console.WriteLine($"Editing Set: {editingSet.SerialNumber} | Force State");
							Console.WriteLine($"Enter new state [{string.Join(", ", Enum.GetNames<SetState.SetStates>())}]: ");
							string? input = Console.ReadLine();

							if ( input == null )
							{
								warning = $"State cannot be empty.";
								state = "Edit Set";
								return;
							}

							bool success = SetState.SetStates.TryParse(input, false, out SetState.SetStates result);
							if (!success || !Enum.IsDefined(result))
							{
								warning = $"{input} is not a valid state.";
								state = "Edit Set";
								return;
							}

							Console.WriteLine($"Add a comment or leave empty: ");
							string? input2 = Console.ReadLine();

							Comment? comment = null;
							if (input2 != null && input2 != "") comment = new(id, input2);

							editingSet.TransitionState(new SetState(result, id, null, comment));
							warning = $"Succesfully updated state to {result} for {editingSet.SerialNumber}";
							state = "Edit Set";

							return;
						}
					case "Delete Set":
						{
							if (editingSet == null)
							{
								warning = "No set selected.";
								state = "Edit Set";
								return;
							}

							Console.WriteLine($"Editing Set: {editingSet.SerialNumber} | Delete Set");
							Console.WriteLine($"Confirm [Y/N]: ");
							string? input = Console.ReadLine();

							if (input == null)
							{
								warning = $"Decision cannot be empty, operation cancelled.";
								state = "Edit Set";
								return;
							}

							List<string> options = ["Y", "N"];
							if (!options.Contains(input))
							{
								warning = $"Decision not recognized, operation cancelled.";
								state = "Edit Set";
								return;
							}

							if (input == "N")
							{
								warning = $"Deletion succesfully canceled.";
								state = "Edit Set";
								return;
							}

							if (input == "Y")
							{
								warning = $"{editingSet.SerialNumber} succesfully deleted.";
								sets.Remove(editingSet.SerialNumber);
								editingSet = null;
								state = "Edit Set";

								return;
							}

							return;
						}
					case "Find Set":
						{
							Console.WriteLine($"Available Sets: {String.Join(", ", sets.Values)}");
							Console.WriteLine("Enter the serial number: ");
							string? input = Console.ReadLine();
							bool success = Int32.TryParse(input, out int result);

							if (!success)
							{
								warning = $"{input} is not a valid number";
								state = "actions";
								return;
							}

							bool success2 = sets.TryGetValue(result, out InstrumentSet? set);
							if (!success2 || set is null)
							{
								warning = $"set {input} does not exist.";
								state = "actions";
								return;
							}

							Console.WriteLine();
							Console.WriteLine($"Serial Number: {set.SerialNumber}");
							Console.WriteLine($"ActiveState: {set.ActiveState}");
							if (set.ActiveComment != null) Console.WriteLine($"ActiveComment: {set.ActiveComment}");
							Console.WriteLine($"Packaging: {set.Packaging}");
							Console.WriteLine($"Instruments: {string.Join(", ", set.Instruments)}");

							Console.WriteLine("Press ENTER to Continue: ");
							Console.ReadLine();

							state = "actions";

							return;
						}
					case "Create Set":
						{
							Console.WriteLine("Enter a serial number: ");
							string? input = Console.ReadLine();
							bool success = Int32.TryParse(input, out int result);
							if (!success)
							{
								warning = $"{input} is not a valid number.";
								state = "actions";
								return;
							}

							bool success2 = sets.TryGetValue(result, out InstrumentSet? existingSet);
							if (success2 || existingSet is not null)
							{
								warning = $"{input} already exists.";
								state = "actions";
								return;
							}

							Console.WriteLine($"Enter a packing type [{string.Join(", ", Enum.GetNames<InstrumentSet.SetPackaging>())}]: ");
							string? input2 = Console.ReadLine();

							bool success3 = InstrumentSet.SetPackaging.TryParse(input2, false, out InstrumentSet.SetPackaging packaging);

							if (!success3 || !Enum.IsDefined(packaging))
							{
								warning = $"{input2} is not a valid packing type.";
								state = "actions";
								return;
							}

							InstrumentSet newSet = new(result, id, packaging, null);
							

							sets.Add(newSet.SerialNumber, newSet);
							warning = "succesfully created set.";
							state = "actions";

							return;
						}
					case "Get History":
						{
							Console.WriteLine($"Available Sets: {String.Join(", ", sets.Values)}");
							Console.WriteLine("Enter the serial number: ");
							string? input = Console.ReadLine();
							bool success = Int32.TryParse(input, out int result);

							if (!success)
							{
								warning = $"{input} is not a valid number";
								state = "actions";
								return;
							}

							bool success2 = sets.TryGetValue(result, out InstrumentSet? set);
							if (!success2 || set is null)
							{
								warning = $"set {input} does not exist.";
								state = "actions";
								return;
							}

							Console.WriteLine();
							Console.WriteLine($"Serial Number: {set.SerialNumber}");
							Console.WriteLine($"StateLog: {string.Join(", ", set.StateLog)}");
							Console.WriteLine();
							Console.WriteLine($"CommentLog: {string.Join(", ", set.CommentLog)}");

							Console.WriteLine("Press ENTER to Continue: ");
							Console.ReadLine();

							state = "actions";


							return;
						}
					case "Logout":
						{
							exit = true;
							return;
						}
					case null:
						return;
				}
			}

			while (exit == false)
			{
				Console.Clear();

				if (warning is not null)
				{
					Console.WriteLine($"Warning: {warning}");
					warning = null;
				}

				ActOnState(state);
			}

			//List<Instrument> instruments = [];
			//instruments.Add(new Instrument("Forcep Mosquite Curved", null));

			//InstrumentSet testSet = new InstrumentSet(12345, "P0001", InstrumentSet.SetPackaging.Gem4, instruments);
			//sets.Add(testSet.SerialNumber, testSet);

			//Console.WriteLine(testSet.SerialNumber);
			//Console.WriteLine(testSet.ActiveState.State);

			//Comment comment = new Comment("P0001", "This is a brand new set");
			//SetState newState = new SetState(SetState.SetStates.PreWashed, "P0001", null, comment);
			//testSet.TransitionState(newState);

			//if (testSet.ActiveComment != null)
			//	Console.WriteLine(testSet.ActiveComment.Text);
			//else
			//	Console.WriteLine("No Comment");

			//newState = new SetState(SetState.SetStates.Packed, "P0001", null, null);
			//testSet.TransitionState(newState);

			//if (testSet.ActiveComment != null)
			//	Console.WriteLine(testSet.ActiveComment.Text);
			//else
			//	Console.WriteLine("No Comment");

			//Console.WriteLine(string.Join(", ", testSet.Instruments));
			//testSet.RemoveInstrument("Forcep Mosquite Curved");
			//Console.WriteLine(string.Join(", ", testSet.Instruments));



			//Console.WriteLine("END");

			//foreach (Instrument instrument in testSet.Instruments)
			//{
			//	if (instrument.RemainingUses is not int uses || uses > 3) continue;

			//	Console.WriteLine(instrument);
			//}
		}
	}
}
