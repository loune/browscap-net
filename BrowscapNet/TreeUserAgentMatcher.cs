using System;
using System.Collections.Generic;
using System.Linq;

namespace BrowscapNet
{
    public class TreeUserAgentMatcher : IUserAgentMatcher, IIniHandler
    {
        private PatternDictionaryTree tree = new PatternDictionaryTree();

        private BrowserCapabilityInfo lastInfo;

        public string BrowscapVersion { get; private set; }
        public DateTimeOffset BrowscapReleased { get; private set; }

        public TreeUserAgentMatcher()
        {
        }

        public void StartSection(string section, long lineNumber, out bool cancelParsing)
        {
            cancelParsing = false;
            AddLastSection();

            if (section == "GJK_Browscap_Version")
            {
                lastInfo = null;
            }
            else
            {
                lastInfo = new BrowserCapabilityInfo();
                lastInfo.Pattern = section;
                lastInfo.Rank = lineNumber;
            }
        }

        public void KeyValue(string key, string value, long lineNumber, out bool cancelParsing)
        {
            cancelParsing = false;
            if (lastInfo == null)
            {
                switch (key.ToLower())
                {
                    case "version": BrowscapVersion = value; break;
                    case "released": BrowscapReleased = DateTimeOffset.Parse(value); break;
                    case "format":
                        if (value != "asp")
                        {
                            throw new ArgumentException($"browscap.ini format expected to be asp, got {value} instead.");
                        }
                        break;
                }

                return;
            }

            switch (key.ToLower())
            {
                case "parent": lastInfo.Parent = value; break;
                case "comment": lastInfo.Comment = value; break;
                case "browser": lastInfo.Browser = value; break;
                case "browser_type": lastInfo.BrowserType = value; break;
                case "browser_bits": lastInfo.BrowserBits = value; break;
                case "browser_maker": lastInfo.BrowserMaker = value; break;
                case "browser_modus": lastInfo.BrowserModus = value; break;
                case "version": lastInfo.Version = value; break;
                case "majorver": lastInfo.Majorver = value; break;
                case "minorver": lastInfo.Minorver = value; break;
                case "platform": lastInfo.Platform = value; break;
                case "platform_version": lastInfo.PlatformVersion = value; break;
                case "platform_description": lastInfo.PlatformDescription = value; break;
                case "platform_bits": lastInfo.PlatformBits = value; break;
                case "platform_maker": lastInfo.PlatformMaker = value; break;
                case "alpha": lastInfo.Alpha = value == "true"; break;
                case "beta": lastInfo.Beta = value == "true"; break;
                case "win16": lastInfo.Win16 = value == "true"; break;
                case "win32": lastInfo.Win32 = value == "true"; break;
                case "win64": lastInfo.Win64 = value == "true"; break;
                case "frames": lastInfo.Frames = value == "true"; break;
                case "iframes": lastInfo.Iframes = value == "true"; break;
                case "tables": lastInfo.Tables = value == "true"; break;
                case "cookies": lastInfo.Cookies = value == "true"; break;
                case "backgroundsounds": lastInfo.Backgroundsounds = value == "true"; break;
                case "javascript": lastInfo.Javascript = value == "true"; break;
                case "vbscript": lastInfo.Vbscript = value == "true"; break;
                case "javaapplets": lastInfo.Javaapplets = value == "true"; break;
                case "activexcontrols": lastInfo.Activexcontrols = value == "true"; break;
                case "ismobiledevice": lastInfo.Ismobiledevice = value == "true"; break;
                case "istablet": lastInfo.Istablet = value == "true"; break;
                case "issyndicationreader": lastInfo.Issyndicationreader = value == "true"; break;
                case "crawler": lastInfo.Crawler = value == "true"; break;
                case "isfake": lastInfo.Isfake = value == "true"; break;
                case "isanonymized": lastInfo.Isanonymized = value == "true"; break;
                case "ismodified": lastInfo.Ismodified = value == "true"; break;
                case "cssversion": lastInfo.Cssversion = value; break;
                case "aolversion": lastInfo.Aolversion = value; break;
                case "device_name": lastInfo.DeviceName = value; break;
                case "device_maker": lastInfo.DeviceMaker = value; break;
                case "device_type": lastInfo.DeviceType = value; break;
                case "device_pointing_method": lastInfo.DevicePointingMethod = value; break;
                case "device_code_name": lastInfo.DeviceCodeName = value; break;
                case "device_brand_name": lastInfo.DeviceBrandName = value; break;
                case "renderingengine_name": lastInfo.RenderingengineName = value; break;
                case "renderingengine_version": lastInfo.RenderingengineVersion = value; break;
                case "renderingengine_description": lastInfo.RenderingengineDescription = value; break;
                case "renderingengine_maker": lastInfo.RenderingengineMaker = value; break;
            }
        }

        public string GetMatches(string userAgent)
        {
            var results = tree.FindAll(userAgent);
            return string.Join("\n", results.Select(m => m.pattern + " - " + m.item));
        }

