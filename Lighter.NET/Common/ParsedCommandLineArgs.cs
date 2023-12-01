namespace Lighter.NET.Common
{
    /// <summary>
    /// 解析過的命令列參數項集合(字典)
    /// </summary>
    public class ParsedCommandLineArgs
    {
        Dictionary<string, CommandLineArg> _argDic = new Dictionary<string, CommandLineArg>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// 命令列參數項：參數名稱必須是--開頭，與參數值之間以空白或是=分隔，若有名無值，則視為flag參數，
        /// 例如(空白分隔)：--param1 value1 --param2 value2 --param3 --param4 value4
        /// 傳入時Windows會自動拆解成string[]: param1,value1,param2,value2,param3,param4,value4
        /// 例如(等號分隔)：--param1=value1 --param2=value2 --param3 --param4=value4
        /// 傳入時Windows會自動拆解成string[]: param1,=,value1,param2,=,value2,param3,param4,=,value4
        /// </param>
        public ParsedCommandLineArgs(string[] args)
        {
            _argDic = ParseArgs(args);
        }

        public CommandLineArg? this[string argName]
        {
            get
            {
                if (_argDic.ContainsKey(argName))
                {
                    return _argDic[argName];
                }
                else
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// 解析命令列參數，並將string[]型態的參數轉換成dictionary型態
        /// </summary>
        /// <param name="args">命令列參數</param>
        /// <returns></returns>
        private Dictionary<string, CommandLineArg> ParseArgs(string[] args)
        {
            Dictionary<string, CommandLineArg> argDic = new Dictionary<string, CommandLineArg>();

            if (args == null || args.Length == 0) return argDic;

            for (int i = 0; i < args.Length; i++)
            {
                //參數項名稱必須是--開頭
                if (args[i].StartsWith("--"))
                {
                    var argItem = new CommandLineArg();
                    //參數名
                    argItem.Name = args[i].TrimStart('-')?.ToLower();

                    if (string.IsNullOrEmpty(argItem.Name)) continue;

                    //參數值
                    /*若i不是最後一項，且
                     * 若下一個不是=號，且不是--開頭，則取值
                     * 若是--開頭，則不用取值，若是=號則取下一個值*/
                    if (i + 1 < args.Length)
                    {
                        if (args[i + 1] != "=" && !args[i + 1].StartsWith("--"))
                        {
                            argItem.Value = args[i + 1];
                            i++;
                        }
                        else if (args[i + 1].StartsWith("--"))
                        {
                            continue;
                        }
                        else if (args[i + 1] == "=")
                        {
                            i++; //略過一個
                            if (i + 1 < args.Length)
                            {
                                if (!args[i + 1].StartsWith("--"))
                                {
                                    argItem.Value = args[i + 1];
                                    i++;
                                }
                            }
                        }
                    }

                    //若參數項已存在=>覆蓋，否則=>新增
                    if (argDic.ContainsKey(argItem.Name))
                    {
                        argDic[argItem.Name] = argItem;
                    }
                    else
                    {
                        argDic.Add(argItem.Name, argItem);
                    }
                }
            }
            return argDic;
        }
    }


}
