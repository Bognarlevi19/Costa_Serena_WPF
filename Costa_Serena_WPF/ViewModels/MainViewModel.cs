using Costa_Serena_Grand_Hotel_WPF.Data;
using Costa_Serena_Grand_Hotel_WPF.Models;
using Costa_Serena_WPF.Models;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Costa_Serena_Grand_Hotel_WPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _osszesVendeg;
        public int OsszesVendeg
        {
            get => _osszesVendeg;
            set { _osszesVendeg = value; OnPropertyChanged(); }
        }

        private int _aktivFoglalasok;
        public int AktivFoglalasok
        {
            get => _aktivFoglalasok;
            set { _aktivFoglalasok = value; OnPropertyChanged(); }
        }

        private int _kihasznaltsagSzazalek;
        public int KihasznaltsagSzazalek
        {
            get => _kihasznaltsagSzazalek;
            set { _kihasznaltsagSzazalek = value; OnPropertyChanged(); }
        }

        private string _haviBevetelSzoveg = "0 Ft";
        public string HaviBevetelSzoveg
        {
            get => _haviBevetelSzoveg;
            set { _haviBevetelSzoveg = value; OnPropertyChanged(); }
        }

        private string _legnepszerubbSzobatipus = "-";
        public string LegnepszerubbSzobatipus
        {
            get => _legnepszerubbSzobatipus;
            set { _legnepszerubbSzobatipus = value; OnPropertyChanged(); }
        }

        private string _atlagosTartozkodas = "-";
        public string AtlagosTartozkodas
        {
            get => _atlagosTartozkodas;
            set { _atlagosTartozkodas = value; OnPropertyChanged(); }
        }

        private string _visszateroVendegekArany = "-";
        public string VisszateroVendegekArany
        {
            get => _visszateroVendegekArany;
            set { _visszateroVendegekArany = value; OnPropertyChanged(); }
        }

        private string _legerosebbHonap = "-";
        public string LegerosebbHonap
        {
            get => _legerosebbHonap;
            set { _legerosebbHonap = value; OnPropertyChanged(); }
        }

        public ISeries[] FoglalasTrendSeries { get; set; } = Array.Empty<ISeries>();
        public ISeries[] BevetelSeries { get; set; } = Array.Empty<ISeries>();
        public ISeries[] SzobaKategoriaSeries { get; set; } = Array.Empty<ISeries>();

        public Axis[] HonapokTengelye { get; set; }
        public Axis[] ErtekTengelye { get; set; }
        public Axis[] PenzTengelye { get; set; }

        public ObservableCollection<TopSzobaModel> TopSzobak { get; set; } = new();

        public MainViewModel()
        {
            HonapokTengelye = new Axis[]
            {
                new Axis
                {
                    Labels = new[] { "Jan", "Febr", "Márc", "Ápr", "Máj", "Jún", "Júl", "Aug", "Szept", "Okt", "Nov", "Dec" },
                    TextSize = 13,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(230, 235, 241))
                }
            };

            ErtekTengelye = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    TextSize = 13,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(230, 235, 241))
                }
            };

            PenzTengelye = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    TextSize = 13,
                    Labeler = value => $"{value / 1000000:0.#} M Ft",
                    SeparatorsPaint = new SolidColorPaint(new SKColor(230, 235, 241))
                }
            };

            BetoltesAdatbazisbol();
        }

        private void BetoltesAdatbazisbol()
        {
            using var context = new HotelDbContext();

            var maiNap = DateTime.Today;
            var aktualisEv = DateTime.Today.Year;

            OsszesVendeg = context.Vendegek.Count();

            AktivFoglalasok = context.Foglalasok.Count(x => x.Fizetett == false);

            var szobakSzama = context.Szobak.Count();

            var foglaltSzobakMost = context.Foglalasok
                .Where(x => x.Mettol <= maiNap && x.Meddig >= maiNap)
                .Select(x => x.SzobaId)
                .Distinct()
                .Count();

            KihasznaltsagSzazalek = szobakSzama == 0
                ? 0
                : (int)Math.Round((double)foglaltSzobakMost / szobakSzama * 100);

            var haviFoglalasok = context.Foglalasok
                .Where(x => x.Mettol.Year == aktualisEv)
                .GroupBy(x => x.Mettol.Month)
                .Select(g => new { Honap = g.Key, Db = g.Count() })
                .ToList();

            var haviFoglalasErtekek = Enumerable.Range(1, 12)
                .Select(h => haviFoglalasok.FirstOrDefault(x => x.Honap == h)?.Db ?? 0)
                .ToArray();

            FoglalasTrendSeries = new ISeries[]
            {
                new LineSeries<int>
                {
                    Name = "Foglalások",
                    Values = haviFoglalasErtekek,
                    GeometrySize = 10,
                    Fill = null,
                    Stroke = new SolidColorPaint(new SKColor(18, 60, 90), 4),
                    GeometryStroke = new SolidColorPaint(new SKColor(18, 60, 90), 3),
                    GeometryFill = new SolidColorPaint(new SKColor(255, 255, 255))
                }
            };
            OnPropertyChanged(nameof(FoglalasTrendSeries));

            var szallasBevetelHonapokra = SzamolSzallasBevetelHonapokra(context, aktualisEv);
            var termekBevetelHonapokra = SzamolTermekRendelesBevetelHonapokra(context, aktualisEv);

            var haviBevetelErtekek = Enumerable.Range(1, 12)
                .Select(h => szallasBevetelHonapokra[h - 1] + termekBevetelHonapokra[h - 1])
                .ToArray();

            HaviBevetelSzoveg = $"{haviBevetelErtekek[maiNap.Month - 1]:N0} Ft";

            BevetelSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Bevétel",
                    Values = haviBevetelErtekek,
                    MaxBarWidth = 42,
                    Fill = new SolidColorPaint(new SKColor(48, 106, 145))
                }
            };
            OnPropertyChanged(nameof(BevetelSeries));

            var kategoriak = context.Foglalasok
                .Include(x => x.Szoba)
                .ThenInclude(x => x.SzobaKategoria)
                .Where(x => x.Szoba != null && x.Szoba.SzobaKategoria != null)
                .ToList()
                .GroupBy(x => x.Szoba.SzobaKategoria.Nev)
                .ToList();

            SzobaKategoriaSeries = kategoriak
                .Select(g => (ISeries)new PieSeries<int>
                {
                    Name = g.Key,
                    Values = new[] { g.Count() }
                })
                .ToArray();

            OnPropertyChanged(nameof(SzobaKategoriaSeries));

            var evEleje = new DateTime(aktualisEv, 1, 1);
            var evVege = new DateTime(aktualisEv, 12, 31);

            var topSzobak = context.Foglalasok
                .Include(x => x.Szoba)
                .Where(x => x.Szoba != null && x.Mettol.Year == aktualisEv)
                .AsEnumerable()
                .GroupBy(x => new
                {
                    x.SzobaId,
                    SzobaNev = $"{x.Szoba.Nev} ({x.Szoba.Szam})"
                })
                .Select(g =>
                {
                    var foglaltNapok = g.Sum(f =>
                    {
                        var start = f.Mettol < evEleje ? evEleje : f.Mettol;
                        var end = f.Meddig > evVege ? evVege : f.Meddig;

                        var napok = (end.Date - start.Date).Days;
                        if (napok <= 0) napok = 1;

                        return napok;
                    });

                    var kihasznaltsag = (int)Math.Round((double)foglaltNapok / 365 * 100);

                    return new TopSzobaModel
                    {
                        Nev = g.Key.SzobaNev,
                        FoglalasDarab = g.Count(),
                        FoglaltNapok = foglaltNapok,
                        KihasznaltsagSzazalek = Math.Min(kihasznaltsag, 100)
                    };
                })
                .OrderByDescending(x => x.FoglalasDarab)
                .Take(5)
                .ToList();

            TopSzobak.Clear();
            foreach (var szoba in topSzobak)
                TopSzobak.Add(szoba);

            LegnepszerubbSzobatipus = kategoriak.FirstOrDefault()?.Key ?? "-";

            var atlagEj = context.Foglalasok
                .AsEnumerable()
                .Select(x =>
                {
                    var napok = (x.Meddig.Date - x.Mettol.Date).Days;
                    if (napok <= 0) napok = 1;
                    return napok;
                })
                .DefaultIfEmpty(0)
                .Average();

            AtlagosTartozkodas = $"{atlagEj:0.0} éj";

            var vendegFoglalasok = context.Foglalasok
                .GroupBy(x => x.VendegId)
                .Select(g => g.Count())
                .ToList();

            var osszesKulonbozoVendeg = vendegFoglalasok.Count;
            var visszateroVendegDb = vendegFoglalasok.Count(x => x > 1);

            var visszateroArany = osszesKulonbozoVendeg == 0
                ? 0
                : (int)Math.Round((double)visszateroVendegDb / osszesKulonbozoVendeg * 100);

            VisszateroVendegekArany = $"{visszateroArany}%";

            var legerosebbHonapIndex = haviBevetelErtekek
                .Select((ertek, index) => new { Ertek = ertek, Index = index })
                .OrderByDescending(x => x.Ertek)
                .FirstOrDefault()?.Index ?? 0;

            var honapNevek = new[]
            {
                "Január", "Február", "Március", "Április", "Május", "Június",
                "Július", "Augusztus", "Szeptember", "Október", "November", "December"
            };

            LegerosebbHonap = honapNevek[legerosebbHonapIndex];
        }

        private static int[] SzamolSzallasBevetelHonapokra(HotelDbContext context, int ev)
        {
            var evEleje = new DateTime(ev, 1, 1);
            var kovetkezoEvEleje = evEleje.AddYears(1);
            var eredmeny = new int[12];

            var foglalasok = context.Foglalasok
                .AsNoTracking()
                .Include(x => x.Szoba)
                .Where(x => x.Mettol < kovetkezoEvEleje && x.Meddig >= evEleje)
                .ToList();

            foreach (var foglalas in foglalasok)
            {
                if (foglalas.Szoba == null)
                    continue;

                var start = foglalas.Mettol.Date < evEleje ? evEleje : foglalas.Mettol.Date;
                var end = foglalas.Meddig.Date > kovetkezoEvEleje ? kovetkezoEvEleje : foglalas.Meddig.Date;

                if (end <= start)
                    end = start.AddDays(1);

                for (var nap = start; nap < end; nap = nap.AddDays(1))
                {
                    eredmeny[nap.Month - 1] += foglalas.Szoba.Ar;
                }
            }

            return eredmeny;
        }

        private static int[] SzamolTermekRendelesBevetelHonapokra(HotelDbContext context, int ev)
        {
            var eredmeny = new int[12];

            var rendelesek = context.Rendelesek
                .AsNoTracking()
                .Include(x => x.Tetelek)
                .Where(x => x.Letrehozva.Year == ev && x.Tetelek.Any())
                .ToList();

            foreach (var rendeles in rendelesek)
            {
                var honapIndex = rendeles.Letrehozva.Month - 1;

                var osszeg = rendeles.Tetelek.Sum(t =>
                    t.Osszeg > 0
                        ? t.Osszeg
                        : t.Egysegar * (t.Mennyiseg <= 0 ? 1 : t.Mennyiseg));

                eredmeny[honapIndex] += osszeg;
            }

            return eredmeny;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}