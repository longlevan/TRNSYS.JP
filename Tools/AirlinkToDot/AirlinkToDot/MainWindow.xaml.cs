﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32; //追加
using System.IO;

namespace AirlinkToDot
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadBui_Click(object sender, RoutedEventArgs e)
        {

            List<string> strList = new List<string>();

            BuiFile buiFile = new BuiFile();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.DefaultExt = "*.b*";
            ofd.Filter = "TRNBuld File (*.bui;*.b17)|*.bui;*.b17";
            if (ofd.ShowDialog() == true)
            {
                buiFile.Load(ofd.FileName);

                string dir = System.IO.Path.GetDirectoryName(ofd.FileName);
                string filename = dir+@"\TRNFlow.gv";
                bool append = false; // 上書き


                #region "TRNFlow/LINK から *.vg形式へ変換する"

                strList.Add("digraph {");
                strList.Add("    rankdir=LR;");

                string zones="";
                string extNodes="";
                string auxNodes="";

                foreach (Link link in buiFile.LinkList.Links)
                {
                    string node;
                    node = link.FromNode;
                    parseNode(ref zones, ref extNodes, ref auxNodes, node);
                    node = link.ToNode;
                    parseNode(ref zones, ref extNodes, ref auxNodes, node);
                }
                strList.Add("    node [shape = doublecircle]; " + zones + ";");
                strList.Add("    node [shape = circle]; " + extNodes + ";");
                strList.Add("    node [shape = rectangle]; " + auxNodes + ";"); 
                foreach (Link link in buiFile.LinkList.Links)
                {
                    string str = "    " + link.FromNode + " -> " + link.ToNode + "[ label = \"" + link.LinkType + "\" ];";
                    strList.Add(str);
                }
                strList.Add("}  ");

                #endregion



                // 画面へ表示
                string vg="";
                foreach (string str in strList)
                {
                    vg += str +"\n";
                }
                this.txtBox.Text = vg;

                // ファイルへ保存
                using (StreamWriter writer = new StreamWriter(filename, append, Encoding.GetEncoding("shift_jis")))　//Shift_JIS
                {
                    foreach (string str in strList)
                    {
                        writer.WriteLine(str);
                    }
                }



            }
        }

        /// <summary>
        /// Parse the node type 
        /// </summary>
        /// <param name="zones"></param>
        /// <param name="extNodes"></param>
        /// <param name="auxNodes"></param>
        /// <param name="node"></param>
        private static void parseNode(ref string zones, ref string extNodes, ref string auxNodes, string node)
        {
            if (node.StartsWith("EN_") )
            {
                if (extNodes.IndexOf(node) < 0) extNodes += node + " ";
                return;
            }
            else if (node.StartsWith("AN_"))
            {
                if (auxNodes.IndexOf(node) < 0) auxNodes += node + " ";
                return;
            }
            else if (zones.IndexOf(node) < 0)
            {
                zones += node + " ";
            }
        }
    }
}
