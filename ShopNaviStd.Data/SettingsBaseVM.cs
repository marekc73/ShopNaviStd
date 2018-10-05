using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShopNavi.Data
{
    [XmlRoot]
    public class SettingsBaseVM : BaseVM
    {
        private ObservableCollection<string> languages = new ObservableCollection<string>();

        [XmlAttribute]
        public bool FromCloud
        {
            get
            {
                return this.fromCloud;
            }
            set
            {
                this.fromCloud = value;
                OnPropertyChanged("FromCloud");
                OnPropertyChanged("FromSMS");
            }
        }

        [XmlIgnore]
        public bool FromSMS
        {
            get
            {
                return !this.FromCloud;
            }
        }

        [XmlAttribute]
        public string CloudKey
        {
            get
            {
                return this.cloudKey;
            }
            set
            {
                this.cloudKey = value;
                OnPropertyChanged("CloudKey");
            }
        }

        [XmlAttribute]
        public string SMSKey
        {
            get
            {
                return this.smsKey;
            }
            set
            {
                this.smsKey = value;
                OnPropertyChanged("SMSKey");
            }
        }

        [XmlAttribute]
        public string SMSNumber
        {
            get
            {
                return this.smsNumber;
            }
            set
            {
                this.smsNumber = value;
                OnPropertyChanged("SMSNumber");
            }
        }

        [XmlArray]
        public ObservableCollection<string> Languages
        {
            get
            {
                return this.languages;
            }
            set
            {
                this.languages = value;
                OnPropertyChanged("Languages");
            }
        }

        [XmlAttribute]
        public string CurrentLanguageName
        {
            get
            {
                return this.currentLanguageName;
            }
            set
            {
                this.currentLanguageName = value;
                OnPropertyChanged("CurrentLanguageName");
                OnPropertyChanged("CurrentLanguage");
            }
        }

        [XmlIgnore]
        public CultureInfo CurrentLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(this.CurrentLanguageName))
                {
                    return CultureInfo.CurrentUICulture;
                }
                else
                {
                    return new CultureInfo(this.CurrentLanguageName);
                }
            }
            set
            {
                OnPropertyChanged("CurrentLanguage");
            }
        }

        [XmlAttribute]
        public string SpeechDelimiter
        {
            get
            {
                return this.speechDelimiter;
            }
            set
            {
                this.speechDelimiter = value;
                OnPropertyChanged("SpeechDelimiter");
            }
        }

        [XmlAttribute]
        public string GmailClientId
        {
            get
            {
                return this.androidClientId;
            }
            set
            {
                this.androidClientId = value;
                OnPropertyChanged("GmailClientId");
            }
        }

        //public string iOSClientId = "<insert IOS client ID here>";
        public string androidClientId = Constants.AndroidClientId;
        private CultureInfo currentLanguage = CultureInfo.CurrentUICulture;
        private string speechDelimiter = "dalej";
        private bool fromCloud = true;
        private string cloudKey;
        private string smsKey;
        private string currentLanguageName;
        private string author;
        private string smsNumber = "0903723852";
        private string gmailSubject = "nakup";
        private string gmailFrom = "silviacaudtova@gmail.com";

        public void Save(Stream file)
        {
            using (TextWriter textWriter = new StreamWriter(file))
            {
                var xmlSerializer = new XmlSerializer(typeof(SettingsBaseVM));
                xmlSerializer.Serialize(textWriter, this);
            }
        }

        public static SettingsBaseVM Read(Stream file)
        {
            using (TextReader textReader = new StreamReader(file))
            {
                var xmlSerializer = new XmlSerializer(typeof(SettingsBaseVM));
                return xmlSerializer.Deserialize(textReader) as SettingsBaseVM;
            }
        }

        public SettingsBaseVM()
        {
            this.CloudKey = "Nakup";
            this.SMSKey = "Nakup";

            this.FromCloud = true;
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant() != "sk")
            {
                this.Languages.Add("sk");
            }

            this.Languages.Add(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            if (string.IsNullOrEmpty(this.CurrentLanguageName))
            {
                this.CurrentLanguageName = this.Languages.FirstOrDefault();
            }
        }

        public SettingsBaseVM(SettingsBaseVM src)
        {
            if (src != null)
            {
                this.CloudKey = src.CloudKey;
                this.FromCloud = src.FromCloud;
                this.SMSKey = src.SMSKey;
                this.CurrentLanguageName = src.CurrentLanguageName;
                this.SpeechDelimiter = src.SpeechDelimiter;
                this.Languages = src.Languages;
                this.Author = src.Author;
            }
            else
            {
                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant() != "sk")
                {
                    this.Languages.Add("sk");
                }
                this.Languages.Add(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                if (string.IsNullOrEmpty(this.CurrentLanguageName))
                {
                    this.CurrentLanguageName = this.Languages.FirstOrDefault();
                }
            }
        }

        public override void Save()
        {
        }

        public string GetAuthor()
        {
            return string.IsNullOrEmpty(this.Author) ? StoreFactory.HalProxy.UserName : this.Author;
        }

        [XmlAttribute]
        public string Author 
        { 
            get
            {
                return this.author;
            }
            set
            {
                this.author = value;
                OnPropertyChanged("Author");
            }
        }

        [XmlAttribute]
        public string GmailSubject {
            get
            {
                return this.gmailSubject;
            }
            set
            {
                this.gmailSubject = value;
            }
        }
        [XmlAttribute]
        public string GmailFrom
        {
            get
            {
                return this.gmailFrom;
            }
            set
            {
                this.gmailFrom = value;
            }
        }
    }
}
