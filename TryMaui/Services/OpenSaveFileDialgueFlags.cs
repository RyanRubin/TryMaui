using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryMaui.Services
{
    public enum OpenSaveFileDialgueFlags : int
    {
        OFN_READONLY = 0x1,
        OFN_OVERWRITEPROMPT = 0x2,
        OFN_HIDEREADONLY = 0x4,
        OFN_NOCHANGEDIR = 0x8,
        OFN_SHOWHELP = 0x10,
        OFN_ENABLEHOOK = 0x20,
        OFN_ENABLETEMPLATE = 0x40,
        OFN_ENABLETEMPLATEHANDLE = 0x80,
        OFN_NOVALIDATE = 0x100,
        OFN_ALLOWMULTISELECT = 0x200,
        OFN_EXTENSIONDIFFERENT = 0x400,
        OFN_PATHMUSTEXIST = 0x800,
        OFN_FILEMUSTEXIST = 0x1000,
        OFN_CREATEPROMPT = 0x2000,
        OFN_SHAREAWARE = 0x4000,
        OFN_NOREADONLYRETURN = 0x8000,
        OFN_NOTESTFILECREATE = 0x10000,
        OFN_NONETWORKBUTTON = 0x20000
    }
}
