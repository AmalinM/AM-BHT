using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Threading;


namespace ConsoleApp1
{
    class Program
        {
        static void Main(string[] args)
            {

            UInt32 w0 = OSDIWord0();
            UInt32 w1 = OSDIWord1();
            UInt32 w2 = OSDIWord2();
            UInt32 w3 = OSDIWord3();
            UInt32 w4 = OSDIWord4();
            UInt32 w5 = OSDIWord5();
            UInt32 w6 = OSDIWord6();
            UInt32 w364 = OSDIWord364();

            var packet = new float[365]; // packet will be the float array that is sent

            //var Header = { (float)w0, (float)w1, (float)w2, (float)w3, (float)w4, (float)w5, (float)w6 }; // Sets the Header array 

            //for (int HeaderWords = 0; HeaderWords <= 6; HeaderWords++) // Puts the header in the first 6 indices of packet
            //{
            //    packet[HeaderWords] = Header[HeaderWords];
            //    HeaderWords += 1;
            //    Console.WriteLine(packet[HeaderWords]);
            //}

            packet.SetValue(OSDIWord0(), 0);
            Console.WriteLine(packet[0]);

            packet.SetValue(OSDIWord1(), 1);
            Console.WriteLine(packet[1]);

            packet.SetValue(OSDIWord2(), 2);
            Console.WriteLine(packet[2]);

            packet.SetValue(OSDIWord3(), 3);
            Console.WriteLine(packet[3]);

            packet.SetValue(OSDIWord4(), 4);
            Console.WriteLine(packet[4]);

            packet.SetValue(OSDIWord5(), 5);
            Console.WriteLine(packet[5]);

            packet.SetValue(OSDIWord6(), 6);
            Console.WriteLine(packet[6]);

            packet.SetValue(OSDIWord364(), 364); // Puts the trailer in the last index of packet

            Console.WriteLine(packet[364]);

            var data = GetDatapointsFromFile("FMWaveDatapoints.txt"); //gives data the float array returned by GetDatapointsFromFile
            Console.WriteLine(data.Count);
            int totalSent = 0;

            while (totalSent <= (data.Count))
            {
                Console.WriteLine(data.Count + "Data Count");
                //if(totalSent/357 > data.Count/357)
                if((data.Count - totalSent) < 357)
                {
                    int x = data.Count - totalSent;

                    var subArray = data.GetRange(totalSent-357, x);

                    for (int i = 7; i <= x; i++)
                    {
                        packet.SetValue(subArray[i - 7], i);
                        Console.WriteLine(packet[i] + ",   i = " + i);
                    }

                    for(int i = x+7; i <= 363; i++ )
                    {
                        packet.SetValue(0, i);
                    }

                    var byteArray = ConvertFloatArrayToByteArray(packet);

                    SendUdpPacket(5000, byteArray, 5001, "192.168.1.203");
                    
                    //var receivedData = ReceiveData(5002).Result;
                    //var byteArrayOfReceivedData = receivedData.Buffer;                
                    Console.WriteLine("Total Sent = " + totalSent / 357);

                    totalSent += 357;

                }
                else
                {
                    var subArray = data.GetRange(totalSent, 357);

                    for (int i = 7; i <= 363; i++)
                    {
                        packet.SetValue(subArray[i - 7], i);
                        Console.WriteLine(packet[i] + ",   i = " + i);
                    }

                    var byteArray = ConvertFloatArrayToByteArray(packet);

                    SendUdpPacket(5000, byteArray, 5001, "192.168.1.203");
                    //Thread.Sleep(2000);
                    //var receivedData = ReceiveData(5002).Result;
                    //var byteArrayOfReceivedData = receivedData.Buffer;
                    totalSent += 357;
                    Console.WriteLine("Total Sent = " + totalSent / 357);
                }


                
            }


             }

