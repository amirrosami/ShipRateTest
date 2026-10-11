using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShipRate.Application.Services.External
{
    public class SaoiCurrencyFetchService
    {
        private readonly HttpClient _http;
        private readonly ApplicationDbContext _db;

        public SaoiCurrencyFetchService(IHttpClientFactory factory, ApplicationDbContext db)
        {
            _http = factory.CreateClient("saoi");
            _db = db;
        }

        public async Task FetchAndSaveAsync()
        {
            var html = await _http.GetStringAsync("https://saoi.ir/currencies/");

            // استخراج محتوای جدول tablepress-2
            var tableMatch = Regex.Match(html,
                @"<table[^>]*id=""tablepress-2""[^>]*>(.*?)</table>",
                RegexOptions.Singleline);

            if (!tableMatch.Success) return;

            // استخراج همه ردیف‌های tbody
            var rows = Regex.Matches(tableMatch.Value,
                @"<tr[^>]*class=""row-\d+""[^>]*>(.*?)</tr>",
                RegexOptions.Singleline);

            var records = new List<SaoiCurrencyRate>();
            var erecords = new List<ExchangeRate>();

            foreach (Match row in rows)
            {
                var cells = Regex.Matches(row.Groups[1].Value,
                    @"<td[^>]*>(.*?)</td>",
                    RegexOptions.Singleline)
                    .Select(m => Regex.Replace(m.Groups[1].Value, "<.*?>", "").Trim())
                    .ToArray();

                if (cells.Length < 8) continue;

                if (cells[0].ToString().Equals("1407/03/29"))
                {
                    var sss = 1;
                }
                if (cells[0].ToString().CompareTo(DateHelper.getSolarDate10(DateTime.Now)) > 0) continue;
                records.Add(new SaoiCurrencyRate
                {
                    DateShamsi = cells[0],
                    DateGregorian = cells[1],
                    ThcEur = ParseRate(cells[2]),
                    ThcUsd = ParseRate(cells[3]),
                    IranianFloatEur = ParseRate(cells[4]),
                    IranianFloatUsd = ParseRate(cells[5]),
                    ForeignFloatEur = ParseRate(cells[6]),
                    ForeignFloatUsd = ParseRate(cells[7]),
                    FetchedAt = DateTime.Now
                });


                try
                {
                    erecords.Add(new ExchangeRate
                    {
                        ExchangeRateBaseId = 10,
                        ExchangeRateDate = DateTime.ParseExact(cells[1].Replace("`", ""), "yyyy/MM/dd", CultureInfo.InvariantCulture),
                        FromCurrencyId = 1,
                        ToCurrencyId = 2,
                        ExchangeRateSource = "Saoi Site",
                        ExchangeRateValue = ParseRate(cells[3]),
                        ExchangeRateValidToDate = DateTime.ParseExact(cells[1].Replace("`", ""), "yyyy/MM/dd", CultureInfo.InvariantCulture)
                    });
                }
                catch (Exception)
                {

                }



            }

            // فقط رکوردهای جدید (بر اساس تاریخ شمسی)
            var existing = _db.SaoiCurrencyRates.Select(r => r.DateShamsi).ToHashSet();
            _db.SaoiCurrencyRates.AddRange(records.Where(r => !existing.Contains(r.DateShamsi) && r.DateShamsi.CompareTo(DateHelper.getSolarDate10(DateTime.Now)) <= 0));

            var startDate = new DateTime(2026, 1, 1);

            var existingKeys = _db.ExchangeRates
                .Select(r => new
                {
                    r.ExchangeRateDate,
                    r.FromCurrencyId,
                    r.ToCurrencyId,
                    r.ExchangeRateBaseId
                })
                .ToList();

            // فیلتر رکوردهای جدید
            var newRecords = erecords
                .Where(r => r.ExchangeRateDate >= startDate)
                .Where(r => !existingKeys.Any(e =>
                    e.ExchangeRateDate == r.ExchangeRateDate &&
                    e.FromCurrencyId == r.FromCurrencyId &&
                    e.ToCurrencyId == r.ToCurrencyId &&
                    e.ExchangeRateBaseId == r.ExchangeRateBaseId))
                .ToList();

            _db.ExchangeRates.AddRange(newRecords);
            await _db.SaveChangesAsync();

            var incorrectData = _db.ExchangeRates.Where(x => x.ExchangeRateDate > DateTime.Now).ToList();
            _db.RemoveRange(incorrectData);
            await _db.SaveChangesAsync();

        }

        private static decimal ParseRate(string s) =>
            decimal.TryParse(s.Replace(".", "").Replace(",", ""), out var v) ? v : 0;
    }
}
