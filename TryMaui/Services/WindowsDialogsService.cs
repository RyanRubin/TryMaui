using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TryMaui.Services
{
    public class WindowsDialogsService : IDialogsService
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct OpenFileName
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public string lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int flagsEx;
        }

        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetSaveFileName(ref OpenFileName lpofn);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public string ShowSaveFileDialog(string fileName, string filter)
        {
            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.lpstrFile = fileName.PadRight(256);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFilter = $"{filter.Replace("|", "\0")}\0";

            if (GetSaveFileName(ref ofn))
            {
                return ofn.lpstrFile;
            }

            return "";
        }

        public DialogBoxCommandID ShowMessageBox(string text, string title, MessageBoxCheckFlags flags)
        {
            var result = (DialogBoxCommandID)MessageBox(IntPtr.Zero, text, title, (uint)flags);

            return result;
        }
    }
}
