using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringHelper
{
    public class LyricHelper
    {
        /// <summary>
        /// 根据歌词路径获取歌词对象
        /// </summary>
        /// <param name="lyricPath">歌词路径</param>
        /// <returns></returns>
        public static List<LyricLine> GetLyrics(string lyricPath)
        {
            var lrcEncoding = EncodingHelper.GetEncoding(lyricPath);
            var lrcList = System.IO.File.ReadLines(lyricPath, lrcEncoding);

            return GetLirics(lrcList.ToList());
        }

        /// <summary>
        /// 根据歌词列表获取歌词对象
        /// </summary>
        /// <param name="lrcList"></param>
        /// <returns></returns>
        public static List<LyricLine> GetLirics(IList<string> lrcList)
        {
            List<LyricLine> lyrics = new List<LyricLine>();
            foreach (var item in lrcList)
            {
                var pattern = "\\[([0-9.:]*)\\]";
                if (RegexHelper.IsMatch(item, pattern))
                {
                    List<List<string>> timeSpans = RegexHelper.MatchesGroups(item, pattern);
                    string liric = RegexHelper.Replace(item, pattern, "");
                    foreach (List<string> line in timeSpans)
                    {
                        lyrics.Add(new LyricLine(liric, TimeSpan.Parse("00:" + line[0])));
                    }
                }
                else
                {
                    lyrics.Add(new LyricLine(item, TimeSpan.Zero));
                }
            }
            return lyrics.OrderBy(m => m.StartTime).ToList();
        }
    }
    public class LyricLine
    {
        public LyricLine(string line, TimeSpan startTime)
        {
            Content = line;
            StartTime = startTime;
        }
        public LyricLine(string line)
        {
            if (line.StartsWith("[ti:"))
            {
                Content = SplitInfo(line);
            }
            else if (line.StartsWith("[ar:"))
            {
                Content = "歌手：" + SplitInfo(line);
            }
            //else if (line.StartsWith("[al:"))
            //{
            //    Console.WriteLine(SplitInfo(line));
            //}
            //else if (line.StartsWith("[by:"))
            //{
            //    Console.WriteLine(SplitInfo(line));
            //}
            //else if (line.StartsWith("[offset:"))
            //{
            //    Console.WriteLine(SplitInfo(line));
            //}
            else if (line.Length > 0)
            {
                var pattern = @"\[([0-9.:]*)\]+(.*)";
                var word = line;
                while (RegexHelper.IsMatch(word, pattern))
                {
                    var mc = RegexHelper.MatchesGroups(word, pattern);
                    TimeSpan time = new TimeSpan();
                    var Iscorrect = TimeSpan.TryParse("00:" + mc[0][0], out time);
                    word = mc[0][1];
                    if (Iscorrect)
                    {
                        Content = RegexHelper.Replace(word, @"\[([0-9.:]*)\]", "");
                        StartTime = time;
                    }
                }
            }
        }
        private static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }

        public TimeSpan StartTime { get; set; } = new TimeSpan();
        public string Content { get; set; }
    }
}
