using System;
using Storage;

namespace ConsoleApplication1
{
    class Program
    {
        private static DatabaseManager DatabaseManager;
        private static string host = "localhost";
        private static string user = "root";
        private static string database = "young";
        private static string pw = "abcdef";
        private static uint mySQLport = 3306;

        static void Main(string[] args)
        {
            Console.Title = "prjHELLO - Habbo Hotel v1";
            Console.WriteLine("#####################");
            Console.WriteLine("### Project HELLO ###");
            Console.WriteLine("#####################\n");

            DatabaseServer dbServer = new DatabaseServer(host, mySQLport, user, pw);
            Database db = new Database(database, uint.Parse("5"), uint.Parse("30"));
            DatabaseManager = new DatabaseManager(dbServer, db);

            Console.Write("MySQL Status: ");
            if (DatabaseManager.GetClient() == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(false + "\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(true + "\n");
            }
            Console.ResetColor();
            Console.WriteLine("Starting up server...");
            NewSocket n = new NewSocket();
            n.Server();
        }
        public static DatabaseManager GetDatabase()
        {
            return DatabaseManager;
        }
    }
}
