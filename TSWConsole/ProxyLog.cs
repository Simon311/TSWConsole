using System;
using TShockAPI;
using System.Diagnostics;
using System.Collections.Generic;

namespace tswConsole
{
	public class ProxyLog : ILog, IDisposable
	{
		private ILog original;
		private readonly int maxItems;
		public readonly List<string> Cache;

		public string FileName
		{
			get { return original.FileName; }
			set { original.FileName = value; }
		}

		public ProxyLog(ILog Original, int MaxItems)
		{
			Cache = new List<string>();
			original = Original;
			maxItems = MaxItems;
		}

		public bool MayWriteType(TraceLevel type)
		{
			return original.MayWriteType(type);
		}

		public void ConsoleInfo(string message) //
		{
			original.ConsoleInfo(message);
			if (Cache.Count >= maxItems) { Cache.RemoveAt(0);  }
			Cache.Add(message);
		}

		public void ConsoleInfo(string format, params object[] args)
		{
			ConsoleInfo(string.Format(format, args));
		}

		public void ConsoleError(string message) //
		{
			original.ConsoleError(message);
			if (Cache.Count >= maxItems) { Cache.RemoveAt(0); }
			Cache.Add(message);
		}

		public void ConsoleError(string format, params object[] args)
		{
			ConsoleError(string.Format(format, args));
		}

		public void ConsoleDebug(string message) //
		{
			if (TShock.Config.Settings.DebugLogs)
			{
				original.ConsoleDebug(message);
				if (Cache.Count >= maxItems) { Cache.RemoveAt(0); }
				Cache.Add("Debug: " + message);
			}
		}

		public void ConsoleDebug(string format, params object[] args)
		{
			if (TShock.Config.Settings.DebugLogs) { ConsoleDebug(string.Format(format, args)); }
		}

		public void Warn(string message)
		{
			Write(message, TraceLevel.Warning);
		}

		public void Warn(string format, params object[] args)
		{
			Warn(string.Format(format, args));
		}

		public void Error(string message)
		{
			Write(message, TraceLevel.Error);
		}

		public void Error(string format, params object[] args)
		{
			Error(string.Format(format, args));
		}

		public void Info(string message)
		{
			Write(message, TraceLevel.Info);
		}

		public void Info(string format, params object[] args)
		{
			Info(string.Format(format, args));
		}

		public void Data(string message)
		{
			Write(message, TraceLevel.Verbose);
		}

		public void Data(string format, params object[] args)
		{
			Data(string.Format(format, args));
		}

		public void Debug(string message)
		{
			if (TShock.Config.Settings.DebugLogs) { Write(message, TraceLevel.Verbose); }
		}

		public void Debug(string format, params object[] args)
		{
			if (TShock.Config.Settings.DebugLogs) { Debug(string.Format(format, args)); }
		}

		public void Write(string message, TraceLevel level) //
		{
			if (!original.MayWriteType(level)) { return; }
			original.Write(message, level);
			if (Cache.Count >= maxItems) { Cache.RemoveAt(0); }
			Cache.Add(message);
		}

		public void Dispose()
		{
			original.Dispose();
		}
	}
}
