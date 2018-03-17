/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.IO;

namespace OpenNos.Core
{
    public class Language
    {
        #region Members

        private static Language _instance;

        private readonly ResourceManager _manager;

        private readonly CultureInfo _resourceCulture;

        private readonly System.IO.StreamWriter _streamWriter;

        #endregion

        #region Instantiation

        private Language()
        {
            _streamWriter = new StreamWriter(File.Open("MissingLanguageKeys.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                AutoFlush = true
            };
            _resourceCulture = new CultureInfo(ConfigurationManager.AppSettings["Language"]);
            if (Assembly.GetEntryAssembly() != null)
            {
                _manager = new ResourceManager(Assembly.GetEntryAssembly().GetName().Name + ".Resource.LocalizedResources", Assembly.GetEntryAssembly());
            }
        }

        #endregion

        #region Properties

        public static Language Instance => _instance ?? (_instance = new Language());

        #endregion

        #region Methods

        public string GetMessageFromKey(string message)
        {
            string resourceMessage = _manager != null ? _manager.GetString(message, _resourceCulture) : string.Empty;

            if (string.IsNullOrEmpty(resourceMessage))
            {
                _streamWriter?.WriteLine(message);
                return $"#<{message}>";
            }

            return resourceMessage;
        }

        #endregion
    }
}