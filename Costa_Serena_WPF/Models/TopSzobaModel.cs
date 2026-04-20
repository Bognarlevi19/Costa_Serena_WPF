namespace Costa_Serena_WPF.Models
{
    public class TopSzobaModel
    {
        public string Nev { get; set; } = string.Empty;
        public int FoglalasDarab { get; set; }
        public int FoglaltNapok { get; set; }
        public int KihasznaltsagSzazalek { get; set; }

        public string KihasznaltsagSzoveg => $"{KihasznaltsagSzazalek}%";
    }
}