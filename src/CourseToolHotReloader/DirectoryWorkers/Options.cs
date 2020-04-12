﻿using CommandLine;

namespace CourseToolHotReloader.DirectoryWorkers
{
        internal class Options
        {
            [Option('p', "path", Required = true, HelpText = "path to your course")]
            public string Path { get; set; }
        }
}