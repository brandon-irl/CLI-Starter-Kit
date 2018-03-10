using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIStarterKit
{
	internal static class Constants
	{
		public const string ReadPrompt = "console> ";
		public const string CommandNamespace = "CLIStarterKit.Commands";
		public const string AbortingMessage = "Aborting...";
		public const string NoResultsMessage = "No results returned";
		public const string LibraryClassDoesntExistMessage = "Library class or command name doesn't exist";
		public const string InvalidArgumentCountMessage = "Invalid argument count";
		public const string ActionSignifier = "/";
	}
}
