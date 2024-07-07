using System;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Reflection;
using CRC32dll;
using HEXToBIN;
using System.Windows.Forms.VisualStyles;




namespace HMI_Flasher
{
   
    public class LoadXML_HMI
    {
        private string startAddress;
        private string Binpath;
        private long dataLength;
        private string startAddresssbl;
        private string Binpathsbl;
        private long dataLengthsbl;
        UInt16 CANID = 0x7F0;
        UInt32 Response_ID = 0x7F1;

        //string filePath = "C:\\Users\\karthik\\Downloads\\BinFiles\\app_bin_file.hex.bin";
        private static int globalValue; // Declare global variable
        string inputFileName = "C:\\Users\\karthik\\Downloads\\ZF_project_V1withFF.hex";
        string inputFileName1= "C:\\Users\\karthik\\Downloads\\VCU_BSW_V4_SBL.hex";
        string outputFileNamePrefix = "F:\\FlasherBinfile\\data_after";
        string outputFileNamePrefixsbl = "F:\\FlasherBinfile\\data_aftersbl";


        public void LoadXMLOf_HMI()
        {
            //binFileInfo
            #region Binfile Creation
            BinFile binFileInfocode = ClassBinToHex.ProcessFile(inputFileName, outputFileNamePrefix);
              // Access the start address and length of bin file data
            startAddress = binFileInfocode.Startaddress;
            dataLength = binFileInfocode.bindatalength;
            Binpath = binFileInfocode.Path;
            BinFile binFileInfoSbl= ClassBinToHex.ProcessFile(inputFileName1, outputFileNamePrefixsbl);
            // Access the start address and length of bin file data
            startAddresssbl = binFileInfoSbl.Startaddress;
            dataLengthsbl = binFileInfoSbl.bindatalength;
            Binpathsbl = binFileInfoSbl.Path;
            #endregion




            #region Loading Templet

            string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
            XDocument xmlDoc = XDocument.Load(filePath);
            XElement rootElement = xmlDoc.Root;
          

            if (rootElement != null)
            {

                #region ProgrammingProcedure
                if (rootElement.Elements("ProgrammingProcedure").Any())
                {
                
                    foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                    {
                   
                        if (programmingProcedureElement.Elements("PreProgrammingProcedure").Any())
                        {
                            var preProgrammingProcedureElement = programmingProcedureElement.Element("PreProgrammingProcedure");

                            if (preProgrammingProcedureElement != null)
                            {
                                Programing(preProgrammingProcedureElement);
                            }
                        }

                     
                        if (programmingProcedureElement.Elements("DataBlockProgrammingProcedure").Any())
                        {
                            var preProgrammingProcedureElement = programmingProcedureElement.Element("DataBlockProgrammingProcedure");
                            if (preProgrammingProcedureElement != null)
                            {
                                DataBlock_Programing(preProgrammingProcedureElement);
                            }

                        }


                        if (programmingProcedureElement.Elements("PostProgrammingProcedure").Any())
                        {
                            var preProgrammingProcedureElement = programmingProcedureElement.Element("PostProgrammingProcedure");
                            if (preProgrammingProcedureElement != null)
                            {
                                Post_ProgramimgProcedure(preProgrammingProcedureElement);
                            }

                        }


                        if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                        {
                            var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                            if (preProgrammingProcedureElement != null)
                            {

                                Fail_ProgramimgProcedure(programmingProcedureElement);

                            }
                        }
                       
                    }

                }
                #endregion

            }
            else
            {
                Console.WriteLine("No services found in XML.");
            }

            #endregion
        }






        #region UDS SERVICES


        private int UDS_SERVICE_11(XElement serviceNode)
        {
            if (serviceNode != null)
            {
                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string resetTypeValue = serviceNode.Element("Request")?.Element("ResetType")?.Value;
                if (resetTypeValue != null)
                {
                    // Remove "0x" prefix if present
                    resetTypeValue = resetTypeValue.StartsWith("0x") ? resetTypeValue.Substring(2) : resetTypeValue;

                    if (byte.TryParse(resetTypeValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte resetType))
                    {
                        data.Add(resetType);
                    }
                }

                CANCONNECTION.SendCANData(data, CANID);
                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();

                if (data[1] == 83)
                {
                    return 0;

                }
                else if (dataArray[0] == 0x7F)
                {

                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }


            }



            return 1;
        }

        private int UDS_SERVICE_37(XElement serviceNode)
        {
            if (serviceNode == null)
            {

                Console.WriteLine("Node is not present");
            }
            else
            {

                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }
                var optionalBytesNode = serviceNode.Element("Request")?.Element("OptionalBytes");
                if (optionalBytesNode != null)
                {
                    string usedValue = optionalBytesNode.Element("Used")?.Value;
                    if (bool.TryParse(usedValue, out bool isUsed) && isUsed)
                    {
                        var byteElements = optionalBytesNode.Elements("Byte");
                        foreach (var byteElement in byteElements)
                        {
                            string byteValue = byteElement.Element("Value")?.Value;
                            if (!string.IsNullOrEmpty(byteValue))
                            {
                                // Remove '0x' if present
                                byteValue = byteValue.Replace("0x", "");

                                // Try parsing the byte value
                                if (byte.TryParse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte parsedByte))
                                {
                                    // Add the parsed byte to the data list
                                    data.Add(parsedByte);
                                }
                            }
                        }
                    }
                }

