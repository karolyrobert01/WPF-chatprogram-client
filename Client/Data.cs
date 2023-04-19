using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{

    enum Command
    {
        Login,      //Log into the server
        Logout,     //Logout of the server
        Message,    //Send a text message to all the chat clients
        List,       //Get a list of users in the chat room from the server
        Register,
        Accept,
        Decline,
        Null        //No command
    }

    class Data
    {
        //Default constructor
        public Data()
        {
            this.cmdCommand = Command.Null;
            this.strMessage = null;
            this.strName = null;
            this.strPass = null;
            this.strAddres = null;
        }

        //Converts the bytes into an object of type Data
        public Data(byte[] data)
        {
            //The first four bytes are for the Command
            this.cmdCommand = (Command)BitConverter.ToInt32(data, 0);

            //The next four store the length of the name
            int nameLen = BitConverter.ToInt32(data, 4);

            //The next four store the length of the message
            int msgLen = BitConverter.ToInt32(data, 8);

            int passLen = BitConverter.ToInt32(data, 12);

            int addresLen = BitConverter.ToInt32(data, 16);

            //This check makes sure that strName has been passed in the array of bytes
            if (nameLen > 0)
                this.strName = Encoding.UTF8.GetString(data, 20, nameLen);
            else
                this.strName = null;

            //This checks for a null message field
            if (msgLen > 0)
                this.strMessage = Encoding.UTF8.GetString(data, 20 + nameLen, msgLen);
            else
                this.strMessage = null;

            if (passLen > 0)
                this.strPass = Encoding.UTF8.GetString(data, 20 + nameLen + msgLen, passLen);
            else
                this.strPass = null;

            if (addresLen > 0)
                this.strAddres = Encoding.UTF8.GetString(data, 20 + nameLen + msgLen + passLen, addresLen);
            else
                this.strAddres = null;
        }

        //Converts the Data structure into an array of bytes
        public byte[] ToByte()
        {
            List<byte> result = new List<byte>();

            //First four are for the Command
            result.AddRange(BitConverter.GetBytes((int)cmdCommand));

            //Add the length of the name
            if (strName != null)
                result.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(strName)));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Length of the message
            if (strMessage != null)
                result.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(strMessage)));
            else
                result.AddRange(BitConverter.GetBytes(0));

            if (strPass != null)
                result.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(strPass)));
            else
                result.AddRange(BitConverter.GetBytes(0));

            if (strAddres != null)
                result.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(strAddres)));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Add the name
            if (strName != null)
                result.AddRange(Encoding.UTF8.GetBytes(strName));

            //And, lastly we add the message text to our array of bytes
            if (strMessage != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage));

            if (strPass != null)
                result.AddRange(Encoding.UTF8.GetBytes(strPass));

            if (strAddres != null)
                result.AddRange(Encoding.UTF8.GetBytes(strAddres));

            return result.ToArray();
        }

        public string? strName;      //Name by which the client logs into the room
        public string? strMessage;   //Message text
        public string? strPass;
        public string? strAddres;
        public Command cmdCommand;  //Command type (login, logout, send message, etcetera)
    }
}
