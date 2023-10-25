using log4net;

namespace Server.Elements
{

    public struct Logger
    {
        public static ILog Log = LogManager.GetLogger(typeof(GameServerApp));

    }
}
