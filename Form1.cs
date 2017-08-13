using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sudoku
{
    public partial class Sudoku : Form
    {
        List<string> unknowns = new List<string>();
        List<Operation> record=new List<Operation>();
        Dictionary<string, HashSet<int>> allowed = new Dictionary<string, HashSet<int>>();
        Random random = new Random();
        bool flag = false;

        struct Operation
        {
            string type;
            string name;
            int value;

            public Operation(string type, string name, int value)
            {
                this.type = type;
                this.name = name;
                this.value = value;
            }

        }


        string getName(int x, int y)
        {
            string res = "";
            res +=(char)( x + 'A');
            res += (char)(y + '1');
            return res;
        }

        public Sudoku()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button_Click(object sender, EventArgs e)
        {
            init();
            countOut();
            
        }


        void init()
        {
            allowed.Clear();
            unknowns.Clear();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string name = getName(i, j);
                    HashSet<int> temp = new HashSet<int>();
                    unknowns.Add(name);
                    for (int k = 1; k <= 9; k++) { temp.Add(k); }
                    allowed.Add(name, temp);
                }
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string name = getName(i, j);
                    string str = this.Controls.Find(name, true)[0].Text;
                    if (str != "") { writeDown(name, str[0] - '0'); }
                }
            }
        }

        bool countOut()
        {
            while (!flag)
            {
                flag = true;
                aha1();
                aha2();
            }
            if (!check()) { return false; }
            for(int i=0;i<9;i++)
            {
                for(int j=0;j<9;j++)
                {
                    if (this.Controls.Find(getName(i, j), true)[0].Text == "") {return guessWhat(); }
                }
            }
            return true;
        }

        bool guessWhat()
        {
            Dictionary<string, HashSet<int>> dicTmp = new Dictionary<string, HashSet<int>>();
            List<string> listTmp = new List<string>();
            foreach(string name in allowed.Keys)
            {
                dicTmp.Add(name, new HashSet<int>());
                foreach (int v in allowed[name]) { dicTmp[name].Add(v); }
            }
            foreach(string name in unknowns) { listTmp.Add(name); }
            int min = 9;string tar = "";
            foreach(string name in unknowns)
            {
                if(allowed[name].Count()<min)
                { min = allowed[name].Count();tar = name; }
            }
            if (tar == "") { return false; }
            while(dicTmp[tar].Count()!=0)
            {
                int v = dicTmp[tar].First<int>();
                writeDown(tar,v);
                if (countOut()) { return true; }
                dicTmp[tar].Remove(v);
                allowed.Clear();unknowns.Clear();
                foreach (string name in dicTmp.Keys)
                {
                    allowed.Add(name, new HashSet<int>());
                    foreach (int x in dicTmp[name]) { allowed[name].Add(x); }
                }
                foreach (string name in listTmp) { unknowns.Add(name); }
            }
            return false;
        }

        bool check()
        {
            for(int i=0;i<9;i++)
            {
                for(int j=0;j<9;j++)
                {
                    if (this.Controls.Find(getName(i, j), true)[0].Text=="")
                    {
                        if(allowed[getName(i, j)].Count() == 0) { return false; }
                    }
                }
            }
            return true;
        }


        void aha1()
        {
            int v;

            foreach (string key in allowed.Keys)
            {
                if (allowed[key].Count == 1)
                {
                    v = allowed[key].First<int>();
                    writeDown(key, v);
                }
            }

        }
        void aha2()
        {
            string temp;
            string record="";
            int cnt;
            for (int k = 1; k <= 9; k++)
            {

                for (int i = 0; i < 9; i += 3)
                {
                    for (int j = 0; j < 9; j += 3)
                    {

                        cnt = 0;
                        for (int m = i; m < i + 3; m++)
                        {
                            for (int n = j; n < j + 3; n++)
                            {
                                temp = getName(m, n);
                                if (allowed[temp].Contains(k))
                                {
                                    cnt++;
                                    if (cnt > 1) { break; }
                                    record = temp;
                                }
                            }if (cnt > 1) { break; }
                        }
                        if (cnt == 1)
                        {
                            writeDown(record, k);
                        }
                    }
                }
            }
        }

        void writeDown(string name, int v)
        {
            //record.Add(new Operation("W", name, v));
            unknowns.Remove(name);
            this.Controls.Find(name, true)[0].Text = v.ToString();
            this.Controls.Find(name, true)[0].Refresh();
            removeAllowed(name, v);
            flag = false;
        }


        void removeAllowed(string name,int v)
        {
            char x = name[0];char y = name[1];
            string temp;
            foreach(int t in allowed[name])
            { record.Add(new Operation("R", name, t)); }
            allowed[name].Clear();
            for(int i=0;i<9;i++)
            {
                temp = "" + x + (char)('1' + i);
                //record.Add(new Operation("R", temp, v));
                allowed[temp].Remove(v);
                //if (allowed[temp].Count() == 1) { writeDown(temp, allowed[temp].First<int>()); }
                temp = "" + (char)('A' + i) + y;
                //record.Add(new Operation("R", temp, v));
                allowed[temp].Remove(v);
                //if (allowed[temp].Count() == 1) { writeDown(temp, allowed[temp].First<int>()); }
            }
            int xint = x - 'A';int yint = y - '1';
            xint = xint / 3 * 3;yint = yint / 3 * 3;
            for (int i = 0; i < 3; i++)
            {
                for(int j=0;j<3;j++)
                {
                    temp = "" + (char)(xint + i + 'A') + (char)(yint + j + '1');
                    //record.Add(new Operation("R", temp, v));
                    allowed[temp].Remove(v);
                    //if (allowed[temp].Count() == 1) { writeDown(temp, allowed[temp].First<int>()); }
                }
            }
        }

        private void guess_Click(object sender, EventArgs e)
        {
            guessWhat();
        }
    }
}
