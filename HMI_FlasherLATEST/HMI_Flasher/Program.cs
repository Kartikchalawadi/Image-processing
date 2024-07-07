using J2534DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace HMI_Flasher
{
    internal class Program
    {

        [STAThread]
        static void Main()
        {

            
                List<string> Can_devices = new List<string>();
                Can_devices = CANCONNECTION.J2534_Get_Devices();
                CANCONNECTION.J2534_INIT("J2534");

                LoadXML_HMI ld = new LoadXML_HMI();
                for(int i=0;i < 1; i++)
                {
                  ld.LoadXMLOf_HMI();
                }


               // Execution will reach here only if LoadXMLOf_HMI completes without throwing an exception
               PrintStatment ps = new PrintStatment();
               ps.Statement();





        }


    }



}

