using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public class StoreFactory
    {
        public static IHal HalProxy
        {
            get
            {
                return halProxy;
            }
            set
            {
                halProxy = value;
            }
        }

        private static IHal halProxy = null;

        public static Store CreateStore(string name, string location)
        {
            Store ret = null;
            if(name == "Kaufland" && location == "Mlynska")
            {
                ret = CreateStoreTemplate(name, location);
            }
            else if (name == "Lidl" && location == "Skultetyho")
            {
                ret = CreateLidl1();
            }
            else if (name == "Billa" && location == "Racianska")
            {
                ret = CreateBilla1();
            }
            else
            {
                ret = CreateStoreTemplate(name, location);
            }
            if (ret != null)
            {
                ret.AssignParent(StoreFactory.CurrentVM);
            }
            return ret;
        }
        
        public static Store CreateStoreTemplate(string name, string location)
        {
            Store st = new Store()
            {
                ImageName = "",
                Name = name,
                Location = location,
                Index = 0
            };

            int i=1;
            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Ovocie",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zelenina",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Korenie",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Konzervy",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Oleje",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Cestoviny",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Elektrika",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zahrada",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Caj",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Kava",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Pecivo",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Syry",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Sunky",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mrazene",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Maso",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Maslo",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Jogurty",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mlieko",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Vajcia",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Vino",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Cokolada",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Detske",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Drogeria",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mineralky",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Pivo",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Prasky",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zmrzlina",
                    Index = i++,
                    Parent = st
                });

            st.InitStore(StoreFactory.Items);
            return st;
        }

        public static Store CreateLidl1()
        {
            Store st = new Store()
            {
                Name = "Lidl",
                Location = "Skultetyho",
                Index = 1
            };

            int i = 1;
            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Caj",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Kava",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Cokolada",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Pecivo",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zelenina",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Ovocie",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Maso",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mineralky",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Korenie",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mrazene",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Sunky",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Maslo",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Syry",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Jogurty",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zmrzlina",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mlieko",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Vajcia",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Elektrika",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zahrada",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Konzervy",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Oleje",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Cestoviny",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Vino",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Detske",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Drogeria",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Pivo",
                    Index = i++
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Prasky",
                    Index = i++
                });

            st.InitStore(StoreFactory.Items);
            return st;
        }

        public static Store CreateBilla1()
        {
            Store st = new Store()
            {
                Name = "Billa",
                Location = "Racianska",
                Index = 2
            };

            int i = 1;
            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zelenina",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Ovocie",
                    Index = i++,
                    Parent = st
                });


            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Maslo",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Jogurty",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Pecivo",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Syry",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Sunky",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mrazene",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Maso",
                    Index = i++,
                    Parent = st
                });


            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mlieko",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Vajcia",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Mineralky",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Pivo",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Vino",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Oleje",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Cestoviny",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Korenie",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Konzervy",
                    Index = i++,
                    Parent = st
                });


            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Caj",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Kava",
                    Index = i++,
                    Parent = st
                });


            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Cokolada",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Detske",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Drogeria",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Prasky",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Zmrzlina",
                    Index = i++,
                    Parent = st
                });

            st.Sections.Add(
                new Section()
                {
                    Id = i,
                    Name = "Elektrika",
                    Index = i++,
                    Parent = st
                });

            st.InitStore(StoreFactory.Items);
            return st;
        }

        private static AllItems allItems = new AllItems();


        public static AllItems Items
        {
            get
            {
                return allItems;
            }
            set
            {
                allItems = value;
            }
        }

        private static Store currentStore = null;
        /// <summary>
        /// Synchronization handle.
        /// </summary>
        private static readonly object SyncLock = new object();
        private static SettingsBaseVM settings;
        private static int LastId;
        private static Object lockObj = new Object();

        public static Store CurrentStore
        {
            get
            {
                lock (SyncLock)
                {
                    return StoreFactory.currentStore;
                }
            }
            set
            {
                lock(SyncLock)
                {
                    StoreFactory.currentStore = value;
                }
            }
        }

        internal static Section FindSection(Item item)
        {
            var ret = CurrentStore.Sections.FirstOrDefault(x => (x.Id == item.LinkId));
            if(ret == null)
            {
                ret = new Section() { Id = 0, Name = string.Empty } ;
            }

            return ret;
        }

        internal static int NewId()
        {
            lock(StoreFactory.lockObj)
            {
                return StoreFactory.LastId ++;
            }
        }

        public static void SetLastId(int val)
        {
            lock(StoreFactory.lockObj)
            {
                StoreFactory.LastId = val + 1;
            }
        }

        internal static int GetSectionIndexForName(string expr, string sectionName)
        {
            string key = string.Empty;
            switch(expr)
            {
                case "rozky":
                case "chleba":
                case "orechovnik":
                case "tvarohovnik":
                    key = "pecivo";
                    break;
                case "jablka":
                case "jahody":
                case "hrusky":
                case "pomarance":
                case "hrozno":
                case "banany":
                    key = "ovocie";
                    break;
                case "cherry":
                case "rajciny":
                case "cibula":
                case "cesnak":
                case "brokolica":
                case "cukina":
                case "mrkva":
                case "petrzlen":
                case "paprika":
                case "salat":
                case "rucola":
                case "zemiaky":
                    key = "zelenina";
                    break;
                case "fazula":
                case "olivy":
                case "pasirovane rajciny":
                case "kukurica":
                case "hrach":
                    key = "konzervy";
                    break;
                case "cestoviny":
                    key = "cestoviny";
                    break;
                case "ovocny caj":
                case "zeleny caj":
                case "prieduskovy caj":
                    key = "caj";
                    break;
                case "eidam":
                case "tvrdy syr":
                case "gouda":
                case "mozarella":
                case "nova":
                    key = "syry";
                    break;
                case "sunka":
                case "salama":
                case "klobasa":
                case "slanina":
                    key = "sunky";
                    break;
                case "kura":
                case "hovadzie":
                case "mlete":
                case "bravcove":
                case "rezne":
                    key = "maso";
                    break;
                case "file":
                case "mrazena zelenina":
                    key = "mrazene";
                    break;
                case "jogurty":
                case "lipanek":
                    key = "jogurty";
                    break;
                case "maslo":
                case "cottage":
                case "smotana":
                case "mozarella balena":
                case "bambino":
                case "baby bell":
                    key = "maslo";
                    break;
                case "pivo":
                case "radler":
                    key = "pivo";
                    break;
                case "mineralka":
                case "lucka":
                case "mineralka pre tobiho":
                case "drobcek":
                case "mineralka pre navstevy":
                    key = "mineralka";
                    break;
                case "perwoll":
                case "ariel":
                    key = "prasky";
                    break;
                case "toaletny papier":
                case "kuchynske utierky":
                case "mydlo":
                case "tekute mydlo":
                    key = "drogeria";
                    break;
                default:
                    key = "#";
                    break;
            }
            try
            {
                return  CurrentStore.Sections.FirstOrDefault(x => x.Name.StartsWith(key,StringComparison.CurrentCultureIgnoreCase)
                    || (!string.IsNullOrEmpty(sectionName) && x.Name.StartsWith(sectionName, StringComparison.CurrentCultureIgnoreCase))).Id;
            }
            catch
            {
                return -1;
            }
        }

        public static List<Store> GetAllStores(Stream[] storeStreams)
        {
            List<Store> storeList = new List<Store>();
            if (storeStreams != null)
            { 
                foreach (var file in storeStreams)
                try
                {
                    storeList.Add(Store.Read(file));                
                }
                catch(Exception ex)
                {                    
                    throw ex;
                }

            }
            GetHardcodedStores(storeList);

            return storeList;
        }

        public static void GetHardcodedStores(List<Store> list)
        {
            ObservableCollection<Store> storeList = new ObservableCollection<Store>();

            storeList.Add(
                StoreFactory.CreateStore("Kaufland", "Mlynska"));
            storeList.Add(
                StoreFactory.CreateStore("Lidl", "Skultetyho"));
            storeList.Add(
                StoreFactory.CreateStore("Billa", "Racianska"));

            int ix = 2;
            storeList.Add(
                new Store()
                {
                    //Image = new Xamarin.Forms.Image(),
                    ImageName="kaufland.png",
                    Name = "Kaufland",
                    Location = "Trnavska",
                    Index = ix++
                });

            storeList.Add(
                new Store()
                {
                    //Image = new Xamarin.Forms.Image(),
                    ImageName = "lidl.png",
                    Name = "Lidl",
                    Location = "Mlada garda",
                    Index = ix++
                });

            storeList.Add(
                new Store()
                {
                    //Image = new Xamarin.Forms.Image(),
                    Name = "Billa",
                    Location = "Tomasikova",
                    Index = ix++
                });

            foreach(var store in storeList)
            {
                if(! list.Any(x => (x.Name == store.Name && x.Location == store.Location)))
                {
                    list.Add(store);
                }
            }
        }

        public static List<Item> GetInitialInput()
        {
            List<string> list = new List<string>();
            list.Add("rozky" );
            list.Add("sunka" );
            list.Add("rajciny");
            list.Add("mozarella");
            list.Add("kura");
            list.Add("jogurty");
            list.Add("cestoviny");
            list.Add("radler" );
            return StoreFactory.Items.InitInput(list);
        }

        public static CommonBaseVM CurrentVM 
        { 
            get; 
            set; 
        }

        public static BaseVM MainVM
        {
            get;
            set;
        }

        public static SettingsBaseVM Settings
        {
            get
            {
                if(settings == null)
                {
                    settings = new SettingsBaseVM();
                }

                return settings;
            }
            set
            {
                settings = value;
            }
        }
    }
}
