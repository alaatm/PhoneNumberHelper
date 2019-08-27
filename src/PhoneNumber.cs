using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PhoneNumbers;

namespace PhoneNumberHelper
{
    public static class PhoneNumber
    {
        private static PhoneNumberUtil _phoneUtil;
        private static Dictionary<string, List<string>> _regionTimezoneMap;

        /// <summary>
        /// Attempts to normalized the given phone number for the given region code.
        /// </summary>
        /// <param name="phoneNumber">The phone number to normalize.</param>
        /// <param name="regionCode">
        /// Region that we are expecting the number to be from. 
        /// This is only used if the number being parsed is not written in international format.
        /// </param>
        /// <param name="normalized">The normalized string or the original string if normalization failed.</param>
        /// <returns>True if normalization succeeds, otherwise false.</returns>
        public static bool TryNormalizeRc(string phoneNumber, string regionCode, out string normalized)
        {
            EnsurePhoneUtil();
            EnsureRegionTimezoneMap();

            phoneNumber = phoneNumber
                .Replace("o", "0")
                .Replace("O", "0");

            normalized = phoneNumber;

            try
            {
                var parsedNumber = _phoneUtil.Parse(phoneNumber, regionCode);
                if (_phoneUtil.IsValidNumber(parsedNumber))
                {
                    normalized = $"+{parsedNumber.CountryCode}{parsedNumber.NationalNumber}";
                    return true;
                }
                // Check the case where country code is included without the + or 00 prefix
                // as libphone wont recoginize as country code unless we add one of the above
                // prefixes.
                else if (phoneNumber.Length >= 10 && Regex.IsMatch(phoneNumber, @"^\d"))
                {
                    foreach (var countryCode in _phoneUtil.GetSupportedCallingCodes())
                    {
                        var region = _phoneUtil.GetRegionCodeForCountryCode(countryCode);
                        var exampleNumber = _phoneUtil.GetExampleNumberForType(region, PhoneNumberType.MOBILE).NationalNumber;

                        var sCountryCode = countryCode.ToString();
                        var totalLength = sCountryCode.Length + exampleNumber.ToString().Length;
                        if (phoneNumber.StartsWith(sCountryCode) && phoneNumber.Length == totalLength)
                        {
                            return TryNormalizeRc($"+{phoneNumber}", regionCode, out normalized);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Attempts to normalized the given phone number for the given timezone.
        /// </summary>
        /// <param name="phoneNumber">The phone number to normalize.</param>
        /// <param name="timezone">
        /// Timezone that we are expecting the number to be from. 
        /// This is only used if the number being parsed is not written in international format.
        /// </param>
        /// <param name="normalized">The normalized string or the original string if normalization failed.</param>
        /// <returns>True if normalization succeeds, otherwise false.</returns>
        public static bool TryNormalizeTz(string phoneNumber, string timezone, out string normalized)
        {
            EnsureRegionTimezoneMap();
            return TryNormalizeRc(phoneNumber, GetRegion(timezone), out normalized);
        }

        /// <summary>
        /// Checks whether the provided international-format phone number is valid or not.
        /// </summary>
        /// <param name="phoneNumber">The phone number in international format.</param>
        /// <returns>True if valid phone number, otherwise false.</returns>
        public static bool IsValidNumber(string phoneNumber)
        {
            EnsurePhoneUtil();
            try
            {
                return _phoneUtil.IsValidNumber(_phoneUtil.Parse(phoneNumber, null));
            }
            catch
            {
                return false;
            }
        }

        private static string GetRegion(string timezone)
        {
            if (String.IsNullOrWhiteSpace(timezone))
            {
                return null;
            }

            var key = _regionTimezoneMap.FirstOrDefault(v => v.Value.IndexOf(timezone) >= 0).Key;
            if (key == null)
            {
                throw new ArgumentOutOfRangeException(nameof(timezone), "The provided timezone does not exist.");
            }

            return key;
        }

        private static void EnsurePhoneUtil()
        {
            if (_phoneUtil == null)
            {
                _phoneUtil = PhoneNumberUtil.GetInstance();
            }
        }

        private static void EnsureRegionTimezoneMap()
        {
            if (_regionTimezoneMap == null)
            {
                _regionTimezoneMap = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(_regionTimezoneMapJaon);
            }
        }

        private static readonly string _regionTimezoneMapJaon = "{\"AD\":[\"Europe/Andorra\"],\"AE\":[\"Asia/Dubai\"],\"AF\":[\"Asia/Kabul\"],\"AL\":[\"Europe/Tirane\"],\"AM\":[\"Asia/Yerevan\"],\"AQ\":[\"Antarctica/Casey\",\"Antarctica/Davis\",\"Antarctica/DumontDUrville\",\"Antarctica/Mawson\",\"Antarctica/Palmer\",\"Antarctica/Rothera\",\"Antarctica/Syowa\",\"Antarctica/Troll\",\"Antarctica/Vostok\"],\"AR\":[\"America/Argentina/Buenos_Aires\",\"America/Argentina/Catamarca\",\"America/Argentina/Cordoba\",\"America/Argentina/Jujuy\",\"America/Argentina/La_Rioja\",\"America/Argentina/Mendoza\",\"America/Argentina/Rio_Gallegos\",\"America/Argentina/Salta\",\"America/Argentina/San_Juan\",\"America/Argentina/San_Luis\",\"America/Argentina/Tucuman\",\"America/Argentina/Ushuaia\"],\"AS\":[\"Pacific/Pago_Pago\"],\"AT\":[\"Europe/Vienna\"],\"AU\":[\"Antarctica/Macquarie\",\"Australia/Adelaide\",\"Australia/Brisbane\",\"Australia/Broken_Hill\",\"Australia/Currie\",\"Australia/Darwin\",\"Australia/Eucla\",\"Australia/Hobart\",\"Australia/Lindeman\",\"Australia/Lord_Howe\",\"Australia/Melbourne\",\"Australia/Perth\",\"Australia/Sydney\"],\"AZ\":[\"Asia/Baku\"],\"BB\":[\"America/Barbados\"],\"BD\":[\"Asia/Dhaka\"],\"BE\":[\"Europe/Brussels\"],\"BG\":[\"Europe/Sofia\"],\"BM\":[\"Atlantic/Bermuda\"],\"BN\":[\"Asia/Brunei\"],\"BO\":[\"America/La_Paz\"],\"BR\":[\"America/Araguaina\",\"America/Bahia\",\"America/Belem\",\"America/Boa_Vista\",\"America/Campo_Grande\",\"America/Cuiaba\",\"America/Eirunepe\",\"America/Fortaleza\",\"America/Maceio\",\"America/Manaus\",\"America/Noronha\",\"America/Porto_Velho\",\"America/Recife\",\"America/Rio_Branco\",\"America/Santarem\",\"America/Sao_Paulo\"],\"BS\":[\"America/Nassau\"],\"BT\":[\"Asia/Thimphu\"],\"BY\":[\"Europe/Minsk\"],\"BZ\":[\"America/Belize\"],\"CA\":[\"America/Atikokan\",\"America/Blanc-Sablon\",\"America/Cambridge_Bay\",\"America/Creston\",\"America/Dawson\",\"America/Dawson_Creek\",\"America/Edmonton\",\"America/Fort_Nelson\",\"America/Glace_Bay\",\"America/Goose_Bay\",\"America/Halifax\",\"America/Inuvik\",\"America/Iqaluit\",\"America/Moncton\",\"America/Nipigon\",\"America/Pangnirtung\",\"America/Rainy_River\",\"America/Rankin_Inlet\",\"America/Regina\",\"America/Resolute\",\"America/St_Johns\",\"America/Swift_Current\",\"America/Thunder_Bay\",\"America/Toronto\",\"America/Vancouver\",\"America/Whitehorse\",\"America/Winnipeg\",\"America/Yellowknife\"],\"CC\":[\"Indian/Cocos\"],\"CH\":[\"Europe/Zurich\"],\"CI\":[\"Africa/Abidjan\"],\"CK\":[\"Pacific/Rarotonga\"],\"CL\":[\"America/Punta_Arenas\",\"America/Santiago\",\"Pacific/Easter\"],\"CN\":[\"Asia/Shanghai\",\"Asia/Urumqi\"],\"CO\":[\"America/Bogota\"],\"CR\":[\"America/Costa_Rica\"],\"CU\":[\"America/Havana\"],\"CV\":[\"Atlantic/Cape_Verde\"],\"CW\":[\"America/Curacao\"],\"CX\":[\"Indian/Christmas\"],\"CY\":[\"Asia/Famagusta\",\"Asia/Nicosia\"],\"CZ\":[\"Europe/Prague\"],\"DE\":[\"Europe/Berlin\"],\"DK\":[\"Europe/Copenhagen\"],\"DO\":[\"America/Santo_Domingo\"],\"DZ\":[\"Africa/Algiers\"],\"EC\":[\"America/Guayaquil\",\"Pacific/Galapagos\"],\"EE\":[\"Europe/Tallinn\"],\"EG\":[\"Africa/Cairo\"],\"EH\":[\"Africa/El_Aaiun\"],\"ES\":[\"Africa/Ceuta\",\"Atlantic/Canary\",\"Europe/Madrid\"],\"FI\":[\"Europe/Helsinki\"],\"FJ\":[\"Pacific/Fiji\"],\"FK\":[\"Atlantic/Stanley\"],\"FM\":[\"Pacific/Chuuk\",\"Pacific/Kosrae\",\"Pacific/Pohnpei\"],\"FO\":[\"Atlantic/Faroe\"],\"FR\":[\"Europe/Paris\"],\"GB\":[\"Europe/London\"],\"GE\":[\"Asia/Tbilisi\"],\"GF\":[\"America/Cayenne\"],\"GH\":[\"Africa/Accra\"],\"GI\":[\"Europe/Gibraltar\"],\"GL\":[\"America/Danmarkshavn\",\"America/Godthab\",\"America/Scoresbysund\",\"America/Thule\"],\"GR\":[\"Europe/Athens\"],\"GS\":[\"Atlantic/South_Georgia\"],\"GT\":[\"America/Guatemala\"],\"GU\":[\"Pacific/Guam\"],\"GW\":[\"Africa/Bissau\"],\"GY\":[\"America/Guyana\"],\"HK\":[\"Asia/Hong_Kong\"],\"HN\":[\"America/Tegucigalpa\"],\"HT\":[\"America/Port-au-Prince\"],\"HU\":[\"Europe/Budapest\"],\"ID\":[\"Asia/Jakarta\",\"Asia/Jayapura\",\"Asia/Makassar\",\"Asia/Pontianak\"],\"IE\":[\"Europe/Dublin\"],\"IL\":[\"Asia/Jerusalem\"],\"IN\":[\"Asia/Kolkata\"],\"IO\":[\"Indian/Chagos\"],\"IQ\":[\"Asia/Baghdad\"],\"IR\":[\"Asia/Tehran\"],\"IS\":[\"Atlantic/Reykjavik\"],\"IT\":[\"Europe/Rome\"],\"JM\":[\"America/Jamaica\"],\"JO\":[\"Asia/Amman\"],\"JP\":[\"Asia/Tokyo\"],\"KE\":[\"Africa/Nairobi\"],\"KG\":[\"Asia/Bishkek\"],\"KI\":[\"Pacific/Enderbury\",\"Pacific/Kiritimati\",\"Pacific/Tarawa\"],\"KP\":[\"Asia/Pyongyang\"],\"KR\":[\"Asia/Seoul\"],\"KZ\":[\"Asia/Almaty\",\"Asia/Aqtau\",\"Asia/Aqtobe\",\"Asia/Atyrau\",\"Asia/Oral\",\"Asia/Qostanay\",\"Asia/Qyzylorda\"],\"LB\":[\"Asia/Beirut\"],\"LK\":[\"Asia/Colombo\"],\"LR\":[\"Africa/Monrovia\"],\"LT\":[\"Europe/Vilnius\"],\"LU\":[\"Europe/Luxembourg\"],\"LV\":[\"Europe/Riga\"],\"LY\":[\"Africa/Tripoli\"],\"MA\":[\"Africa/Casablanca\"],\"MC\":[\"Europe/Monaco\"],\"MD\":[\"Europe/Chisinau\"],\"MH\":[\"Pacific/Kwajalein\",\"Pacific/Majuro\"],\"MM\":[\"Asia/Yangon\"],\"MN\":[\"Asia/Choibalsan\",\"Asia/Hovd\",\"Asia/Ulaanbaatar\"],\"MO\":[\"Asia/Macau\"],\"MQ\":[\"America/Martinique\"],\"MT\":[\"Europe/Malta\"],\"MU\":[\"Indian/Mauritius\"],\"MV\":[\"Indian/Maldives\"],\"MX\":[\"America/Bahia_Banderas\",\"America/Cancun\",\"America/Chihuahua\",\"America/Hermosillo\",\"America/Matamoros\",\"America/Mazatlan\",\"America/Merida\",\"America/Mexico_City\",\"America/Monterrey\",\"America/Ojinaga\",\"America/Tijuana\"],\"MY\":[\"Asia/Kuala_Lumpur\",\"Asia/Kuching\"],\"MZ\":[\"Africa/Maputo\"],\"NA\":[\"Africa/Windhoek\"],\"NC\":[\"Pacific/Noumea\"],\"NF\":[\"Pacific/Norfolk\"],\"NG\":[\"Africa/Lagos\"],\"NI\":[\"America/Managua\"],\"NL\":[\"Europe/Amsterdam\"],\"NO\":[\"Europe/Oslo\"],\"NP\":[\"Asia/Kathmandu\"],\"NR\":[\"Pacific/Nauru\"],\"NU\":[\"Pacific/Niue\"],\"NZ\":[\"Pacific/Auckland\",\"Pacific/Chatham\"],\"PA\":[\"America/Panama\"],\"PE\":[\"America/Lima\"],\"PF\":[\"Pacific/Gambier\",\"Pacific/Marquesas\",\"Pacific/Tahiti\"],\"PG\":[\"Pacific/Bougainville\",\"Pacific/Port_Moresby\"],\"PH\":[\"Asia/Manila\"],\"PK\":[\"Asia/Karachi\"],\"PL\":[\"Europe/Warsaw\"],\"PM\":[\"America/Miquelon\"],\"PN\":[\"Pacific/Pitcairn\"],\"PR\":[\"America/Puerto_Rico\"],\"PS\":[\"Asia/Gaza\",\"Asia/Hebron\"],\"PT\":[\"Atlantic/Azores\",\"Atlantic/Madeira\",\"Europe/Lisbon\"],\"PW\":[\"Pacific/Palau\"],\"PY\":[\"America/Asuncion\"],\"QA\":[\"Asia/Qatar\"],\"RE\":[\"Indian/Reunion\"],\"RO\":[\"Europe/Bucharest\"],\"RS\":[\"Europe/Belgrade\"],\"RU\":[\"Asia/Anadyr\",\"Asia/Barnaul\",\"Asia/Chita\",\"Asia/Irkutsk\",\"Asia/Kamchatka\",\"Asia/Khandyga\",\"Asia/Krasnoyarsk\",\"Asia/Magadan\",\"Asia/Novokuznetsk\",\"Asia/Novosibirsk\",\"Asia/Omsk\",\"Asia/Sakhalin\",\"Asia/Srednekolymsk\",\"Asia/Tomsk\",\"Asia/Ust-Nera\",\"Asia/Vladivostok\",\"Asia/Yakutsk\",\"Asia/Yekaterinburg\",\"Europe/Astrakhan\",\"Europe/Kaliningrad\",\"Europe/Kirov\",\"Europe/Moscow\",\"Europe/Samara\",\"Europe/Saratov\",\"Europe/Simferopol\",\"Europe/Ulyanovsk\",\"Europe/Volgograd\"],\"SA\":[\"Asia/Riyadh\"],\"SB\":[\"Pacific/Guadalcanal\"],\"SC\":[\"Indian/Mahe\"],\"SD\":[\"Africa/Khartoum\"],\"SE\":[\"Europe/Stockholm\"],\"SG\":[\"Asia/Singapore\"],\"SR\":[\"America/Paramaribo\"],\"SS\":[\"Africa/Juba\"],\"ST\":[\"Africa/Sao_Tome\"],\"SV\":[\"America/El_Salvador\"],\"SY\":[\"Asia/Damascus\"],\"TC\":[\"America/Grand_Turk\"],\"TD\":[\"Africa/Ndjamena\"],\"TF\":[\"Indian/Kerguelen\"],\"TH\":[\"Asia/Bangkok\"],\"TJ\":[\"Asia/Dushanbe\"],\"TK\":[\"Pacific/Fakaofo\"],\"TL\":[\"Asia/Dili\"],\"TM\":[\"Asia/Ashgabat\"],\"TN\":[\"Africa/Tunis\"],\"TO\":[\"Pacific/Tongatapu\"],\"TR\":[\"Europe/Istanbul\"],\"TT\":[\"America/Port_of_Spain\"],\"TV\":[\"Pacific/Funafuti\"],\"TW\":[\"Asia/Taipei\"],\"UA\":[\"Europe/Kiev\",\"Europe/Uzhgorod\",\"Europe/Zaporozhye\"],\"UM\":[\"Pacific/Wake\"],\"US\":[\"America/Adak\",\"America/Anchorage\",\"America/Boise\",\"America/Chicago\",\"America/Denver\",\"America/Detroit\",\"America/Indiana/Indianapolis\",\"America/Indiana/Knox\",\"America/Indiana/Marengo\",\"America/Indiana/Petersburg\",\"America/Indiana/Tell_City\",\"America/Indiana/Vevay\",\"America/Indiana/Vincennes\",\"America/Indiana/Winamac\",\"America/Juneau\",\"America/Kentucky/Louisville\",\"America/Kentucky/Monticello\",\"America/Los_Angeles\",\"America/Menominee\",\"America/Metlakatla\",\"America/New_York\",\"America/Nome\",\"America/North_Dakota/Beulah\",\"America/North_Dakota/Center\",\"America/North_Dakota/New_Salem\",\"America/Phoenix\",\"America/Sitka\",\"America/Yakutat\",\"Pacific/Honolulu\"],\"UY\":[\"America/Montevideo\"],\"UZ\":[\"Asia/Samarkand\",\"Asia/Tashkent\"],\"VE\":[\"America/Caracas\"],\"VN\":[\"Asia/Ho_Chi_Minh\"],\"VU\":[\"Pacific/Efate\"],\"WF\":[\"Pacific/Wallis\"],\"WS\":[\"Pacific/Apia\"],\"ZA\":[\"Africa/Johannesburg\"]}";
    }
}
