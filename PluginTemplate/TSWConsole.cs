using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using System.Collections.Generic;
using Rests;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using TShockAPI.ServerSideCharacters;

namespace tswConsole
{
    [ApiVersion(1, 23)]
    public class TSWConsole : TerrariaPlugin
    {
        public static string SavePath = "tshock";
        public static IDbConnection DB;

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "XGhozt & Khoatic"; }
        }
        public override string Name
        {
            get { return "TSWConsole"; }
        }

        public override string Description
        {
            get { return "[tserverweb.com] REST endpoint for viewing server log file"; }
        }

        public static ConfigFile Config;

        public static ServerSideConfig ServerSideCharacterConfig;

        public override void Initialize()
        {
            TShock.RestApi.Register(new SecureRestCommand("/tswconsole", getLog, "AdminRest.allow"));
        }

        // Initial credit to: https://github.com/Grandpa-G/ExtraRestAdmin
        private object getLog(RestRequestArgs args)
        {

            String lineCount = "200";

            if (!string.IsNullOrWhiteSpace(args.Parameters["count"])) {
                lineCount = args.Parameters["count"];
            }

            var directory = new DirectoryInfo(TShock.Config.LogPath);

            String searchPattern = @"(19|20)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01]).*.log";

            var log = Directory.GetFiles(TShock.Config.LogPath).Where(path => Regex.Match(path, searchPattern).Success).Last();
            String logFile = Path.GetFullPath(log);

            FileStream logFileStream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logFileReader = new StreamReader(logFileStream);

            int limit = System.Convert.ToInt32(lineCount);
            var buffor = new Queue<string>(limit);
            while (!logFileReader.EndOfStream)
            {
                string line = logFileReader.ReadLine();
                if (buffor.Count >= limit)
                    buffor.Dequeue();
                buffor.Enqueue(line);
            }

            string[] LogLinesEnd = buffor.ToArray();
            
            // Clean up
            logFileReader.Close();
            logFileStream.Close();

            return new RestObject() { { "log", LogLinesEnd } };

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

    }
}