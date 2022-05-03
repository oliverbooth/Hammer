//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hammer.Resources {
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
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Hammer.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to Cannot delete non-guild message..
        /// </summary>
        internal static string CannotDeleteNonGuildMessage {
            get {
                return ResourceManager.GetString("CannotDeleteNonGuildMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expiration time cannot be in the past..
        /// </summary>
        internal static string ExpirationTimeInPast {
            get {
                return ResourceManager.GetString("ExpirationTimeInPast", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified guild is invalid..
        /// </summary>
        internal static string InvalidGuild {
            get {
                return ResourceManager.GetString("InvalidGuild", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message does not appear in the same guild as the staff member..
        /// </summary>
        internal static string MessageStaffMemberGuildMismatch {
            get {
                return ResourceManager.GetString("MessageStaffMemberGuildMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Issuer cannot indefinitely mute user because they are a moderator, and the maximum moderator mute duration is set..
        /// </summary>
        internal static string ModeratorCannotPermanentlyMute {
            get {
                return ResourceManager.GetString("ModeratorCannotPermanentlyMute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An embed cannot be built for a Gag infraction..
        /// </summary>
        internal static string NoEmbedForGag {
            get {
                return ResourceManager.GetString("NoEmbedForGag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The duration of an infraction cannot be negative..
        /// </summary>
        internal static string NoNegativeDuration {
            get {
                return ResourceManager.GetString("NoNegativeDuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No note with the ID {id} could be found..
        /// </summary>
        internal static string NoSuchNote {
            get {
                return ResourceManager.GetString("NoSuchNote", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {user.UsernameWithDiscriminator} is not a staff member of {guild.Name}..
        /// </summary>
        internal static string NotAStaffMember {
            get {
                return ResourceManager.GetString("NotAStaffMember", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {higher.UsernameWithDiscriminator} is a higher permission level than {lower.UsernameWithDiscriminator}..
        /// </summary>
        internal static string StaffIsHigherLevel {
            get {
                return ResourceManager.GetString("StaffIsHigherLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified staff member do not belong to the specified guild..
        /// </summary>
        internal static string StaffMemberGuildMismatch {
            get {
                return ResourceManager.GetString("StaffMemberGuildMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified staff member and recipient do not belong to the same guild..
        /// </summary>
        internal static string StaffMemberRecipientGuildMismatch {
            get {
                return ResourceManager.GetString("StaffMemberRecipientGuildMismatch", resourceCulture);
            }
        }
    }
}
