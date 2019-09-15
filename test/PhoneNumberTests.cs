using System;
using Xunit;

namespace PhoneNumberHelper.Test
{
    public class PhoneNumberHelperTests
    {
        [Theory]
        [InlineData("  501111111  ", "SA", true, "+966501111111")]
        [InlineData("501111111", "SA", true, "+966501111111")]
        [InlineData("0501111111", "SA", true, "+966501111111")]
        [InlineData("O5o1111111", "SA", true, "+966501111111")]
        [InlineData("+966501111111", "SA", true, "+966501111111")]
        [InlineData("+9660501111111", "SA", true, "+966501111111")]
        [InlineData("+966O5o1111111", "SA", true, "+966501111111")]
        [InlineData("+966501111111", "AE", true, "+966501111111")]
        [InlineData("+9660501111111", "AE", true, "+966501111111")]
        [InlineData("+966O5o1111111", "AE", true, "+966501111111")]
        [InlineData("+966501111111", null, true, "+966501111111")]
        [InlineData("+9660501111111", null, true, "+966501111111")]
        [InlineData("+966O5o1111111", null, true, "+966501111111")]
        [InlineData("966501111111", "SA", true, "+966501111111")]
        [InlineData("9660501111111", "SA", true, "+966501111111")]
        [InlineData("966O5o1111111", "SA", true, "+966501111111")]
        [InlineData("9669901111111", "SA", false, "9669901111111")]
        [InlineData("501111111", null, false, "501111111")]
        [InlineData("", null, false, "")]
        [InlineData(null, null, false, null)]
        public void TryNormalizeRcTests(string phoneNumber, string regionCode, bool expectedResult, string expectedNormalizedPhoneNumber)
        {
            var result = PhoneNumber.TryNormalizeRc(phoneNumber, regionCode, out var normalizedPhoneNumber);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedNormalizedPhoneNumber, normalizedPhoneNumber);
        }

