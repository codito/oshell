//-----------------------------------------------------------------------
// <copyright file="GetOpt.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A simple command line parser
    /// </summary>
    internal class GetOpt
    {
        // shortOptions and longOptions map the _short_option_char_ and _long_option_string_ to their ids.
        // the _id_ will be +ve if the _option_ has argument else the id will be -ve, we would return
        // the _short_option_char_ to the user once we parse an option using the _id_ and the map
        // shortOptionMap. All this complex stuff for O(1) map operations. Is it worth? Anything better?
        private Dictionary<char, int> shortOptions;
        private Dictionary<string, int> longOptions;
        private Dictionary<int, char> shortOptionMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOpt"/> class.
        /// </summary>
        /// <param name="options">Array of all possible options</param>
        public GetOpt(Option[] options)
        {
            int idCount = 1;

            this.shortOptions = new Dictionary<char, int>();
            this.longOptions = new Dictionary<string, int>();
            this.shortOptionMap = new Dictionary<int, char>();
            this.Description = string.Empty;

            foreach (Option option in options)
            {
                if (option.HasArgument == false)
                {
                    idCount = -idCount;
                }

                try
                {
                    this.shortOptions.Add(option.ShortOption, idCount);
                    this.longOptions.Add(option.LongOption, idCount);
                }
                catch (ArgumentException e)
                {
                    throw new DuplicateOptionException("Duplicate option found", e);
                }

                this.shortOptionMap.Add(idCount, option.ShortOption);

                // FIXME make this a stringbuilder
                this.Description += "-" + option.ShortOption + ", --" + option.LongOption + "\t" + option.Description + "\n";

                idCount = idCount < 0 ? -idCount : idCount;
                idCount++;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the description for the options.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
        #endregion

        /// <summary>
        /// Parse the command line arguments.
        /// </summary>
        /// <param name="userOptions">Array of command line arguments</param>
        /// <param name="optionIndex">Keeps count of the argument to be parsed in next call</param>
        /// <param name="optionArgument">Contains the argument passed to a command line option</param>
        /// <returns>Short option for the user entered argument. If no more options are left, returns <c>space</c>.</returns>
        public char Parse(string[] userOptions, ref int optionIndex, out string optionArgument)
        {
            char parsedOption = ' ';
            optionArgument = null;

            if (optionIndex >= 0 && optionIndex < userOptions.Length)
            {
                int id = 0;
                string currentOption = userOptions[optionIndex];

                if (currentOption.StartsWith("--"))
                {
                    // long option
                    currentOption = currentOption.Substring(2, currentOption.Length - 2);
                    try
                    {
                        id = this.longOptions[currentOption];
                    }
                    catch (KeyNotFoundException e)
                    {
                        // stop the culprit
                        optionIndex = -1;
                        throw new InvalidOptionException(currentOption + ": Invalid option", e);
                    }

                    if (id > 0)
                    {
                        optionArgument = userOptions[optionIndex++];
                    }
                }
                else if (currentOption.StartsWith("-"))
                {
                    // short option
                    currentOption = currentOption.Substring(1, currentOption.Length - 1);
                    try
                    {
                        id = this.shortOptions[currentOption.ToCharArray()[0]];
                    }
                    catch (KeyNotFoundException e)
                    {
                        optionIndex = -1;
                        throw new InvalidOptionException(currentOption + ": Invalid option", e);
                    }

                    if (id > 0)
                    {
                        optionArgument = userOptions[optionIndex++];
                    }
                }
                else
                {
                    optionIndex = -1;
                    throw new InvalidOptionException(currentOption + ": Invalid option", new KeyNotFoundException());
                }

                parsedOption = this.shortOptionMap[id];

                optionIndex++;
            }

            return parsedOption;
        }

        /// <summary>
        /// Represents a command line option
        /// </summary>
        public class Option
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Option"/> class.
            /// </summary>
            /// <param name="shortOption">Single char for an option, prefixed with -</param>
            /// <param name="hasArgument">Does this option need an argument</param>
            /// <param name="longOption">Long string for an option, prefixed with --</param>
            /// <param name="description">An one line description of what the option does</param>
            public Option(char shortOption, bool hasArgument, string longOption, string description)
            {
                this.ShortOption = shortOption;
                this.HasArgument = hasArgument;
                this.LongOption = longOption;
                this.Description = description;
            }

            #region Properties

            /// <summary>
            /// Gets if the option is a short property.
            /// </summary>
            public char ShortOption
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value indicating whether the option has an argument.
            /// </summary>
            public bool HasArgument
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the alternate long text for this option.
            /// </summary>
            public string LongOption
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the description for the option.
            /// </summary>
            public string Description
            {
                get;
                private set;
            }
            #endregion
        }

        /// <summary>
        /// Exception thrown when the options array contains duplicate keys
        /// </summary>
        public class DuplicateOptionException : ArgumentException
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DuplicateOptionException"/> class.
            /// </summary>
            /// <param name="message">Error message</param>
            /// <param name="e">Inner exception</param>
            public DuplicateOptionException(string message, Exception e)
                : base(message, e)
            {
            }
        }

        /// <summary>
        /// Exception generated when user enters an invalid command line argument
        /// </summary>
        public class InvalidOptionException : KeyNotFoundException
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidOptionException"/> class.
            /// </summary>
            /// <param name="message">Error message</param>
            /// <param name="e">Inner exception</param>
            public InvalidOptionException(string message, Exception e)
                : base(message, e)
            {
            }
        }
    }
}