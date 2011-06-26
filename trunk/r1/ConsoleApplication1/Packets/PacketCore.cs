using System;
using System.Data;
using System.Net.Sockets;
using System.Text;
using Storage;
using ConsoleApplication1.Packets;

namespace ConsoleApplication1
{
    public partial class PacketHandler
    {
        private NetworkStream clientStream = NewSocket.clientStream;
        private serverMessage fuseMessage = null;
        private static string Username;

        public void VERSIONCHECK()
        {
            fuseMessage = new serverMessage("ENCRYPTION_OFF");
            NewSocket.SendData(clientStream, fuseMessage);

            fuseMessage = new serverMessage("SECRET_KEY");
            fuseMessage.AppendInteger(1337);
            NewSocket.SendData(clientStream, fuseMessage);
        }
        public void INFORETRIEVE()
        {
            string username = NewSocket.split[2];
            string password = NewSocket.split[3];

            using (DatabaseClient dbClient = Program.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("username", username);
                dbClient.AddParamWithValue("password", password);

                try
                {
                    string checkdata = dbClient.ReadString("SELECT * FROM members WHERE username = @username AND password = @password");

                    if (checkdata != null)
                    {
                        DataRow dbRow = dbClient.ReadDataRow("SELECT * FROM members WHERE username = @username;");
                        Username = (String)dbRow["username"];

                        fuseMessage = new serverMessage("USEROBJECT");
                        fuseMessage.AppendString("name=");
                        fuseMessage.Append((String)dbRow["username"]);
                        fuseMessage.AppendString("figure=");
                        fuseMessage.Append((String)dbRow["figure"]);
                        fuseMessage.AppendString("birthday=");
                        fuseMessage.Append((String)dbRow["birthday"]);
                        fuseMessage.AppendString("phonenumber=");
                        fuseMessage.AppendString("customData=");
                        fuseMessage.Append((String)dbRow["motto"]);
                        fuseMessage.AppendString("had_read_agreement=");
                        fuseMessage.Append((Int32)dbRow["had_read_agreement"]);
                        fuseMessage.AppendString("sex=");
                        fuseMessage.Append((String)dbRow["sex"]);
                        fuseMessage.AppendString("country=");
                        fuseMessage.AppendString("has_special_rights=0");
                        fuseMessage.AppendString("badge_type=");
                        fuseMessage.Append((Int32)dbRow["badge"]);
                        NewSocket.SendData(clientStream, fuseMessage);
                    }
                    else
                    {
                        fuseMessage = new serverMessage("SYSTEMBROADCAST");
                        fuseMessage.AppendString("Wrong username or password.");
                        NewSocket.SendData(clientStream, fuseMessage);
                    }
                }
                catch
                {
                    fuseMessage = new serverMessage("SYSTEMBROADCAST");
                    fuseMessage.AppendString("Wrong username or password.");
                    NewSocket.SendData(clientStream, fuseMessage);
                }
            }
        }
        public void INITUNITLISTENER()
        {
            string builder = null;
            string pBuilder = null;

            using (DatabaseClient dbClient = Program.GetDatabase().GetClient())
            {
                DataTable Table = dbClient.ReadDataTable("SELECT * FROM rooms_private");
                DataTable pTable = dbClient.ReadDataTable("SELECT * FROM rooms_public");

                foreach (DataRow Row in Table.Rows)
                {
                    builder = builder + (Char)13 + (Int32)Row["id"] + "/" + (String)Row["name"] + "/" + (String)Row["owner"] + "/" + (String)Row["status"] + "/" + (String)Row["password"] + "/floor1/127.0.0.1/127.0.0.1/90/1/null" + "/" + (String)Row["desc"];
                }

                if (builder != null)
                {
                    fuseMessage = new serverMessage("BUSY_FLAT_RESULTS 1");
                    fuseMessage.Append(builder);
                    NewSocket.SendData(clientStream, fuseMessage);
                }


                foreach (DataRow Row in pTable.Rows)
                {
                    pBuilder = pBuilder + (Char)13 + (String)Row["name"] + "," + (Int32)Row["curr_in"] + "," + (Int32)Row["max_in"] + ",127.0.0.1/127.0.0.1,90," + (String)Row["name"] + " " + (String)Row["name_tolower"] + "," + (Int32)Row["curr_in"] + "," + (Int32)Row["max_in"] + "," + (String)Row["model"];
                }
                if (pBuilder != null)
                {   fuseMessage = new serverMessage("ALLUNITS 1");
                    fuseMessage.Append(pBuilder);
                    NewSocket.SendData(clientStream, fuseMessage);
                }
            }

            //NewSocket.SendData(clientStream, "ALLUNITS" + (Char)13 + "Habbo Lido,0,25,90/90,90,Habbo Lido lido,1,25,pool_a" + (char)13 + "Hotel Kitchen,0,25,90/90,22009,Hotel Kitchen kitchen,0,25,cr_kitchen" + (char)13 + "The Dirty Duck Pub,0,25,1337/1337,22009,The Dirty Duck Pub pub,0,25,pub_a");
        }
        public void GETCREDITS()
        {
            using (DatabaseClient dbClient = Program.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("username", Username);
                DataRow dbRow = dbClient.ReadDataRow("SELECT * FROM members WHERE username = @username;");

                fuseMessage = new serverMessage("MYPERSISTENTMSG"); //Console Motto
                fuseMessage.AppendString((String)dbRow["console_motto"]);
                NewSocket.SendData(clientStream, fuseMessage);

                fuseMessage = new serverMessage("WALLETBALANCE"); //Your coins
                fuseMessage.AppendInteger((Int32)dbRow["coins"]);
                NewSocket.SendData(clientStream, fuseMessage);

                fuseMessage = new serverMessage("MESSENGERSMSACCOUNT");
                fuseMessage.AppendString("noaccount");
                NewSocket.SendData(clientStream, fuseMessage);

                fuseMessage = new serverMessage("MESSENGERREADY"); //Enable messenger
                NewSocket.SendData(clientStream, fuseMessage);

            }
        }
    }
}
