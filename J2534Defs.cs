﻿#region Copyright (c) 2010, Michael Kelly
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
using System.Runtime.InteropServices;

namespace J2534DotNet
{
    public class PassThruMsg
    {
        public PassThruMsg() { }
        public PassThruMsg(ProtocolID myProtocolId, TxFlag myTxFlag, byte[] myByteArray)
        {
            ProtocolID = myProtocolId;
            TxFlags = myTxFlag;
            Data = myByteArray;
         //  DataSize = myByteArray.Length;
        }

        public ProtocolID ProtocolID { get; set; }
        public RxStatus RxStatus { get; set; }
        public TxFlag TxFlags { get; set; }
        public int Timestamp { get; set; }
        public int DataSize;
        public int ExtraDataIndex { get; set; }
        public byte[] Data { get; set; }
    }

    [Flags]
    public enum RxStatus
    {
        NONE = 0x00000000,
        TX_MSG_TYPE = 0x00000001,
        START_OF_MESSAGE = 0x00000002,
        RX_BREAK = 0x00000004,
        TX_INDICATION = 0x00000008,
        ISO15765_PADDING_ERROR = 0x00000010,
        ISO15765_ADDR_TYPE = 0x00000080,
        CAN_29BIT_ID = 0x00000100
    }

    [Flags]
    public enum ConnectFlag
    {
        NONE = 0x0000,
        ISO9141_K_LINE_ONLY = 0x1000,
        CAN_ID_BOTH = 0x0800,
        ISO9141_NO_CHECKSUM = 0x0200,
        CAN_29BIT_ID = 0x0100
    }

    [Flags]
    public enum TxFlag
    {
        NONE = 0x00000000,
        SCI_TX_VOLTAGE = 0x00800000,
        SCI_MODE = 0x00400000,
        WAIT_P3_MIN_ONLY = 0x00000200,
        CAN_29BIT_ID = 0x00000100,
        ISO15765_ADDR_TYPE = 0x00000080,
        ISO15765_FRAME_PAD = 0x00000040
    }

    public enum ProtocolID
    {
        J1850VPW = 0x01,
        J1850PWM = 0x02,
        ISO9141 = 0x03,
        ISO14230 = 0x04,
        CAN = 0x05,
        ISO15765 = 0x06,
        SCI_A_ENGINE = 0x07,
        SCI_A_TRANS = 0x08,
        SCI_B_ENGINE = 0x09,
        SCI_B_TRANS = 0x0A
    }

    public enum BaudRate
    {
        ISO9141 = 10400,
        ISO9141_10400 = 10400,
        ISO9141_10000 = 10000,

        ISO14230 = 10400,
        ISO14230_10400 = 10400,
        ISO14230_10000 = 10000,

        J1850PWM = 41600,
        J1850PWM_41600 = 41600,
        J1850PWM_83300 = 83300,

        J1850VPW = 10400,
        J1850VPW_10400 = 10400,
        J1850VPW_41600 = 41600,

        CAN = 500000,
        CAN_125000 = 125000,
        CAN_250000 = 250000,
        CAN_500000 = 500000,

        ISO15765 = 500000,
        ISO15765_125000 = 125000,
        ISO15765_250000 = 250000,
        ISO15765_500000 = 500000,
       // ISO15765_1000000 = 1000000
    }

    public enum PinNumber
    {
        AUX = 0,
        PIN_6 = 6,
        PIN_9 = 9,
        PIN_11 = 11,
        PIN_12 = 12,
        PIN_13 = 13,
        PIN_14 = 14,
        PIN_15 = 15
    }

    public enum FilterType
    {
        PASS_FILTER = 0x01,
        BLOCK_FILTER = 0x02,
        FLOW_CONTROL_FILTER = 0x03
    }

    enum Ioctl
    {
        GET_CONFIG = 0x01,
        SET_CONFIG = 0x02,
        READ_VBATT = 0x03,
        FIVE_BAUD_INIT = 0x04,
        FAST_INIT = 0x05,
        CLEAR_TX_BUFFER = 0x07,
        CLEAR_RX_BUFFER = 0x08,
        CLEAR_PERIODIC_MSGS = 0x09,
        CLEAR_MSG_FILTERS = 0x0A,
        CLEAR_FUNCT_MSG_LOOKUP_TABLE = 0x0B,
        ADD_TO_FUNCT_MSG_LOOKUP_TABLE = 0x0C,
        DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE = 0x0D,
        READ_PROG_VOLTAGE = 0x0E,
        CLEAR_FLAG = 0x10010,
        INITIALIZE_TIMESTAMPCOUNTER,
        CLEAR_TIMESTAMPCOUNTER,
        J2534_DEBUG_START,
        J2534_DEBUG_STOP,
    }

