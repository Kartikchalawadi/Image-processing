using NUnit.Framework;
using NUinit_Test_Pro;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Media;
using DirectShowLib;
using OpenCvSharp;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace HMI_Segment_test
{
    #region global_variables

    public enum Response
    {
        PASS_E,
        FAIL_E,
        EXCEPTION_E,

    }

    #endregion

    [TestFixture, Category("HMI_SEGMENT_TEST")]
    public class TestsCategory2
    {
        static int imageNumber = 1;

        string DefualtImgePath = "D:\\Defualt Images\\LatestImages\\";
        public static string fullPath = "";
        string capturedImgePath = "D:\\Imagecapture\\";
        public static string imagePath = "";
        Mat firstImg = null;
        Mat inputImage = null;


        [SetUp]
        public void SetUpCategory2()
        {

            List<string> Can_devices = new List<string>();
            Can_devices = CAN_Driver.J2534_Get_Devices();
            CAN_Driver.J2534_INIT("J2534");


        }



        [TestCase("1", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("2", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("3", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("4", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("5", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("6", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("7", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("8", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]

        [TestCase("9", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("10", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("11", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("12", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("13", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("14", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("15", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("16", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]


        [TestCase("17", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("18", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("19", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("20", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("21", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("22", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("23", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("24", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]


        [TestCase("25", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("26", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("27", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("28", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("29", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("30", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("31", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("32", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]


        [TestCase("33", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("34", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("35", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("36", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("37", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("38", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("39", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("40", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]

        [TestCase("41", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("42", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("43", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("44", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("45", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("46", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("47", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("48", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]


        [TestCase("49", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("50", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("51", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("52", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("53", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("54", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("55", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("56", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]


        [TestCase("57", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("58", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("59", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("60", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("61", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("62", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("63", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("64", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //[TestCase("1", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        //[TestCase("10", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        //[TestCase("19", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        //[TestCase("28", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        //[TestCase("37", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        //[TestCase("46", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        //[TestCase("55", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        //[TestCase("64", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]
        //////////////////////////////////////////////////////////////////////////////////////////////////////////





        public void Test1_Category2(string Tag, int Send_CANID, int[] Request_data, string imagepath, Response Excepted_Response)
        {

            string fullPath = Path.Combine(DefualtImgePath, imagepath);

            Response Actual_Response = Response.FAIL_E;
            //List<Byte> Response_Array = new List<Byte>();
            CAN_Driver.Set_Filter((UInt32)Send_CANID);

            CAN_Driver.SendNONTPCANData(Request_data, (UInt32)Send_CANID);
            //CAN_Driver.SendCANData(Request_data, (UInt32)Send_CANID);
            //Thread.Sleep(10);
            Actual_Response = ImageCapture(fullPath);


            if (Excepted_Response == Actual_Response)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }



        }


        [TestCase("65", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.FAIL_E)]
        [TestCase("66", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("67", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("68", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("69", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("70", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("71", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("72", 0x7EA, new int[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]


        [TestCase("73", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("74", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.FAIL_E)]
        [TestCase("75", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("76", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("77", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("78", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("79", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("80", 0x7EA, new int[] { 0x10, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]


        [TestCase("81", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("82", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("83", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.FAIL_E)]
        [TestCase("84", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("85", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("86", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("87", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("88", 0x7EA, new int[] { 0x10, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]


        [TestCase("89", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("90", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("91", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("92", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.FAIL_E)]
        [TestCase("93", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("94", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("95", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("96", 0x7EA, new int[] { 0x10, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]


        [TestCase("97", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("98", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("99", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("100", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("101", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.FAIL_E)]
        [TestCase("102", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("103", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("104", 0x7EA, new int[] { 0x10, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]

        [TestCase("105", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("106", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("107", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("108", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("109", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("110", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.FAIL_E)]
        [TestCase("111", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("112", 0x7EA, new int[] { 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]


        [TestCase("113", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("114", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("115", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("116", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("117", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("118", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("119", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.FAIL_E)]
        [TestCase("120", 0x7EA, new int[] { 0x10, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.PASS_E)]


        [TestCase("121", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image1.jpg", Response.PASS_E)]
        [TestCase("122", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image2.jpg", Response.PASS_E)]
        [TestCase("123", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image3.jpg", Response.PASS_E)]
        [TestCase("124", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image4.jpg", Response.PASS_E)]
        [TestCase("125", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image5.jpg", Response.PASS_E)]
        [TestCase("126", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image6.jpg", Response.PASS_E)]
        [TestCase("127", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image7.jpg", Response.PASS_E)]
        [TestCase("128", 0x7EA, new int[] { 0x10, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "image8.jpg", Response.FAIL_E)]



        public void Test1_Category3(string Tag, int Send_CANID, int[] Request_data, string imagepath, Response Excepted_Response)
        {

            string fullPath = Path.Combine(DefualtImgePath, imagepath);

            Response Actual_Response = Response.PASS_E;
            //List<Byte> Response_Array = new List<Byte>();
            CAN_Driver.Set_Filter((UInt32)Send_CANID);

            CAN_Driver.SendNONTPCANData(Request_data, (UInt32)Send_CANID);
            //CAN_Driver.SendCANData(Request_data, (UInt32)Send_CANID);
            //Thread.Sleep(10);
            Actual_Response = ImageCapture(fullPath);


            if (Excepted_Response == Actual_Response)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }



        }






        Response ImageCapture(string fullPath)
        {
            Response Actual_Response = Response.FAIL_E;

            {
                UInt16 counter = 10;
                while (counter != 0)
                {
                    // Automatic image capture without waiting for user input
                    CaptureFirstImage();

                    if (firstImg != null && fullPath != null)
                    {

                        ORB orb = ORB.Create();

                        KeyPoint[] keypoints1, keypoints2;

                        Mat descriptors1 = new Mat(), descriptors2 = new Mat();
                        try
                        {
                            inputImage = Cv2.ImRead(fullPath);

                            // Check if the image is loaded successfully
                            if (inputImage.Empty())
                            {
                                //Console.WriteLine($"Error: Unable to load the image from '{imagepath}'.");
                                //return;
                                Actual_Response = Response.EXCEPTION_E;
                                return Actual_Response;
                            }


                            orb.DetectAndCompute(inputImage, null, out keypoints1, descriptors1);
                            orb.DetectAndCompute(firstImg, null, out keypoints2, descriptors2);
                            if (!descriptors1.Empty() && !descriptors2.Empty() && descriptors1.Type() == descriptors2.Type())
                            {
                                BFMatcher bFMatcher = new BFMatcher();
                                DMatch[][] knnMatchesArray = bFMatcher.KnnMatch(descriptors1, descriptors2, k: 2);
                                List<List<DMatch>> knnMatches = knnMatchesArray.Select(matches => matches.ToList()).ToList();

                                double distanceThreshold = 0.75;

                                List<DMatch> goodMatches = new List<DMatch>();
                                foreach (var match in knnMatches)
                                {
                                    if (match[0].Distance < distanceThreshold * match[1].Distance)
                                    {
                                        goodMatches.Add(match[0]);
                                    }
                                }

                                // Print the length of good matches
                                //Console.WriteLine($"Number of good matches: {goodMatches.Count}");
                                if (goodMatches.Count >= 70)
                                {
                                    //Console.WriteLine("Images are same");
                                    Actual_Response = Response.PASS_E;

                                }
                                else
                                {
                                    //Console.WriteLine("Images are not same");
                                    //PlayBuzzerSound();
                                    Actual_Response = Response.FAIL_E;

                                }
                                break;
                            }
                            else
                            {
                                imageNumber--;
                                counter--;
                                if (counter == 0)
                                {
                                    Actual_Response = Response.EXCEPTION_E;
                                    imageNumber++;
                                }
                                else
                                {
                                    if (File.Exists(imagePath))
                                    {
                                        File.Delete(imagePath);
                                        //Console.WriteLine("File deleted successfully.");
                                    }
                                    else
                                    {
                                        //Console.WriteLine("File does not exist.");
                                    }
                                    continue;
                                    Actual_Response = Response.FAIL_E;

                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            Actual_Response = Response.EXCEPTION_E;
                            break;
                        }
                    }
                    else
                    {
                        Actual_Response = Response.EXCEPTION_E;
                        break;
                    }
                }





            }
            return Actual_Response;
        }

        //private void PlayBuzzerSound()
        //{
        //    // Load and play the buzzer sound
        //    using (SoundPlayer player = new SoundPlayer("path/to/buzzer.wav"))
        //    {
        //        player.Play();
        //    }
        //}

        private void CaptureFirstImage()
        {
            bool Found_camera = false;
            int cameraIndex = 0;
            int Count = 0;
            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].Name == "doccamera")
                {
                    cameraIndex = i;
                    Found_camera = true;
                    break;
                }
                Count++;
            }
            if (Found_camera == false)
            {
                return;
            }
            if (Count == devices.Length)
            {
                Console.WriteLine($"Error: No external cameras are connected");
                return;
            }

            VideoCapture capture = new VideoCapture(cameraIndex);
            Thread.Sleep(100); // 1-second delay


            // Check if the camera is opened successfully
            if (!capture.IsOpened())
            {
                Console.WriteLine($"Error: Unable to open camera with index {cameraIndex}.");
                return;
            }

            //Console.WriteLine("Capturing the first image...");


            /////////////////////////////////////////////////////////
            // Check if a frame is available before trying to capture it
            if (capture.Grab())
            {
                //// Set resolution if needed
                //capture.Set(CaptureProperty.FrameWidth, 640); // Adjust as needed
                //capture.Set(CaptureProperty.FrameHeight, 480); // Adjust as needed

                // Capture an image
                firstImg = new Mat();
                if (capture.Retrieve(firstImg))
                {
                    //capture.Read(firstImg);

                    // Define the path and filename for the captured image
                    imagePath = $"image{imageNumber}.jpg";

                    string DoccameraImgpath = Path.Combine(capturedImgePath, imagePath);
                    // Save the captured image with the specified filename
                    Cv2.ImWrite(DoccameraImgpath, firstImg);
                    // Console.WriteLine($"Image captured and saved as '{imagePath}'");

                    // Increment the image number for the next capture
                    imageNumber++;
                }
                else
                {
                    Console.WriteLine("Error: Unable to capture the first image.");
                }
            }
            else
            {
                Console.WriteLine("Error: No frame available for capture.");
            }

            capture.Dispose();
        }

        [TearDown]
        public void Teardown()
        {

            CAN_Driver.J2534_DEINIT();
            //Thread.Sleep(2000);
        }
    }
}
