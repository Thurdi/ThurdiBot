using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ThurdiBot {
    class squidItem {
        private object slot;
        private object time;
        private string name;
        private string brand;
        private string rarity;
        private string slots;
        private object main;
        private object sub1;
        private object sub2;
        private object sub3;
        private object common;
        private string purity;
        private string[] discordUsers;


        public squidItem() { }

        public squidItem(object time,
                         string name,
                         string brand,
                         string rarity,
                         string slots,
                         object main,
                         object sub1,
                         object sub2,
                         object sub3,
                         object common,
                         string purity,
                         string[] discordUsers,
                         string slot) {
            this.time = time;
            this.name = name;
            this.brand = brand;
            this.rarity = rarity;
            this.slots = slots;
            this.main = main;
            this.sub1 = sub1;
            this.sub2 = sub2;
            this.sub3 = sub3;
            this.common = common;
            this.purity = purity;
            this.discordUsers = discordUsers;
            this.slot = slot;
        }
        public squidItem(string name) {
            this.name = name;
        }

        public object gettime() { return this.time; }
        public string getname() { return this.name; }
        public string getbrand() { return this.brand; }
        public string getrarity() { return this.rarity; }
        public string getslots() { return this.slots; }
        public object getmain() { return this.main; }
        public object getsub1() { return this.sub1; }
        public object getsub2() { return this.sub2; }
        public object getsub3() { return this.sub3; }
        public object getcommon() { return this.common; }
        public string getpurity() { return this.purity; }
        public string getdiscordUsers() {
            string users = "";
            foreach (string user in this.discordUsers) {
                users += user;
            }
            return users;
        }
        public object getslot() { return this.slot; }
        public string toString() {
            return this.slot + " " + this.name + " " + this.brand + " " + this.rarity + " " + this.slots + " " + this.main + " " + this.sub1 + " " + this.sub2 + " " + this.sub3 + " " + getdiscordUsers();
        }

    }

}
