using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryMaui.Services
{
    public interface IDialogsService
    {
        string ShowSaveFileDialog(string fileName, string filter);
        int ShowMessageBox(string text, string title, MessageBoxCheckFlags flags);
    }
}
