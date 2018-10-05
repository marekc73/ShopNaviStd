using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using ShopNavi.Data;

namespace ShopNavi
{
    [XmlRoot]
    [XmlInclude(typeof(SettingsBaseVM))]
    public class SettingsVM : SettingsBaseVM
    {
        public SettingsVM()
            : base()
        {
            StoreFactory.Settings = this;
            settings = this;

            this.SaveCmd = new CommandHandler(() =>
            {
                StoreFactory.HalProxy.FinalizeData();
            });
        }

        public SettingsVM(SettingsBaseVM src)
            : base(src)
        {
            StoreFactory.Settings = this;
            settings = this;

            this.SaveCmd = new CommandHandler(() =>
            {
                StoreFactory.HalProxy.FinalizeData();
            });
        }

        private static SettingsBaseVM settings = null;

        [XmlIgnore]
        public static SettingsBaseVM Settings
        {
            get
            {
                return settings;
            }
        }

        [XmlIgnore]
        public ICommand SaveCmd { get; protected set; }

    }
}
