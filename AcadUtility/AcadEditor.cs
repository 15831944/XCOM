using Autodesk.AutoCAD.EditorInput;

namespace AcadUtility
{
    // Editor utilities
    public static class AcadEditor
    {
        public class PromptChainageOptions
        {
            private string MessageAndKeywords { get; set; }
            private string GlobalKeywords { get; set; }

            public bool AppendKeywordsToMessage { get; set; }
            public string DefaultValue { get; set; }
            public bool UseDefaultValue { get; set; }

            public bool HasKeywords
            {
                get
                {
                    var pattern = new System.Text.RegularExpressions.Regex(@"\[[^\]]*\]");
                    return (pattern.IsMatch(MessageAndKeywords));
                }
            }

            public PromptChainageOptions(string message) : this(message, "")
            {
            }

            public PromptChainageOptions(string messageAndKeywords, string globalKeywords)
            {
                MessageAndKeywords = messageAndKeywords;
                GlobalKeywords = globalKeywords;
            }

            protected internal PromptKeywordOptions ToKeywordOptions()
            {
                PromptKeywordOptions opts;
                if (string.IsNullOrEmpty(GlobalKeywords))
                {
                    opts = new PromptKeywordOptions(MessageAndKeywords);
                }
                else
                {
                    opts = new PromptKeywordOptions(MessageAndKeywords, GlobalKeywords);
                }

                opts.AllowArbitraryInput = true;
                opts.AllowNone = true;
                opts.AppendKeywordsToMessage = true;

                return opts;
            }

            protected internal PromptStringOptions ToStringOptions()
            {
                PromptStringOptions opts = new PromptStringOptions(MessageAndKeywords);
                opts.AllowSpaces = false;
                opts.DefaultValue = DefaultValue;
                opts.UseDefaultValue = UseDefaultValue;
                opts.AppendKeywordsToMessage = true;

                return opts;
            }
        }

        public class PromptChainageResult
        {
            public PromptStatus Status { get; }
            public string StringResult { get; }
            public double DoubleResult { get; }

            public PromptChainageResult(PromptStatus status, string stringResult)
            {
                Status = status;
                StringResult = stringResult;
                if (AcadText.TryChainageFromString(stringResult, out double ch))
                {
                    DoubleResult = ch;
                }
            }
        }

        public static PromptChainageResult GetChainage(this Editor ed, string message)
        {
            return ed.GetChainage(new PromptChainageOptions(message));
        }

        public static PromptChainageResult GetChainage(this Editor ed, PromptChainageOptions opts)
        {
            while (true)
            {
                PromptResult res;
                if (opts.HasKeywords)
                {
                    res = ed.GetKeywords(opts.ToKeywordOptions());
                }
                else
                {
                    res = ed.GetString(opts.ToStringOptions());
                }

                if (res.Status == PromptStatus.None)
                {
                    if (AcadText.TryChainageFromString(opts.DefaultValue, out _))
                    {
                        return new PromptChainageResult(PromptStatus.OK, opts.DefaultValue);
                    }
                    else if (AcadText.TryChainageFromString(res.StringResult, out _))
                    {
                        return new PromptChainageResult(PromptStatus.OK, res.StringResult);
                    }
                    else
                    {
                        ed.WriteMessage("Invalid chainage string.");
                        continue;
                    }
                }
                else if (res.Status == PromptStatus.OK || res.Status == PromptStatus.Keyword)
                {
                    KeywordCollection keywords = (opts.HasKeywords ? opts.ToKeywordOptions().Keywords : opts.ToStringOptions().Keywords);
                    for (int i = 0; i < keywords.Count; i++)
                    {
                        var keyword = keywords[i];
                        if (string.Compare(keyword.GlobalName, res.StringResult, true) == 0)
                        {
                            return new PromptChainageResult(PromptStatus.Keyword, keyword.GlobalName);
                        }
                    }

                    if (AcadText.TryChainageFromString(res.StringResult, out _))
                    {
                        return new PromptChainageResult(PromptStatus.OK, res.StringResult);
                    }
                    else
                    {
                        ed.WriteMessage("Invalid chainage string.");
                        continue;
                    }
                }

                return new PromptChainageResult(res.Status, res.StringResult);
            }
        }
    }
}