        [Theory]
        [InlineData("501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("0501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("O5o1111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+966501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+9660501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+966O5o1111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+966501111111", "Asia/Dubai", true, "+966501111111")]
        [InlineData("+9660501111111", "Asia/Dubai", true, "+966501111111")]
        [InlineData("+966O5o1111111", "Asia/Dubai", true, "+966501111111")]
        [InlineData("+966501111111", null, true, "+966501111111")]
        [InlineData("+9660501111111", null, true, "+966501111111")]
        [InlineData("+966O5o1111111", null, true, "+966501111111")]
        [InlineData("966501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("9660501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("966O5o1111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("9669901111111", "Asia/Riyadh", false, "9669901111111")]
        [InlineData("501111111", null, false, "501111111")]
        public void TryNormalizeTzTests(string phoneNumber, string timezone, bool expectedResult, string expectedNormalizedPhoneNumber)
        {
            var result = PhoneNumber.TryNormalizeTz(phoneNumber, timezone, out var normalizedPhoneNumber);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedNormalizedPhoneNumber, normalizedPhoneNumber);
        }

        [Fact]
        public void TryNormalizeTz_throws_for_invalid_timezone()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => PhoneNumber.TryNormalizeTz("501111111", "invalidTZ", out var normalized));
            Assert.Equal("timezone", ex.ParamName);
            Assert.Equal("The provided timezone does not exist.\r\nParameter name: timezone", ex.Message);
        }

        [Theory]
        [InlineData("+966501111111", true)]
        [InlineData("+9660501111111", true)]
        [InlineData("+966901111111", false)]
        [InlineData("966o55555555o", false)]
        [InlineData("invalid", false)]
        public void IsValidNumber(string phoneNumber, bool expectedResult)
        {
            var result = PhoneNumber.IsValidNumber(phoneNumber);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SupportedTimeZones_returns_supported_timezones()
        {
            var supportedTimeZones = new[] { "Europe/Andorra", "Asia/Dubai", "Asia/Kabul", "Europe/Tirane", "Asia/Yerevan", "Antarctica/Casey", "Antarctica/Davis", "Antarctica/DumontDUrville", "Antarctica/Mawson", "Antarctica/Palmer", "Antarctica/Rothera", "Antarctica/Syowa", "Antarctica/Troll", "Antarctica/Vostok", "America/Argentina/Buenos_Aires", "America/Argentina/Catamarca", "America/Argentina/Cordoba", "America/Argentina/Jujuy", "America/Argentina/La_Rioja", "America/Argentina/Mendoza", "America/Argentina/Rio_Gallegos", "America/Argentina/Salta", "America/Argentina/San_Juan", "America/Argentina/San_Luis", "America/Argentina/Tucuman", "America/Argentina/Ushuaia", "Pacific/Pago_Pago", "Europe/Vienna", "Antarctica/Macquarie", "Australia/Adelaide", "Australia/Brisbane", "Australia/Broken_Hill", "Australia/Currie", "Australia/Darwin", "Australia/Eucla", "Australia/Hobart", "Australia/Lindeman", "Australia/Lord_Howe", "Australia/Melbourne", "Australia/Perth", "Australia/Sydney", "Asia/Baku", "America/Barbados", "Asia/Dhaka", "Europe/Brussels", "Europe/Sofia", "Atlantic/Bermuda", "Asia/Brunei", "America/La_Paz", "America/Araguaina", "America/Bahia", "America/Belem", "America/Boa_Vista", "America/Campo_Grande", "America/Cuiaba", "America/Eirunepe", "America/Fortaleza", "America/Maceio", "America/Manaus", "America/Noronha", "America/Porto_Velho", "America/Recife", "America/Rio_Branco", "America/Santarem", "America/Sao_Paulo", "America/Nassau", "Asia/Thimphu", "Europe/Minsk", "America/Belize", "America/Atikokan", "America/Blanc-Sablon", "America/Cambridge_Bay", "America/Creston", "America/Dawson", "America/Dawson_Creek", "America/Edmonton", "America/Fort_Nelson", "America/Glace_Bay", "America/Goose_Bay", "America/Halifax", "America/Inuvik", "America/Iqaluit", "America/Moncton", "America/Nipigon", "America/Pangnirtung", "America/Rainy_River", "America/Rankin_Inlet", "America/Regina", "America/Resolute", "America/St_Johns", "America/Swift_Current", "America/Thunder_Bay", "America/Toronto", "America/Vancouver", "America/Whitehorse", "America/Winnipeg", "America/Yellowknife", "Indian/Cocos", "Europe/Zurich", "Africa/Abidjan", "Pacific/Rarotonga", "America/Punta_Arenas", "America/Santiago", "Pacific/Easter", "Asia/Shanghai", "Asia/Urumqi", "America/Bogota", "America/Costa_Rica", "America/Havana", "Atlantic/Cape_Verde", "America/Curacao", "Indian/Christmas", "Asia/Famagusta", "Asia/Nicosia", "Europe/Prague", "Europe/Berlin", "Europe/Copenhagen", "America/Santo_Domingo", "Africa/Algiers", "America/Guayaquil", "Pacific/Galapagos", "Europe/Tallinn", "Africa/Cairo", "Africa/El_Aaiun", "Africa/Ceuta", "Atlantic/Canary", "Europe/Madrid", "Europe/Helsinki", "Pacific/Fiji", "Atlantic/Stanley", "Pacific/Chuuk", "Pacific/Kosrae", "Pacific/Pohnpei", "Atlantic/Faroe", "Europe/Paris", "Europe/London", "Asia/Tbilisi", "America/Cayenne", "Africa/Accra", "Europe/Gibraltar", "America/Danmarkshavn", "America/Godthab", "America/Scoresbysund", "America/Thule", "Europe/Athens", "Atlantic/South_Georgia", "America/Guatemala", "Pacific/Guam", "Africa/Bissau", "America/Guyana", "Asia/Hong_Kong", "America/Tegucigalpa", "America/Port-au-Prince", "Europe/Budapest", "Asia/Jakarta", "Asia/Jayapura", "Asia/Makassar", "Asia/Pontianak", "Europe/Dublin", "Asia/Jerusalem", "Asia/Kolkata", "Indian/Chagos", "Asia/Baghdad", "Asia/Tehran", "Atlantic/Reykjavik", "Europe/Rome", "America/Jamaica", "Asia/Amman", "Asia/Tokyo", "Africa/Nairobi", "Asia/Bishkek", "Pacific/Enderbury", "Pacific/Kiritimati", "Pacific/Tarawa", "Asia/Pyongyang", "Asia/Seoul", "Asia/Almaty", "Asia/Aqtau", "Asia/Aqtobe", "Asia/Atyrau", "Asia/Oral", "Asia/Qostanay", "Asia/Qyzylorda", "Asia/Beirut", "Asia/Colombo", "Africa/Monrovia", "Europe/Vilnius", "Europe/Luxembourg", "Europe/Riga", "Africa/Tripoli", "Africa/Casablanca", "Europe/Monaco", "Europe/Chisinau", "Pacific/Kwajalein", "Pacific/Majuro", "Asia/Yangon", "Asia/Choibalsan", "Asia/Hovd", "Asia/Ulaanbaatar", "Asia/Macau", "America/Martinique", "Europe/Malta", "Indian/Mauritius", "Indian/Maldives", "America/Bahia_Banderas", "America/Cancun", "America/Chihuahua", "America/Hermosillo", "America/Matamoros", "America/Mazatlan", "America/Merida", "America/Mexico_City", "America/Monterrey", "America/Ojinaga", "America/Tijuana", "Asia/Kuala_Lumpur", "Asia/Kuching", "Africa/Maputo", "Africa/Windhoek", "Pacific/Noumea", "Pacific/Norfolk", "Africa/Lagos", "America/Managua", "Europe/Amsterdam", "Europe/Oslo", "Asia/Kathmandu", "Pacific/Nauru", "Pacific/Niue", "Pacific/Auckland", "Pacific/Chatham", "America/Panama", "America/Lima", "Pacific/Gambier", "Pacific/Marquesas", "Pacific/Tahiti", "Pacific/Bougainville", "Pacific/Port_Moresby", "Asia/Manila", "Asia/Karachi", "Europe/Warsaw", "America/Miquelon", "Pacific/Pitcairn", "America/Puerto_Rico", "Asia/Gaza", "Asia/Hebron", "Atlantic/Azores", "Atlantic/Madeira", "Europe/Lisbon", "Pacific/Palau", "America/Asuncion", "Asia/Qatar", "Indian/Reunion", "Europe/Bucharest", "Europe/Belgrade", "Asia/Anadyr", "Asia/Barnaul", "Asia/Chita", "Asia/Irkutsk", "Asia/Kamchatka", "Asia/Khandyga", "Asia/Krasnoyarsk", "Asia/Magadan", "Asia/Novokuznetsk", "Asia/Novosibirsk", "Asia/Omsk", "Asia/Sakhalin", "Asia/Srednekolymsk", "Asia/Tomsk", "Asia/Ust-Nera", "Asia/Vladivostok", "Asia/Yakutsk", "Asia/Yekaterinburg", "Europe/Astrakhan", "Europe/Kaliningrad", "Europe/Kirov", "Europe/Moscow", "Europe/Samara", "Europe/Saratov", "Europe/Simferopol", "Europe/Ulyanovsk", "Europe/Volgograd", "Asia/Riyadh", "Pacific/Guadalcanal", "Indian/Mahe", "Africa/Khartoum", "Europe/Stockholm", "Asia/Singapore", "America/Paramaribo", "Africa/Juba", "Africa/Sao_Tome", "America/El_Salvador", "Asia/Damascus", "America/Grand_Turk", "Africa/Ndjamena", "Indian/Kerguelen", "Asia/Bangkok", "Asia/Dushanbe", "Pacific/Fakaofo", "Asia/Dili", "Asia/Ashgabat", "Africa/Tunis", "Pacific/Tongatapu", "Europe/Istanbul", "America/Port_of_Spain", "Pacific/Funafuti", "Asia/Taipei", "Europe/Kiev", "Europe/Uzhgorod", "Europe/Zaporozhye", "Pacific/Wake", "America/Adak", "America/Anchorage", "America/Boise", "America/Chicago", "America/Denver", "America/Detroit", "America/Indiana/Indianapolis", "America/Indiana/Knox", "America/Indiana/Marengo", "America/Indiana/Petersburg", "America/Indiana/Tell_City", "America/Indiana/Vevay", "America/Indiana/Vincennes", "America/Indiana/Winamac", "America/Juneau", "America/Kentucky/Louisville", "America/Kentucky/Monticello", "America/Los_Angeles", "America/Menominee", "America/Metlakatla", "America/New_York", "America/Nome", "America/North_Dakota/Beulah", "America/North_Dakota/Center", "America/North_Dakota/New_Salem", "America/Phoenix", "America/Sitka", "America/Yakutat", "Pacific/Honolulu", "America/Montevideo", "Asia/Samarkand", "Asia/Tashkent", "America/Caracas", "Asia/Ho_Chi_Minh", "Pacific/Efate", "Pacific/Wallis", "Pacific/Apia", "Africa/Johannesburg" };

            Assert.Equal(supportedTimeZones, PhoneNumber.SupportedTimeZones);
        }
    }
}
