using System;
using System.Collections.Generic;
using System.Linq;

namespace BrowscapNet
{
    public class TreeUserAgentMatcher : IUserAgentMatcher, IIniHandler
    {
        public PatternDictionaryTree tree = new PatternDictionaryTree();

        public BrowserCapabilityInfo lastInfo;

        private static Dictionary<string, Action<BrowserCapabilityInfo, string>> propSetters = new Dictionary<string, Action<BrowserCapabilityInfo, string>>
        {
            ["parent"] = (info, value) => info.Parent = value,
            ["comment"] = (info, value) => info.Comment = value,
            ["browser"] = (info, value) => info.Browser = value,
            ["browser_type"] = (info, value) => info.BrowserType = value,
            ["browser_bits"] = (info, value) => info.BrowserBits = value,
            ["browser_maker"] = (info, value) => info.BrowserMaker = value,
            ["browser_modus"] = (info, value) => info.BrowserModus = value,
            ["version"] = (info, value) => info.Version = value,
            ["majorver"] = (info, value) => info.Majorver = value,
            ["minorver"] = (info, value) => info.Minorver = value,
            ["platform"] = (info, value) => info.Platform = value,
            ["platform_version"] = (info, value) => info.PlatformVersion = value,
            ["platform_description"] = (info, value) => info.PlatformDescription = value,
            ["platform_bits"] = (info, value) => info.PlatformBits = value,
            ["platform_maker"] = (info, value) => info.PlatformMaker = value,
            ["alpha"] = (info, value) => info.Alpha = value == "true",
            ["beta"] = (info, value) => info.Beta = value == "true",
            ["win16"] = (info, value) => info.Win16 = value == "true",
            ["win32"] = (info, value) => info.Win32 = value == "true",
            ["win64"] = (info, value) => info.Win64 = value == "true",
            ["frames"] = (info, value) => info.Frames = value == "true",
            ["iframes"] = (info, value) => info.Iframes = value == "true",
            ["tables"] = (info, value) => info.Tables = value == "true",
            ["cookies"] = (info, value) => info.Cookies = value == "true",
            ["backgroundsounds"] = (info, value) => info.Backgroundsounds = value == "true",
            ["javascript"] = (info, value) => info.Javascript = value == "true",
            ["vbscript"] = (info, value) => info.Vbscript = value == "true",
            ["javaapplets"] = (info, value) => info.Javaapplets = value == "true",
            ["activexcontrols"] = (info, value) => info.Activexcontrols = value == "true",
            ["ismobiledevice"] = (info, value) => info.Ismobiledevice = value == "true",
            ["istablet"] = (info, value) => info.Istablet = value == "true",
            ["issyndicationreader"] = (info, value) => info.Issyndicationreader = value == "true",
            ["crawler"] = (info, value) => info.Crawler = value == "true",
            ["isfake"] = (info, value) => info.Isfake = value == "true",
            ["isanonymized"] = (info, value) => info.Isanonymized = value == "true",
            ["ismodified"] = (info, value) => info.Ismodified = value == "true",
            ["cssversion"] = (info, value) => info.Cssversion = value,
            ["aolversion"] = (info, value) => info.Aolversion = value,
            ["device_name"] = (info, value) => info.DeviceName = value,
            ["device_maker"] = (info, value) => info.DeviceMaker = value,
            ["device_type"] = (info, value) => info.DeviceType = value,
            ["device_pointing_method"] = (info, value) => info.DevicePointingMethod = value,
            ["device_code_name"] = (info, value) => info.DeviceCodeName = value,
            ["device_brand_name"] = (info, value) => info.DeviceBrandName = value,
            ["renderingengine_name"] = (info, value) => info.RenderingengineName = value,
            ["renderingengine_version"] = (info, value) => info.RenderingengineVersion = value,
            ["renderingengine_description"] = (info, value) => info.RenderingengineDescription = value,
            ["renderingengine_maker"] = (info, value) => info.RenderingengineMaker = value
        };

        public TreeUserAgentMatcher()
        {
        }

        public void StartSection(string section, long lineNumber)
        {
            AddLastSection();

            lastInfo = new BrowserCapabilityInfo();
            lastInfo.Pattern = section;
            lastInfo.Rank = lineNumber;
        }

        public void KeyValue(string key, string value, long lineNumber)
        {
            if (propSetters.TryGetValue(key.ToLower(), out var f))
            {
                f(lastInfo, value);
            }

            /*switch (key.ToLower())
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
            }*/
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
