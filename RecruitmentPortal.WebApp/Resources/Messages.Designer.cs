﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RecruitmentPortal.WebApp.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RecruitmentPortal.WebApp.Resources.Messages", typeof(Messages).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email is Required.
        /// </summary>
        public static string Emailrequired {
            get {
                return ResourceManager.GetString("Emailrequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Activate Item for Editing.
        /// </summary>
        public static string EnableEntity {
            get {
                return ResourceManager.GetString("EnableEntity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End date must be greater than or equal to start date.
        /// </summary>
        public static string EndDateGreaterThanStartDate {
            get {
                return ResourceManager.GetString("EndDateGreaterThanStartDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please Provide valid Email Address.
        /// </summary>
        public static string InvalidEmail {
            get {
                return ResourceManager.GetString("InvalidEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password is Required.
        /// </summary>
        public static string Passwordrequired {
            get {
                return ResourceManager.GetString("Passwordrequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --- Please Select ---.
        /// </summary>
        public static string PleaseSelect {
            get {
                return ResourceManager.GetString("PleaseSelect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something Went wrong. Please try again later..
        /// </summary>
        public static string SomethingWrong {
            get {
                return ResourceManager.GetString("SomethingWrong", resourceCulture);
            }
        }
    }
}