                CANCONNECTION.SendCANData(data, CANID);

                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();
                if (dataArray[0] == 0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);

                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }

            }
            return 1;
        }

        private int UDS_SERVICE_36(XElement serviceNode)
        {
            if (serviceNode != null && File.Exists(Binpath))
            {
                List<byte> data = new List<byte>();

                data.Clear();

                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string blockSequenceCounterValue = serviceNode.Element("Request")?.Element("BlockSequenceCounter")?.Value;
                if (blockSequenceCounterValue != null)
                {
                    // Remove "0x" prefix if present
                    blockSequenceCounterValue = blockSequenceCounterValue.StartsWith("0x") ? blockSequenceCounterValue.Substring(2) : blockSequenceCounterValue;

                    if (byte.TryParse(blockSequenceCounterValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte blockSequenceCounter))
                    {
                        data.Add(blockSequenceCounter);
                    }
                }
               
                using (FileStream fs = new FileStream(Binpath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        byte[] buffer = new byte[dataLength];


                        int bytesRead;
                        byte SN_counter = 0;
                       
                        //UInt64 Number_of_bytes_sent = 0;
                        UInt64 Number_of_bytes_Total = (UInt64)fs.Length;
                        //UInt64 Result = ((UInt64)dataLength / (UInt64)globalValue);
                       // UInt64 Result2 = Result + 2;
                       bytesRead = reader.Read(buffer, 0, (int)Math.Min(dataLength, buffer.Length));

                        UInt64 Result = ((UInt64)dataLength % ((UInt64)globalValue - 2) == 0)
                                           ? ((UInt64)dataLength / ((UInt64)globalValue - 2))
                                           : (((UInt64)dataLength / ((UInt64)globalValue - 2)) + 1);
                        int index = 0;

                        for (UInt64 i =0; i< Result; i++)
                        {
                           
                            if (bytesRead > 0)
                            {
                                data.Clear();
                                data.Add(0x36);
                                data.Add(SN_counter);
                                for (UInt32 j = 0; j < globalValue - 2; j++)
                                {
                                    data.Add(buffer[index]);
                                    index++;
                                    if (index == bytesRead)
                                    {
                                        break;
                                    }

                                }
                               
                                CANCONNECTION.SendCANData(data, CANID);

                                //Number_of_bytes_sent += (UInt64)bytesRead;
                                if (SN_counter == 0xFF)
                                {
                                    SN_counter = 0;
                                }
                                else
                                {
                                    SN_counter++;
                                }
                            }
                            else
                            {
                                // If no bytes were read, it means we've reached the end of the file or there's no more data to send.
                                // You might want to handle this case differently, depending on your application's requirements.
                                // For example, you could break out of the loop or log a message.
                                break; // This will exit the loop if no more data is available to read.
                            }
                        }



                        List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                        byte[] dataArray = responseArray.ToArray();

                        if (dataArray[0] == 0x7F)
                        {
                            string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                            XDocument xmlDoc = XDocument.Load(filePath);
                            XElement rootElement = xmlDoc.Root;
                            if (rootElement != null)
                            {
                                if (rootElement.Elements("ProgrammingProcedure").Any())
                                {
                                    //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                                    foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                                    {
                                        if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                        {
                                            var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                            if (preProgrammingProcedureElement != null)
                                            {
                                                Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                                Environment.Exit(0);
                                            }
                                        }
                                    }
                                }
                            }
                            return 0;

                        }
                        data.Clear();



                        fs.Close();

                    }



                }

            }
            return 1;
        }
        private int UDS_SERVICE_36_sbl(XElement serviceNode)
        {
            if (serviceNode != null && File.Exists(Binpathsbl))
            {
                List<byte> data = new List<byte>();

                data.Clear();

                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string blockSequenceCounterValue = serviceNode.Element("Request")?.Element("BlockSequenceCounter")?.Value;
                if (blockSequenceCounterValue != null)
                {
                    // Remove "0x" prefix if present
                    blockSequenceCounterValue = blockSequenceCounterValue.StartsWith("0x") ? blockSequenceCounterValue.Substring(2) : blockSequenceCounterValue;

                    if (byte.TryParse(blockSequenceCounterValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte blockSequenceCounter))
                    {
                        data.Add(blockSequenceCounter);
                    }
                }

                using (FileStream fs = new FileStream(Binpathsbl, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        byte[] buffer = new byte[dataLengthsbl+2];


                        int bytesRead;
                        byte SN_counter = 0;

                        //UInt64 Number_of_bytes_sent = 0;
                        UInt64 Number_of_bytes_Total = (UInt64)fs.Length;
                        //UInt64 Result = ((UInt64)dataLength / (UInt64)globalValue);
                        // UInt64 Result2 = Result + 2;
                        //bytesRead = reader.Read(buffer, 0, (int)Math.Min(dataLengthsbl, buffer.Length));
                        bytesRead = reader.Read(buffer, 0, buffer.Length);

                        UInt64 Result = ((UInt64)dataLengthsbl % ((UInt64)globalValue - 2) == 0)
                                        ? ((UInt64)dataLengthsbl / ((UInt64)globalValue - 2))
                                        : (((UInt64)dataLengthsbl / ((UInt64)globalValue - 2)) + 1);
                        int index = 0;

                        for (UInt64 i = 0; i < Result; i++)
                        {

                            if (bytesRead > 0)
                            {
                                data.Clear();
                                data.Add(0x36);
                                data.Add(SN_counter);
                                for (UInt32 j = 0; j < globalValue; j++)
                                {


                                    data.Add(buffer[index]);

                                 //   data.Add(reader);
                                    index++;
                                    if (index == bytesRead)
                                    {
                                        break;
                                    }

                                }

                                CANCONNECTION.SendCANData(data, CANID);

                                //Number_of_bytes_sent += (UInt64)bytesRead;
                                if (SN_counter == 0xFF)
                                {
                                    SN_counter = 0;
                                }
                                else
                                {
                                    SN_counter++;
                                }
                            }
                            else
                            {
                                // If no bytes were read, it means we've reached the end of the file or there's no more data to send.
                                // You might want to handle this case differently, depending on your application's requirements.
                                // For example, you could break out of the loop or log a message.
                                break; // This will exit the loop if no more data is available to read.
                            }
                        }



                        List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                        byte[] dataArray = responseArray.ToArray();

                        if (dataArray[0] == 0x7F)
                        {
                            string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                            XDocument xmlDoc = XDocument.Load(filePath);
                            XElement rootElement = xmlDoc.Root;
                            if (rootElement != null)
                            {
                                if (rootElement.Elements("ProgrammingProcedure").Any())
                                {
                                    //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                                    foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                                    {
                                        if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                        {
                                            var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                            if (preProgrammingProcedureElement != null)
                                            {
                                                Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                                Environment.Exit(0);
                                            }
                                        }
                                    }
                                }
                            }
                            return 0;

                        }
                        data.Clear();



                        fs.Close();

                    }



                }

                


            }
            return 1;
        }

        private int UDS_SERVICE_34(XElement serviceNode)
        {
            if (serviceNode != null)
            {
                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string dataFormatIdentifier = serviceNode.Element("Request")?.Element("DataFormatIdentifier")?.Value;
                if (dataFormatIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    dataFormatIdentifier = dataFormatIdentifier.StartsWith("0x") ? dataFormatIdentifier.Substring(2) : dataFormatIdentifier;

                    if (byte.TryParse(dataFormatIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte dataFormat))
                    {
                        data.Add(dataFormat);
                    }
                }

                string addressAndLengthFormatIdentifier = serviceNode.Element("Request")?.Element("AddressAndLengthFormatIdentifier")?.Value;
                if (addressAndLengthFormatIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    addressAndLengthFormatIdentifier = addressAndLengthFormatIdentifier.StartsWith("0x") ? addressAndLengthFormatIdentifier.Substring(2) : addressAndLengthFormatIdentifier;

                    if (byte.TryParse(addressAndLengthFormatIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte addressAndLength))
                    {
                        data.Add(addressAndLength);
                    }
                }



                string MemoryAddress = serviceNode.Element("Request")?.Element("MemoryAddress")?.Value;
                if (!string.IsNullOrEmpty(MemoryAddress))
                {
                    // Check if the MemoryAddress node contains a <Length> element
                    var memoryAddressLengthElement = serviceNode.Element("Request")?.Element("MemoryAddress")?.Element("Lenght");
                    if (memoryAddressLengthElement != null)
                    {
                        int length = int.Parse(memoryAddressLengthElement.Value); // Parse the length value

                        // Split the MemoryAddress into chunks of two characters and convert them to bytes
                        byte[] addressBytes = new byte[length];

                        // Fill the first two bytes with zeroes
                        addressBytes[0] = 0x00;
                        addressBytes[1] = 0x00;

                        // Start storing the address bytes from index 2
                        for (int i = 0; i < length; i += 2)
                        {
                            // Skip the first two characters (00) and start from index 2
                            string hexByte = startAddress.Substring(i, 2);
                            if (i == 0)
                            {
                                // Extract the first two bytes from the start address
                                addressBytes[2] = Convert.ToByte(hexByte, 16);
                                addressBytes[3] = Convert.ToByte(startAddress.Substring(2, 2), 16);
                            }

                        }

                        // Add the addressBytes to your data array
                        data.AddRange(addressBytes);
                    }
                    else
                    {
                        // Convert the hexadecimal string into a byte array
                        byte[] addressBytes = MemoryAddress.Select(x => Convert.ToByte(x.ToString(), 16)).ToArray();

                        // Add the bytes to your data array
                        data.AddRange(addressBytes);
                    }
                }

                string MemorySize = serviceNode.Element("Request")?.Element("MemorySize")?.Value;
                if (!string.IsNullOrEmpty(MemorySize))
                {

                    // Check if the MemoryAddress node contains a <Length> element
                    var memorySizeLengthElement = serviceNode.Element("Request")?.Element("MemorySize")?.Element("Lenght");
                    if (memorySizeLengthElement != null)
                    {
                        int length = int.Parse(memorySizeLengthElement.Value); // Parse the length value

                        // Convert data length to hexadecimal string
                        string dataLengthHex = dataLength.ToString("X");

                        // Ensure the hexadecimal string has an even number of characters
                        if (dataLengthHex.Length % 2 != 0)
                        {
                            dataLengthHex = "0" + dataLengthHex; // Add a leading zero if necessary
                        }

                        // Split the hexadecimal string into two parts
                        string highByteHex = dataLengthHex.Substring(0, 2);
                        string lowByteHex = dataLengthHex.Substring(2, 2);

                        // Convert each part into its byte representation and add to the list
                        byte highByte = Convert.ToByte(highByteHex, 16);
                        byte lowByte = Convert.ToByte(lowByteHex, 16);

                        data.Add(0x00); // Add a placeholder byte (0x00) for the first byte
                        data.Add(0x00);
                        data.Add(highByte); // Add the high byte
                        data.Add(lowByte); // Add the low byte
                    }
                    else
                    {
                        // Convert the hexadecimal string into a byte array
                        byte[] addressBytes = MemorySize.Select(x => Convert.ToByte(x.ToString(), 16)).ToArray();

                        // Add the bytes to your data array
                        data.AddRange(addressBytes);
                    }
                }

                CANCONNECTION.SendCANData(data, CANID);

                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();

                if (dataArray.Length >= 2)
                {
                    // Extract the last 2 bytes
                    byte[] lastTwoBytes = new byte[2];
                    Array.Copy(dataArray, dataArray.Length - 2, lastTwoBytes, 0, 2);

                    // Convert bytes to a single ushort
                    ushort value = (ushort)((lastTwoBytes[0] << 8) | lastTwoBytes[1]);

                    // Convert the ushort value to its decimal equivalent
                    int decimalValue = Convert.ToInt32(value);

                    // Assign value to the global variable
                    globalValue = decimalValue;

                    //Console.WriteLine("globalValue:" + globalValue);


                }






                if (dataArray[0] == 0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }

                data.Clear();
            }
            //CANCONNECTION.Stop_Filter((UInt32)Response_ID);
            return 1;
        }
        private int UDS_SERVICE_34_sbl(XElement serviceNode)
        {
            if (serviceNode != null)
            {
                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H') ;

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string dataFormatIdentifier = serviceNode.Element("Request")?.Element("DataFormatIdentifier")?.Value;
                if (dataFormatIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    dataFormatIdentifier = dataFormatIdentifier.StartsWith("0x") ? dataFormatIdentifier.Substring(2) : dataFormatIdentifier;

                    if (byte.TryParse(dataFormatIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte dataFormat))
                    {
                        data.Add(dataFormat);
                    }
                }

                string addressAndLengthFormatIdentifier = serviceNode.Element("Request")?.Element("AddressAndLengthFormatIdentifier")?.Value;
                if (addressAndLengthFormatIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    addressAndLengthFormatIdentifier = addressAndLengthFormatIdentifier.StartsWith("0x") ? addressAndLengthFormatIdentifier.Substring(2) : addressAndLengthFormatIdentifier;

                    if (byte.TryParse(addressAndLengthFormatIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte addressAndLength))
                    {
                        data.Add(addressAndLength);
                    }
                }



                string MemoryAddress = serviceNode.Element("Request")?.Element("MemoryAddress")?.Value;
                if (!string.IsNullOrEmpty(MemoryAddress))
                {
                    // Check if the MemoryAddress node contains a <Length> element
                    var memoryAddressLengthElement = serviceNode.Element("Request")?.Element("MemoryAddress")?.Element("Lenght");
                    if (memoryAddressLengthElement != null)
                    {
                        int length = int.Parse(memoryAddressLengthElement.Value); // Parse the length value
                        if (length == 4)
                        {
                            byte[] addressBytes = new byte[length];

                            // Manually set the first two bytes
                            addressBytes[0] = 0xFE;
                            addressBytes[1] = 0xDE;

                            // Ensure startAddresssbl is long enough to contain the remaining 4 characters
                            if (startAddresssbl.Length >= 4)
                            {
                                // Parse the remaining two bytes from startAddresssbl
                                addressBytes[2] = 0x40;
                                addressBytes[3] = 0x00;
                            }
                            else
                            {
                                // Handle error if startAddresssbl is not long enough
                                throw new ArgumentException("startAddresssbl must be at least 4 characters long.");
                            }

                            // Add the addressBytes to your data array
                            data.AddRange(addressBytes);
                        }
                        else
                        {
                            // Handle error if the length is not 4 bytes
                            throw new ArgumentException("MemoryAddress length must be 4 bytes.");
                        }
                    }
                    else
                    {
                        // Convert the hexadecimal string into a byte array
                        byte[] addressBytes = MemoryAddress.Select(x => Convert.ToByte(x.ToString(), 16)).ToArray();

                        // Add the bytes to your data array
                        data.AddRange(addressBytes);
                    }
                }

                string MemorySize = serviceNode.Element("Request")?.Element("MemorySize")?.Value;
                if (!string.IsNullOrEmpty(MemorySize))
                {

                    // Check if the MemoryAddress node contains a <Length> element
                    var memorySizeLengthElement = serviceNode.Element("Request")?.Element("MemorySize")?.Element("Lenght");
                    if (memorySizeLengthElement != null)
                    {
                        int length = int.Parse(memorySizeLengthElement.Value); // Parse the length value

                        // Convert data length to hexadecimal string
                        string dataLengthHex = dataLengthsbl.ToString("X");

                        // Ensure the hexadecimal string has an even number of characters
                        if (dataLengthHex.Length % 2 != 0)
                        {
                            dataLengthHex = "0" + dataLengthHex; // Add a leading zero if necessary
                        }

                        // Split the hexadecimal string into two parts
                        string highByteHex = dataLengthHex.Substring(0, 2);
                        string lowByteHex = dataLengthHex.Substring(2, 2);

                        // Convert each part into its byte representation and add to the list
                        byte highByte = Convert.ToByte(highByteHex, 16);
                        byte lowByte = Convert.ToByte(lowByteHex, 16);

                        data.Add(0x00); // Add a placeholder byte (0x00) for the first byte
                        data.Add(0x00);
                        data.Add(highByte); // Add the high byte
                        data.Add(lowByte); // Add the low byte
                    }
                    else
                    {
                        // Convert the hexadecimal string into a byte array
                        byte[] addressBytes = MemorySize.Select(x => Convert.ToByte(x.ToString(), 16)).ToArray();

                        // Add the bytes to your data array
                        data.AddRange(addressBytes);
                    }
                }

                CANCONNECTION.SendCANData(data, CANID);

                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();

                if (dataArray.Length >= 2)
                {
                    // Extract the last 2 bytes
                    byte[] lastTwoBytes = new byte[2];
                    Array.Copy(dataArray, dataArray.Length - 2, lastTwoBytes, 0, 2);

                    // Convert bytes to a single ushort
                    ushort value = (ushort)((lastTwoBytes[0] << 8) | lastTwoBytes[1]);

                    // Convert the ushort value to its decimal equivalent
                    int decimalValue = Convert.ToInt32(value);

                    // Assign value to the global variable
                    globalValue = decimalValue;

                    //Console.WriteLine("globalValue:" + globalValue);


                }






                if (dataArray[0] == 0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }

                data.Clear();
            }
            //CANCONNECTION.Stop_Filter((UInt32)Response_ID);
            return 1;
        }

        private int UDS_SERVICE_2E(XElement serviceNode)
        {

            if (serviceNode == null)
            {

                Console.WriteLine("Node is not present");
            }
            else
            {

                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }
                string dataIdentifier = serviceNode.Element("Request")?.Element("DataIdentifier")?.Value;
                if (dataIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    dataIdentifier = dataIdentifier.StartsWith("0x") ? dataIdentifier.Substring(2) : dataIdentifier;

                    if (ushort.TryParse(dataIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort identifier))
                    {
                        // Convert ushort to byte array (2 bytes)
                        byte[] identifierBytes = BitConverter.GetBytes(identifier);

                        // If BitConverter.IsLittleEndian is true, reverse the byte array
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(identifierBytes);

                        // Add the bytes to your data array
                        data.AddRange(identifierBytes);
                    }
                }
                string writeDataBytes = serviceNode.Element("Request")?.Element("Write_Data_Bytes")?.Value;
                if (!string.IsNullOrEmpty(writeDataBytes))
                {
                    // Remove double quotation marks if present
                    writeDataBytes = writeDataBytes.Trim('"');

                    if (writeDataBytes.ToUpper() == "TODAY")
                    {
                        // Get the current date in the desired format
                        string todayDateString = DateTime.Now.ToString("MMddyyyy");
                        foreach (char c in todayDateString)
                        {
                            // Convert the character to its hexadecimal byte representation
                            byte hexByte = Convert.ToByte(c.ToString(), 16);

                            // Add the byte to your data array
                            data.Add(hexByte);
                        }


                    }
                    else
                    {
                        // Convert the hexadecimal string into a byte array
                        byte[] dataBytes = writeDataBytes.Select(x => Convert.ToByte(x.ToString(), 16)).ToArray();


                        // Add the bytes to your data array
                        data.AddRange(dataBytes);
                    }

                   
                }



                CANCONNECTION.SendCANData(data, CANID);
                data.Clear();

                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();
                if (dataArray[0]==7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                }


            }

            //    CANCONNECTION.Stop_Filter((UInt32)Response_ID);
            return 1;
        }

        private int UDS_SERVICE_85(XElement serviceNode)
        {


            if (serviceNode != null)
            {
                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string sessionTypeValue = serviceNode.Element("Request")?.Element("SessionType")?.Value;
                if (sessionTypeValue != null)
                {
                    // Remove "0x" prefix if present
                    sessionTypeValue = sessionTypeValue.StartsWith("0x") ? sessionTypeValue.Substring(2) : sessionTypeValue;

                    if (byte.TryParse(sessionTypeValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sessionType))
                    {
                        data.Add(sessionType);
                    }
                }

                CANCONNECTION.SendCANData(data, CANID);
              

                if (data[1] == 83)
                {
                    List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                    byte[] dataArray = responseArray.ToArray();

                    
                }
               
                else if (data[0]==0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }


            }

           // CANCONNECTION.Stop_Filter((UInt32)Response_ID);

            return 1;

        }

        public int UDS_SERVICE_10(XElement serviceNode)
        {
           
            if (serviceNode != null)
            {
                List<byte> data = new List<byte>();

                data.Clear();

                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string sessionTypeValue = serviceNode.Element("Request")?.Element("SessionType")?.Value;
                if (sessionTypeValue != null)
                {
                    // Remove "0x" prefix if present
                    sessionTypeValue = sessionTypeValue.StartsWith("0x") ? sessionTypeValue.Substring(2) : sessionTypeValue;

                    if (byte.TryParse(sessionTypeValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sessionType))
                    {
                        data.Add(sessionType);
                    }
                }

                CANCONNECTION.SendCANData(data, CANID);
               
                if (data[1] == 0x83 || data[1] == 0x82 || data[1] == 0x81)
                {

                    return 0;
                    
                }
               
                else
                {
                   
                    List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                    byte[] dataArray = responseArray.ToArray();
                    if (dataArray.Length > 0)
                    {
                        if (dataArray[0] != 0x50 && dataArray[0] == 0x7F)
                        {
                            string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                            XDocument xmlDoc = XDocument.Load(filePath);
                            XElement rootElement = xmlDoc.Root;
                            if (rootElement != null)
                            {
                                if (rootElement.Elements("ProgrammingProcedure").Any())
                                {
                                    //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                                    foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                                    {
                                        if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                        {
                                            var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                            if (preProgrammingProcedureElement != null)
                                            {
                                                Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                                Environment.Exit(0);
                                            }
                                        }
                                    }
                                }
                            }
                            return 0;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        // Handle the case where dataArray is empty
                    }

                }

                data.Clear();
            }
         
           // CANCONNECTION.Stop_Filter((UInt32)Response_ID);

            return 1;
        }

        public int UDS_SERVICE_27(XElement serviceNode)
        {

            string sidValue = serviceNode.Element("SID")?.Value;
            if (sidValue != null)
            {
                // Remove double quotation marks and 'H' if present
                sidValue = sidValue.Trim('"', 'H');

                // Try parsing the SID value as a byte
                if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                {
                    // Add the parsed SID to the data list
                    List<byte> data_1 = new List<byte> { };
                    List<byte> data_2 = new List<byte> { };


                    data_1.Add(sid);
                    byte[] dataArray = { };
                    foreach (var requestNode in serviceNode.Elements("Request"))
                    {
                        string requestingValue = requestNode.Element("Requesting")?.Value;
                        requestingValue = requestingValue.Trim('"');
                        
                        string securityLevelValue = requestNode.Element("SecurityLevel")?.Value;
                      

                        if (requestingValue == "Seed" && securityLevelValue == "ONE")
                        {
                            // Modify the security level to 0x01
                            data_1.Add(0x01);

                            // Send the "Seed" request
                            CANCONNECTION.SendCANData(data_1, CANID);

                            List<byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);
                            dataArray = responseArray.ToArray();

                        }

                        else if (requestingValue == "Key" && securityLevelValue == "ONE")
                        {

                            data_2.Add(sid);
                            data_2.Add(0x02);
                            // Add one's complement values to data_2
                            data_2.Add((byte)~dataArray[2]); 
                            data_2.Add((byte)~dataArray[3]); 
                            data_2.Add((byte)~dataArray[4]);
                            data_2.Add((byte)~dataArray[5]);

                            // Send the modified data twice for "Key" request

                            CANCONNECTION.SendCANData(data_2, CANID);
                            // Wait for response
                            List<byte> responseArray_1 = CANCONNECTION.Read_CAN_data(CANID, 50);
                            byte[] dataArray_1 = responseArray_1.ToArray();

                            if (dataArray_1[0] == 0x67 && dataArray_1[1] == 0x02 || dataArray_1[0] == 0x67 && dataArray_1[1] == 0x01)
                            {
                                return 1;
                            }
                            else if (dataArray_1[0]==0x7F)
                            {
                                string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                                XDocument xmlDoc = XDocument.Load(filePath);
                                XElement rootElement = xmlDoc.Root;
                                if (rootElement != null)
                                {
                                    if (rootElement.Elements("ProgrammingProcedure").Any())
                                    {
                                        //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                                        foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                                        {
                                            if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                            {
                                                var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                                if (preProgrammingProcedureElement != null)
                                                {
                                                    Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                                    Environment.Exit(0);
                                                }
                                            }
                                        }
                                    }
                                }
                                return 0;
                            }
                        }
                    }
                }

            }
            return 1;

        }



        public int UDS_SERVICE_28(XElement serviceNode)
        {
            if (serviceNode != null)
            {
                List<byte> data = new List<byte>();


                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                    }
                }

                // Add Session Type from XML
                string sessionTypeValue = serviceNode.Element("Request")?.Element("SessionType")?.Value;
                if (sessionTypeValue != null)
                {
                    // Remove "0x" prefix if present
                    sessionTypeValue = sessionTypeValue.StartsWith("0x") ? sessionTypeValue.Substring(2) : sessionTypeValue;

                    if (byte.TryParse(sessionTypeValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sessionType))
                    {
                        data.Add(sessionType);
                    }
                }
                data.Add(0x01);
                CANCONNECTION.SendCANData(data, CANID);
                if (data[1] == 81)
                {
                    List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                    byte[] dataArray = responseArray.ToArray();

                   
                    
                }
                else if (data[1]==0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }

            }

            return 1;
        }


        private int UDS_SERVICE_31(XElement serviceNode)
        {
            List<byte> data = new List<byte>();
            List<byte> data_Array = new List<byte>();

            if (serviceNode != null)
            {

                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');

                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);
                        
                    }
                }

                // Add Session Type from XML
                string routineControlType = serviceNode.Element("Request")?.Element("RoutineControlType")?.Value;
                if (routineControlType != null)
                {
                    // Remove "0x" prefix if present
                    routineControlType = routineControlType.StartsWith("0x") ? routineControlType.Substring(2) : routineControlType;

                    if (byte.TryParse(routineControlType, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte controlType))
                    {
                        data.Add(controlType);
                    }
                }
                string routineIdentifier = serviceNode.Element("Request")?.Element("RoutineIdentifier")?.Value;
                if (routineIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    routineIdentifier = routineIdentifier.StartsWith("0x") ? routineIdentifier.Substring(2) : routineIdentifier;

                    if (ushort.TryParse(routineIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort identifier))
                    {
                        // Convert ushort to byte array (2 bytes)
                        byte[] identifierBytes = BitConverter.GetBytes(identifier);

                        // If BitConverter.IsLittleEndian is true, reverse the byte array
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(identifierBytes);

                        // Add the bytes to your data array
                        data.AddRange(identifierBytes);
                       
                    }
                }

                var routineControlOptionRecord = serviceNode.Element("Request")?.Element("RoutineControlOptionRecord");

                if (routineControlOptionRecord != null)
                {
                    var usedElement = routineControlOptionRecord.Element("Used");

                    if (usedElement != null && usedElement.Value.ToLower() == "true")
                    {



                        var startAddressElement = routineControlOptionRecord.Element("StartAddress");
                        var usedElement1 = startAddressElement.Element("Used");
                        if (usedElement1 != null && usedElement1.Value.ToLower()=="true")
                        {
                            // Check if the StartAddress node contains a <Length> element
                            var startAddressLengthElement = startAddressElement.Element("Lenght");
                            if (startAddressLengthElement != null)
                            {
                                int length = int.Parse(startAddressLengthElement.Value); // Parse the length value
                                                                                         // Access the start address and length of bin file data

                                // Split the StartAddress into chunks of two characters and convert them to bytes
                                byte[] addressBytes = new byte[length];

                                // Fill the first two bytes with zeroes
                                addressBytes[0] = 0x00;
                                addressBytes[1] = 0x00;

                                // Start storing the address bytes from index 2
                                for (int i = 0; i < length; i += 2)
                                {
                                    // Skip the first two characters (00) and start from index 2
                                    string hexByte = startAddress.Substring(i, 2);
                                    if (i == 0)
                                    {
                                        // Extract the first two bytes from the start address
                                        addressBytes[2] = Convert.ToByte(hexByte, 16);
                                        addressBytes[3] = Convert.ToByte(startAddress.Substring(2, 2), 16);
                                    }

                                }

                                // Add the addressBytes to your data array
                                data.AddRange(addressBytes);




                            }
                        }

                        var sizeElement = routineControlOptionRecord.Element("Size");
                        var usedElement2 = sizeElement.Element("Used");
                        if (usedElement2 != null && usedElement2.Value.ToLower() == "true")
                        {
                            // Check if the Size node contains a <Length> element
                            var sizeLengthElement = sizeElement.Element("Lenght");
                            if (sizeLengthElement != null)
                            {
                                int length = int.Parse(sizeLengthElement.Value); // Parse the length value


                                // Convert data length to hexadecimal string
                                string dataLengthHex = dataLength.ToString("X");

                                // Ensure the hexadecimal string has an even number of characters
                                if (dataLengthHex.Length % 2 != 0)
                                {
                                    dataLengthHex = "0" + dataLengthHex; // Add a leading zero if necessary
                                }

                                // Split the hexadecimal string into two parts
                                string highByteHex = dataLengthHex.Substring(0, 2);
                                string lowByteHex = dataLengthHex.Substring(2, 2);

                                // Convert each part into its byte representation and add to the list
                                byte highByte = Convert.ToByte(highByteHex, 16);
                                byte lowByte = Convert.ToByte(lowByteHex, 16);

                                data.Add(0x00); // Add a placeholder byte (0x00) for the first byte
                                data.Add(0x00);
                                data.Add(highByte); // Add the high byte
                                data.Add(lowByte); // Add the low byte
                            }
                        }
                        var optionalBytesElement = routineControlOptionRecord.Element("OptionalBytes");
                        if (optionalBytesElement != null)
                        {
                            var usedElement3 = optionalBytesElement.Element("Used");
                            if (usedElement3 != null && bool.TryParse(usedElement3.Value, out bool isUsed) && isUsed)
                            {
                                var byteElement = optionalBytesElement.Element("Byte");
                                if (byteElement != null)
                                {
                                    var indexElement = byteElement.Element("Index");
                                    var valueElement = byteElement.Element("Value");

                                    if (indexElement != null && valueElement != null)
                                    {
                                        int index = int.Parse(indexElement.Value);
                                        byte value = Convert.ToByte(valueElement.Value, 16); // assuming the value is in hexadecimal format (0x00)

                                        // Ensure the data list is large enough to accommodate the index
                                        if (data.Count <= index)
                                        {
                                            for (int i = data.Count; i <= index; i++)
                                            {
                                                data.Add(0); // Add default byte values to expand the list
                                            }
                                        }

                                        data[index] = value; // Set the value at the specified index
                                    }
                                }
                            }
                        }
                        
                        var checksumElement = routineControlOptionRecord.Element("Checksum");
                        if (checksumElement != null)
                        {


                            UInt32 ClientCrc_u32 = 0xFFFFFFFF;
                            var checksumCRCElement = checksumElement.Element("CRC");
                            var checksumLengthElement = checksumElement.Element("Lenght");

                            if (checksumLengthElement != null)
                            {
                                int length = int.Parse(checksumLengthElement.Value); // Parse the length value

                                using (FileStream fs = new FileStream(Binpath, FileMode.Open))
                                {
                                    using (BinaryReader reader = new BinaryReader(fs))
                                    {

                                        byte[] buffer = new byte[dataLength];

                                        int bytesRead = reader.Read(buffer, 0, buffer.Length);

                                        if (bytesRead > 0)
                                        {


                                            // Calculate CRC32 checksum
                                            ClassCRC.Crc_CalculateCRC32(buffer, (UInt32)((buffer.Length / 4) * 4), ref ClientCrc_u32);

                                            // Convert checksum to byte array
                                            byte[] checksumBytes = BitConverter.GetBytes(ClientCrc_u32);

                                            if (BitConverter.IsLittleEndian)
                                                Array.Reverse(checksumBytes);

                                            // Add the checksumBytes to your data array
                                            data.AddRange(checksumBytes);


                                        }







                                    }
                                    fs.Close();
                                }




                            }







                        }
                    }

                }
               

                // Continue with the rest of your code


                CANCONNECTION.SendCANData(data, CANID);

                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();

                if (dataArray[0] != 0x71 && dataArray[0] == 0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }
            }

            //CANCONNECTION.Stop_Filter((UInt32)Response_ID);

            return 1;
        }


        private int UDS_SERVICE_31_sbl (XElement serviceNode)
        {
            List<byte> data = new List<byte>();
            List<byte> data_Array = new List<byte>();

            if (serviceNode != null)
            {

                // Add Service ID (0x10)
                string sidValue = serviceNode.Element("SID")?.Value;
                if (sidValue != null)
                {
                    // Remove double quotation marks and 'H' if present
                    sidValue = sidValue.Trim('"', 'H');
                    // Try parsing the SID value as a byte
                    if (byte.TryParse(sidValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte sid))
                    {
                        // Add the parsed SID to the data list
                        data.Add(sid);

                    }
                }

                // Add Session Type from XML
                string routineControlType = serviceNode.Element("Request")?.Element("RoutineControlType")?.Value;
                if (routineControlType != null)
                {
                    // Remove "0x" prefix if present
                    routineControlType = routineControlType.StartsWith("0x") ? routineControlType.Substring(2) : routineControlType;

                    if (byte.TryParse(routineControlType, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte controlType))
                    {
                        data.Add(controlType);
                    }
                }
                string routineIdentifier = serviceNode.Element("Request")?.Element("RoutineIdentifier")?.Value;
                if (routineIdentifier != null)
                {
                    // Remove "0x" prefix if present
                    routineIdentifier = routineIdentifier.StartsWith("0x") ? routineIdentifier.Substring(2) : routineIdentifier;

                    if (ushort.TryParse(routineIdentifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort identifier))
                    {
                        // Convert ushort to byte array (2 bytes)
                        byte[] identifierBytes = BitConverter.GetBytes(identifier);

                        // If BitConverter.IsLittleEndian is true, reverse the byte array
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(identifierBytes);

                        // Add the bytes to your data array
                        data.AddRange(identifierBytes);

                    }
                }

                var routineControlOptionRecord = serviceNode.Element("Request")?.Element("RoutineControlOptionRecord");

                if (routineControlOptionRecord != null)
                {
                    var usedElement = routineControlOptionRecord.Element("Used");

                    if (usedElement != null && usedElement.Value.ToLower() == "true")
                    {



                        var startAddressElement = routineControlOptionRecord.Element("StartAddress");
                        var usedElement1 = startAddressElement.Element("Used");
                        if (usedElement1 != null && usedElement1.Value.ToLower() == "true")
                        {
                            // Check if the StartAddress node contains a <Length> element
                            var startAddressLengthElement = startAddressElement.Element("Lenght");
                            if (startAddressLengthElement != null)
                            {
                                int length = int.Parse(startAddressLengthElement.Value); // Parse the length value
                                                                                         // Access the start address and length of bin file data

                                // Split the StartAddress into chunks of two characters and convert them to bytes
                                byte[] addressBytes = new byte[length];

                                // Fill the first two bytes with zeroes
                                addressBytes[0] = 0x00;
                                addressBytes[1] = 0x00;

                                // Start storing the address bytes from index 2
                                for (int i = 0; i < length; i += 2)
                                {
                                    // Skip the first two characters (00) and start from index 2
                                    string hexByte = startAddresssbl.Substring(i, 2);
                                    if (i == 0)
                                    {
                                        // Extract the first two bytes from the start address
                                        addressBytes[2] = Convert.ToByte(hexByte, 16);
                                        addressBytes[3] = Convert.ToByte(startAddresssbl.Substring(2, 2), 16);
                                    }

                                }

                                // Add the addressBytes to your data array
                                data.AddRange(addressBytes);




                            }
                        }

                        var sizeElement = routineControlOptionRecord.Element("Size");
                        var usedElement2 = sizeElement.Element("Used");
                        if (usedElement2 != null && usedElement2.Value.ToLower() == "true")
                        {
                            // Check if the Size node contains a <Length> element
                            var sizeLengthElement = sizeElement.Element("Lenght");
                            if (sizeLengthElement != null)
                            {
                                int length = int.Parse(sizeLengthElement.Value); // Parse the length value


                                // Convert data length to hexadecimal string
                                string dataLengthHex = dataLengthsbl.ToString("X");

                                // Ensure the hexadecimal string has an even number of characters
                                if (dataLengthHex.Length % 2 != 0)
                                {
                                    dataLengthHex = "0" + dataLengthHex; // Add a leading zero if necessary
                                }

                                // Split the hexadecimal string into two parts
                                string highByteHex = dataLengthHex.Substring(0, 2);
                                string lowByteHex = dataLengthHex.Substring(2, 2);

                                // Convert each part into its byte representation and add to the list
                                byte highByte = Convert.ToByte(highByteHex, 16);
                                byte lowByte = Convert.ToByte(lowByteHex, 16);

                                data.Add(0x00); // Add a placeholder byte (0x00) for the first byte
                                data.Add(0x00);
                                data.Add(highByte); // Add the high byte
                                data.Add(lowByte); // Add the low byte
                            }
                        }
                        var optionalBytesElement = routineControlOptionRecord.Element("OptionalBytes");
                        if (optionalBytesElement != null)
                        {
                            var usedElement3 = optionalBytesElement.Element("Used");
                            if (usedElement3 != null && bool.TryParse(usedElement3.Value, out bool isUsed) && isUsed)
                            {
                                var byteElement = optionalBytesElement.Element("Byte");
                                if (byteElement != null)
                                {
                                    var indexElement = byteElement.Element("Index");
                                    var valueElement = byteElement.Element("Value");

                                    if (indexElement != null && valueElement != null)
                                    {
                                        int index = int.Parse(indexElement.Value);
                                        byte value = Convert.ToByte(valueElement.Value, 16); // assuming the value is in hexadecimal format (0x00)

                                        // Ensure the data list is large enough to accommodate the index
                                        if (data.Count <= index)
                                        {
                                            for (int i = data.Count; i <= index; i++)
                                            {
                                                data.Add(0); // Add default byte values to expand the list
                                            }
                                        }

                                        data[index] = value; // Set the value at the specified index
                                    }
                                }
                            }
                        }
                        var checksumElement = routineControlOptionRecord.Element("Checksum");
                        if (checksumElement != null)
                        {


                            UInt32 ClientCrc_u32 = 0xFFFFFFFF;
                            var checksumCRCElement = checksumElement.Element("CRC");
                            var checksumLengthElement = checksumElement.Element("Lenght");

                            if (checksumLengthElement != null)
                            {
                                int length = int.Parse(checksumLengthElement.Value); // Parse the length value

                                using (FileStream fs = new FileStream(Binpathsbl, FileMode.Open))
                                {
                                    using (BinaryReader reader = new BinaryReader(fs))
                                    {

                                        byte[] buffer = new byte[dataLengthsbl];

                                        int bytesRead = reader.Read(buffer, 0, buffer.Length);

                                        if (bytesRead > 0)
                                        {


                                            // Calculate CRC32 checksum
                                            ClassCRC.Crc_CalculateCRC32(buffer, (UInt32)((buffer.Length / 4) * 4), ref ClientCrc_u32);

                                            // Convert checksum to byte array
                                            byte[] checksumBytes = BitConverter.GetBytes(ClientCrc_u32);

                                            if (BitConverter.IsLittleEndian)
                                                Array.Reverse(checksumBytes);

                                            // Add the checksumBytes to your data array
                                            data.AddRange(checksumBytes);


                                        }







                                    }
                                    fs.Close();
                                }




                            }







                        }
                    }

                }


                // Continue with the rest of your code


                CANCONNECTION.SendCANData(data, CANID);

                List<Byte> responseArray = CANCONNECTION.Read_CAN_data(CANID, 50);

                byte[] dataArray = responseArray.ToArray();

                if (dataArray[0] != 0x71 && dataArray[0] == 0x7F)
                {
                    string filePath = "C:\\Program Files (x86)\\Sloki\\FLASHER\\ExistingTempletes\\ZF_VCU.sTemp";
                    XDocument xmlDoc = XDocument.Load(filePath);
                    XElement rootElement = xmlDoc.Root;
                    if (rootElement != null)
                    {
                        if (rootElement.Elements("ProgrammingProcedure").Any())
                        {
                            //     CANCONNECTION.Set_Filter(CANID, Response_ID);
                            foreach (var programmingProcedureElement in rootElement.Elements("ProgrammingProcedure"))
                            {
                                if (programmingProcedureElement.Elements("FailProgrammingProcedure").Any())
                                {
                                    var preProgrammingProcedureElement = programmingProcedureElement.Element("FailProgrammingProcedure");
                                    if (preProgrammingProcedureElement != null)
                                    {
                                        Fail_ProgramimgProcedure(preProgrammingProcedureElement);
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        }
                    }
                    return 0;
                }
            }

            //CANCONNECTION.Stop_Filter((UInt32)Response_ID);

            return 1;
        }

        #endregion


        #region PROGRAMING
        public void Programing(XElement programmingProcedureElement)
        {

            if (programmingProcedureElement != null)
            {
                //Get all the<Services> nodes
                var serviceNodes = programmingProcedureElement.Elements("Services");
                // 
                // Iterate through each <Services> node
                foreach (var serviceNode in serviceNodes)
                {
                    // Extract data from the <Services> node
                    string sid = serviceNode.Element("SID")?.Value;
                    string serviceName = serviceNode.Element("Service_Name")?.Value;
                    string serviceProcessName = serviceNode.Element("Service_Process_Name")?.Value;
                    string serviceRequestType = serviceNode.Element("Service_Request_Type")?.Value;
                    int serviceGapTimeout = int.Parse(serviceNode.Element("ServiceGapTimeout")?.Value ?? "0");


                    if (!string.IsNullOrEmpty(sid))
                    {
                        // Trim double quotation marks and 'H'
                        sid = sid.Trim('"', 'H');
                        switch (sid)
                        {
                            case "10":
                                int result10 = UDS_SERVICE_10(serviceNode);
                                if (result10 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "31":

                                int result31 = UDS_SERVICE_31(serviceNode);
                                if (result31 != 1)
                                    continue;  // Proceed to the next iteration
                                break;
                            case "85":

                                int result85 = UDS_SERVICE_85(serviceNode);
                                if (result85 != 1)
                                    continue;  // Proceed to the next iteration
                                break;
                            case "28":

                                int result28 = UDS_SERVICE_28(serviceNode);
                                if (result28 != 1)
                                    continue;  // Proceed to the next iteration
                                break;

                            case "27":

                                int result27 = UDS_SERVICE_27(serviceNode);
                                if (result27 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "2E":

                                int result2E = UDS_SERVICE_2E(serviceNode);
                                if (result2E != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "34":

                                int result34 = UDS_SERVICE_34(serviceNode);
                                if (result34 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "36":

                                int result36 = UDS_SERVICE_36(serviceNode);
                                if (result36 != 1)
                                    continue;// Proceed to the next iteration
                                break;
                            case "37":

                                int result37 = UDS_SERVICE_37(serviceNode);
                                if (result37 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "11":

                                int result11 = UDS_SERVICE_11(serviceNode);
                                if (result11 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                        }


                    }

                    else
                    {
                        Console.WriteLine("No nodes are found");
                    }
                }
            }
        }
        #endregion


        #region DATABLOCKPROGRAMING
        private void DataBlock_Programing(XElement preProgrammingProcedureElement)
        {
           
            //CANCONNECTION.Set_Filter(CANID, Response_ID);
            if (preProgrammingProcedureElement != null)
            {
                var dataBlocks = preProgrammingProcedureElement.Elements("DataBlock");
                //Get all the<Services> nodes
                
                foreach (var dataBlock in dataBlocks)
                {
                    string dataBlockName = dataBlock.Attribute("Name")?.Value;

                    if (dataBlockName == "CODE")
                    {
                        // Get all the <Services> nodes directly under <DataBlock>
                        var serviceNodes = dataBlock.Elements("Services");

                        // Iterate through each <Services> node
                        foreach (var serviceNode in serviceNodes)
                        {
                            // Extract data from the <Services> node
                            string sid = serviceNode.Element("SID")?.Value;
                            string serviceName = serviceNode.Element("Service_Name")?.Value;
                            string serviceProcessName = serviceNode.Element("Service_Process_Name")?.Value;
                            string serviceRequestType = serviceNode.Element("Service_Request_Type")?.Value;
                            int serviceGapTimeout = int.Parse(serviceNode.Element("ServiceGapTimeout")?.Value ?? "0");


                            if (!string.IsNullOrEmpty(sid))
                            {
                                // Trim double quotation marks and 'H'
                                sid = sid.Trim('"', 'H');

                                switch (sid)
                                {
                                    case "10":

                                        int result10 = UDS_SERVICE_10(serviceNode);
                                        if (result10 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "31":

                                        int result31 = UDS_SERVICE_31(serviceNode);
                                        if (result31 != 1)
                                            continue; // Proceed to the next iteration
                                        break;

                                    

                                    case "34":

                                        int result34 = UDS_SERVICE_34(serviceNode);
                                        if (result34 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    
                                    case "36":

                                        int result36 = UDS_SERVICE_36(serviceNode);
                                        if (result36 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    
                                    case "37":

                                        int result37 = UDS_SERVICE_37(serviceNode);
                                        if (result37 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "27":

                                        int result27 = UDS_SERVICE_27(serviceNode);
                                        if (result27 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "11":
                                        int result11 = UDS_SERVICE_11(serviceNode);
                                        if (result11 != 1)
                                            continue;
                                        break;
                                }


                            }

                            else
                            {
                                Console.WriteLine("No nodes are found");
                            }
                        }
                    }
                    else if(dataBlockName == "SBL")
                    {
                        // Get all the <Services> nodes directly under <DataBlock>
                        var serviceNodes = dataBlock.Elements("Services");

                        // Iterate through each <Services> node
                        foreach (var serviceNode in serviceNodes)
                        {
                            // Extract data from the <Services> node
                            string sid = serviceNode.Element("SID")?.Value;
                            string serviceName = serviceNode.Element("Service_Name")?.Value;
                            string serviceProcessName = serviceNode.Element("Service_Process_Name")?.Value;
                            string serviceRequestType = serviceNode.Element("Service_Request_Type")?.Value;
                            int serviceGapTimeout = int.Parse(serviceNode.Element("ServiceGapTimeout")?.Value ?? "0");


                            if (!string.IsNullOrEmpty(sid))
                            {
                                // Trim double quotation marks and 'H'
                                sid = sid.Trim('"', 'H');
                               

                                switch (sid)
                                {
                                    case "10":

                                        int result10 = UDS_SERVICE_10(serviceNode);
                                        if (result10 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "31":
                                        int result31sbl = UDS_SERVICE_31_sbl(serviceNode);
                                        if (result31sbl != 1)
                                        {
                                            continue; // Proceed to the next iteration
                                        }
                                        break;
                                    case "34":

                                        int result34sbl = UDS_SERVICE_34_sbl(serviceNode);
                                        if (result34sbl != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "36":

                                        int result36sbl = UDS_SERVICE_36_sbl(serviceNode);
                                        if (result36sbl != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "37":

                                        int result37 = UDS_SERVICE_37(serviceNode);
                                        if (result37 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "27":

                                        int result27 = UDS_SERVICE_27(serviceNode);
                                        if (result27 != 1)
                                            continue; // Proceed to the next iteration
                                        break;
                                    case "11":
                                        int result11 = UDS_SERVICE_11(serviceNode);
                                        if (result11 != 1)
                                            continue;
                                        break;
                                }


                            }

                            else
                            {
                                Console.WriteLine("No nodes are found");
                            }
                        }
                    }

                    
                }
            }
            else
            {
                Console.WriteLine("No nodes are found");
            }




        }
        #endregion

        #region POSTPROGRAMING
        private void Post_ProgramimgProcedure(XElement programmingProcedureElement)
        {
            if (programmingProcedureElement != null)
            {
                //Get all the<Services> nodes
                var serviceNodes = programmingProcedureElement.Elements("Services");
                // 
                // Iterate through each <Services> node
                foreach (var serviceNode in serviceNodes)
                {
                    // Extract data from the <Services> node
                    string sid = serviceNode.Element("SID")?.Value;
                    string serviceName = serviceNode.Element("Service_Name")?.Value;
                    string serviceProcessName = serviceNode.Element("Service_Process_Name")?.Value;
                    string serviceRequestType = serviceNode.Element("Service_Request_Type")?.Value;
                    int serviceGapTimeout = int.Parse(serviceNode.Element("ServiceGapTimeout")?.Value ?? "0");


                    if (!string.IsNullOrEmpty(sid))
                    {
                        // Trim double quotation marks and 'H'
                        sid = sid.Trim('"', 'H');
                        switch (sid)
                        {

                            case "31":

                                int result31 =UDS_SERVICE_31(serviceNode);
                                if (result31 != 1)
                                    continue; // Proceed to the next iteration
                                break;

                            case "11":

                                int result11 = UDS_SERVICE_11(serviceNode);
                                if (result11 != 1)
                                    continue;// Proceed to the next iteration
                                break;

                            case "10":

                                int result10 =UDS_SERVICE_10(serviceNode);
                                if (result10 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "28":

                                int result28 = UDS_SERVICE_28(serviceNode);
                                if (result28 != 1)
                                    continue; // Proceed to the next iteration
                                break;

                            case "85":

                                int result27 = UDS_SERVICE_85(serviceNode);
                                if (result27 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                           
                        }


                    }

                    else
                    {
                        Console.WriteLine("No nodes are found");
                    }
                }
            }
        }
        #endregion


        #region  FAILPROGRAMING
        private void Fail_ProgramimgProcedure( XElement ProgrammingProcedureElement)
        {
            if (ProgrammingProcedureElement != null)
            {
                //Get all the<Services> nodes
               // var serviceNodes = ProgrammingProcedureElement.Elements("Services");
                var serviceNodes = ProgrammingProcedureElement.Elements("Services");
               

                foreach (var serviceNode in serviceNodes)
                {
                    // Extract data from the <Services> node
                    string sid = serviceNode.Element("SID")?.Value;
                    string serviceName = serviceNode.Element("Service_Name")?.Value;
                    string serviceProcessName = serviceNode.Element("Service_Process_Name")?.Value;
                    string serviceRequestType = serviceNode.Element("Service_Request_Type")?.Value;
                    int serviceGapTimeout = int.Parse(serviceNode.Element("ServiceGapTimeout")?.Value ?? "0");


                    if (!string.IsNullOrEmpty(sid))
                    {
                        // Trim double quotation marks and 'H'
                        sid = sid.Trim('"', 'H');
                        switch (sid)
                        {
                            case "10":

                                int result10 = UDS_SERVICE_10(serviceNode);
                                if (result10 != 1)
                                    continue; // Proceed to the next iteration
                                break;
                            case "27":

                                int result27 = UDS_SERVICE_27(serviceNode);
                                if (result27 != 1)
                                    continue; // Proceed to the next iteration
                                break;

                            case "11":

                                int result11 =  UDS_SERVICE_11(serviceNode);
                                if (result11 != 1)
                                    continue; // Proceed to the next iteration
                                break;

                            
                           

                        }


                    }

                    else
                    {
                        Console.WriteLine("No nodes are found");
                    }
                }
            }
        }
        #endregion




    }



    public class PrintStatment
    {
        public void Statement()
        {
            CANCONNECTION.J2534_DEINIT();
            while (true)
            {
                Console.WriteLine("programe execution Ended...");
            }
        }

    }



}
    
    




   
        

    


