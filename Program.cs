using System;
using CLIStarterKit.Commands;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Common;

namespace CLIStarterKit
{
	class Program
	{
		static Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>> commandLibraries = new Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>>();

		static void Main(string[] args)
		{
			Console.Title = typeof(Program).Name;

			Assembly.GetExecutingAssembly().GetTypes()
				.Where(_ => _.IsClass && _.Namespace == Constants.CommandNamespace)
				.ToList()
				.ForEach(_ => commandLibraries.Add(
					_.Name,
					_.GetMethods(BindingFlags.Static | BindingFlags.Public)
					.ToDictionary(m => m.Name, m => m.GetParameters() as IEnumerable<ParameterInfo>))    //TODO: why does this 'as' have to be here? ParameterInfo[] can't be casted to IEnumerable<ParameterInfo> if it's being put into a Generic?
				);

			Run(args);
		}

		static void Run(string[] args = null)
		{
			var input = args != null ? string.Join(" ", args) : default(string);

			while (true)
			{
				if (string.IsNullOrWhiteSpace(input)) input = ReadFromConsole();
				if (string.IsNullOrWhiteSpace(input)) continue;

				try
				{
					if (input.StartsWith(Constants.ActionSignifier))
						WriteToConsole(RunAction(input.Substring(Constants.ActionSignifier.Length)));
					else
					{
						var cmd = new ConsoleCommand(input);

						foreach (var result in Execute(cmd))
							WriteToConsole(result ?? Constants.NoResultsMessage);
					}
				}
				catch (Exception ex)
				{
					WriteToConsole($"ERROR: {ex.Message}");
					WriteToConsole(Constants.AbortingMessage);
				}
				finally
				{
					input = default(string);
				}
			}
		}

		private static string RunAction(string actionString)
		{
			switch (actionString)
			{
				case "?":
					return $"Available Commands are {commandLibraries}";
				default:
					throw new ArgumentException($"'{actionString} is not a valid action.");
			}
		}

		static IEnumerable<string> Execute(BaseConsoleCommand command)
		{
			if (!commandLibraries.ContainsKey(command.LibraryClassName) || !commandLibraries[command.LibraryClassName].ContainsKey(command.Name))
				throw new Exception(Constants.LibraryClassDoesntExistMessage);

			var parameterInfo = commandLibraries[command.LibraryClassName][command.Name];

			if (parameterInfo.Where(_ => _.IsOptional == false).Count() > command.Arguments.Count())
				throw new Exception(Constants.InvalidArgumentCountMessage);

			object result;
			try
			{
				result = Assembly.GetExecutingAssembly()
					.GetType($"{Constants.CommandNamespace}.{command.LibraryClassName}")
					.InvokeMember(
					command.Name,
					BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
					null,
					null,
					command.Arguments.ZipAll(parameterInfo, (a, b) => Convert.ChangeType(a == default(string) ? b.DefaultValue : a, b.ParameterType)).ToArray());
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}

			if (result == null)
				yield return null;
			else
				if (result is IEnumerable<string>)
				foreach (var str in result.As<IEnumerable<string>>())
					yield return str;
			else
				yield return result.ToString();
		}

		private static void WriteToConsole(string message)
		{
			if (string.IsNullOrEmpty(message)) return;
			Console.WriteLine(message);
		}

		private static string ReadFromConsole(string promptMessage = "")
		{
			Console.Write(Constants.ReadPrompt + promptMessage);
			return Console.ReadLine();
		}
	}
}
