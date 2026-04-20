namespace Costa_Serena_Grand_Hotel_WPF.Models
{
    public class Vendeg
    {
        public int Id { get; set; }
        public string Nev { get; set; } = string.Empty;
        public string? SzemelyiIgazolvanySzam { get; set; }
        public int IranyitoSzam { get; set; }
        public string Varos { get; set; } = string.Empty;
        public string Utca { get; set; } = string.Empty;
        public string Hazszam { get; set; } = string.Empty;

        public ICollection<Foglalas> Foglalasok { get; set; } = new List<Foglalas>();
    }

    public class Szoba
    {
        public int Id { get; set; }
        public string Szam { get; set; } = string.Empty;
        public int Emelet { get; set; }
        public double Alapterulet { get; set; }
        public int Ar { get; set; }
        public string Nev { get; set; } = string.Empty;
        public string? RovidLeiras { get; set; }
        public string? Leiras { get; set; }
        public int Ferohely { get; set; }
        public string? KepekJson { get; set; }

        public int SzobaKategoriaId { get; set; }
        public SzobaKategoria? SzobaKategoria { get; set; }

        public ICollection<Foglalas> Foglalasok { get; set; } = new List<Foglalas>();
    }

    public class SzobaKategoria
    {
        public int Id { get; set; }
        public string Nev { get; set; } = string.Empty;
        public string? Leiras { get; set; }
        public string? KepekJson { get; set; }

        public ICollection<Szoba> Szobak { get; set; } = new List<Szoba>();
    }

    public class Foglalas
    {
        public int Id { get; set; }

        public int SzobaId { get; set; }
        public Szoba Szoba { get; set; } = null!;

        public int VendegId { get; set; }
        public Vendeg Vendeg { get; set; } = null!;

        public DateTime Mettol { get; set; }
        public DateTime Meddig { get; set; }
        public bool Fizetett { get; set; }
    }

    public class Rendeles
    {
        public int Id { get; set; }

        public int VendegId { get; set; }
        public Vendeg Vendeg { get; set; } = null!;

        public string Nev { get; set; } = string.Empty;
        public string SzemelyiIgazolvanySzam { get; set; } = string.Empty;
        public int IranyitoSzam { get; set; }
        public string Varos { get; set; } = string.Empty;
        public string Utca { get; set; } = string.Empty;
        public string Hazszam { get; set; } = string.Empty;

        public DateTime Letrehozva { get; set; }
        public int Vegosszeg { get; set; }
        public bool Fizetett { get; set; }
        public bool Elkuldve { get; set; }

        public ICollection<RendelesTetel> Tetelek { get; set; } = new List<RendelesTetel>();
    }

    public class RendelesTetel
    {
        public int Id { get; set; }

        public int RendelesId { get; set; }
        public Rendeles Rendeles { get; set; } = null!;

        public int TermekId { get; set; }
        public int Mennyiseg { get; set; }
        public int Egysegar { get; set; }
        public int Osszeg { get; set; }
    }
}