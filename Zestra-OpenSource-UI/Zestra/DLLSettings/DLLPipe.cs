using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zestra.DLLSettings
{
    class DLLPipe
    {
        public static string zespipe = "ZestraPipe";
        public static string cmdpipe = "ZestraCmd";
        public static string cpipe = "ZestraLuaC";

        public bool DLLIsInjected()
        {
            return Directory.GetFiles(@"\\.\pipe\").Contains($@"\\.\pipe\ZestraPipe");
        }

        public void DLLExecute(string script)
        {
            if (DLLIsInjected())
            {
                new Thread(delegate ()
                {
                    try
                    {
                        using (NamedPipeClientStream cspipestream = new NamedPipeClientStream(".", zespipe, PipeDirection.Out))
                        {
                            cspipestream.Connect();
                            using (StreamWriter streamWriter = new StreamWriter(cspipestream, Encoding.Default, 999999))
                            {
                                streamWriter.Write(script);
                                streamWriter.Dispose();
                            }
                            cspipestream.Dispose();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("A error occurred while connecting to the pipe!", "Problem accessing pipe!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception)
                    {
                    }
                }).Start();
            }
            else
            {
                MessageBox.Show("Please Inject Zestra before Executing Scripts!", "Please Inject Zestra!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
