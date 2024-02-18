using System;

namespace TodoAppFunction
{
    public static class DBConfig
    {
        public const string DATABASE = "TodoApp";
        public const string CONTAINERTODO = "Todo";
        public const string CONTAINERTODOHISTORY = "TodoHistory";
        public const string CONNECTION = "CosmosDBConnection";
        public static string CONNECTIONSTRING =
            Environment.GetEnvironmentVariable(CONNECTION);
    }
}
