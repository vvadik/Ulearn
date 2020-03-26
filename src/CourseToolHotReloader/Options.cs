﻿using CommandLine;

namespace CourseToolHotReloader
{
        internal class Options
        {
            [Option('p', "path", Required = true, HelpText = "path to your course")]
            public string Path { get; set; }
        }
}