        static UInt32 OSDIWord0()
            {
                static UInt32 PT()
                {
                    int PacketTypeint = 1; // Hard coded for now for IF Data Format. GUI will provide input for Packet Type of choice.

                    for (int i = 0; i <= 4; i++)
                    {
                        PacketTypeint = i;
                    }

                    UInt32 ReturnTo = (UInt32)PacketTypeint;

                    return ReturnTo;
                }

                static UInt32 CBID()
                {
                    int ClassIDBitint = 1;

                    UInt32 ReturnTo = (UInt32)ClassIDBitint;

                    return ReturnTo;
                }

                static UInt32 TBID()
                {
                    int TrailerBitint = 1;

                    UInt32 ReturnTo = (UInt32)TrailerBitint;

                    return ReturnTo;
                }

                static UInt32 RB()
                {
                    UInt32 ReturnTo = 0;

                    return ReturnTo;
                }

                static UInt32 TSIB()
                {
                    int TSIBit = 2;

                    UInt32 ReturnTo = (UInt32)TSIBit;

                    return ReturnTo;
                }

                static UInt32 TSFB()
                {
                    int TSFBit = 2;

                    UInt32 ReturnTo = (UInt32)TSFBit;

                    return ReturnTo;
                }

                static UInt32 PC()
                {
                    int PacketCount = 9; // Calculate Packet Count using the payload

                    UInt32 ReturnTo = (UInt32)PacketCount;

                    return ReturnTo;
                }

                static UInt32 PS()
                {
                    int PacketSize = 32769; // Calculate Packet Size using the payload

                    UInt32 ReturnTo = (UInt32)PacketSize;

                    return ReturnTo;
                }

                UInt32 PacketType = PT();
                PacketType = PacketType << 26;
                UInt32 ClassBitID = CBID();
                ClassBitID = ClassBitID << 25;
                UInt32 TrailerBitID = TBID();
                TrailerBitID = TrailerBitID << 24;
                UInt32 ReservedBits = RB();
                ReservedBits = ReservedBits << 22;
                UInt32 TSIBit = TSIB();
                TSIBit = TSIBit << 20;
                UInt32 TSFBit = TSFB();
                TSFBit = TSFBit << 18;
                UInt32 PacketCount = PC();                
                PacketCount = PacketCount << 14;
                UInt32 PacketSize = PS();


                UInt32 Word0 = PacketType + ClassBitID + TrailerBitID + ReservedBits + TSIBit + TSFBit + PacketCount + PacketSize;
                    
                return Word0;
    

            }

        static UInt32 OSDIWord1()
            {
                UInt32 ToReturn = 1073741825;

                return ToReturn;
            }

        static UInt32 OSDIWord2()
            {
               UInt32 OUI = 8388608;

               return OUI;
            }

        static UInt32 OSDIWord3()
            {
                UInt32 ICC = 16384;
                ICC = ICC << 14;

                UInt32 PCC = 16384;

                UInt32 thirdword = ICC + PCC;

                return thirdword;
            }

        static UInt32 OSDIWord4()
            {
                UInt32 IS = 1073741825;                

                return IS;
            }

        static UInt32 OSDIWord5()
            {
                 UInt32 FSM = 1073741825;

                 return FSM;
            }

        static UInt32 OSDIWord6()
            {
                 UInt32 FSL = 1073741825;

                 return FSL;
            }

        static List<float> GetDatapointsFromFile(string filename)
            {
                List<float> toReturn = new List<float>();

                using (StreamReader sr = new StreamReader(filename))
                {
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (float.TryParse(s, out float value))
                        {
                            toReturn.Add(value);
                        }
                    }
                }
                return toReturn;
            }

        static UInt32 OSDIWord364()
            {
                static UInt32 Enables()
                {
                    int en = 1;

                    UInt32 ReturnTo = (UInt32)en;

                    return ReturnTo;
                }

                static UInt32 SandEInd()
                {
                    int sei = 1;

                    UInt32 ReturnTo = (UInt32)sei;

                    return ReturnTo;
                }

                static UInt32 EBit()
                {
                    UInt32 enablebit = 0;

                    return enablebit;
                }

                static UInt32 ACPC()
                {
                    int ACtxtPktCount = 2;

                    UInt32 ReturnTo = (UInt32)ACtxtPktCount;

                    return ReturnTo;
                }

                UInt32 EnablesBits = Enables();
                EnablesBits = EnablesBits << 18;

                UInt32 StatesandEventsInd = SandEInd();
                StatesandEventsInd = StatesandEventsInd << 6;

                UInt32 EnableBit = EBit();
                EnableBit = EnableBit << 5;

                UInt32 ACPktCount = ACPC();

                UInt32 Word364 = EnablesBits + StatesandEventsInd + EnableBit + ACPktCount;

                return Word364;
                
            }
        
        static void SendUdpPacket(int localPortNumber, byte[] dataToSend, int destinationPortNumber, string destinationIpOrHostname)
            {
                using (UdpClient client = new UdpClient(localPortNumber))
                {
                    client.Send(dataToSend, dataToSend.Length, destinationIpOrHostname, destinationPortNumber);
                }
            }

            static async Task<UdpReceiveResult> ReceiveData(int portNumberToListenOn)
            {
                using (UdpClient client = new UdpClient(portNumberToListenOn))
                {
                    return await client.ReceiveAsync();
                }
            }

            static byte[] ConvertFloatArrayToByteArray(float[] toConvert)
            {
                byte[] toReturn = new byte[toConvert.Length * sizeof(float)];
                int toReturnIndex = 0;

                foreach (var value in toConvert)
                {
                    var bytes = BitConverter.GetBytes(value);
                    foreach (var b in bytes)
                    {
                        toReturn[toReturnIndex] = b;
                        toReturnIndex++;
                    }
                }

                return toReturn;
            }
    }
}
