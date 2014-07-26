﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.35312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OShell.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OShell.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to definekey keymap key command
        ///    Add a new key binding in keymap for key to execute command. Default keymaps are top normally only containing C-t, which reads a key from root, containing all the normal commands.
        ///
        ///    Note that you have to describe &apos;:&apos; by &apos;colon&apos;, &apos;!&apos; by &apos;exclam&apos; and so on. If you cannot guess a name of a key, try either C-t key and look at the error message, or try :describekey root and pressing the key..
        /// </summary>
        internal static string Command_Definekey_Help {
            get {
                return ResourceManager.GetString("Command_Definekey_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to delkmap keymap.
        ///    Deletes the keymap named keymap, that was generated with newkmap. The keymaps top (or whatever was specified by set topkmap) and root cannot be deleted..
        /// </summary>
        internal static string Command_DelKMap_Help {
            get {
                return ResourceManager.GetString("Command_DelKMap_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to newkmap keymap
        ///    Generate a new keymap names keymap. This keymap can be used to add new key-command mapping to it with definekey and can be called with readkey..
        /// </summary>
        internal static string Command_NewKMap_Help {
            get {
                return ResourceManager.GetString("Command_NewKMap_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to readkey keymap
        ///    Grab the next key pressed, and execute the command associated to this key in keymap. To show it is waiting for a key, ratpoison will change the rat cursor to a square if waitcursor is set.
        ///    This command is perhaps best described with its usage in the default configuration: By pressing C-t, which is the only key in the keymap top, the command readkey root is executed. The next key then executes the command in keymap root belonging to this command..
        /// </summary>
        internal static string Command_Readkey_Help {
            get {
                return ResourceManager.GetString("Command_Readkey_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to set [variable [value]]
        ///    If no argument is given, output all ratpoison variables and their values.
        ///    If one argument is given, output the value of ratpoison variable variable.
        ///    Otherwise set ratpoison variable variable to value. What values are valid depends on the variable. See the section VARIABLES later in this document for details..
        /// </summary>
        internal static string Command_Set_Help {
            get {
                return ResourceManager.GetString("Command_Set_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to source file
        ///    Read file and execute each line as ratpoison command..
        /// </summary>
        internal static string Command_Source_Help {
            get {
                return ResourceManager.GetString("Command_Source_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to undefinekey keymap key
        ///    Remove the binding for key from keymap..
        /// </summary>
        internal static string Command_Undefinekey_Help {
            get {
                return ResourceManager.GetString("Command_Undefinekey_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error creating log file. Exception: {0}.
        /// </summary>
        internal static string Logger_Error_CreatingFile {
            get {
                return ResourceManager.GetString("Logger_Error_CreatingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Send a command to running instance of OShell..
        /// </summary>
        internal static string Option_Command_Description {
            get {
                return ResourceManager.GetString("Option_Command_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Location of alternate rc file..
        /// </summary>
        internal static string Option_File_Description {
            get {
                return ResourceManager.GetString("Option_File_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Displays this help..
        /// </summary>
        internal static string Option_Help_Description {
            get {
                return ResourceManager.GetString("Option_Help_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use -h or --help for instructions..
        /// </summary>
        internal static string Option_Help_Error {
            get {
                return ResourceManager.GetString("Option_Help_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resets all windows to regular style, in case things get ad..
        /// </summary>
        internal static string Option_Rescue_Description {
            get {
                return ResourceManager.GetString("Option_Rescue_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Display version information..
        /// </summary>
        internal static string Option_Version_Description {
            get {
                return ResourceManager.GetString("Option_Version_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to topkmap kmap
        ///    Make kmap the top keymap ratpoison graps directly. The default value is top..
        /// </summary>
        internal static string Variable_TopKMap_Help {
            get {
                return ResourceManager.GetString("Variable_TopKMap_Help", resourceCulture);
            }
        }
    }
}
