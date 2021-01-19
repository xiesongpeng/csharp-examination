using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Examination2
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args != null && args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                Console.WriteLine("请输入程序同目录文件名或完整路径：");
                path = Console.ReadLine();
            }
            foreach (var item in MatchJobAndCandidate(path).OrderBy(p => p.Candidate.Sort))
            {
                Console.WriteLine($"{item.Candidate.Name} {item.Job.Name}");
            }
            Console.ReadLine();
        }
        public class Job
        {
            public string Name { get; set; }
        }
        public class Candidate
        {
            public string Name { get; set; }
            public int Sort { get; set; }
        }
        public class Match
        {
            public Job Job { get; set; }
            public Candidate Candidate { get; set; }
            public int JobCandidateSort { get; set; }
            public int CandidateJobSort { get; set; }
        }
        /// <summary>
        /// 匹配工作与候选人
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IList<Match> MatchJobAndCandidate(string path)
        {

            Dictionary<string, Job> dictJobs = new Dictionary<string, Job>();
            Dictionary<string, Candidate> dictCandidates = new Dictionary<string, Candidate>();
            Dictionary<string, Match> dictMatches = new Dictionary<string, Match>();

            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string line;
            int readMode = 0;//0读job匹配candidate集 1读candidate匹配job集
            bool isCandidateReaded = false;//第一排candidate集可读完
            int index = 0;//输出时排序用
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    readMode += 1;
                    continue;
                }

                var name = line.Substring(0, line.IndexOf(':'));
                var arr = line.Split(' ');
                if (readMode == 0)
                {
                    var job = new Job() { Name = name };
                    dictJobs.Add(name, job);
                    if (!isCandidateReaded)
                    {
                        for (int i = 1; i < arr.Length; i++)
                        {
                            var candidate = new Candidate() { Name = arr[i] };
                            dictCandidates.Add(arr[i], candidate);
                            dictMatches.Add(name + arr[i], new Match() { Job = job, Candidate = candidate, JobCandidateSort = i });
                        }
                        isCandidateReaded = true;
                    }
                    else
                    {
                        for (int i = 1; i < arr.Length; i++)
                        {
                            dictMatches.Add(name + arr[i], new Match() { Job = job, Candidate = dictCandidates[arr[i]], JobCandidateSort = i });
                        }
                    }
                }
                else if (readMode == 1)
                {
                    index++;
                    dictCandidates[name].Sort = index;
                    for (int i = 1; i < arr.Length; i++)
                    {
                        dictMatches[arr[i] + name].CandidateJobSort = i;
                    }
                }
            }
            List<Match> matches = new List<Match>();
            foreach (var item in dictMatches.Values.OrderBy(p => p.JobCandidateSort + p.CandidateJobSort))//按升序，最小为匹配度最高
            {
                if (dictJobs.ContainsKey(item.Job.Name) && dictCandidates.ContainsKey(item.Candidate.Name))
                {
                    matches.Add(item);
                    dictJobs.Remove(item.Job.Name);
                    dictCandidates.Remove(item.Candidate.Name);
                }
            }

            return matches;
        }
    }
}