    public enum J2534Err
    {
        STATUS_NOERROR = 0x00,
        ERR_NOT_SUPPORTED = 0x01,
        ERR_INVALID_CHANNEL_ID = 0x02,
        ERR_INVALID_PROTOCOL_ID = 0x03,
        ERR_NULL_PARAMETER = 0x04,
        ERR_INVALID_FLAGS = 0x06,
        ERR_FAILED = 0x07,
        ERR_DEVICE_NOT_CONNECTED = 0x08,
        ERR_TIMEOUT = 0x09,
        ERR_INVALID_MSG = 0x0A,
        ERR_INVALID_TIME_INTERVAL = 0x0B,
        ERR_EXCEEDED_LIMIT = 0x0C,
        ERR_INVALID_MSG_ID = 0x0D,
        ERR_DEVICE_IN_USE = 0x0E,
        ERR_INVALID_IOCTL_ID = 0x0F,
        ERR_BUFFER_EMPTY = 0x10,
        ERR_BUFFER_FULL = 0x11,
        ERR_BUFFER_OVERFLOW = 0x12,
        ERR_PIN_INVALID = 0x13,
        ERR_CHANNEL_IN_USE = 0x14,
        ERR_MSG_PROTOCOL_ID = 0x15,
        ERR_INVALID_FILTER_ID = 0x16,
        ERR_NO_FLOW_CONTROL = 0x17,
        ERR_NOT_UNIQUE = 0x18,
        ERR_INVALID_BAUDRATE = 0x19,
        ERR_INVALID_DEVICE_ID = 0x1A,
        STATUS_NOERROR_COMPLETE=0x1B,
        Error = 0x1C
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SConfig
    {
        public int Parameter;
        public int Value;
    }

    public enum SetConfigurationParameter
    {
        FinalParam = 0,
        DataRate = 1,
        Unused2 = 2,
        Loopback = 3,
        NodeAddress = 4,
        NetworkLine = 5,
        P1Min = 6,
        P1Max = 7,
        P2Min = 8,
        P2Max = 9,
        P3Min = 0x0A,
        P3Max = 0x0B,
        P4Min = 0x0C,
        P4Max = 0x0D,
        W0 = 0x19, // ???
        W1 = 0x0E,
        W2 = 0x0F,
        W3 = 0x10,
        W4 = 0x11,
        W5 = 0x12,
        TIdle = 0x13,
        TIniL = 0x14,
        TWUp = 0x15,
        Parity = 0x16,
        BitSamplePoint = 0x17,
        SyncJumpWidth = 0x18,
        T1Max = 0x1A,
        T2Max = 0x1B,
        T3Max = 0x24, // ???
        T4Max = 0x1C,
        T5Max = 0x1D,
        Iso15765BlockSize = 0x1E,
        Iso15765SeparateTimeMinimum = 0x0,
        Iso15765BlockSizeTransmit = 0x22,
        Iso15765SeparationTimeMinimum = 0x23,
        DataBits = 0x20,
        FiveBaudMod = 0x21,
        Iso15765WaitFrameTransferMax = 0x25,
        canXONXOFFStMin = 268435457,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SetConfigurationList : IDisposable
    {
        [MarshalAs(UnmanagedType.U4)]
        private UInt32 numberOfParameters;

        [MarshalAs(UnmanagedType.U4)]
        private IntPtr configurationArrayPointer;

        [MarshalAs(UnmanagedType.U4)] // Ignored
        private SetConfiguration[] configuration;

        private IntPtr thisPointer;

        public IntPtr Pointer
        {
            get
            {
                return this.thisPointer;
            }
        }

        public SetConfigurationList(SetConfiguration[] array)
        {
            this.configuration = array;
            this.numberOfParameters = (UInt32)array.Length;
            this.configurationArrayPointer = Marshal.AllocCoTaskMem(8 * this.configuration.Length);
            for (int i = 0; i < this.configuration.Length; i++)
            {
                //IntPtr temp = Marshal.AllocCoTaskMem(sizeof(SetConfiguration));
                Marshal.StructureToPtr(
                    this.configuration[i],
                    (IntPtr)((int)this.configurationArrayPointer + (i * 8)),
                    false);
                //Marshal.Copy(
                //    temp, 
                //    0, 
                //    this.configurationArrayPointer + (i * sizeof(SetConfiguration)),
                //    sizeof(SetConfiguration));
                //Marshal.FreeCoTaskMem(temp);
            }

            this.thisPointer = Marshal.AllocCoTaskMem(8);
            Marshal.StructureToPtr(this.numberOfParameters, this.thisPointer, false);
            Marshal.StructureToPtr(this.configurationArrayPointer, (IntPtr)((int)this.thisPointer + 4), false);
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(this.configurationArrayPointer);
            Marshal.FreeCoTaskMem(this.thisPointer);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct SetConfiguration
    {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Parameter;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Value;

        public SetConfiguration(SetConfigurationParameter parameter, UInt32 value)
        {
            this.Parameter = (UInt32)parameter;
            this.Value = value;
        }
    }

}
