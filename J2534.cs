#region Copyright (c) 2010, Michael Kelly
/* 
 * Copyright (c) 2010, Michael Kelly
 * michael.e.kelly@gmail.com
 * http://michael-kelly.com/
 * 
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the organization nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
#endregion License
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace J2534DotNet
{
    public class J2534 : IJ2534
    {
        private J2534Device m_device;
        private J2534DllWrapper m_wrapper;
        bool Read_Complete = false;
        public bool datasent = false;
        public bool LoadLibrary(J2534Device device)
        {
            m_device = device;
            m_wrapper = new J2534DllWrapper();
            return m_wrapper.LoadJ2534Library(m_device.FunctionLibrary);
        }

        public bool FreeLibrary()
        {
            return m_wrapper.FreeLibrary();
        }

        public J2534Err Open(ref int deviceId)
        {
            int nada = 0;
            return (J2534Err)m_wrapper.Open(nada, ref deviceId);
        }

        public J2534Err Close(int deviceId)
        {
            return (J2534Err)m_wrapper.Close(deviceId);
        }

        public J2534Err Connect(int deviceId, ProtocolID protocolId, ConnectFlag flags, BaudRate baudRate, ref int channelId)
        {
            return (J2534Err)m_wrapper.Connect(deviceId, (int)protocolId, (int)flags, (int)baudRate, ref channelId);
        }

        public J2534Err Connect(int deviceId, ProtocolID protocolId, ConnectFlag flags, int baudRate, ref int channelId)
        {
            return (J2534Err)m_wrapper.Connect(deviceId, (int)protocolId, (int)flags, baudRate, ref channelId);
        }

        public J2534Err Disconnect(int channelId)
        {
            return (J2534Err)m_wrapper.Disconnect(channelId);
        }

       
        public J2534Err ReadMsgs(int channelId, ref List<PassThruMsg> msgs, ref int numMsgs, int timeout, int Protocol_ID)
        {

            UnsafePassThruMsg uMsg;
            uMsg.ProtocolID = Protocol_ID;

            IntPtr pMsg = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UnsafePassThruMsg)) * 1000);
            IntPtr pNextMsg = IntPtr.Zero;
            IntPtr[] pMsgs = new IntPtr[50];

            unsafe
            {
                UnsafePassThruMsg* src =(UnsafePassThruMsg *) pMsg.ToPointer();
                src->ProtocolID = Protocol_ID;
            }
                //UnsafePassThruMsg uMsg = (UnsafePassThruMsg)Marshal.PtrToStructure(pMsg, typeof(UnsafePassThruMsg));
                //uMsg.ProtocolID = 5;
                J2534Err returnValue = (J2534Err)m_wrapper.ReadMsgs(channelId, pMsg, ref numMsgs, timeout);
            int count = 0;
            if (returnValue == J2534Err.STATUS_NOERROR)
            {

                for (int i = 0; i < numMsgs; i++)
                {
                    pNextMsg = (IntPtr)(Marshal.SizeOf(typeof(UnsafePassThruMsg)) * i + (int)pMsg);
                    UnsafePassThruMsg uMsg1 = (UnsafePassThruMsg)Marshal.PtrToStructure(pMsg, typeof(UnsafePassThruMsg));
                    msgs.Add(ConvertPassThruMsg(uMsg1));
                  
                    count = msgs.Count;
                    returnValue = (J2534Err)uMsg1.RxStatus;
                    // Console.WriteLine(msgs[i].ProtocolID);

                     for(int k=0;k< 7; k++)
                    {
                      //  Console.WriteLine(msgs[count-1].Data[k]);


                        if(msgs[count - 1].Data[i]== 0xFF)
                        {
                            datasent = true;
                        }
                        else
                        {
                            datasent = false;
                        }
                    }

                }
            }
          
            Marshal.FreeHGlobal(pMsg);

            return returnValue;
        }

        public J2534Err WriteMsgs(int channelId, ref PassThruMsg msg, ref int numMsgs, int timeout)
        {
            try
            {
                UnsafePassThruMsg uMsg = ConvertPassThruMsg(msg);
                // TODO: change function to accept a list? of PassThruMsg

               m_wrapper.WriteMsgs(channelId, ref uMsg, ref numMsgs, timeout);

               return J2534Err.STATUS_NOERROR;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception in WriteMsgsAsync: {e.Message}");
                return J2534Err.Error;
            }
        }

        public J2534Err StartPeriodicMsg(int channelId, ref PassThruMsg msg, ref int msgId, int timeInterval)
        {
            UnsafePassThruMsg uMsg = ConvertPassThruMsg(msg);
            return (J2534Err)m_wrapper.StartPeriodicMsg(channelId, ref uMsg, ref msgId, timeInterval);
        }

        public J2534Err StopPeriodicMsg(int channelId, int msgId)
        {
            return (J2534Err)m_wrapper.StopPeriodicMsg(channelId, msgId);
        }

        public J2534Err StartMsgFilter
        (
            int channelid,
            FilterType filterType,
            ref PassThruMsg maskMsg,
            ref PassThruMsg patternMsg,
            ref PassThruMsg flowControlMsg,
            ref int filterId
        )
        {
            UnsafePassThruMsg uMaskMsg = ConvertPassThruMsg(maskMsg);
            UnsafePassThruMsg uPatternMsg = ConvertPassThruMsg(patternMsg);
            UnsafePassThruMsg uFlowControlMsg = ConvertPassThruMsg(flowControlMsg);
            return (J2534Err)m_wrapper.StartMsgFilter
                    (
                        channelid,
                        (int)filterType,
                        ref uMaskMsg,
                        ref uPatternMsg,
                        ref uFlowControlMsg,
                        ref filterId
                    );
        }

        public J2534Err StartMsgFilter
        (
            int channelid,
            FilterType filterType,
            ref PassThruMsg maskMsg,
            ref PassThruMsg patternMsg,
            ref int filterId
        )
        {
            int nada = 0;
            UnsafePassThruMsg uMaskMsg = ConvertPassThruMsg(maskMsg);
            UnsafePassThruMsg uPatternMsg = ConvertPassThruMsg(patternMsg);
            return (J2534Err)m_wrapper.StartPassBlockMsgFilter
                    (
                        channelid,
                        (int)filterType,
                        ref uMaskMsg,
                        ref uPatternMsg,
                        nada,
                        ref filterId
                    );
        }

        public J2534Err StopMsgFilter(int channelId, int filterId)
        {
            return (J2534Err)m_wrapper.StopMsgFilter(channelId, filterId);
        }

        public J2534Err SetProgrammingVoltage(int deviceId, PinNumber pinNumber, int voltage)
        {
            return (J2534Err)m_wrapper.SetProgrammingVoltage(deviceId, (int)pinNumber, voltage);
        }

        public J2534Err ReadVersion(int deviceId, ref string firmwareVersion, ref string dllVersion, ref string apiVersion)
        {
            IntPtr pFirmwareVersion = Marshal.AllocHGlobal(120);
            IntPtr pDllVersion = Marshal.AllocHGlobal(120);
            IntPtr pApiVersion = Marshal.AllocHGlobal(120);
            J2534Err returnValue = (J2534Err)m_wrapper.ReadVersion(deviceId, pFirmwareVersion, pDllVersion, pApiVersion);
            if (returnValue == J2534Err.STATUS_NOERROR)
            {
                firmwareVersion = Marshal.PtrToStringAnsi(pFirmwareVersion);
                dllVersion = Marshal.PtrToStringAnsi(pDllVersion);
                apiVersion = Marshal.PtrToStringAnsi(pApiVersion);
            }

            Marshal.FreeHGlobal(pFirmwareVersion);
            Marshal.FreeHGlobal(pDllVersion);
            Marshal.FreeHGlobal(pApiVersion);

            return returnValue;
        }

        public J2534Err GetLastError(ref string errorDescription)
        {
            IntPtr pErrorDescription = Marshal.AllocHGlobal(120);
            J2534Err returnValue = (J2534Err)m_wrapper.GetLastError(pErrorDescription);
            if (returnValue == J2534Err.STATUS_NOERROR)
            {
                errorDescription = Marshal.PtrToStringAnsi(pErrorDescription);
            }

            Marshal.FreeHGlobal(pErrorDescription);

            return returnValue;
        }

        public J2534Err GetConfig(int channelId, ref List<SConfig> config)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.GET_CONFIG, input, output);
        }
  
        public J2534Err SetConfig(int channelId, SetConfigurationParameter Parameter, UInt32 value)
        {
            IntPtr output = Marshal.AllocHGlobal(150);
         
     
            SetConfiguration DataRate = new SetConfiguration(Parameter, value);
            SetConfiguration[] setConfigurationArray = new SetConfiguration[] { DataRate };
            using (SetConfigurationList setConfigurationList = new SetConfigurationList(setConfigurationArray))
            {
                return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, setConfigurationList.Pointer, output);
            }

          //  (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, DataRate.Pointer, output);

            //    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, DataRate.Pointer, output);
        }

        public J2534Err ReadBatteryVoltage(int deviceId, ref int voltage)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = Marshal.AllocHGlobal(8);

            J2534Err returnValue = (J2534Err)m_wrapper.Ioctl(deviceId, (int)Ioctl.READ_VBATT, input, output);
            if (returnValue == J2534Err.STATUS_NOERROR)
            {
                voltage = Marshal.ReadInt32(output);
            }

            Marshal.FreeHGlobal(output);

            return returnValue;
        }

        public J2534Err FiveBaudInit(int channelId, byte targetAddress, ref byte keyword1, ref byte keyword2)
        {
            J2534Err returnValue;
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            SByteArray inputArray = new SByteArray();
            SByteArray outputArray = new SByteArray();
            inputArray.NumOfBytes = 1;
            unsafe
            {
                //inputArray.BytePtr[0] = targetAddress;
                outputArray.NumOfBytes = 2;

                Marshal.StructureToPtr(inputArray, input, true);
                Marshal.StructureToPtr(outputArray, output, true);

                returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.FIVE_BAUD_INIT, input, output);

                Marshal.PtrToStructure(output, outputArray);
            }
            return returnValue;
        }

        public J2534Err ISO15765BSMAX(int channelId, byte targetAddress, ref byte keyword1, ref byte keyword2)
        {
            J2534Err returnValue;
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            SByteArray inputArray = new SByteArray();
            SByteArray outputArray = new SByteArray();
            inputArray.NumOfBytes = 1;
            

            unsafe
            {
                //inputArray.BytePtr[0] = targetAddress;
                outputArray.NumOfBytes = 2;

                Marshal.StructureToPtr(inputArray, input, true);
                Marshal.StructureToPtr(outputArray, output, true);

                returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, input, output);

                Marshal.PtrToStructure(output, outputArray);
            }
            return returnValue;
        }

        public J2534Err FastInit(int channelId, PassThruMsg txMsg, ref PassThruMsg rxMsg)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            UnsafePassThruMsg uTxMsg = ConvertPassThruMsg(txMsg);
            UnsafePassThruMsg uRxMsg = new UnsafePassThruMsg();

            Marshal.StructureToPtr(uTxMsg, input, true);
            Marshal.StructureToPtr(uRxMsg, output, true);

            J2534Err returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.FAST_INIT, input, output);
            if (returnValue == J2534Err.STATUS_NOERROR)
            {
                Marshal.PtrToStructure(output, uRxMsg);
            }

            rxMsg = ConvertPassThruMsg(uRxMsg);
            return returnValue;
        }

        public J2534Err ClearTxBuffer(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_TX_BUFFER, input, output);
        }

        public J2534Err ClearRxBuffer(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_RX_BUFFER, input, output);
        }

        public J2534Err ClearPeriodicMsgs(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_PERIODIC_MSGS, input, output);
        }

        public J2534Err ClearMsgFilters(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_MSG_FILTERS, input, output);
        }
        public J2534Err J2534_DEBUG_Enable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.J2534_DEBUG_START, input, output);
        }

        public J2534Err ClearFunctMsgLookupTable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_FUNCT_MSG_LOOKUP_TABLE, input, output);
        }

        public J2534Err AddToFunctMsgLookupTable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            // TODO: fix this
            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.ADD_TO_FUNCT_MSG_LOOKUP_TABLE, input, output);
        }

        public J2534Err DeleteFromFunctMsgLookupTable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            // TODO: fix this
            return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE, input, output);
        }

        private UnsafePassThruMsg ConvertPassThruMsg(PassThruMsg msg)
        {
            UnsafePassThruMsg uMsg = new UnsafePassThruMsg();

            uMsg.ProtocolID = (int)msg.ProtocolID;
            uMsg.RxStatus = (int)msg.RxStatus;
            uMsg.Timestamp = msg.Timestamp;
            uMsg.TxFlags = (int)msg.TxFlags;
            uMsg.ExtraDataIndex = msg.ExtraDataIndex;
            uMsg.DataSize = msg.Data.Length;
            unsafe
            {
                for (int i = 0; i < msg.Data.Length; i++)
                {
                    uMsg.Data[i] = msg.Data[i];
                }
            }

            return uMsg;
        }
        //static ulong counter = 0;
        //static long loopcounter = 0;
       public static ulong counter = 0;
        public static long loopcounter = 0;
        private static PassThruMsg ConvertPassThruMsg(UnsafePassThruMsg uMsg)
        {
            PassThruMsg msg = new PassThruMsg();
            byte countu8 = 0;
            msg.ProtocolID = (ProtocolID)uMsg.ProtocolID;
            msg.RxStatus = (RxStatus)uMsg.RxStatus;
            msg.Timestamp = uMsg.Timestamp;
            msg.TxFlags = (TxFlag)uMsg.TxFlags;
            msg.ExtraDataIndex = uMsg.ExtraDataIndex;
            msg.Data = new byte[uMsg.DataSize];
            unsafe
            {
                counter += (ulong)uMsg.DataSize-4;
                loopcounter++;
                // if (loopcounter % 2 ==0)
                {
                    Console.Write(counter + "\n" + loopcounter + "\n");
                }
               for (int i = 0; i < uMsg.DataSize; i++)
               {
                    msg.Data[i] = uMsg.Data[i + 4];
                    //    counter++;
                      //  Console.Write(msg.Data[i]+"\t");
                    //    Console.Write(counter+"\n");
                    //  File.AppendAllText(@"D:\Project\J2534\csc.txt", msg.Data[i]+"\t");
                }
               if((0xFF == msg.Data[0]) && (0xFF == msg.Data[1]) && (0xFF == msg.Data[2]))
                {
                    countu8 = 1;
                }
                else
                {
                    countu8 = 2;
                }
                //File.AppendAllText(@"D:\Project\J2534\csc.txt", "\n");
                //for (int i = 0; i < uMsg.DataSize; i++)
                //{
                //    msg.Data[i] = uMsg.Data[i+4];
                //    counter++;
                //    Console.Write(msg.Data[i]+"\t");
                //    Console.Write(counter+"\n");
                //}
            }
            return msg;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct SByteArray
        {
            public int NumOfBytes;
            public fixed byte BytePtr[2];
        }
    }
}   
