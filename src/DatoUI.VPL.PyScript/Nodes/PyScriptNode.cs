using DatoUI.VPL.Core;
using DatoUI.VPL.PyScript.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatoUI.VPL.PyScript.Nodes
{
    public class PyScriptNode : Node
    {
        PyScriptControl pyScript;
        public PyScriptNode(VplControl vplControl) : base(vplControl)
        {
            AddInputPortToNode("Input", typeof(object));

            AddOutputPortToNode("Output", typeof(object));
            pyScript = new PyScriptControl() { Width=350,Height=200};
            AddControlToNode(pyScript);
            BottomComment.Text = $"Description:{Environment.NewLine}Native CPython3 script execution embed in WPF, support native python packages.";
        }
        public override void Calculate(object userState = null)
        {
            
        }

        public override Node Clone()
        {
            var newNode = new PyScriptNode(HostCanvas)
            {
                Top = Top,
                Left = Left,
            };
            newNode.BottomComment.Text = BottomComment.Text;
            return newNode;
        }
    }
}
