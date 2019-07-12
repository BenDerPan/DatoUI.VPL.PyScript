using DatoUI.VPL.Core;
using DatoUI.VPL.PyScript.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;

namespace DatoUI.VPL.PyScript.Nodes
{
    public class PyScriptNode : Node
    {
        PyScriptControl pyScript;
        public PyScriptNode(VplControl vplControl) : base(vplControl)
        {
            AddInputPortToNode("Input", typeof(object));

            AddOutputPortToNode("Output", typeof(object));
            pyScript = new PyScriptControl() { Width=450,Height=250};
            pyScript.StartCompilingEventHandler += PyScript_StartCompilingEventHandler;
            AddControlToNode(pyScript);
            BottomComment.Text = $"Description:{Environment.NewLine}Native CPython3 script execution embed in WPF, support native python packages.";
        }

        private void PyScript_StartCompilingEventHandler(object sender, EventArgs e)
        {
            var code = pyScript.CurrentFile.ScriptContent;
            if (string.IsNullOrEmpty(code))
            {
                return;
            }
            try
            {
                using (Py.GIL())
                {
                    using (var scope = Py.CreateScope())
                    {
                        scope.Exec(code);

                        dynamic analyzeClass = scope.Get<dynamic>("Analyze");
                        dynamic analyze = analyzeClass();
                        analyze.run();
                        analyze.show_graph();
                    }
                }
            }
            catch (Exception ex)
            {
                pyScript.SetError(ex.ToString());
            }
           
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
