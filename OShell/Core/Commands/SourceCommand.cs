namespace OShell.Core.Commands
{
    using System;
    using System.IO;

    internal class SourceCommand
    {
        public bool Execute(string args)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reads the init file for oshell
        /// </summary>
        /// <param name="path">Path to the oshell config file</param>
        private void ReadInitFile(string path)
        {
            int linenum = 0;

            try
            {
                using (StreamReader rcreader = new StreamReader(path.ToString()))
                {
                    String line;
                    while (++linenum != 0 && (line = rcreader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#") || line.Equals(' '))
                            continue;

                        string[] args = line.Split(' ');
                        //Command.SendCommand(args[0], args);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
