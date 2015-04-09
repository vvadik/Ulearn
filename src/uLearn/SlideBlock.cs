using System;

namespace uLearn
{
    public abstract class SlideBlock
    {
    }

    public class ImageGaleryBlock : SlideBlock
    {
        public string[] ImageUrls { get; set; }

        public override string ToString()
        {
            return string.Format("Images {0}", string.Join("\n", ImageUrls));
        }
    }

    public class TexBlock : SlideBlock
    {
        public TexBlock(string[] texLines)
        {
            TexLines = texLines;
        }

        public string[] TexLines { get; set; }

        public override string ToString()
        {
            return string.Format("Tex {0}", string.Join("\n", TexLines));
        }
    }

    public class MdBlock : SlideBlock
    {
        public MdBlock(string markdown)
        {
            if (markdown != null)
                Markdown = markdown.TrimEnd();
        }

        public string Markdown { get; private set; }

        public override string ToString()
        {
            return string.Format("Markdown {0}", Markdown);
        }
    }


    public class CodeBlock : SlideBlock
    {
        public readonly string Code;
        public readonly string Lang;

        public CodeBlock(string code, string lang)
        {
            Code = code;
            Lang = lang;
        }

        public override string ToString()
        {
            return string.Format("{0} code {1}", Lang, Code);
        }
    }

    public class YoutubeBlock : SlideBlock
    {
        public readonly string VideoId;

        public YoutubeBlock(string videoId)
        {
            VideoId = videoId;
        }

        public override string ToString()
        {
            return string.Format("Video {0}", VideoId);
        }
    }

    public static class SlideBlockExtensions
    {
        public static bool IsCode(this SlideBlock block)
        {
            return block is CodeBlock;
        }

        public static string Text(this SlideBlock block)
        {
            var md = block as MdBlock;
            if (md != null)
                return md.Markdown;
            var code = block as CodeBlock;
            if (code != null)
                return code.Code;
            throw new Exception(block.ToString());
        }
    }
}