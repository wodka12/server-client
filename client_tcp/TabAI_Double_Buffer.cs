using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

using System.Reflection;

namespace tcp_client_new
{
    class TabAI_Double_Buffer : TabPage
    {
        public TabAI_Double_Buffer()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);           
            this.UpdateStyles();
         }
    }

    class TabControl_Double_Buffer : TabControl
    {
        public TabControl_Double_Buffer()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.UpdateStyles();
        }
    }
}
