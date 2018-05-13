using System;
namespace net.loune.BrowscapNet
{
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
        public string Alpha { get; internal set; }
        public string Beta { get; internal set; }
        public string Win16 { get; internal set; }
        public string Win32 { get; internal set; }
        public string Win64 { get; internal set; }
        public string Frames { get; internal set; }
        public string Iframes { get; internal set; }
        public string Tables { get; internal set; }
        public string Cookies { get; internal set; }
        public string Backgroundsounds { get; internal set; }
        public string Javascript { get; internal set; }
        public string Vbscript { get; internal set; }
        public string Javaapplets { get; internal set; }
        public string Activexcontrols { get; internal set; }
        public string Ismobiledevice { get; internal set; }
        public string Istablet { get; internal set; }
        public string Issyndicationreader { get; internal set; }
        public string Crawler { get; internal set; }
        public string Isfake { get; internal set; }
        public string Isanonymized { get; internal set; }
        public string Ismodified { get; internal set; }
        public string Cssversion { get; internal set; }
        public string Aolversion { get; internal set; }
        public string Device_name { get; internal set; }
        public string Device_maker { get; internal set; }
        public string Device_type { get; internal set; }
        public string DevicePointingMethod { get; internal set; }
        public string DeviceCodeName { get; internal set; }
        public string DeviceBrandName { get; internal set; }
        public string RenderingengineName { get; internal set; }
        public string RenderingengineVersion { get; internal set; }
        public string RenderingengineDescription { get; internal set; }
        public string RenderingengineMaker { get; internal set; }


        public BrowserCapabilityInfo()
        {
        }
    }
}
