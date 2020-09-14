using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using Rests;

namespace tswConsole
{
    [ApiVersion(2, 1)]
    public class TSWConsole : TerrariaPlugin
    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "XGhozt & Khoatic & Simon311"; }
        }
        public override string Name
        {
            get { return "TSWConsole"; }
        }

        public override string Description
        {
            get { return "[tserverweb.com] REST endpoint for viewing server log file"; }
        }

		const int defaultLimit = 200;
		const int absoluteLimit = 1000;
		static ProxyLog ourLog;

		public override void Initialize()
        {
			ourLog = new ProxyLog(TShock.Log, absoluteLimit);
			TShock.Log = ourLog;
			TShock.RestApi.Register(new SecureRestCommand("/tswconsole", GetLog, "AdminRest.allow"));
        }

        private object GetLog(RestRequestArgs args)
        {
			int lineCount;
			int sx;

			if (!string.IsNullOrWhiteSpace(args.Parameters["count"]))
			{
				int.TryParse(args.Parameters["count"].Trim(), out lineCount);
				sx = sex(absoluteLimit - lineCount);
				lineCount = sx * -absoluteLimit + (1 + sx) * lineCount;
			}
			else lineCount = defaultLimit;

			sx = sex(ourLog.Cache.Count - lineCount);
			lineCount = sx * -ourLog.Cache.Count + (1 + sx) * lineCount;
			int start = ourLog.Cache.Count - lineCount;
			string[] buffer = new string[lineCount];
			for (int i = 0; i < lineCount; i++)
			{
				buffer[i] = ourLog.Cache[start+i];
			}

            return new RestObject() { { "log", buffer } };
        }

        private static RestObject RestError(string message, string status = "400")
        {
            return new RestObject(status) { Error = message };
        }

        public TSWConsole(Main game)
            : base(game)
        {
            Order = 1;
        }

		static int sex(int x)
		{
			return x >> (8 * sizeof(int) - 1);
		}
	}
}