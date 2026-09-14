--Sterilization--

A Console app coded in C#. It tracks surgical sets following a sterilization services department track and trace workflow, based on the real processes I follow at my current job as an SSD Operator.
Built to help me learn C# by mapping something I am already familiar with rather then a generic tutorial project. 
First actual project with C# so there is of course things I would like to improve.

--Functionality--

	~Creation and management of instrument sets, using a serial number for identification, and other information like packaging type and a list of instruments
	~Instrument tracking, using individual names and an optional max use count (single use, reusable, infinite uses)
	~State tracking, tracks the state of each set so it can be tracked to where it currently sits, each transition being logged for compliance audits
	~Comments, allows Techs to add a comment to each state which is also logged, so if something is wrong with the set, like a broken instrument, it is tracked.
	~Menu / Console Interface, different menu options for the different functions to make it easier to navigate in console.

--Some of the Choices I made--

	~Records / Logs can't be edited after creation, only adding new entries on new states / comments. This immutability allows the logs to be trustworthy for audits
	~Null types instead of magic numbers. Forexample infinite uses is Null instead of -1, as it felt cleaner then a random undefined number.
	~Used Enums for State and PackingTypes, with input validation using Enum.TryParse alongside Enum.IsDefined (learned the hard way that TryParse will accept an enum's number index like "2" as a valid Enum value even if there is no Enum at that given index)

--Things to try for upcoming projects--

	~Presistent data, forexample with SQL so you data and logs actually persists instead of just having it disappear when the console is closed
	~User Permissions, right now anyone can access every function, I'd want to limit things like moving a set to one at a time, and only admins to be able to roll back or skip steps
	~State Order, making sure the set cant jump between states without admin override
	~State Filtering, with saved data being able to pull every set for a given state, for example every state that has been "preWashed"

--To Run--

Needs .Net 8+, open in Visual Studio and run.

on launch just log in with any ID e.g ID_001