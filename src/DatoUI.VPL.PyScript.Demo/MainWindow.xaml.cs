using System;
using System.IO;
using System.Linq;
using System.Windows;
using DatoUI.VPL.Core;
using System.Reflection;

namespace DatoUI.VPL.PyScript.Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitVPL();
        }

        void InitVPL()
        {
            KeyDown += vplControl.VplControl_KeyDown;
            KeyUp += vplControl.VplControl_KeyUp;

           
            var externalNodes = DatoUI.VPL.Utilities.Utilities.GetTypesInNamespace(Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "DatoUI.VPL.PyScript.dll")), "DatoUI.VPL.PyScript.Nodes")
                   .ToList();
            while (true)
            {
                var allOk = true;
                for (var i = 0; i < externalNodes.Count; i++)
                {
                    var nodeType = externalNodes[i];
                    if (!nodeType.IsSubclassOf(typeof(Node)))
                    {
                        externalNodes.Remove(nodeType);
                        allOk = false;
                    }
                }
                if (allOk)
                {
                    break;
                }
            }

            

            vplControl.ExternalNodeTypes.AddRange(externalNodes);

            vplControl.NodeTypeMode = NodeTypeModes.OnlyExternalTypes;

        }
    }
}
