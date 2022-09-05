using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FullExportPwc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IntPtr GetRootNodeHandle(IntPtr tvHandle, int NodeIndex)
        {
            const int TVGN_ROOT = 0x0;
            const int TVGN_NEXT = 0x1;

            IntPtr hNode = SendMessage(tvHandle, (int)TVM.TVM_GETNEXTITEM, new IntPtr(TVGN_ROOT), IntPtr.Zero); // Get 1st root node handle
                                                                                                                // Iterate to the node wanted if (NodeIndex) is not not the 1st node (0) and get its handle
            for (int i = 1; i <= NodeIndex; i++)
            {
                hNode = SendMessage(tvHandle, (int)TVM.TVM_GETNEXTITEM, new IntPtr(TVGN_NEXT), hNode);
            }
            return hNode; // Return the root node handle
        }

        public static ManagedWinapi.Windows.SystemWindow GetWindowFromTitle(string title)
        {
            foreach (var window in ManagedWinapi.Windows.SystemWindow.AllToplevelWindows)
            {
                if (window.Title.StartsWith(title))
                {
                    return window;
                }
            }
            return null;
        }

        public static ManagedWinapi.Windows.SystemWindow GetFirstLV(ManagedWinapi.Windows.SystemWindow sw)
        {
            foreach (var window in sw.AllChildWindows)
            {
                if (window.PreviewContent.ComponentType == "DetailsListView")
                {
                    return window;
                }
            }
            return null;
        }

        public static ManagedWinapi.Windows.SystemWindow GetFirstTV(ManagedWinapi.Windows.SystemWindow sw)
        {
            foreach (var window in sw.AllChildWindows)
            {
                if (window.PreviewContent.ComponentType == "TreeView")
                {
                    return window;
                }
            }
            return null;
        }

        public static ManagedWinapi.Windows.SystemWindow GetTextBox(ManagedWinapi.Windows.SystemWindow sw)
        {
            foreach (var window in sw.AllChildWindows)
            {
                if (window.PreviewContent.ComponentType == "TextBox")
                {
                    return window;
                }
            }
            return null;
        }

        private void SeleccionaTreeNode(
            ManagedWinapi.Windows.SystemWindow appHandle, ManagedWinapi.Windows.SystemListView lv,
            ManagedWinapi.Windows.SystemWindow tvWindow, ManagedWinapi.Windows.SystemTreeViewItem[] stvia, string carpeta)
        {
            if (stvia == null) return;

            for (int i = 0; i < stvia.Count(); i++)
            {
                SeleccionaTreeNode(appHandle, lv, tvWindow, stvia[i].handle, concatenaCarpeta(carpeta, stvia[i].Title));

                if (stvia[i].Children.Count() > 0)
                {
                    SeleccionaTreeNode(appHandle, lv, tvWindow, stvia[i].Children, concatenaCarpeta(carpeta, stvia[i].Title));
                }
            }
        }

        private string concatenaCarpeta(string carpeta, string title)
        {
            if (string.IsNullOrWhiteSpace(carpeta))
            {
                return title.Trim();
            }

            return carpeta.Trim() + "/" + title.Trim();
        }

        private void SeleccionaTreeNode(ManagedWinapi.Windows.SystemWindow appHandle, ManagedWinapi.Windows.SystemListView lv,
            ManagedWinapi.Windows.SystemWindow tvWindow, IntPtr kaklskdf, string carpeta)
        {
            uint TV_FIRST = 0x1100;
            uint TVM_SELECTITEM = (TV_FIRST + 11);
            uint TVM_EXPAND = (TV_FIRST + 2);
            //uint TVE_EXPAND = 0x2;
            uint TVGN_CARET = 0x9;

            //tvWindow.SendSetMessage1(TVM_EXPAND, TVE_EXPAND, kaklskdf);
            tvWindow.SendSetMessage1(TVM_SELECTITEM, TVGN_CARET, kaklskdf);

            //uint WM_LBUTTONDOWN = 0x201;
            //uint WM_LBUTTONUP = 0x202;
            //tvWindow.SendSetMessage1(WM_LBUTTONDOWN, 0, kaklskdf);
            //tvWindow.SendSetMessage1(WM_LBUTTONUP, 0, kaklskdf);

            //  SendKeys.SendWait("{ENTER}");

            ManagedWinapi.Windows.SystemWindow.ForegroundWindow = tvWindow;
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait("{UP}");

            //
            var tv3 = ManagedWinapi.Windows.SystemTreeView.FromSystemWindow(tvWindow);

            // Exportamos Treeview
            ExportLv(appHandle, lv, carpeta);
        }

        internal void ExportLv(ManagedWinapi.Windows.SystemWindow appHandle, ManagedWinapi.Windows.SystemListView lv, string carpeta)
        {
            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 2));
            var fechaEntrada = "1970-01-01";

            for (int i = 0; i < lv.Count; i++)
            {
                lv.SetItemSelectedState(i, false);
            }

            for (int i = 0; i < lv.Count; i++)
            {
                var dtDescription = lv[i, 0].Title;
                var dtUsername = lv[i, 1].Title;
                var dtPassword = lv[i, 2].Title;
                var dtInternetUrl = lv[i, 3].Title;
                var dtEmail = lv[i, 4].Title;
                var dtExpiration = lv[i, 5].Title;

                lv.SetItemSelectedState(i, true);
                var dtNotas = GetTextBox(appHandle).Text;

                var notillas = "";

                if (carpeta.EndsWith(dtEmail))
                {
                    dtEmail = "";
                }

                if (dtUsername.Trim() == dtEmail.Trim())
                {
                    dtEmail = "";
                }

                if (dtNotas.Trim() == dtEmail.Trim())
                {
                    dtNotas = "";
                }

                if (dtNotas.Trim() == dtUsername.Trim())
                {
                    dtNotas = "";
                }

                if (string.IsNullOrWhiteSpace(dtUsername) && !string.IsNullOrWhiteSpace(dtEmail))
                {
                    dtUsername = dtEmail;
                    dtEmail = "";
                }

                if (string.IsNullOrWhiteSpace(dtEmail))
                {
                    notillas = dtNotas.Trim();
                }
                else
                {
                    notillas = "email: " + dtEmail + Environment.NewLine + dtNotas.Trim();
                }

                System.IO.File.AppendAllText(textBox1.Text,
                    string.Format("{0},{1},{2},{3},{4},{5},{6}" + Environment.NewLine,
                    AddSep(carpeta), AddSep(dtDescription), AddSep(dtUsername), AddSep(dtPassword), AddSep(dtInternetUrl), AddSep(notillas),
                    AddSep(fechaEntrada)));

                lv.SetItemSelectedState(i, false);
            }
        }

        private string AddSep(string ss)
        {
            //var ss1 = ss.TrimEnd().Replace("\\", "\\\\");
            //var ss2 = ss1.TrimEnd().Replace("\"", "\\\"");

            var ss2 = ss.Replace("\"", "\"\"");

            if (ss2 != ss)
            {
                // MessageBox.Show(ss2);
            }

            return "\"" + ss2.Trim() + "\"";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!button2.Enabled) return;

            button2.Enabled = false;
            MessageBox.Show("Init");

            System.IO.File.Delete(textBox1.Text);

            // https://stackoverflow.com/questions/4857602/get-listview-items-from-other-windows

            var appHandle = GetWindowFromTitle("Password Corral");
            var lvWindow = GetFirstLV(appHandle);

            //var tssdf = ManagedWinapi.Windows.Contents.WindowContentParser()

            // Create a SystemWindow object from the HWND of the ListView
            //var lvWindow = new ManagedWinapi.Windows.SystemWindow(lvHandle);

            // Create a ListView object from the SystemWindow object

            var lv = ManagedWinapi.Windows.SystemListView.FromSystemWindow(lvWindow);

            // Read text from a row
            //var dtDescription = lv[0, 0].Title;
            //var dtUsername = lv[0, 1].Title;
            //var dtPassword = lv[0, 2].Title;
            //var dtInternetUrl = lv[0, 3].Title;
            //var dtEmail = lv[0, 4].Title;
            //var dtExpiration = lv[0, 5].Title;

            //
            var tvWindow = GetFirstTV(appHandle);
            var tv = ManagedWinapi.Windows.SystemTreeView.FromSystemWindow(tvWindow);

            // Expande todo
            for (int i = 0; i < tv.Roots.Count(); i++)
            {
                var kaklskdf = tv.Roots[i].handle;

                if (tv.Roots[i].Children.Count() > 0)
                {
                    uint TV_FIRST = 0x1100;
                    //uint TVM_SELECTITEM = (TV_FIRST + 11);
                    uint TVM_EXPAND = (TV_FIRST + 2);
                    uint TVE_EXPAND = 0x2;
                    //uint TVGN_CARET = 0x9;

                    tvWindow.SendSetMessage1(TVM_EXPAND, TVE_EXPAND, kaklskdf);
                    //  tvWindow.SendSetMessage1(TVM_SELECTITEM, TVGN_CARET, kaklskdf);

                    //uint WM_LBUTTONDOWN = 0x201;
                    //uint WM_LBUTTONUP = 0x202;
                    //tvWindow.SendSetMessage1(WM_LBUTTONDOWN, 0, kaklskdf);
                    //tvWindow.SendSetMessage1(WM_LBUTTONUP, 0, kaklskdf);

                    //  SendKeys.SendWait("{ENTER}");

                    //ManagedWinapi.Windows.SystemWindow.ForegroundWindow = tvWindow;
                    //SendKeys.SendWait("{DOWN}");
                    //SendKeys.SendWait("{UP}");
                }
            }

            //for (int i = 0; i < tv.Roots.Count(); i++)
            {
                SeleccionaTreeNode(appHandle, lv, tvWindow, tv.Roots, "Root");
            }

            //for (int i = 0; i < lv.Count; i++)
            //{
            //    lv.SetItemSelectedState(i, false);
            //}

            //for (int i = 0; i < lv.Count; i++)
            //{
            //    var dtDescription = lv[i, 0].Title;
            //    var dtUsername = lv[i, 1].Title;
            //    var dtPassword = lv[i, 2].Title;
            //    var dtInternetUrl = lv[i, 3].Title;
            //    var dtEmail = lv[i, 4].Title;
            //    var dtExpiration = lv[i, 5].Title;

            //    lv.SetItemSelectedState(i, true);
            //    var dtNotas = GetTextBox(appHandle).Text;

            //    MessageBox.Show(string.Format("{0} {1} {2}", dtDescription, dtUsername, dtNotas));

            //    lv.SetItemSelectedState(i, false);
            //}

            //

            MessageBox.Show("Fin");

            //            SendMessage(t, TVM_EXPAND, TVE_EXPAND, kaklskdf);

            //SendMessage(hTv, TVM_EXPAND, TVE_EXPAND, hRootNode) 'Expand the node
            //SendMessage(hTv, TVM_SELECTITEM, TVGN_CARET, hRootNode) 'Select the node

            //RECT tvRect;
            //GetWindowRect(tv.Roots[2].handle, out tvRect);

            button2.Enabled = true;
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void DoMouseClick(int X, int Y)
        {
            //Call the imported function with the cursor's current position
            //int X = Cursor.Position.X;
            //int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT rectangle);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Process process = new Process();
            //// Configure the process using the StartInfo properties.
            //process.StartInfo.FileName = @"D:\svn\Programas\pcns\password4.exe";
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            //process.Start();
            ////process.WaitForExit();// Waits here for the process to exi

            //// NOTA: Por el momento sacar valor con spy++
            //IntPtr hwndTreeView = new IntPtr(0x001611EA);

            //// Seleccionamos un Nodo
            //IntPtr hwndItem = GetRootNodeHandle(hwndTreeView, 5);

            //// Leemos su texto
            //var kkkk = AllocTest(process, hwndTreeView, hwndItem);

            ////
            //IntPtr hwndListView = new IntPtr(0x000C11E0);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        // privileges
        private const int PROCESS_CREATE_THREAD = 0x0002;

        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_VM_READ = 0x0010;

        // used for memory allocation
        private const uint MEM_COMMIT = 0x00001000;

        private const int MEM_DECOMMIT = 0x4000;
        private const uint MEM_RESERVE = 0x00002000;
        private const uint PAGE_READWRITE = 4;

        ///<summary>Returns the tree node information from another process.</summary>
        ///<param name="hwndItem">Handle to a tree node item.</param>
        ///<param name="hwndTreeView">Handle to a tree view control.</param>
        ///<param name="process">Process hosting the tree view control.</param>
        private static NodeData AllocTest(Process process, IntPtr hwndTreeView, IntPtr hwndItem)
        {
            // https://stackoverflow.com/questions/27682170/sendmessage-treeview-tvm-getitem-crashes-that-process

            // code based on article posted here: http://www.codingvision.net/miscellaneous/c-inject-a-dll-into-a-process-w-createremotethread

            // handle of the process with the required privileges
            IntPtr procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, process.Id);

            // Write TVITEM to memory
            // Invoke TVM_GETITEM
            // Read TVITEM from memory

            var item = new TVITEMEX();
            item.hItem = hwndItem;
            item.mask = (int)(TVIF.TVIF_HANDLE | TVIF.TVIF_CHILDREN | TVIF.TVIF_TEXT);
            item.cchTextMax = 1024;
            item.pszText = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)item.cchTextMax, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE); // node text pointer

            byte[] data = getBytes(item);

            uint dwSize = (uint)data.Length;
            IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, dwSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE); // TVITEM pointer

            uint nSize = dwSize;
            UIntPtr bytesWritten;
            bool successWrite = WriteProcessMemory(procHandle, allocMemAddress, data, nSize, out bytesWritten);

            var sm = SendMessage(hwndTreeView, (int)TVM.TVM_GETITEM, IntPtr.Zero, allocMemAddress);

            UIntPtr bytesRead;
            bool successRead = ReadProcessMemory(procHandle, allocMemAddress, data, nSize, out bytesRead);

            UIntPtr bytesReadText;
            byte[] nodeText = new byte[item.cchTextMax];
            bool successReadText = ReadProcessMemory(procHandle, item.pszText, nodeText, (uint)item.cchTextMax, out bytesReadText);

            bool success1 = VirtualFreeEx(procHandle, allocMemAddress, dwSize, MEM_DECOMMIT);
            bool success2 = VirtualFreeEx(procHandle, item.pszText, (uint)item.cchTextMax, MEM_DECOMMIT);

            var item2 = fromBytes<TVITEMEX>(data);

            String name = Encoding.Default.GetString(nodeText);
            int x = name.IndexOf('\0');
            if (x >= 0)
                name = name.Substring(0, x);

            NodeData node = new NodeData();
            node.Text = name;
            node.HasChildren = (item2.cChildren == 1);

            return node;
        }

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public class NodeData
        {
            public String Text { get; set; }
            public bool HasChildren { get; set; }
        }

        private static byte[] getBytes(Object item)
        {
            int size = Marshal.SizeOf(item);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(item, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        private static T fromBytes<T>(byte[] arr)
        {
            T item = default(T);
            int size = Marshal.SizeOf(item);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            item = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return item;
        }

        // Note: different layouts required depending on OS versions.
        // https://msdn.microsoft.com/en-us/library/windows/desktop/bb773459%28v=vs.85%29.aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct TVITEMEX
        {
            public uint mask;
            public IntPtr hItem;
            public uint state;
            public uint stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
            public int iIntegral;
            public uint uStateEx;
            public IntPtr hwnd;
            public int iExpandedImage;
            public int iReserved;
        }
    }
}