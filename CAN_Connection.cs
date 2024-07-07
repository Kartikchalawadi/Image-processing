using J2534DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUinit_Test_Pro
{
    public class CAN_Driver
    {


        //public const Int32 MAX_LENGTH_BUFFER = 4096;

        //public static byte[] i15765_tp_requestBuff = new byte[MAX_LENGTH_BUFFER];
        //public static byte[] i15765_tp_responseBuff = new byte[MAX_LENGTH_BUFFER];
        //public const byte RESP = 0x40;
        //public static bool requestIndication_b = false;
        //private const byte PADDING = 0x55;
        //private const int BYTESPERFRAME = 8;
        //public static byte[] i15765_nlrequestBuff = new byte[MAX_LENGTH_BUFFER];
        //public static byte[] i15765_nlrespBuff = new byte[MAX_LENGTH_BUFFER];

        //private static Int32 I15765_TP_BufferLen = 0;
        //private static byte seqNum = 0x00;
        //private static Int32 resplen = 0;
        //private static Int32 responselistOffset = 0;
        //private static Int32 responselistRem = 0;
        //private static Int32 count = 0;
        //private static Int32 responselistLen = 0;
        //private static Int32 NumFrames = 0;
        //public static Int32 TotalFrames = 0;

        ///* Types of frames */
        //public const byte DCAN_PCI_SF = 0x00;  /* PCI for Single frame       */
        //public const byte DCAN_PCI_FF = 0x10;  /* PCI for First frame        */
        //public const byte DCAN_PCI_CF = 0x20;  /* PCI for consecutive frame  */
        //public const byte DCAN_PCI_FC = 0x30;  /* PCI for Flow control frame */

        ///* Types of Flow control */
        //public const byte DCAN_FC_CTS = 0x00;  /* Continue to send flow status */
        //public const byte DCAN_FC_WAIT = 0x01;  /* Wait to send flow status     */
        //public const byte DCAN_FC_OVERFLOW = 0x02;  /* Overcflow status         */
        //public const byte HIGHER_NIBBLE_MASK = 0xF0;
        //public const byte LOWER_NIBBLE_MASK = 0x0F;
        //public static UInt16 bufferindex_u16 = 0;
        //public static UInt16 bufferlength_u16 = 0;
        //public static UInt16 datatoRxTx_u16 = 0;
        //public static byte SNCtr_u8 = 0;
        //public static byte NumofConFramesServer = 0;
        //public static I1576_TP_State_En_t I1576_TP_State = I1576_TP_State_En_t.I15765_ST_IDLE_E;
        //public static I1576_TP_Frame_En_t I1576_TP_Frame;
        //private static byte[] buffer_u8;
        //private static Int32 total_bytes_u32 = 0;
        //private static Int32 buffer_index_u32 = 0;
        //private static Int32 bytes_recv_u32 = 0;
        //private static Int32 bytes_pend_u32 = 0;


        //private static double milisec = 0;
        public static J2534 PassThru = new J2534();
        public static int channelId = 0;
        public static int deviceid = 0;

        static List<J2534Device> availableJ2534Devices = new List<J2534Device>();
        public static int MASK_CAN_ID_u32 = 0xFFFF;
        public static int ECU_CAN_ID_u32 = 0x7F1;
        public static int Tester_CAN_ID_u32 = 0;
        public static int Functional_CAN_ID_u32 = 0;
        public static BaudRate CAN_baudrate = BaudRate.ISO15765_500000;


        public const Int32 MAX_LENGTH_BUFFER = 4096;
        public static byte[] i15765_nlrespBuff = new byte[MAX_LENGTH_BUFFER];
        private static Int32 I15765_TP_BufferLen = 0;
        private static int CAN_TP_filterId = 0;
        //J2534 passThru = new J2534();
        public static List<string> J2534_Get_Devices()
        {
            bool J2534_status = false;
            // Open a new channel configured for ISO15765 (CAN)
            //int channelId = 0;
            //int deviceid = 0;

            availableJ2534Devices = J2534Detect.ListDevices();

            List<string> selected_device = new List<string>();
            try
            {
                if (availableJ2534Devices.Count > 0)
                {

                    for (int i = 0; i < availableJ2534Devices.Count; i++)
                    {
                        selected_device.Add(availableJ2534Devices[i].Name);
                    }

                }
                else
                {

                }
            }
            catch
            {
                selected_device.Clear();
                return selected_device;
            }
            return selected_device;
        }

        public static bool J2534_INIT(string Can_Select)
        {
            bool J2534_status = false;
            // Attempt to open a communication link with the pass thru device
          //  if ((Tester_CAN_ID_u32 > 0) && (ECU_CAN_ID_u32 > 0))
            {
                for (int i = 0; i < availableJ2534Devices.Count; i++)
                {
                    if (availableJ2534Devices[i].Name == Can_Select)
                    {
                        J2534_status = PassThru.LoadLibrary(availableJ2534Devices[i]);//Todo
                        break;
                    }
                }
            }
            if (J2534_status == true)
            {

                J2534Err status = J2534Err.ERR_FAILED;
                status = PassThru.Open(ref deviceid);
                if (status == J2534Err.STATUS_NOERROR)
                {
                    if (deviceid != 0)
                    {

                        status = PassThru.Connect(deviceid, ProtocolID.ISO15765, ConnectFlag.NONE, CAN_baudrate, ref channelId);
                        if (J2534Err.ERR_DEVICE_NOT_CONNECTED == status)
                        {
                            //Main.RichTxtBx.AppendText(DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " - Device not connected! \n");
                            //Main.RichTxtBx.ScrollToEnd();
                            return false;
                        }
                        else if (J2534Err.STATUS_NOERROR == status)
                        {

                            status = PassThru.ClearMsgFilters(channelId);


                            byte[] data = new byte[4];
                            byte m = 0;
                            data[0] = (byte)(MASK_CAN_ID_u32 >> (8 * 3));
                            data[1] = (byte)(MASK_CAN_ID_u32 >> (8 * 2));
                            data[2] = (byte)(MASK_CAN_ID_u32 >> (8 * 1));
                            data[3] = (byte)(MASK_CAN_ID_u32 >> (8 * 0));
                            PassThruMsg maskMsg = new PassThruMsg(
                                ProtocolID.ISO15765,
                                TxFlag.ISO15765_FRAME_PAD,
                                 data);
                            data[0] = (byte)(ECU_CAN_ID_u32 >> (8 * 3));
                            data[1] = (byte)(ECU_CAN_ID_u32 >> (8 * 2));
                            data[2] = (byte)(ECU_CAN_ID_u32 >> (8 * 1));
                            data[3] = (byte)(ECU_CAN_ID_u32 >> (8 * 0));
                            PassThruMsg patternMsg = new PassThruMsg(
                                ProtocolID.ISO15765,
                                TxFlag.ISO15765_FRAME_PAD,
                                 data);
                            data[0] = (byte)(Tester_CAN_ID_u32 >> (8 * 3));
                            data[1] = (byte)(Tester_CAN_ID_u32 >> (8 * 2));
                            data[2] = (byte)(Tester_CAN_ID_u32 >> (8 * 1));
                            data[3] = (byte)(Tester_CAN_ID_u32 >> (8 * 0));
                            PassThruMsg flowControlMsg = new PassThruMsg(
                                ProtocolID.ISO15765,
                                TxFlag.ISO15765_FRAME_PAD,
                                 data);
                            //     new byte[] { 0x00, 0x00, 0x07, 0xF1 });
                            status = PassThru.StartMsgFilter(channelId, FilterType.FLOW_CONTROL_FILTER, ref maskMsg, ref patternMsg, ref flowControlMsg, ref CAN_TP_filterId);

                            SetConfigurationParameter Parameter = SetConfigurationParameter.Iso15765SeparateTimeMinimum;

                            status = PassThru.SetConfig(channelId, Parameter, 0);
                            Parameter = SetConfigurationParameter.Iso15765BlockSize;
                            status = PassThru.SetConfig(channelId, Parameter, 0);
                            return true;

                        }

                    }
                    else if (J2534Err.ERR_DEVICE_NOT_CONNECTED == PassThru.Connect(deviceid, ProtocolID.ISO15765, ConnectFlag.NONE, CAN_baudrate, ref channelId))
                    {
                        //Main.RichTxtBx.AppendText(DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " - Device not connected! \n");
                        //Main.RichTxtBx.ScrollToEnd();
                        return false;
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (J2534Err.ERR_DEVICE_NOT_CONNECTED == PassThru.Connect(deviceid, ProtocolID.ISO15765, ConnectFlag.NONE, CAN_baudrate, ref channelId))
                {
                    //Main.RichTxtBx.AppendText(DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " - Device not connected! \n");
                    //Main.RichTxtBx.ScrollToEnd();
                    return false;
                }
                else
                {

                    return false;

                }
            }
            return J2534_status;

        }

        public static bool J2534_DEINIT()
        {
            PassThru.StopMsgFilter(channelId, CAN_TP_filterId);
            // Disconnect this channel
            PassThru.Disconnect(channelId);
            PassThru.Close(deviceid);
            // When we are done with the device, we can free the library.
            PassThru.FreeLibrary();
            return true;

        }





        ///// <summary>
        ///// 
        ///// </summary>
        public static List<Byte> Read_data(UInt16 timeout)
        {
            int numMsgs = 1;
            List<PassThruMsg> rxMsgs = new List<PassThruMsg>();
            J2534Err status = J2534Err.STATUS_NOERROR;

            //byte[] buffer_u8 = new byte[4095];

            List<Byte> buffer_u8 = new List<Byte>();
            numMsgs = 1;
            status = J2534Err.ERR_BUFFER_EMPTY;

            //rxMsgs.Clear();

            status = PassThru.ReadMsgs(channelId, ref rxMsgs, ref numMsgs, timeout, (int)ProtocolID.ISO15765);
            if (status == J2534Err.STATUS_NOERROR)
            {

                for (int i = 0; i < (rxMsgs[0].Data.Length - 4); i++)
                {

                    buffer_u8.Add(rxMsgs[0].Data[i]);
                }
                rxMsgs.Clear();
            }
            else
            {

            }

            return buffer_u8;
        }
        public static void SendData(List<byte> resplist)
        {
            Buffer.BlockCopy(resplist.ToArray(), 0, i15765_nlrespBuff, 0, resplist.Count);
            I15765_TP_BufferLen = resplist.Count;

            PassThru.ClearRxBuffer(channelId);
            PassThru.ClearRxBuffer(channelId);
            PassThru.ClearTxBuffer(channelId);
            SetConfigurationParameter Parameter = SetConfigurationParameter.Iso15765SeparateTimeMinimum;

            PassThru.SetConfig(channelId, Parameter, 0);
            Parameter = SetConfigurationParameter.Iso15765BlockSize;
            PassThru.SetConfig(channelId, Parameter, 0);

            byte[] data = new byte[4 + resplist.Count];
            byte m = 0;
            data[0] = 0;//0x1b
            data[1] = 0;//0xda
            data[2] = 7;//0xf0
            data[3] = 0xf0;//0x62
            for (int i = 0; i < resplist.Count; i++)
            {
                data[i + 4] = i15765_nlrespBuff[i];
            }

            PassThruMsg txMsg13 = new PassThruMsg(ProtocolID.ISO15765, TxFlag.NONE, data);
            int numMsgs = 1;
            PassThru.WriteMsgs(channelId, ref txMsg13, ref numMsgs, 0);
            J2534Err status = J2534Err.STATUS_NOERROR;


        }
        public static void Set_Filter( UInt32 CAN_ID)
        {
            UInt32 Response_ID = 0xff;
            int filterId = 0;
            byte[] data = new byte[4];
            byte m = 0;
            data[0] = 0xFF;// (byte)(MASK_CAN_ID_u32 >> (8 * 3));
            data[1] = 0xFF;//(byte)(MASK_CAN_ID_u32 >> (8 * 2));
            data[2] = 0xFF;//(byte)(MASK_CAN_ID_u32 >> (8 * 1));
            data[3] = 0xFF;//(byte)(MASK_CAN_ID_u32 >> (8 * 0));
            PassThruMsg maskMsg = new PassThruMsg(
                ProtocolID.CAN,
                TxFlag.NONE,
                 data);

            data[0] = (byte)(Response_ID >> (8 * 3));
            data[1] = (byte)(Response_ID >> (8 * 2));
            data[2] = (byte)(Response_ID >> (8 * 1));
            data[3] = (byte)(Response_ID >> (8 * 0));
            PassThruMsg patternMsg = new PassThruMsg(
                ProtocolID.CAN,
                TxFlag.NONE,
                 data);

            data[0] = (byte)(CAN_ID >> (8 * 3));
            data[1] = (byte)(CAN_ID >> (8 * 2));
            data[2] = (byte)(CAN_ID >> (8 * 1));
            data[3] = (byte)(CAN_ID >> (8 * 0));
            PassThruMsg flowControlMsg = new PassThruMsg(
                ProtocolID.CAN,
                TxFlag.NONE,
                 data);
            //     new byte[] { 0x00, 0x00, 0x07, 0xF1 });
            PassThru.StartMsgFilter(channelId, FilterType.FLOW_CONTROL_FILTER, ref maskMsg, ref patternMsg, ref flowControlMsg, ref filterId);


        }
        public static void SendCANData(int[] resplist, UInt32 CAN_ID)
        {
            Buffer.BlockCopy(resplist.ToArray(), 0, i15765_nlrespBuff, 0, resplist.Length);
            I15765_TP_BufferLen = resplist.Length;

            //PassThru.ClearRxBuffer(channelId);
            //PassThru.ClearRxBuffer(channelId);
            //PassThru.ClearTxBuffer(channelId);

            byte[] data_Send = new byte[4 + resplist.Length];

            data_Send[0] = (byte)(CAN_ID >> (8 * 3));
            data_Send[1] = (byte)(CAN_ID >> (8 * 2));
            data_Send[2] = (byte)(CAN_ID >> (8 * 1));
            data_Send[3] = (byte)(CAN_ID >> (8 * 0));

            for (int i = 0; i < resplist.Length; i++)
            {
                data_Send[i + 4] = (byte)resplist[i];
            }

            PassThruMsg txMsg13 = new PassThruMsg(ProtocolID.ISO15765, TxFlag.ISO15765_FRAME_PAD, data_Send);
            int numMsgs = 1;
            PassThru.WriteMsgs(channelId, ref txMsg13, ref numMsgs, 0);
            J2534Err status = J2534Err.STATUS_NOERROR;


        }
        public static void SendNONTPCANData(int[] resplist, UInt32 CAN_ID)
        {
            Buffer.BlockCopy(resplist.ToArray(), 0, i15765_nlrespBuff, 0, resplist.Length);
            I15765_TP_BufferLen = resplist.Length;

            //PassThru.ClearRxBuffer(channelId);
            //PassThru.ClearRxBuffer(channelId);
            //PassThru.ClearTxBuffer(channelId);

            byte[] data_Send = new byte[4 + resplist.Length];

            data_Send[0] = (byte)(CAN_ID >> (8 * 3));
            data_Send[1] = (byte)(CAN_ID >> (8 * 2));
            data_Send[2] = (byte)(CAN_ID >> (8 * 1));
            data_Send[3] = (byte)(CAN_ID >> (8 * 0));

            for (int i = 0; i < resplist.Length; i++)
            {
                data_Send[i + 4] = (byte)resplist[i];
            }

            PassThruMsg txMsg13 = new PassThruMsg(ProtocolID.CAN, TxFlag.NONE, data_Send);
            int numMsgs = 1;
            PassThru.WriteMsgs(channelId, ref txMsg13, ref numMsgs, 0);
            J2534Err status = J2534Err.STATUS_NOERROR;


        }





        public static void Stop_Filter(UInt32 Response_ID)
        {
            PassThru.StopMsgFilter(channelId, (int)Response_ID);
        }

        public static void Clear_CAN_data()
        {
            PassThru.ClearTxBuffer(channelId);
            PassThru.ClearRxBuffer(channelId);
        }
        public static List<Byte> Read_CAN_data(UInt16 timeout, UInt32 CAN_ID)
        {
            //PassThru.ClearTxBuffer(channelId);
            int filterId = 0;
            byte[] data = new byte[4];
            byte m = 0;
            data[0] = (byte)(MASK_CAN_ID_u32 >> (8 * 3));
            data[1] = (byte)(MASK_CAN_ID_u32 >> (8 * 2));
            data[2] = (byte)(MASK_CAN_ID_u32 >> (8 * 1));
            data[3] = (byte)(MASK_CAN_ID_u32 >> (8 * 0));
            PassThruMsg maskMsg = new PassThruMsg(
                ProtocolID.CAN,
                TxFlag.NONE,
                 data);
            data[0] = (byte)(CAN_ID >> (8 * 3));
            data[1] = (byte)(CAN_ID >> (8 * 2));
            data[2] = (byte)(CAN_ID >> (8 * 1));
            data[3] = (byte)(CAN_ID >> (8 * 0));
            PassThruMsg patternMsg = new PassThruMsg(
                ProtocolID.CAN,
                TxFlag.NONE,
                 data);
            data[0] = (byte)(MASK_CAN_ID_u32 >> (8 * 3));
            data[1] = (byte)(MASK_CAN_ID_u32 >> (8 * 2));
            data[2] = (byte)(MASK_CAN_ID_u32 >> (8 * 1));
            data[3] = (byte)(MASK_CAN_ID_u32 >> (8 * 0));
            PassThruMsg flowControlMsg = new PassThruMsg(
                ProtocolID.CAN,
                TxFlag.NONE,
                 data);
            //     new byte[] { 0x00, 0x00, 0x07, 0xF1 });
            PassThru.StartMsgFilter(channelId, FilterType.PASS_FILTER, ref maskMsg, ref patternMsg, ref flowControlMsg, ref filterId);



            int numMsgs = 1;
            List<PassThruMsg> rxMsgs = new List<PassThruMsg>();
            J2534Err status = J2534Err.STATUS_NOERROR;

            //byte[] buffer_u8 = new byte[4095];

            List<Byte> buffer_u8 = new List<Byte>();
            numMsgs = 1;
            status = J2534Err.ERR_BUFFER_EMPTY;

            //rxMsgs.Clear();
            uint k = 0;
            uint l = 0;
            //if((UInt16)(timeout % 100) != 0)
            //{
            //    l++;
            //}
            //for (k = 0; k < ((UInt16)(timeout / 100) + l);k++)
            {
                status = PassThru.ReadMsgs(channelId, ref rxMsgs, ref numMsgs, timeout, (int)ProtocolID.CAN);
                if (status == J2534Err.STATUS_NOERROR)
                {

                    for (int i = 0; i < (rxMsgs[0].Data.Length - 4); i++)
                    {

                        buffer_u8.Add(rxMsgs[0].Data[i]);
                    }
                    rxMsgs.Clear();
                    //break;
                }
                else
                {

                }
            }
            PassThru.StopMsgFilter(channelId, filterId);
            return buffer_u8;
        }
    }
}
