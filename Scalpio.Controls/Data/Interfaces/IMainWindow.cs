using Scalpio.Controls.Terminal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Scalpio.Controls.Data.Interfaces
{
    public interface IMainWindow
    {
        public void Over()
        {
            //Status.Text = "Over";
        }

        public void NotOver()
        {
            //Status.Text = "Not Over";
        }

        public IEnumerable<TradingFrame> GetTradingFrame();
    }
}
