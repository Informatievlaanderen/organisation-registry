/*
 * IBAN4Net
 * Copyright 2015 Vaclav Beca [sinkien]
 *
 * Based on Artur Mkrtchyan's project IBAN4j (https://github.com/arturmkrtchyan/iban4j).
 *
 *
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace OrganisationRegistry.IbanBic;

using System.Collections.Generic;
using System.Linq;

public class CountryCode
{

    /// <summary>
    /// A list of all supported country codes
    /// ISO 3166-1
    ///
    /// </summary>
    private static readonly SortedDictionary<string, CountryCodeEntry> Alpha3Map = new();

    static CountryCode()
    {
        LoadMap();
    }

    /// <summary>
    /// Gets CountryCode object from map
    /// </summary>
    /// <param name="maybeCode">2 or 3 letters code for country</param>
    /// <returns>Found CountryCodeItem object or null if it is not found</returns>
    public static CountryCodeEntry? GetCountryCode(string? maybeCode)
        => maybeCode is { } code
            ? code.Length switch
            {
                2 => GetByAlpha2(code.ToUpper()),
                3 => GetByAlpha3(code.ToUpper()),
                _ => null,
            }
            : null;

    private static CountryCodeEntry? GetByAlpha2(string code)
        => Alpha3Map.ContainsKey(code) ? Alpha3Map[code] : null;

    private static CountryCodeEntry? GetByAlpha3(string code)
        => Alpha3Map.Values.SingleOrDefault(x => x.Alpha3.Equals(code));

    private static void LoadMap()
    {
        Alpha3Map.Add("AF", new CountryCodeEntry() { Alpha2 = "AF", Alpha3 = "AFG", CountryName = "Afghanistan" });
        Alpha3Map.Add("AX", new CountryCodeEntry() { Alpha2 = "AX", Alpha3 = "ALA", CountryName = "Åland Islands" });
        Alpha3Map.Add("AL", new CountryCodeEntry() { Alpha2 = "AL", Alpha3 = "ALB", CountryName = "Albania" });
        Alpha3Map.Add("DZ", new CountryCodeEntry() { Alpha2 = "DZ", Alpha3 = "DZA", CountryName = "Algeria" });
        Alpha3Map.Add("AS", new CountryCodeEntry() { Alpha2 = "AS", Alpha3 = "ASM", CountryName = "American Samoa" });
        Alpha3Map.Add("VI", new CountryCodeEntry() { Alpha2 = "VI", Alpha3 = "VIR", CountryName = "Virgin Islands (U.S.)" });
        Alpha3Map.Add("AD", new CountryCodeEntry() { Alpha2 = "AD", Alpha3 = "AND", CountryName = "Andorra" });
        Alpha3Map.Add("AO", new CountryCodeEntry() { Alpha2 = "AO", Alpha3 = "AGO", CountryName = "Angola" });
        Alpha3Map.Add("AI", new CountryCodeEntry() { Alpha2 = "AI", Alpha3 = "AIA", CountryName = "Anguilla" });
        Alpha3Map.Add("AQ", new CountryCodeEntry() { Alpha2 = "AQ", Alpha3 = "ATA", CountryName = "Antarctica" });
        Alpha3Map.Add("AG", new CountryCodeEntry() { Alpha2 = "AG", Alpha3 = "ATG", CountryName = "Antigua and Barbuda" });
        Alpha3Map.Add("AR", new CountryCodeEntry() { Alpha2 = "AR", Alpha3 = "ARG", CountryName = "Argentina" });
        Alpha3Map.Add("AM", new CountryCodeEntry() { Alpha2 = "AM", Alpha3 = "ARM", CountryName = "Armenia" });
        Alpha3Map.Add("AN", new CountryCodeEntry() { Alpha2 = "AN", Alpha3 = "ANT", CountryName = "Netherlands Antilles" });
        Alpha3Map.Add("AW", new CountryCodeEntry() { Alpha2 = "AW", Alpha3 = "ABW", CountryName = "Aruba" });
        Alpha3Map.Add("AU", new CountryCodeEntry() { Alpha2 = "AU", Alpha3 = "AUS", CountryName = "Australia" });
        Alpha3Map.Add("AZ", new CountryCodeEntry() { Alpha2 = "AZ", Alpha3 = "AZE", CountryName = "Azerbaijan" });
        Alpha3Map.Add("BS", new CountryCodeEntry() { Alpha2 = "BS", Alpha3 = "BHS", CountryName = "Bahamas" });
        Alpha3Map.Add("BH", new CountryCodeEntry() { Alpha2 = "BH", Alpha3 = "BHR", CountryName = "Bahrain" });
        Alpha3Map.Add("BD", new CountryCodeEntry() { Alpha2 = "BD", Alpha3 = "BGD", CountryName = "Bangladesh" });
        Alpha3Map.Add("BB", new CountryCodeEntry() { Alpha2 = "BB", Alpha3 = "BRB", CountryName = "Barbados" });
        Alpha3Map.Add("BE", new CountryCodeEntry() { Alpha2 = "BE", Alpha3 = "BEL", CountryName = "Belgium" });
        Alpha3Map.Add("BZ", new CountryCodeEntry() { Alpha2 = "BZ", Alpha3 = "BLZ", CountryName = "Belize" });
        Alpha3Map.Add("BY", new CountryCodeEntry() { Alpha2 = "BY", Alpha3 = "BLR", CountryName = "Belarus" });
        Alpha3Map.Add("BJ", new CountryCodeEntry() { Alpha2 = "BJ", Alpha3 = "BEN", CountryName = "Benin" });
        Alpha3Map.Add("BM", new CountryCodeEntry() { Alpha2 = "BM", Alpha3 = "BMU", CountryName = "Bermuda" });
        Alpha3Map.Add("BT", new CountryCodeEntry() { Alpha2 = "BT", Alpha3 = "BTN", CountryName = "Bhutan" });
        Alpha3Map.Add("BO", new CountryCodeEntry() { Alpha2 = "BO", Alpha3 = "BOL", CountryName = "Bolivia, Plurinational State of" });
        Alpha3Map.Add("BQ", new CountryCodeEntry() { Alpha2 = "BQ", Alpha3 = "BES", CountryName = "Bonaire, Sint Eustatius and Saba" });
        Alpha3Map.Add("BA", new CountryCodeEntry() { Alpha2 = "BA", Alpha3 = "BIH", CountryName = "Bosnia and Herzegovina" });
        Alpha3Map.Add("BW", new CountryCodeEntry() { Alpha2 = "BW", Alpha3 = "BWA", CountryName = "Botswana" });
        Alpha3Map.Add("BV", new CountryCodeEntry() { Alpha2 = "BV", Alpha3 = "BVT", CountryName = "Bouvet Island" });
        Alpha3Map.Add("BR", new CountryCodeEntry() { Alpha2 = "BR", Alpha3 = "BRA", CountryName = "Brazil" });
        Alpha3Map.Add("IO", new CountryCodeEntry() { Alpha2 = "IO", Alpha3 = "IOT", CountryName = "British Indian Ocean Territory" });
        Alpha3Map.Add("VG", new CountryCodeEntry() { Alpha2 = "VG", Alpha3 = "VGB", CountryName = "Virgin Islands (British)" });
        Alpha3Map.Add("BN", new CountryCodeEntry() { Alpha2 = "BN", Alpha3 = "BRN", CountryName = "Brunei Darussalam" });
        Alpha3Map.Add("BG", new CountryCodeEntry() { Alpha2 = "BG", Alpha3 = "BGR", CountryName = "Bulgaria" });
        Alpha3Map.Add("BF", new CountryCodeEntry() { Alpha2 = "BF", Alpha3 = "BFA", CountryName = "Burkina Faso" });
        Alpha3Map.Add("BI", new CountryCodeEntry() { Alpha2 = "BI", Alpha3 = "BDI", CountryName = "Burundi" });
        Alpha3Map.Add("CK", new CountryCodeEntry() { Alpha2 = "CK", Alpha3 = "COK", CountryName = "Cook Islands" });
        Alpha3Map.Add("CW", new CountryCodeEntry() { Alpha2 = "CW", Alpha3 = "CUW", CountryName = "Curaçao" });
        Alpha3Map.Add("TD", new CountryCodeEntry() { Alpha2 = "TD", Alpha3 = "TCD", CountryName = "Chad" });
        Alpha3Map.Add("ME", new CountryCodeEntry() { Alpha2 = "ME", Alpha3 = "MNE", CountryName = "Montenegro" });
        Alpha3Map.Add("CZ", new CountryCodeEntry() { Alpha2 = "CZ", Alpha3 = "CZE", CountryName = "Czech Republic" });
        Alpha3Map.Add("CN", new CountryCodeEntry() { Alpha2 = "CN", Alpha3 = "CHN", CountryName = "China" });
        Alpha3Map.Add("DK", new CountryCodeEntry() { Alpha2 = "DK", Alpha3 = "DNK", CountryName = "Denmark" });
        Alpha3Map.Add("CD", new CountryCodeEntry() { Alpha2 = "CD", Alpha3 = "COD", CountryName = "Congo (the Democratic Republic of the)" });
        Alpha3Map.Add("DM", new CountryCodeEntry() { Alpha2 = "DM", Alpha3 = "DMA", CountryName = "Dominica" });
        Alpha3Map.Add("DO", new CountryCodeEntry() { Alpha2 = "DO", Alpha3 = "DOM", CountryName = "Dominican Republic (the)" });
        Alpha3Map.Add("DJ", new CountryCodeEntry() { Alpha2 = "DJ", Alpha3 = "DJI", CountryName = "Djibouti" });
        Alpha3Map.Add("EG", new CountryCodeEntry() { Alpha2 = "EG", Alpha3 = "EGY", CountryName = "Egypt" });
        Alpha3Map.Add("EC", new CountryCodeEntry() { Alpha2 = "EC", Alpha3 = "ECU", CountryName = "Ecuador" });
        Alpha3Map.Add("ER", new CountryCodeEntry() { Alpha2 = "ER", Alpha3 = "ERI", CountryName = "Eritrea" });
        Alpha3Map.Add("EE", new CountryCodeEntry() { Alpha2 = "EE", Alpha3 = "EST", CountryName = "Estonia" });
        Alpha3Map.Add("ET", new CountryCodeEntry() { Alpha2 = "ET", Alpha3 = "ETH", CountryName = "Ethiopia" });
        Alpha3Map.Add("FO", new CountryCodeEntry() { Alpha2 = "FO", Alpha3 = "FRO", CountryName = "Faroe Islands" });
        Alpha3Map.Add("FK", new CountryCodeEntry() { Alpha2 = "FK", Alpha3 = "FLK", CountryName = "Falkland Islands (the) (Malvinas)" });
        Alpha3Map.Add("FJ", new CountryCodeEntry() { Alpha2 = "FJ", Alpha3 = "FJI", CountryName = "Fiji" });
        Alpha3Map.Add("PH", new CountryCodeEntry() { Alpha2 = "PH", Alpha3 = "PHL", CountryName = "Philippines" });
        Alpha3Map.Add("FI", new CountryCodeEntry() { Alpha2 = "FI", Alpha3 = "FIN", CountryName = "Finland" });
        Alpha3Map.Add("FR", new CountryCodeEntry() { Alpha2 = "FR", Alpha3 = "FRA", CountryName = "France" });
        Alpha3Map.Add("GF", new CountryCodeEntry() { Alpha2 = "GF", Alpha3 = "GUF", CountryName = "French Guiana" });
        Alpha3Map.Add("TF", new CountryCodeEntry() { Alpha2 = "TF", Alpha3 = "ATF", CountryName = "French Southern Territories" });
        Alpha3Map.Add("PF", new CountryCodeEntry() { Alpha2 = "PF", Alpha3 = "PYF", CountryName = "French Polynesia" });
        Alpha3Map.Add("GA", new CountryCodeEntry() { Alpha2 = "GA", Alpha3 = "GAB", CountryName = "Gabon" });
        Alpha3Map.Add("GM", new CountryCodeEntry() { Alpha2 = "GM", Alpha3 = "GMB", CountryName = "Gambia" });
        Alpha3Map.Add("GH", new CountryCodeEntry() { Alpha2 = "GH", Alpha3 = "GHA", CountryName = "Ghana" });
        Alpha3Map.Add("GI", new CountryCodeEntry() { Alpha2 = "GI", Alpha3 = "GIB", CountryName = "Gibraltar" });
        Alpha3Map.Add("GD", new CountryCodeEntry() { Alpha2 = "GD", Alpha3 = "GRD", CountryName = "Grenada" });
        Alpha3Map.Add("GL", new CountryCodeEntry() { Alpha2 = "GL", Alpha3 = "GRL", CountryName = "Greenland" });
        Alpha3Map.Add("GE", new CountryCodeEntry() { Alpha2 = "GE", Alpha3 = "GEO", CountryName = "Georgia" });
        Alpha3Map.Add("GP", new CountryCodeEntry() { Alpha2 = "GP", Alpha3 = "GLP", CountryName = "Guadeloupe" });
        Alpha3Map.Add("GU", new CountryCodeEntry() { Alpha2 = "GU", Alpha3 = "GUM", CountryName = "Guam" });
        Alpha3Map.Add("GT", new CountryCodeEntry() { Alpha2 = "GT", Alpha3 = "GTM", CountryName = "Guatemala" });
        Alpha3Map.Add("GG", new CountryCodeEntry() { Alpha2 = "GG", Alpha3 = "GGY", CountryName = "Guernsey" });
        Alpha3Map.Add("GN", new CountryCodeEntry() { Alpha2 = "GN", Alpha3 = "GIN", CountryName = "Guinea" });
        Alpha3Map.Add("GW", new CountryCodeEntry() { Alpha2 = "GW", Alpha3 = "GNB", CountryName = "Guinea-Bissau" });
        Alpha3Map.Add("GY", new CountryCodeEntry() { Alpha2 = "GY", Alpha3 = "GUY", CountryName = "Guyana" });
        Alpha3Map.Add("HT", new CountryCodeEntry() { Alpha2 = "HT", Alpha3 = "HTI", CountryName = "Haiti" });
        Alpha3Map.Add("HM", new CountryCodeEntry() { Alpha2 = "HM", Alpha3 = "HMD", CountryName = "Heard Island and McDonald Islands" });
        Alpha3Map.Add("HN", new CountryCodeEntry() { Alpha2 = "HN", Alpha3 = "HND", CountryName = "Honduras" });
        Alpha3Map.Add("HK", new CountryCodeEntry() { Alpha2 = "HK", Alpha3 = "HKG", CountryName = "Hong Kong" });
        Alpha3Map.Add("CL", new CountryCodeEntry() { Alpha2 = "CL", Alpha3 = "CHL", CountryName = "Chile" });
        Alpha3Map.Add("HR", new CountryCodeEntry() { Alpha2 = "HR", Alpha3 = "HRV", CountryName = "Croatia" });
        Alpha3Map.Add("IN", new CountryCodeEntry() { Alpha2 = "IN", Alpha3 = "IND", CountryName = "India" });
        Alpha3Map.Add("ID", new CountryCodeEntry() { Alpha2 = "ID", Alpha3 = "IDN", CountryName = "Indonesia" });
        Alpha3Map.Add("IQ", new CountryCodeEntry() { Alpha2 = "IQ", Alpha3 = "IRQ", CountryName = "Iraq" });
        Alpha3Map.Add("IR", new CountryCodeEntry() { Alpha2 = "IR", Alpha3 = "IRN", CountryName = "Iran (the Islamic Republic of)" });
        Alpha3Map.Add("IE", new CountryCodeEntry() { Alpha2 = "IE", Alpha3 = "IRL", CountryName = "Ireland" });
        Alpha3Map.Add("IS", new CountryCodeEntry() { Alpha2 = "IS", Alpha3 = "ISL", CountryName = "Iceland" });
        Alpha3Map.Add("IT", new CountryCodeEntry() { Alpha2 = "IT", Alpha3 = "ITA", CountryName = "Italy" });
        Alpha3Map.Add("IL", new CountryCodeEntry() { Alpha2 = "IL", Alpha3 = "ISR", CountryName = "Israel" });
        Alpha3Map.Add("JM", new CountryCodeEntry() { Alpha2 = "JM", Alpha3 = "JAM", CountryName = "Jamaica" });
        Alpha3Map.Add("JP", new CountryCodeEntry() { Alpha2 = "JP", Alpha3 = "JPN", CountryName = "Japan" });
        Alpha3Map.Add("YE", new CountryCodeEntry() { Alpha2 = "YE", Alpha3 = "YEM", CountryName = "Yemen" });
        Alpha3Map.Add("JE", new CountryCodeEntry() { Alpha2 = "JE", Alpha3 = "JEY", CountryName = "Jersey" });
        Alpha3Map.Add("ZA", new CountryCodeEntry() { Alpha2 = "ZA", Alpha3 = "ZAF", CountryName = "South Africa" });
        Alpha3Map.Add("GS", new CountryCodeEntry() { Alpha2 = "GS", Alpha3 = "SGS", CountryName = "South Georgia and the South Sandwich Islands" });
        Alpha3Map.Add("SS", new CountryCodeEntry() { Alpha2 = "SS", Alpha3 = "SSD", CountryName = "South Sudan" });
        Alpha3Map.Add("JO", new CountryCodeEntry() { Alpha2 = "JO", Alpha3 = "JOR", CountryName = "Jordan" });
        Alpha3Map.Add("KY", new CountryCodeEntry() { Alpha2 = "KY", Alpha3 = "CYM", CountryName = "Cayman Islands" });
        Alpha3Map.Add("KH", new CountryCodeEntry() { Alpha2 = "KH", Alpha3 = "KHM", CountryName = "Cambodia" });
        Alpha3Map.Add("CM", new CountryCodeEntry() { Alpha2 = "CM", Alpha3 = "CMR", CountryName = "Cameroon" });
        Alpha3Map.Add("CA", new CountryCodeEntry() { Alpha2 = "CA", Alpha3 = "CAN", CountryName = "Canada" });
        Alpha3Map.Add("CV", new CountryCodeEntry() { Alpha2 = "CV", Alpha3 = "CPV", CountryName = "Cape Verde" });
        Alpha3Map.Add("QA", new CountryCodeEntry() { Alpha2 = "QA", Alpha3 = "QAT", CountryName = "Qatar" });
        Alpha3Map.Add("KZ", new CountryCodeEntry() { Alpha2 = "KZ", Alpha3 = "KAZ", CountryName = "Kazakhstan" });
        Alpha3Map.Add("KE", new CountryCodeEntry() { Alpha2 = "KE", Alpha3 = "KEN", CountryName = "Kenya" });
        Alpha3Map.Add("KI", new CountryCodeEntry() { Alpha2 = "KI", Alpha3 = "KIR", CountryName = "Kiribati" });
        Alpha3Map.Add("CC", new CountryCodeEntry() { Alpha2 = "CC", Alpha3 = "CCK", CountryName = "Cocos (Keeling) Islands (the)" });
        Alpha3Map.Add("CO", new CountryCodeEntry() { Alpha2 = "CO", Alpha3 = "COL", CountryName = "Colombia" });
        Alpha3Map.Add("KM", new CountryCodeEntry() { Alpha2 = "KM", Alpha3 = "COM", CountryName = "Comoros" });
        Alpha3Map.Add("CG", new CountryCodeEntry() { Alpha2 = "CG", Alpha3 = "COG", CountryName = "Congo" });
        Alpha3Map.Add("KP", new CountryCodeEntry() { Alpha2 = "KP", Alpha3 = "PRK", CountryName = "Korea (the Democratic People's Republic of)" });
        Alpha3Map.Add("KR", new CountryCodeEntry() { Alpha2 = "KR", Alpha3 = "KOR", CountryName = "Korea (the Republic of)" });
        Alpha3Map.Add("XK", new CountryCodeEntry() { Alpha2 = "XK", Alpha3 = "XXK", CountryName = "Kosovo" });
        Alpha3Map.Add("CR", new CountryCodeEntry() { Alpha2 = "CR", Alpha3 = "CRI", CountryName = "Costa Rica" });
        Alpha3Map.Add("CU", new CountryCodeEntry() { Alpha2 = "CU", Alpha3 = "CUB", CountryName = "Cuba" });
        Alpha3Map.Add("KW", new CountryCodeEntry() { Alpha2 = "KW", Alpha3 = "KWT", CountryName = "Kuwait" });
        Alpha3Map.Add("CY", new CountryCodeEntry() { Alpha2 = "CY", Alpha3 = "CYP", CountryName = "Cyprus" });
        Alpha3Map.Add("KG", new CountryCodeEntry() { Alpha2 = "KG", Alpha3 = "KGZ", CountryName = "Kyrgyzstan" });
        Alpha3Map.Add("LA", new CountryCodeEntry() { Alpha2 = "LA", Alpha3 = "LAO", CountryName = "Lao People's Democratic Republic (the)" });
        Alpha3Map.Add("LS", new CountryCodeEntry() { Alpha2 = "LS", Alpha3 = "LSO", CountryName = "Lesotho" });
        Alpha3Map.Add("LB", new CountryCodeEntry() { Alpha2 = "LB", Alpha3 = "LBN", CountryName = "Lebanon" });
        Alpha3Map.Add("LR", new CountryCodeEntry() { Alpha2 = "LR", Alpha3 = "LBR", CountryName = "Liberia" });
        Alpha3Map.Add("LY", new CountryCodeEntry() { Alpha2 = "LY", Alpha3 = "LBY", CountryName = "Libya" });
        Alpha3Map.Add("LI", new CountryCodeEntry() { Alpha2 = "LI", Alpha3 = "LIE", CountryName = "Liechtenstein" });
        Alpha3Map.Add("LT", new CountryCodeEntry() { Alpha2 = "LT", Alpha3 = "LTU", CountryName = "Lithuania" });
        Alpha3Map.Add("LV", new CountryCodeEntry() { Alpha2 = "LV", Alpha3 = "LVA", CountryName = "Latvia" });
        Alpha3Map.Add("LU", new CountryCodeEntry() { Alpha2 = "LU", Alpha3 = "LUX", CountryName = "Luxembourg" });
        Alpha3Map.Add("MO", new CountryCodeEntry() { Alpha2 = "MO", Alpha3 = "MAC", CountryName = "Macao" });
        Alpha3Map.Add("MG", new CountryCodeEntry() { Alpha2 = "MG", Alpha3 = "MDG", CountryName = "Madagascar" });
        Alpha3Map.Add("HU", new CountryCodeEntry() { Alpha2 = "HU", Alpha3 = "HUN", CountryName = "Hungary" });
        Alpha3Map.Add("MK", new CountryCodeEntry() { Alpha2 = "MK", Alpha3 = "MKD", CountryName = "Macedonia (the former Yugoslav Republic of)" });
        Alpha3Map.Add("MY", new CountryCodeEntry() { Alpha2 = "MY", Alpha3 = "MYS", CountryName = "Malaysia" });
        Alpha3Map.Add("MW", new CountryCodeEntry() { Alpha2 = "MW", Alpha3 = "MWI", CountryName = "Malawi" });
        Alpha3Map.Add("MV", new CountryCodeEntry() { Alpha2 = "MV", Alpha3 = "MDV", CountryName = "Maldives" });
        Alpha3Map.Add("ML", new CountryCodeEntry() { Alpha2 = "ML", Alpha3 = "MLI", CountryName = "Mali" });
        Alpha3Map.Add("MT", new CountryCodeEntry() { Alpha2 = "MT", Alpha3 = "MLT", CountryName = "Malta" });
        Alpha3Map.Add("IM", new CountryCodeEntry() { Alpha2 = "IM", Alpha3 = "IMN", CountryName = "Isle of Man" });
        Alpha3Map.Add("MA", new CountryCodeEntry() { Alpha2 = "MA", Alpha3 = "MAR", CountryName = "Morocco" });
        Alpha3Map.Add("MH", new CountryCodeEntry() { Alpha2 = "MH", Alpha3 = "MHL", CountryName = "Marshall Islands (the)" });
        Alpha3Map.Add("MQ", new CountryCodeEntry() { Alpha2 = "MQ", Alpha3 = "MTQ", CountryName = "Martinique" });
        Alpha3Map.Add("MU", new CountryCodeEntry() { Alpha2 = "MU", Alpha3 = "MUS", CountryName = "Mauritius" });
        Alpha3Map.Add("MR", new CountryCodeEntry() { Alpha2 = "MR", Alpha3 = "MRT", CountryName = "Mauritania" });
        Alpha3Map.Add("YT", new CountryCodeEntry() { Alpha2 = "YT", Alpha3 = "MYT", CountryName = "Mayotte" });
        Alpha3Map.Add("UM", new CountryCodeEntry() { Alpha2 = "UM", Alpha3 = "UMI", CountryName = "United States Minor Outlying Islands (the)" });
        Alpha3Map.Add("MX", new CountryCodeEntry() { Alpha2 = "MX", Alpha3 = "MEX", CountryName = "Mexico" });
        Alpha3Map.Add("FM", new CountryCodeEntry() { Alpha2 = "FM", Alpha3 = "FSM", CountryName = "Micronesia (the Federated States of)" });
        Alpha3Map.Add("MD", new CountryCodeEntry() { Alpha2 = "MD", Alpha3 = "MDA", CountryName = "Moldova (the Republic of)" });
        Alpha3Map.Add("MC", new CountryCodeEntry() { Alpha2 = "MC", Alpha3 = "MCO", CountryName = "Monaco" });
        Alpha3Map.Add("MN", new CountryCodeEntry() { Alpha2 = "MN", Alpha3 = "MNG", CountryName = "Mongolia" });
        Alpha3Map.Add("MS", new CountryCodeEntry() { Alpha2 = "MS", Alpha3 = "MSR", CountryName = "Montserrat" });
        Alpha3Map.Add("MZ", new CountryCodeEntry() { Alpha2 = "MZ", Alpha3 = "MOZ", CountryName = "Mozambique" });
        Alpha3Map.Add("MM", new CountryCodeEntry() { Alpha2 = "MM", Alpha3 = "MMR", CountryName = "Myanmar" });
        Alpha3Map.Add("NA", new CountryCodeEntry() { Alpha2 = "NA", Alpha3 = "NAM", CountryName = "Namibia" });
        Alpha3Map.Add("NR", new CountryCodeEntry() { Alpha2 = "NR", Alpha3 = "NRU", CountryName = "Nauru" });
        Alpha3Map.Add("DE", new CountryCodeEntry() { Alpha2 = "DE", Alpha3 = "DEU", CountryName = "Germany" });
        Alpha3Map.Add("NP", new CountryCodeEntry() { Alpha2 = "NP", Alpha3 = "NPL", CountryName = "Nepal" });
        Alpha3Map.Add("NE", new CountryCodeEntry() { Alpha2 = "NE", Alpha3 = "NER", CountryName = "Niger (the)" });
        Alpha3Map.Add("NG", new CountryCodeEntry() { Alpha2 = "NG", Alpha3 = "NGA", CountryName = "Nigeria" });
        Alpha3Map.Add("NI", new CountryCodeEntry() { Alpha2 = "NI", Alpha3 = "NIC", CountryName = "Nicaragua" });
        Alpha3Map.Add("NU", new CountryCodeEntry() { Alpha2 = "NU", Alpha3 = "NIU", CountryName = "Niue" });
        Alpha3Map.Add("NL", new CountryCodeEntry() { Alpha2 = "NL", Alpha3 = "NLD", CountryName = "Netherlands (the)" });
        Alpha3Map.Add("NF", new CountryCodeEntry() { Alpha2 = "NF", Alpha3 = "NFK", CountryName = "Norfolk Island" });
        Alpha3Map.Add("NO", new CountryCodeEntry() { Alpha2 = "NO", Alpha3 = "NOR", CountryName = "Norway" });
        Alpha3Map.Add("NC", new CountryCodeEntry() { Alpha2 = "NC", Alpha3 = "NCL", CountryName = "New Caledonia" });
        Alpha3Map.Add("NZ", new CountryCodeEntry() { Alpha2 = "NZ", Alpha3 = "NZL", CountryName = "New Zealand" });
        Alpha3Map.Add("OM", new CountryCodeEntry() { Alpha2 = "OM", Alpha3 = "OMN", CountryName = "Oman" });
        Alpha3Map.Add("PK", new CountryCodeEntry() { Alpha2 = "PK", Alpha3 = "PAK", CountryName = "Pakistan" });
        Alpha3Map.Add("PW", new CountryCodeEntry() { Alpha2 = "PW", Alpha3 = "PLW", CountryName = "Palau" });
        Alpha3Map.Add("PS", new CountryCodeEntry() { Alpha2 = "PS", Alpha3 = "PSE", CountryName = "Palestine, State of" });
        Alpha3Map.Add("PA", new CountryCodeEntry() { Alpha2 = "PA", Alpha3 = "PAN", CountryName = "Panama" });
        Alpha3Map.Add("PG", new CountryCodeEntry() { Alpha2 = "PG", Alpha3 = "PNG", CountryName = "Papua New Guinea" });
        Alpha3Map.Add("PY", new CountryCodeEntry() { Alpha2 = "PY", Alpha3 = "PRY", CountryName = "Paraguay" });
        Alpha3Map.Add("PE", new CountryCodeEntry() { Alpha2 = "PE", Alpha3 = "PER", CountryName = "Peru" });
        Alpha3Map.Add("PN", new CountryCodeEntry() { Alpha2 = "PN", Alpha3 = "PCN", CountryName = "Pitcairn" });
        Alpha3Map.Add("CI", new CountryCodeEntry() { Alpha2 = "CI", Alpha3 = "CIV", CountryName = "Côte d'Ivoire" });
        Alpha3Map.Add("PL", new CountryCodeEntry() { Alpha2 = "PL", Alpha3 = "POL", CountryName = "Poland" });
        Alpha3Map.Add("PR", new CountryCodeEntry() { Alpha2 = "PR", Alpha3 = "PRI", CountryName = "Puerto Rico" });
        Alpha3Map.Add("PT", new CountryCodeEntry() { Alpha2 = "PT", Alpha3 = "PRT", CountryName = "Portugal" });
        Alpha3Map.Add("AT", new CountryCodeEntry() { Alpha2 = "AT", Alpha3 = "AUT", CountryName = "Austria" });
        Alpha3Map.Add("RE", new CountryCodeEntry() { Alpha2 = "RE", Alpha3 = "REU", CountryName = "Réunion" });
        Alpha3Map.Add("GQ", new CountryCodeEntry() { Alpha2 = "GQ", Alpha3 = "GNQ", CountryName = "Equatorial Guinea" });
        Alpha3Map.Add("RO", new CountryCodeEntry() { Alpha2 = "RO", Alpha3 = "ROU", CountryName = "Romania" });
        Alpha3Map.Add("RU", new CountryCodeEntry() { Alpha2 = "RU", Alpha3 = "RUS", CountryName = "Russian Federation (the)" });
        Alpha3Map.Add("RW", new CountryCodeEntry() { Alpha2 = "RW", Alpha3 = "RWA", CountryName = "Rwanda" });
        Alpha3Map.Add("GR", new CountryCodeEntry() { Alpha2 = "GR", Alpha3 = "GRC", CountryName = "Greece" });
        Alpha3Map.Add("PM", new CountryCodeEntry() { Alpha2 = "PM", Alpha3 = "SPM", CountryName = "Saint Pierre and Miquelon" });
        Alpha3Map.Add("SV", new CountryCodeEntry() { Alpha2 = "SV", Alpha3 = "SLV", CountryName = "El Salvador" });
        Alpha3Map.Add("WS", new CountryCodeEntry() { Alpha2 = "WS", Alpha3 = "WSM", CountryName = "Samoa" });
        Alpha3Map.Add("SM", new CountryCodeEntry() { Alpha2 = "SM", Alpha3 = "SMR", CountryName = "San Marino" });
        Alpha3Map.Add("SA", new CountryCodeEntry() { Alpha2 = "SA", Alpha3 = "SAU", CountryName = "Saudi Arabia" });
        Alpha3Map.Add("SN", new CountryCodeEntry() { Alpha2 = "SN", Alpha3 = "SEN", CountryName = "Senegal" });
        Alpha3Map.Add("MP", new CountryCodeEntry() { Alpha2 = "MP", Alpha3 = "MNP", CountryName = "Northern Mariana Islands (the)" });
        Alpha3Map.Add("SC", new CountryCodeEntry() { Alpha2 = "SC", Alpha3 = "SYC", CountryName = "Seychelles" });
        Alpha3Map.Add("SL", new CountryCodeEntry() { Alpha2 = "SL", Alpha3 = "SLE", CountryName = "Sierra Leone" });
        Alpha3Map.Add("SG", new CountryCodeEntry() { Alpha2 = "SG", Alpha3 = "SGP", CountryName = "Singapore" });
        Alpha3Map.Add("SK", new CountryCodeEntry() { Alpha2 = "SK", Alpha3 = "SVK", CountryName = "Slovakia" });
        Alpha3Map.Add("SI", new CountryCodeEntry() { Alpha2 = "SI", Alpha3 = "SVN", CountryName = "Slovenia" });
        Alpha3Map.Add("SO", new CountryCodeEntry() { Alpha2 = "SO", Alpha3 = "SOM", CountryName = "Somalia" });
        Alpha3Map.Add("AE", new CountryCodeEntry() { Alpha2 = "AE", Alpha3 = "ARE", CountryName = "United Arab Emirates (the)" });
        Alpha3Map.Add("US", new CountryCodeEntry() { Alpha2 = "US", Alpha3 = "USA", CountryName = "United States (the)" });
        Alpha3Map.Add("RS", new CountryCodeEntry() { Alpha2 = "RS", Alpha3 = "SRB", CountryName = "Serbia" });
        Alpha3Map.Add("CF", new CountryCodeEntry() { Alpha2 = "CF", Alpha3 = "CAF", CountryName = "Central African Republic (the)" });
        Alpha3Map.Add("SD", new CountryCodeEntry() { Alpha2 = "SD", Alpha3 = "SDN", CountryName = "Sudan (the)" });
        Alpha3Map.Add("SR", new CountryCodeEntry() { Alpha2 = "SR", Alpha3 = "SUR", CountryName = "Suriname" });
        Alpha3Map.Add("SH", new CountryCodeEntry() { Alpha2 = "SH", Alpha3 = "SHN", CountryName = "Saint Helena, Ascension and Tristan da Cunha" });
        Alpha3Map.Add("LC", new CountryCodeEntry() { Alpha2 = "LC", Alpha3 = "LCA", CountryName = "Saint Lucia" });
        Alpha3Map.Add("BL", new CountryCodeEntry() { Alpha2 = "BL", Alpha3 = "BLM", CountryName = "Saint Barthélemy" });
        Alpha3Map.Add("KN", new CountryCodeEntry() { Alpha2 = "KN", Alpha3 = "KNA", CountryName = "Saint Kitts and Nevis" });
        Alpha3Map.Add("MF", new CountryCodeEntry() { Alpha2 = "MF", Alpha3 = "MAF", CountryName = "Saint Martin (French part)" });
        Alpha3Map.Add("SX", new CountryCodeEntry() { Alpha2 = "SX", Alpha3 = "SXM", CountryName = "Sint Maarten (Dutch part)" });
        Alpha3Map.Add("ST", new CountryCodeEntry() { Alpha2 = "ST", Alpha3 = "STP", CountryName = "Sao Tome and Principe" });
        Alpha3Map.Add("VC", new CountryCodeEntry() { Alpha2 = "VC", Alpha3 = "VCT", CountryName = "Saint Vincent and the Grenadines" });
        Alpha3Map.Add("SZ", new CountryCodeEntry() { Alpha2 = "SZ", Alpha3 = "SWZ", CountryName = "Swaziland" });
        Alpha3Map.Add("SY", new CountryCodeEntry() { Alpha2 = "SY", Alpha3 = "SYR", CountryName = "Syrian Arab Republic (the)" });
        Alpha3Map.Add("SB", new CountryCodeEntry() { Alpha2 = "SB", Alpha3 = "SLB", CountryName = "Solomon Islands (the)" });
        Alpha3Map.Add("ES", new CountryCodeEntry() { Alpha2 = "ES", Alpha3 = "ESP", CountryName = "Spain" });
        Alpha3Map.Add("SJ", new CountryCodeEntry() { Alpha2 = "SJ", Alpha3 = "SJM", CountryName = "Svalbard and Jan Mayen" });
        Alpha3Map.Add("LK", new CountryCodeEntry() { Alpha2 = "LK", Alpha3 = "LKA", CountryName = "Sri Lanka" });
        Alpha3Map.Add("SE", new CountryCodeEntry() { Alpha2 = "SE", Alpha3 = "SWE", CountryName = "Sweden" });
        Alpha3Map.Add("CH", new CountryCodeEntry() { Alpha2 = "CH", Alpha3 = "CHE", CountryName = "Switzerland" });
        Alpha3Map.Add("TJ", new CountryCodeEntry() { Alpha2 = "TJ", Alpha3 = "TJK", CountryName = "Tajikistan" });
        Alpha3Map.Add("TZ", new CountryCodeEntry() { Alpha2 = "TZ", Alpha3 = "TZA", CountryName = "Tanzania, United Republic of" });
        Alpha3Map.Add("TH", new CountryCodeEntry() { Alpha2 = "TH", Alpha3 = "THA", CountryName = "Thailand" });
        Alpha3Map.Add("TW", new CountryCodeEntry() { Alpha2 = "TW", Alpha3 = "TWN", CountryName = "Taiwan (Province of China)" });
        Alpha3Map.Add("TG", new CountryCodeEntry() { Alpha2 = "TG", Alpha3 = "TGO", CountryName = "Togo" });
        Alpha3Map.Add("TK", new CountryCodeEntry() { Alpha2 = "TK", Alpha3 = "TKL", CountryName = "Tokelau" });
        Alpha3Map.Add("TO", new CountryCodeEntry() { Alpha2 = "TO", Alpha3 = "TON", CountryName = "Tonga" });
        Alpha3Map.Add("TT", new CountryCodeEntry() { Alpha2 = "TT", Alpha3 = "TTO", CountryName = "Trinidad and Tobago" });
        Alpha3Map.Add("TN", new CountryCodeEntry() { Alpha2 = "TN", Alpha3 = "TUN", CountryName = "Tunisia" });
        Alpha3Map.Add("TR", new CountryCodeEntry() { Alpha2 = "TR", Alpha3 = "TUR", CountryName = "Turkey" });
        Alpha3Map.Add("TM", new CountryCodeEntry() { Alpha2 = "TM", Alpha3 = "TKM", CountryName = "Turkmenistan" });
        Alpha3Map.Add("TC", new CountryCodeEntry() { Alpha2 = "TC", Alpha3 = "TCA", CountryName = "Turks and Caicos Islands (the)" });
        Alpha3Map.Add("TV", new CountryCodeEntry() { Alpha2 = "TV", Alpha3 = "TUV", CountryName = "Tuvalu" });
        Alpha3Map.Add("UG", new CountryCodeEntry() { Alpha2 = "UG", Alpha3 = "UGA", CountryName = "Uganda" });
        Alpha3Map.Add("UA", new CountryCodeEntry() { Alpha2 = "UA", Alpha3 = "UKR", CountryName = "Ukraine" });
        Alpha3Map.Add("UY", new CountryCodeEntry() { Alpha2 = "UY", Alpha3 = "URY", CountryName = "Uruguay" });
        Alpha3Map.Add("UZ", new CountryCodeEntry() { Alpha2 = "UZ", Alpha3 = "UZB", CountryName = "Uzbekistan" });
        Alpha3Map.Add("CX", new CountryCodeEntry() { Alpha2 = "CX", Alpha3 = "CXR", CountryName = "Christmas Island" });
        Alpha3Map.Add("VU", new CountryCodeEntry() { Alpha2 = "VU", Alpha3 = "VUT", CountryName = "Vanuatu" });
        Alpha3Map.Add("VA", new CountryCodeEntry() { Alpha2 = "VA", Alpha3 = "VAT", CountryName = "Holy See (the) (Vatican City State)" });
        Alpha3Map.Add("GB", new CountryCodeEntry() { Alpha2 = "GB", Alpha3 = "GBR", CountryName = "United Kingdom (the)" });
        Alpha3Map.Add("VE", new CountryCodeEntry() { Alpha2 = "VE", Alpha3 = "VEN", CountryName = "Venezuela, Bolivarian Republic of" });
        Alpha3Map.Add("VN", new CountryCodeEntry() { Alpha2 = "VN", Alpha3 = "VNM", CountryName = "Viet Nam" });
        Alpha3Map.Add("TL", new CountryCodeEntry() { Alpha2 = "TL", Alpha3 = "TLS", CountryName = "Timor-Leste" });
        Alpha3Map.Add("WF", new CountryCodeEntry() { Alpha2 = "WF", Alpha3 = "WLF", CountryName = "Wallis and Futuna" });
        Alpha3Map.Add("ZM", new CountryCodeEntry() { Alpha2 = "ZM", Alpha3 = "ZMB", CountryName = "Zambia" });
        Alpha3Map.Add("EH", new CountryCodeEntry() { Alpha2 = "EH", Alpha3 = "ESH", CountryName = "Western Sahara" });
        Alpha3Map.Add("ZW", new CountryCodeEntry() { Alpha2 = "ZW", Alpha3 = "ZWE", CountryName = "Zimbabwe" });
    }
}

/// <summary>
/// CountryCode map item
/// </summary>
public class CountryCodeEntry
{
    /// <summary>
    /// 2-letters code for country
    /// ISO 3166-1 Alpha2
    /// </summary>
    public string Alpha2 { get; init; } = null!;

    /// <summary>
    /// 3-letters code for country
    /// ISO 3166-1 Alpha3
    /// </summary>
    public string Alpha3 { get; init; } = null!;

    /// <summary>
    /// English abbreviation of country name
    /// </summary>
    public string CountryName { get; init; } = null!;

    public override int GetHashCode()
        => Alpha2.GetHashCode() + Alpha3.GetHashCode() + CountryName.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is CountryCodeEntry other)
        {
            return Alpha2.Equals(other.Alpha2) & Alpha3.Equals(other.Alpha3) & CountryName.Equals(other.CountryName);
        }

        return false;
    }
}