        public BrowserCapabilityInfo FindMatch(string userAgent)
        {
            var results = tree.FindAll(userAgent.ToLower());
            if (results.Count == 0)
            {
                return null;
            }

            var info = ((BrowserCapabilityInfo)results.OrderBy(r => ((BrowserCapabilityInfo)r.item).Rank).FirstOrDefault().item).Clone();

            var parentInfo = info;
            while (parentInfo != null && parentInfo.Parent != null)
            {
                parentInfo = (BrowserCapabilityInfo)tree.FindPatternIdentity(parentInfo.Parent.ToLower()).item;
                if (parentInfo != null)
                {
                    if (info.Comment == null) info.Comment = parentInfo.Comment;
                    if (info.Browser == null) info.Browser = parentInfo.Browser;
                    if (info.BrowserType == null) info.BrowserType = parentInfo.BrowserType;
                    if (info.BrowserBits == null) info.BrowserBits = parentInfo.BrowserBits;
                    if (info.BrowserMaker == null) info.BrowserMaker = parentInfo.BrowserMaker;
                    if (info.BrowserModus == null) info.BrowserModus = parentInfo.BrowserModus;
                    if (info.Version == null) info.Version = parentInfo.Version;
                    if (info.Majorver == null) info.Majorver = parentInfo.Majorver;
                    if (info.Minorver == null) info.Minorver = parentInfo.Minorver;
                    if (info.Platform == null) info.Platform = parentInfo.Platform;
                    if (info.PlatformVersion == null) info.PlatformVersion = parentInfo.PlatformVersion;
                    if (info.PlatformDescription == null) info.PlatformDescription = parentInfo.PlatformDescription;
                    if (info.PlatformBits == null) info.PlatformBits = parentInfo.PlatformBits;
                    if (info.PlatformMaker == null) info.PlatformMaker = parentInfo.PlatformMaker;
                    if (info.Alpha == null) info.Alpha = parentInfo.Alpha;
                    if (info.Beta == null) info.Beta = parentInfo.Beta;
                    if (info.Win16 == null) info.Win16 = parentInfo.Win16;
                    if (info.Win32 == null) info.Win32 = parentInfo.Win32;
                    if (info.Win64 == null) info.Win64 = parentInfo.Win64;
                    if (info.Frames == null) info.Frames = parentInfo.Frames;
                    if (info.Iframes == null) info.Iframes = parentInfo.Iframes;
                    if (info.Tables == null) info.Tables = parentInfo.Tables;
                    if (info.Cookies == null) info.Cookies = parentInfo.Cookies;
                    if (info.Backgroundsounds == null) info.Backgroundsounds = parentInfo.Backgroundsounds;
                    if (info.Javascript == null) info.Javascript = parentInfo.Javascript;
                    if (info.Vbscript == null) info.Vbscript = parentInfo.Vbscript;
                    if (info.Javaapplets == null) info.Javaapplets = parentInfo.Javaapplets;
                    if (info.Activexcontrols == null) info.Activexcontrols = parentInfo.Activexcontrols;
                    if (info.Ismobiledevice == null) info.Ismobiledevice = parentInfo.Ismobiledevice;
                    if (info.Istablet == null) info.Istablet = parentInfo.Istablet;
                    if (info.Issyndicationreader == null) info.Issyndicationreader = parentInfo.Issyndicationreader;
                    if (info.Crawler == null) info.Crawler = parentInfo.Crawler;
                    if (info.Isfake == null) info.Isfake = parentInfo.Isfake;
                    if (info.Isanonymized == null) info.Isanonymized = parentInfo.Isanonymized;
                    if (info.Ismodified == null) info.Ismodified = parentInfo.Ismodified;
                    if (info.Cssversion == null) info.Cssversion = parentInfo.Cssversion;
                    if (info.Aolversion == null) info.Aolversion = parentInfo.Aolversion;
                    if (info.DeviceName == null) info.DeviceName = parentInfo.DeviceName;
                    if (info.DeviceMaker == null) info.DeviceMaker = parentInfo.DeviceMaker;
                    if (info.DeviceType == null) info.DeviceType = parentInfo.DeviceType;
                    if (info.DevicePointingMethod == null) info.DevicePointingMethod = parentInfo.DevicePointingMethod;
                    if (info.DeviceCodeName == null) info.DeviceCodeName = parentInfo.DeviceCodeName;
                    if (info.DeviceBrandName == null) info.DeviceBrandName = parentInfo.DeviceBrandName;
                    if (info.RenderingengineName == null) info.RenderingengineName = parentInfo.RenderingengineName;
                    if (info.RenderingengineVersion == null) info.RenderingengineVersion = parentInfo.RenderingengineVersion;
                    if (info.RenderingengineDescription == null) info.RenderingengineDescription = parentInfo.RenderingengineDescription;
                    if (info.RenderingengineMaker == null) info.RenderingengineMaker = parentInfo.RenderingengineMaker;
                }
            }

            return info;
        }

        public void EndFile(long lineNumber)
        {
            AddLastSection();
        }

        private void AddLastSection()
        {
            if (lastInfo == null)
            {
                return;
            }

            tree.Add(lastInfo.Pattern.ToLower(), item: lastInfo);

            lastInfo = null;
        }
    }
}
