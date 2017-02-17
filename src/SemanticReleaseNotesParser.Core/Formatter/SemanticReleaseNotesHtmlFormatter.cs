using CommonMark.Formatters;
using CommonMark;
using System.IO;
using CommonMark.Syntax;

namespace SemanticReleaseNotesParser.Core.Formatter
{
    internal class SemanticReleaseNotesHtmlFormatter : HtmlFormatter
    {
        public SemanticReleaseNotesHtmlFormatter(TextWriter target, CommonMarkSettings settings) : base(target, settings)
        {
        }

        protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;

            switch (block.Tag)
            {
                case BlockTag.ListItem:
                    if (isOpening)
                    {
                        EnsureNewLine();
                        Write("<li");
                        if (block.ListData.ListType == ListType.Ordered)
                        {
                            Write(string.Format(" srn-priority=\"{0}\"", block.ListData.Start));
                        }
                        Write(">");
                    }

                    if (isClosing)
                    {
                        WriteLine("</li>");
                    }
                    break;

                case BlockTag.List:
                    if (isOpening)
                    {
                        EnsureNewLine();
                        Write("<ul");
                        if(block.ListData.ListType == ListType.Ordered)
                        { 
                            Write(" class=\"srn-priorities\"");
                        }
                        Write(">");
                        RenderTightParagraphs.Push(block.ListData.IsTight);
                    }

                    if (isClosing)
                    {
                        WriteLine("</ul>");
                        RenderTightParagraphs.Pop();
                    }

                    break;

                default: base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
                    break;
            }
        }
    }
}
