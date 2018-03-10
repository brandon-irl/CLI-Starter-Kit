using System;
using RandomConsoleUtility;
using InterviewQuestions;

namespace CLIStarterKit.Commands
{
	static class Util
	{
		public static void DecryptChromePasswords()
		{
			Start.DecryptChromePasswords();
		}
	}

	static class Interview
	{
		public static void Cake()
		{
			var a = new Node(1);
			var b = new Node(2);
			var c = new Node(3);
			var d = new Node(4);
			var e = new Node(5);

			a.Next = b;
			b.Next = c;
			c.Next = d;
			d.Next = e;

			Console.WriteLine($"KthToLastNode: {InterviewCake.KthToLastNode(2, a)}");
		}
	}
}
