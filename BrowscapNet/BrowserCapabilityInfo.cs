using System;

namespace BrowscapNet
{
    /// <summary>
    /// Browser capabilities.
    /// </summary>
    public class BrowserCapabilityInfo
    {
        public string Pattern { get; internal set; }
        public long Rank { get; internal set; }

        public string Parent { get; internal set; }
        public string Comment { get; internal set; }
        public string Browser { get; internal set; }
        public string BrowserType { get; internal set; }
        public string BrowserBits { get; internal set; }
        public string BrowserMaker { get; internal set; }
        public string BrowserModus { get; internal set; }
        public string Version { get; internal set; }
        public string Majorver { get; internal set; }
        public string Minorver { get; internal set; }
        public string Platform { get; internal set; }
        public string PlatformVersion { get; internal set; }
        public string PlatformDescription { get; internal set; }
        public string PlatformBits { get; internal set; }
        public string PlatformMaker { get; internal set; }
        public bool? Alpha { get; internal set; }
        public bool? Beta { get; internal set; }
        public bool? Win16 { get; internal set; }
        public bool? Win32 { get; internal set; }
        public bool? Win64 { get; internal set; }
        public bool? Frames { get; internal set; }
        public bool? Iframes { get; internal set; }
        public bool? Tables { get; internal set; }
        public bool? Cookies { get; internal set; }
        public bool? Backgroundsounds { get; internal set; }
        public bool? Javascript { get; internal set; }
        public bool? Vbscript { get; internal set; }
        public bool? Javaapplets { get; internal set; }
        public bool? Activexcontrols { get; internal set; }
        public bool? Ismobiledevice { get; internal set; }
        public bool? Istablet { get; internal set; }
        public bool? Issyndicationreader { get; internal set; }
        public bool? Crawler { get; internal set; }
        public bool? Isfake { get; internal set; }
        public bool? Isanonymized { get; internal set; }
        public bool? Ismodified { get; internal set; }
        public string Cssversion { get; internal set; }
        public string Aolversion { get; internal set; }
        public string DeviceName { get; internal set; }
        public string DeviceMaker { get; internal set; }
        public string DeviceType { get; internal set; }
        public string DevicePointingMethod { get; internal set; }
        public string DeviceCodeName { get; internal set; }
        public string DeviceBrandName { get; internal set; }
        public string RenderingengineName { get; internal set; }
        public string RenderingengineVersion { get; internal set; }
        public string RenderingengineDescription { get; internal set; }
        public string RenderingengineMaker { get; internal set; }

        /// <summary>
        /// Clone this browser capabilities info.
        /// </summary>
        /// <returns>The clone.</returns>
        public BrowserCapabilityInfo Clone()
        {
            return (BrowserCapabilityInfo)this.MemberwiseClone();
        }

        public BrowserCapabilityInfo()
        {
        }
    }
}
