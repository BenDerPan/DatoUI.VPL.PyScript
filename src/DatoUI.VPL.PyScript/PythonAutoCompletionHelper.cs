using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatoUI.VPL.PyScript
{
    public class PythonAutoCompletionHelper
    {
        /// <summary>
        /// Python的一些默认方法
        /// </summary>
        string[] completeMethods = { "print()", "str()", "ascii()", "bool()", "int()", "float()", "hex()", "bin()", "bytes()", "isinstance()", "type()", "set()", "map()", "list()", "ord()", "open()", "super()" };
        /// <summary>
        /// 代码块模板
        /// </summary>
        string[] completeSnippets = { "if ^:\n", "if ^:\n\nelse:\n", "for in ^:\n", "switch ^:\n    case : \n    break\n" };
        /// <summary>
        /// 声明代码块
        /// </summary>
        string[] completeDeclarationSnippets = {
               "class ^:\n","def ^():\n", "def __init__(self^):\n" };

        public static readonly Dictionary<char, bool> BreakChars = new Dictionary<char, bool>()
        {
            { ' ',true},
            { '.',true},
            { '"',true},
            { '\n',true},
            { '\t',true},
            {'\'' ,true},
        };

        public const string PythonKeywords = "from|import|as|while|pass|yield|break|except|raise|in|return|for|lambda|try|finally|def|class|and|or|with|else|if|elif|global|assert|print|exec|continue|lambda";

        public Dictionary<string, List<PyScriptCompletionData>> CustomKeywords;

        public PythonAutoCompletionHelper()
        {
            CustomKeywords = new Dictionary<string, List<PyScriptCompletionData>>();
            foreach (var keyword in PythonKeywords.Split(new[] { '|'},StringSplitOptions.RemoveEmptyEntries))
            {
                AddAutoCompletionByType(keyword, null);
            }
        }
        
        public void AddAutoCompletionByType(string name,Type type)
        {
            if (CustomKeywords.Keys.Contains(name))
            {
                CustomKeywords[name] = new List<PyScriptCompletionData>();
            }
            else
            {
                CustomKeywords.Add(name, new List<PyScriptCompletionData>());
            }
            if (type!=null)
            {
                foreach (var pi in type.GetProperties())
                {
                    CustomKeywords[name].Add(new PyScriptCompletionData(pi.Name));
                }
                foreach (var methodName in type.GetMethods().AsEnumerable().Select(mi => mi.Name).Distinct())
                {
                    CustomKeywords[name].Add(new PyScriptCompletionData($"{methodName}()"));
                }
            }
            
        }
    }
}
